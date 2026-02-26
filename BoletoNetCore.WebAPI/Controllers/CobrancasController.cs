using BoletoNetCore;
using BoletoNetCore.Cobrancas;
using BoletoNetCore.Enums;
using BoletoNetCore.WebAPI.Extensions;
using BoletoNetCore.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoletoNetCore.WebAPI.Controllers
{
    /// <summary>
    /// Listagem de cobranças/pagamentos (multi-banco).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CobrancasController : ControllerBase
    {
        MetodosUteis _metodosUteis = new MetodosUteis();

        public CobrancasController()
        {

        }
        /// <summary>
        /// Lista cobranças do banco informado (paginação e filtros opcionais).
        /// </summary>
        /// <remarks>
        /// ## Tipo de banco emissor
        /// - Asaas = 461 (implementado; retorna cobranças da API Asaas)
        /// - Itaú = 341, Sicredi = 748 (retornam lista vazia)
        ///
        /// ## Filtros (query string, todos opcionais)
        /// - offset, limit (máx. 100)
        /// - customer, billingType (BOLETO, CREDIT_CARD, PIX), status (PENDING, RECEIVED, etc.)
        /// - dueDateGe, dueDateLe (datas formato yyyy-MM-dd)
        /// - dateCreatedGe, dateCreatedLe, paymentDateGe, paymentDateLe
        /// - externalReference, subscription, installment, user, checkoutSession, pixQrCodeId
        /// - anticipated, anticipable (true/false)
        /// </remarks>
        /// <param name="tipoBancoEmissor">Código do banco (ex.: 461 para Asaas).</param>
        /// <param name="chaveApi">Chave de API do banco (obrigatória).</param>
        /// <param name="offset">Elemento inicial da lista (filtro opcional).</param>
        /// <param name="limit">Número de elementos (máx. 100, filtro opcional).</param>
        /// <param name="customer">Id do cliente (filtro opcional).</param>
        /// <param name="customerGroupName">Nome do grupo de cliente (filtro opcional).</param>
        /// <param name="billingType">Forma de pagamento: BOLETO, CREDIT_CARD, PIX (filtro opcional).</param>
        /// <param name="status">Status da cobrança (filtro opcional).</param>
        /// <param name="dueDateGe">Data de vencimento inicial yyyy-MM-dd (filtro opcional).</param>
        /// <param name="dueDateLe">Data de vencimento final yyyy-MM-dd (filtro opcional).</param>
        /// <param name="subscription">Id da assinatura (filtro opcional).</param>
        /// <param name="installment">Id do parcelamento (filtro opcional).</param>
        /// <param name="externalReference">Referência externa (filtro opcional).</param>
        /// <param name="paymentDate">Data de pagamento (filtro opcional).</param>
        /// <param name="invoiceStatus">Status da nota fiscal (filtro opcional).</param>
        /// <param name="estimatedCreditDate">Data estimada de crédito (filtro opcional).</param>
        /// <param name="anticipated">Cobranças antecipadas (filtro opcional).</param>
        /// <param name="anticipable">Cobranças antecipáveis (filtro opcional).</param>
        /// <param name="dateCreatedGe">Data de criação inicial (filtro opcional).</param>
        /// <param name="dateCreatedLe">Data de criação final (filtro opcional).</param>
        /// <param name="paymentDateGe">Data de recebimento inicial (filtro opcional).</param>
        /// <param name="paymentDateLe">Data de recebimento final (filtro opcional).</param>
        /// <param name="estimatedCreditDateGe">Data estimada de crédito inicial (filtro opcional).</param>
        /// <param name="estimatedCreditDateLe">Data estimada de crédito final (filtro opcional).</param>
        /// <param name="user">E-mail do usuário que criou a cobrança (filtro opcional).</param>
        /// <param name="checkoutSession">Id da checkout (filtro opcional).</param>
        /// <param name="pixQrCodeId">Id do QrCode PIX (filtro opcional).</param>
        [HttpGet()]
        [ProducesResponseType(typeof(ListaCobrancasResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPayments(
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi,
            [FromQuery] int? offset = null,
            [FromQuery] int? limit = null,
            [FromQuery] string? customer = null,
            [FromQuery] string? customerGroupName = null,
            [FromQuery] string? billingType = null,
            [FromQuery] string? status = null,
            [FromQuery] string? subscription = null,
            [FromQuery] string? installment = null,
            [FromQuery] string? externalReference = null,
            [FromQuery] string? paymentDate = null,
            [FromQuery] string? invoiceStatus = null,
            [FromQuery] string? estimatedCreditDate = null,
            [FromQuery] bool? anticipated = null,
            [FromQuery] bool? anticipable = null,
            [FromQuery] string? dateCreatedGe = null,
            [FromQuery] string? dateCreatedLe = null,
            [FromQuery] string? paymentDateGe = null,
            [FromQuery] string? paymentDateLe = null,
            [FromQuery] string? estimatedCreditDateGe = null,
            [FromQuery] string? estimatedCreditDateLe = null,
            [FromQuery] string? dueDateGe = null,
            [FromQuery] string? dueDateLe = null,
            [FromQuery] string? user = null,
            [FromQuery] string? checkoutSession = null,
            [FromQuery] string? pixQrCodeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        "/api/Payments");
                    return BadRequest(err);
                }

                if (limit.HasValue && limit.Value > 100)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "O parâmetro limit não pode ser maior que 100.",
                        "/api/Payments");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta listagem de cobranças.",
                        "/api/Payments");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var filtros = new ListaCobrancasFiltros
                {
                    Offset = offset,
                    Limit = limit,
                    Customer = customer,
                    CustomerGroupName = customerGroupName,
                    BillingType = billingType,
                    Status = status,
                    Subscription = subscription,
                    Installment = installment,
                    ExternalReference = externalReference,
                    PaymentDate = paymentDate,
                    InvoiceStatus = invoiceStatus,
                    EstimatedCreditDate = estimatedCreditDate,
                    Anticipated = anticipated,
                    Anticipable = anticipable,
                    DateCreatedGe = dateCreatedGe,
                    DateCreatedLe = dateCreatedLe,
                    PaymentDateGe = paymentDateGe,
                    PaymentDateLe = paymentDateLe,
                    EstimatedCreditDateGe = estimatedCreditDateGe,
                    EstimatedCreditDateLe = estimatedCreditDateLe,
                    DueDateGe = dueDateGe,
                    DueDateLe = dueDateLe,
                    User = user,
                    CheckoutSession = checkoutSession,
                    PixQrCodeId = pixQrCodeId
                };

                var response = await rest.ListarCobrancas(filtros);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.BadRequest,
                    "Requisição inválida",
                    ex.Message,
                    "/api/Payments");
                return BadRequest(retorno);
            }
            catch (Exception ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.InternalServerError,
                    "Erro interno",
                    $"Detalhe do erro: {ex.Message}",
                    string.Empty);
                return StatusCode(StatusCodes.Status500InternalServerError, retorno);
            }
        }

        /// <summary>
        /// Gera cobrança via API do banco (cartão de crédito ou boleto).
        /// Para link de pagamento, use o endpoint POST /api/LinkPagamentos/GerarLinkPagamento.
        /// </summary>
        /// <remarks>
        /// ## Tipo de banco emissor
        /// - Itau = 341
        /// - Asaas = 461
        /// - Sicredi = 748
        /// ## Tipo Cobrança
        /// - CREDIT_CARD = Cartão de crédito
        /// - BOLETO_PIX = Boleto via API (inclui PIX)
        /// - BOLETO = Boleto
        /// </remarks>
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("GerarCobranca")]
        public async Task<IActionResult> GerarCobranca(
            [FromBody] RequestGerarCobranca request,
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string tipoCobranca,
            [FromQuery] string chaveApi)
        {
            try
            {
                if (request?.RequestCobranca == null)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "RequestCobranca não informado.",
                        "/api/Cobrancas/GerarCobranca");
                    return BadRequest(err);
                }

                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        "/api/Cobrancas/GerarCobranca");
                    return BadRequest(err);
                }

                if (!Enum.TryParse<TipoCobranca>(tipoCobranca, ignoreCase: true, out var tipo) ||
                    (tipo != TipoCobranca.CREDIT_CARD && tipo != TipoCobranca.BOLETO_PIX))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Tipo de cobrança inválido. Use CREDIT_CARD, BOLETO ou BOLETO_PIX. Para link de pagamento, use /api/LinkPagamentos/GerarLinkPagamento.",
                        "/api/Cobrancas/GerarCobranca");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta geração de cobrança via API.",
                        "/api/Cobrancas/GerarCobranca");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var requestCobranca = new GerarCobrancaRequest { RequestCobranca = request.RequestCobranca };
                var retorno = await rest.GerarCobrancaPorTipo(tipo, requestCobranca);

                return Ok(retorno);
            }
            catch (ArgumentException ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.BadRequest,
                    "Requisição inválida",
                    ex.Message,
                    "/api/Cobrancas/GerarCobranca");
                return BadRequest(retorno);
            }
            catch (Exception ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.InternalServerError,
                    "Erro interno",
                    $"Detalhe do erro: {ex.Message}",
                    string.Empty);
                return StatusCode(StatusCodes.Status500InternalServerError, retorno);
            }
        }

        /// <summary>
        /// Endpoint para Atualizar boleto via webservice no banco.
        /// </summary>
        /// <remarks>
        /// ## Tipo de banco emissor
        /// O tipo de banco e chave API deve ser informado dentro do parâmetro para que nossa API possa identificar de que banco se trata
        /// - Itau = 341
        /// - Asaas = 461
        /// - Sicredi = 748
        /// </remarks>
        /// <returns>Retornar o HTML do boleto.</returns>
        [ProducesResponseType(typeof(DadosBoleto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPut("AtualizarCobranca")]
        public async Task<IActionResult> AtualizarCobranca(WebHookAssasResponse dadosBoleto, int tipoBancoEmissor, string chaveApi)
        {
            try
            {
                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                IBancoOnlineRest b = (IBancoOnlineRest)banco;
                b.ChaveApi = chaveApi;
                b.GerarToken();
                return Ok(b.AtualizarCobranca(dadosBoleto));
            }
            catch (Exception ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado((int)HttpStatusCode.InternalServerError, "Requisição Inválida", $"Detalhe do erro: {ex.Message}", string.Empty);
                return StatusCode(StatusCodes.Status400BadRequest, retorno);
            }
        }

    }
}
