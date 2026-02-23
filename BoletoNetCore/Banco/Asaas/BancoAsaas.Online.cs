using System;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using BoletoNetCore.LinkPagamento;
using System.Collections.Generic;
using System.Drawing;
using BoletoNetCore.Extensions;
using BoletoNetCore.CartãoDeCredito;
using BoletoNetCore.Clientes;
using System.Linq;
using Leader.Infrasctruture.Repositories.Base;

namespace BoletoNetCore
{
    partial class BancoAsaas : IBancoOnlineRest
    {
        #region HttpClient
        private HttpClient _httpClient;
        private HttpClient httpClient
        {
            get
            {
                if (this._httpClient == null)
                {
                    this._httpClient = new HttpClient();
                    this._httpClient.BaseAddress = new Uri("https://sandbox.asaas.com/api/v3/");// Homologação
                    //this._httpClient.BaseAddress = new Uri("https://api.asaas.com/v3/");//Prod
                    
                    this._httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                }

                return this._httpClient;
            }
        }
        #endregion

        #region Chaves de Acesso Api

        // Chave Master que deve ser gerada pelo portal do Asaas
        // Menu Cobrança, Sub Menu Lateral Código de Acesso / Gerar
        public string ChaveApi { get; set; }

        // Chave de Transação valida por 24 horas
        // Segundo o manual, não é permitido gerar uma nova chave de transação antes da atual estar expirada.
        // Caso seja necessário gerar uma chave de transação antes, é necessário criar uma nova chave master, o que invalida a anterior.
        public string Token { get; set; }

        #endregion

        public BancoAsaas(string chaveApi)
        {
            Codigo = 461;
            Nome = "ASAAS";
            Digito = "0";
            ChaveApi = chaveApi;
        }

        public Task<string> GerarToken()
        {
            this.Token = this.ChaveApi;
            this.httpClient.DefaultRequestHeaders.Add("access_token", this.Token);
            return Task.FromResult(this.Token);
        }

        public async Task RegistrarBoleto(Boleto boleto)
        {
            var emissao = new EmissaoBoletoAsaasApiRequest();
            emissao.Agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia;
            emissao.Posto = boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia;
            emissao.Cedente = boleto.Banco.Beneficiario.Codigo;
            emissao.NossoNumero = boleto.NossoNumero + boleto.NossoNumeroDV;
            emissao.TipoPessoa = boleto.Pagador.TipoCPFCNPJ("0");
            emissao.CpfCnpj = boleto.Pagador.CPFCNPJ;
            emissao.Nome = boleto.Pagador.Nome;
            emissao.Endereco = boleto.Pagador.Endereco.FormataLogradouro(0);
            emissao.Cidade = boleto.Pagador.Endereco.Cidade;
            emissao.Uf = boleto.Pagador.Endereco.UF;
            emissao.Cep = boleto.Pagador.Endereco.CEP;

            // todo
            emissao.CodigoPagador = string.Empty;

            // manual: "Opcional. Será obrigatório se o código do pagador não for informado"
            emissao.Telefone = boleto.Pagador.Telefone;

            emissao.Email = "";
            //emissao.EspecieDocumento = this.AjustaEspecieCnab400(boleto.EspecieDocumento);
            emissao.SeuNumero = boleto.NumeroDocumento;
            emissao.DataVencimento = boleto.DataVencimento.ToString("dd/MM/yyyy");
            emissao.Valor = boleto.ValorTitulo;
            emissao.TipoDesconto = "A"; // todo: 

            if (boleto.ValorDesconto != 0)
            {
                emissao.ValorDesconto1 = boleto.ValorDesconto;
                emissao.DataDesconto1 = boleto.DataDesconto.ToString("dd/MM/yyyy");
            }

            emissao.TipoJuros = "A"; // todo
            emissao.Juros = boleto.ValorJurosDia;
            emissao.Multas = boleto.ValorMulta;
            emissao.DescontoAntecipado = 0; // todo
            emissao.Informativo = ""; // todo
            emissao.Mensagem = boleto.MensagemInstrucoesCaixaFormatado;
            emissao.NumDiasNegativacaoAuto = boleto.DiasProtesto;

            var request = new HttpRequestMessage(HttpMethod.Post, "payments");
            request.Headers.Add("access_token", this.Token);
            request.Content = JsonContent.Create(emissao);
            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);

            // todo: verificar a necessidade de preencher dados do boleto com o retorno do Asaas
            var boletoEmitido = await response.Content.ReadFromJsonAsync<BoletoEmitidoAsaasApi>();
            boletoEmitido.LinhaDigitável.ToString();
            boletoEmitido.CodigoBarra.ToString();
        }

