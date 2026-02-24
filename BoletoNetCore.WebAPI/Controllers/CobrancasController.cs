using BoletoNetCore;
using BoletoNetCore.Cobrancas;
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
        private readonly MetodosUteis _metodosUteis = new MetodosUteis();

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
    }
}
