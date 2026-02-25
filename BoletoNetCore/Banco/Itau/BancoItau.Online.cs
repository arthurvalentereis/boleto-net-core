using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoletoNetCore.CartãoDeCredito;
using BoletoNetCore.Clientes;
using BoletoNetCore.Cobrancas;
using BoletoNetCore.Exceptions;
using BoletoNetCore.LinkPagamento;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoItau : IBancoOnlineRest
    {
        public string ChaveApi { get; set; }
        public string Token { get; set; }

        public Task ConsultarStatus(Boleto boleto)
        {
            throw new NotImplementedException();
        }

       
        public Task<LinkPagamentoResponse> GerarLinkPagamento(LinkPagamentoRequest boleto)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentCreditCardResponse> GerarCobrancaCartao(RequestCobranca boleto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: Necessário verificar quais os métodos necessários
        /// </summary>
        /// <returns></returns>
        public Task<string> GerarToken()
        {
            throw new NotImplementedException();
        }

        public Task RegistrarBoleto(Boleto boleto)
        {
            throw new NotImplementedException();
        }

        public Task<BankSlip> GerarCobrancaBoleto(RequestCobranca boleto)
        {
            throw new NotImplementedException();
        }
        
        public Task<Pix> GerarPix(string idCobranca)
        {
            throw new NotImplementedException();
        }

        public Task<WebHookAssasResponse> AtualizarCobranca(WebHookAssasResponse request)
        {
            throw new NotImplementedException();
        }

        public Task<ListaCobrancasResponse> ListarCobrancas(ListaCobrancasFiltros filtros)
        {
            return Task.FromResult(new ListaCobrancasResponse
            {
                Data = new List<CobrancaItemResponse>(),
                TotalCount = 0,
                HasMore = false,
                Limit = filtros?.Limit ?? 10,
                Offset = filtros?.Offset ?? 0
            });
        }

        public Task<ListaClientesResponse> ListarClientes(ListaClientesFiltros filtros)
        {
            return Task.FromResult(new ListaClientesResponse
            {
                Data = new List<Customer>(),
                TotalCount = 0,
                HasMore = false,
                Limit = filtros?.Limit ?? 10,
                Offset = filtros?.Offset ?? 0
            });
        }
    }
}


