using BoletoNetCore;
using BoletoNetCore.Cobrancas;
using BoletoNetCore.Enums;
using BoletoNetCore.LinkPagamento;
using BoletoNetCore.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoletoNetCore.WebAPI.Controllers
{
    /// <summary>
    /// Geração de links de pagamento (checkout).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LinkPagamentosController : ControllerBase
    {
        private readonly MetodosUteis _metodosUteis = new();

        /// <summary>
        /// Gera um link de pagamento no banco informado.
        /// </summary>
        /// <remarks>
        /// ## Tipo de banco emissor
        /// - Itau = 341
        /// - Asaas = 461
        /// - Sicredi = 748
        /// </remarks>
        [ProducesResponseType(typeof(LinkPagamentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("GerarLinkPagamento")]
        public async Task<IActionResult> GerarLinkPagamento(
            [FromBody] LinkPagamentoRequest linkPagamento,
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi)
        {
            try
            {
                if (linkPagamento == null)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Dados do link de pagamento não informados.",
                        "/api/LinkPagamentos/GerarLinkPagamento");
                    return BadRequest(err);
                }

                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        "/api/LinkPagamentos/GerarLinkPagamento");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta geração de link de pagamento.",
                        "/api/LinkPagamentos/GerarLinkPagamento");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var request = new GerarCobrancaRequest { LinkPagamento = linkPagamento };
                var retorno = await rest.GerarCobrancaPorTipo(TipoCobranca.LINK, request);

                return Ok(retorno);
            }
            catch (ArgumentException ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.BadRequest,
                    "Requisição inválida",
                    ex.Message,
                    "/api/LinkPagamentos/GerarLinkPagamento");
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
    }
}