        private async Task CheckHttpResponseError(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            if (response.StatusCode == HttpStatusCode.BadRequest || (response.StatusCode == HttpStatusCode.NotFound && response.Content.Headers.ContentType.MediaType == "application/json"))
            {
                var teste = response.Content.ReadAsStringAsync();
                var bad = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(string.Format("{0} {1}", bad.Errors[0].Code, bad.Errors[0].Description).Trim());
            }
            else
                throw new Exception(string.Format("Erro desconhecido: {0}", response.StatusCode));
        }

        public async Task ConsultarStatus(Boleto boleto)
        {
            var agencia = boleto.Banco.Beneficiario.ContaBancaria.Agencia;
            var posto = boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia;
            var cedente = boleto.Banco.Beneficiario.Codigo;
            var nossoNumero = boleto.NossoNumero + boleto.NossoNumeroDV;

            // existem outros parametros no manual para consulta de multiplos boletos
            var url = string.Format("consulta?agencia={0}&cedente={1}&posto={2}&nossoNumero={3}", agencia, cedente, posto, nossoNumero);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("token", this.Token);
            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);
            var ret = await response.Content.ReadFromJsonAsync<RetornoConsultaBoletoAsaasApi[]>();

            // todo: verificar quais dados necessarios para preencher boleto
            ret[0].Situacao.ToString();
        }

        public async Task<LinkPagamentoResponse> GerarLinkPagamento(LinkPagamentoRequest linkPagamento)
        {

            var linkPagamentoAsaasRequest = new LinkPagamentoAsaasRequest();
            linkPagamentoAsaasRequest.endDate = linkPagamento.DataFinalLink.ToString("yyyy-MM-dd");
            linkPagamentoAsaasRequest.name = linkPagamento.NomeLinkCobranca;
            linkPagamentoAsaasRequest.description = linkPagamento.Descricao;
            linkPagamentoAsaasRequest.billingType = linkPagamento.FormaCobranca;
            linkPagamentoAsaasRequest.chargeType = linkPagamento.TipoCobranca;
            linkPagamentoAsaasRequest.value = linkPagamento.Valor;
            linkPagamentoAsaasRequest.dueDateLimitDays = linkPagamento.DataVencimentoLimite;
            linkPagamentoAsaasRequest.subscriptionCycle = linkPagamento.PeriodicidadeCobranca;
            linkPagamentoAsaasRequest.maxInstallmentCount = linkPagamento.QuantidadeMaximaParcelamento;
            linkPagamentoAsaasRequest.notificationEnabled = linkPagamento.HabilitaNotificacao;
            linkPagamentoAsaasRequest.callback = new LinkPagamentoAsaasCallbackRequest()
            {
                autoRedirect = linkPagamento.RedicionarAutomaticamente,
                successUrl = linkPagamento.UrlPagamentoSucesso
            };



            var request = new HttpRequestMessage(HttpMethod.Post, "paymentLinks");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("access_token", this.Token);
            request.Content = JsonContent.Create(linkPagamentoAsaasRequest);
            var response = await this.httpClient.SendAsync(request);
            await this.CheckHttpResponseError(response);

            var ret = await response.Content.ReadFromJsonAsync<LinkPagamentoAsaasResponse>();

            var linkPagamentoResponse = new LinkPagamentoResponse();
            // todo: verificar quais dados necessarios para preencher linkpagamentoresponse
            linkPagamentoResponse.Id = ret.id;
            linkPagamentoResponse.FormaPagamento = ret.billingType;
            linkPagamentoResponse.FormaCobranca = ret.chargeType;
            linkPagamentoResponse.UrlLink = ret.url;
            linkPagamentoResponse.NomeLink = ret.name;
            linkPagamentoResponse.StatusLink = ret.active;
            linkPagamentoResponse.DataFinal = ret.endDate;
            linkPagamentoResponse.Descricao = ret.description;
            linkPagamentoResponse.Deletado = ret.deleted;
            linkPagamentoResponse.Visualizacoes = ret.viewCount;
            linkPagamentoResponse.Valor = ret.value;
            linkPagamentoResponse.Periodicidade = ret.subscriptionCycle;
            linkPagamentoResponse.QtdMaximaParcelas = ret.maxInstallmentCount;
            linkPagamentoResponse.DiasUteisBoleto = ret.dueDateLimitDays;
            linkPagamentoResponse.NotificacaoAtivada = ret.notificationEnabled;


            return linkPagamentoResponse;

        }
        public async Task<PaymentCreditCardResponse> GerarCobrancaCartao(RequestCobranca requestCreditCard)
        {
                var customer = "";
                var retor = await VerificaCustomer(requestCreditCard.CustomerInfo.CpfCnpj);

                if (retor.Data.Count == 0)
                    customer = AddCustomer(requestCreditCard.CustomerInfo).Result.Id;

                if (retor.Data.Count > 0)
                    customer = retor.Data.FirstOrDefault().Id;

                requestCreditCard.Customer = customer;
                requestCreditCard.CreditCardHolderInfo = await TrataInfoCartao(requestCreditCard.CustomerInfo);
                var request = new HttpRequestMessage(HttpMethod.Post, "payments");
                request.Content = JsonContent.Create(requestCreditCard);
                var retorno = await AbstractProxy.GenericRequest<PaymentCreditCardResponse>(this.httpClient, request);

            //var ret = await response.Content.ReadFromJsonAsync<PaymentCreditCardResponse>();
                return retorno;

        }
        public async Task<BankSlip> GerarCobrancaBoleto(RequestCobranca requestInvoice)
        {
            var customer = "";
            var retor = await VerificaCustomer(requestInvoice.CustomerInfo.CpfCnpj);
            if (retor.Data.Count == 0)
                customer = AddCustomer(requestInvoice.CustomerInfo).Result.Id;

            if (retor.Data.Count > 0)
                customer = retor.Data.FirstOrDefault().Id;

            requestInvoice.Customer = customer;

            var request = new HttpRequestMessage(HttpMethod.Post, "payments");
            request.Content = JsonContent.Create(requestInvoice);
            var retorno = await AbstractProxy.GenericRequest<BankSlip>(this.httpClient, request);

            //var ret = await response.Content.ReadFromJsonAsync<BankSlip>();
            
            return retorno;

        }
        public async Task<Pix> GerarPix(string idCobranca)
        {
            var requestCustomer = new HttpRequestMessage(HttpMethod.Get, $"payments/{idCobranca}/pixQrCode");
            requestCustomer.Headers.Add("accept", "application/json");
            requestCustomer.Headers.Add("access_token", this.Token);
            //var responseCustomer = await this.httpClient.SendAsync(requestCustomer);
            var retor = await AbstractProxy.GenericRequest<Pix>(this.httpClient, requestCustomer);

            return retor;
        }
        private async Task<CustomerList> VerificaCustomer(string cpfCnpj)
        {   
            
            var url = $"customers?cpfCnpj={cpfCnpj}";
            //var requestCustomer = new HttpRequestMessage(HttpMethod.Get, "customers?cpfCnpj=" + cpfCnpj);
            var requestCustomer = new HttpRequestMessage(HttpMethod.Get, url);
            //requestCustomer.Headers.Add("accept", "application/json");
            //requestCustomer.Headers.Add("access_token", this.Token);
            var retor = await AbstractProxy.GenericRequest<CustomerList>(this.httpClient, requestCustomer);
            //var responseCustomer = await this.httpClient.SendAsync(requestCustomer);
            //if (responseCustomer.StatusCode == HttpStatusCode.Unauthorized)
            //    throw new UnauthorizedAccessException(responseCustomer.StatusCode.ToString());

            //var retor = await responseCustomer.Content.ReadFromJsonAsync<CustomerList>();
            return retor;
        }
        private async Task<CreditCardHolderInfo> TrataInfoCartao(CustomerInfo info)
        {
            var response = new CreditCardHolderInfo();
            response.CpfCnpj = info.CpfCnpj;
            response.Phone = info.Phone;
            response.AddressNumber = info.AddressNumber;
            response.AddressComplement = info.AddressComplement;
            response.Email = info.Email;
            response.Name = info.Name;
            response.PostalCode = info.PostalCode;
            return response;
        }
        private async Task<Customer> AddCustomer(CustomerInfo request)
        {
            var requestCustomer = new HttpRequestMessage(HttpMethod.Post, "customers");
            requestCustomer.Content = JsonContent.Create(request);
            var retor = await AbstractProxy.GenericRequest<Customer>(this.httpClient, requestCustomer);
            
            //var response = await this.httpClient.SendAsync(requestCustomer);
            //await this.CheckHttpResponseError(response);

            //var ret = await response.Content.ReadFromJsonAsync<Customer>();
            return retor;
        }

        public async Task<WebHookAssasResponse> AtualizarCobranca(WebHookAssasResponse request)
        {
            var url = $"payments/{request.payment.id}";
            var requestCustomer = new HttpRequestMessage(HttpMethod.Put, url);
            requestCustomer.Headers.Add("accept", "application/json");
            requestCustomer.Headers.Add("access_token", this.ChaveApi);
            requestCustomer.Headers.Add("user-agent", "C# API");
            requestCustomer.Content = JsonContent.Create(request);
            var retor = await AbstractProxy.GenericRequest<WebHookAssasResponse>(this.httpClient, requestCustomer);
            return retor;
        }
    }

    #region Classes Auxiliares (json) Asaas

    class InstrucaoAsaasApi
    {
        public string Agencia { get; set; }
        public string Posto { get; set; }
        public string Cedente { get; set; }
        public string NossoNumero { get; set; }

        /*
            PEDIDO_BAIXA
            CONCESSAO_ABATIMENTO
            CANCELAMENTO_ABATIMENTO_CONCEDIDO
            ALTERACAO_VENCIMENTO
            ALTERACAO_SEU_NUMERO
            PEDIDO_PROTESTO
            SUSTAR_PROTESTO_BAIXAR _TITULO
            SUSTAR_PROTESTO_MANTER_CARTEIRA
            ALTERACAO_OUTROS_DADOS
        */
        public string InstrucaoComando { get; set; }

        /*
            DESCONTO
            JUROS_DIA
            DESCONTO_DIA_ANTECIPACAO
            DATA_LIMITE_CONCESSAO_DESCONTO
            CANCELAMENTO_PROTESTO _AUTOMATICO
            CANCELAMENTO_NEGATIVACAO_AUTOMATICA
        */
        public string ComplementoInstrucao { get; set; }
    }

    class BadRequestAsaasApi
    {
        public string Codigo { get; set; }
        public string Mensagem { get; set; }
        public string Parametro { get; set; }
    }
    class ResponseErrorModel
    {
        public string message { get; set; }
    }
    class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    class ErrorResponse
    {
        public List<Error> Errors { get; set; }
    }

    class ChaveTransacaoAsaasApi
    {
        public string ChaveTransacao { get; set; }
        public DateTime dataExpiracao { get; set; }
    }

    class RetornoConsultaBoletoAsaasApi
    {
        public string SeuNumero { get; set; }
        public string NossoNumero { get; set; }
        public string NomePagador { get; set; }
        public string Valor { get; set; }
        public string ValorLiquidado { get; set; }
        public string DataEmissao { get; set; }
        public string DataVencimento { get; set; }
        public string DataLiquidacao { get; set; }
        public string Situacao { get; set; }
    }

    class BoletoEmitidoAsaasApi
    {
        public string LinhaDigitável { get; set; }
        public string CodigoBanco { get; set; }
        public string NomeBeneficiario { get; set; }
        public string EnderecoBeneficiario { get; set; }
        public string CpfCnpjBeneficiario { get; set; }
        public string CooperativaBeneficiario { get; set; }
        public string PostoBeneficiario { get; set; }
        public string CodigoBeneficiario { get; set; }
        public DateTime DataDocumento { get; set; }
        public string SeuNumero { get; set; }
        public string EspecieDocumento { get; set; }
        public string Aceite { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string NossoNumero { get; set; }
        public string Especie { get; set; }
        public decimal ValorDocumento { get; set; }
        public DateTime DataVencimento { get; set; }
        public string NomePagador { get; set; }
        public string CpfCnpjPagador { get; set; }
        public string EnderecoPagador { get; set; }
        public DateTime DataLimiteDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal JurosMulta { get; set; }
        public string Instrucao { get; set; }
        public string Informativo { get; set; }
        public string CodigoBarra { get; set; }
    }

    class EmissaoBoletoAsaasApiRequest
    {
        public string Agencia { get; set; }
        public string Posto { get; set; }
        public string Cedente { get; set; }
        public string NossoNumero { get; set; }
        public string CodigoPagador { get; set; }
        /// <summary>
        /// 1 fisica - 2 juridica
        /// </summary>
        public string TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string EspecieDocumento { get; set; }
        public string CodigoSacadorAvalista { get; set; }
        public string SeuNumero { get; set; }
        public string DataVencimento { get; set; }
        public decimal Valor { get; set; }
        /// <summary>
        /// A valor / B percentual
        /// </summary>
        public string TipoDesconto { get; set; }
        public decimal ValorDesconto1 { get; set; }
        public string DataDesconto1 { get; set; }
        public decimal ValorDesconto2 { get; set; }
        public string DataDesconto2 { get; set; }
        public decimal ValorDesconto3 { get; set; }
        public string DataDesconto3 { get; set; }
        /// <summary>
        /// A valor / B percentual
        /// </summary>
        public string TipoJuros { get; set; }
        public decimal Juros { get; set; }
        public decimal Multas { get; set; }
        public decimal DescontoAntecipado { get; set; }
        public string Informativo { get; set; }
        public string Mensagem { get; set; }
        public string CodigoMensagem { get; set; }
        public int NumDiasNegativacaoAuto { get; set; }
    }

    #endregion
}
