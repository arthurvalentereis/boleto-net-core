using BoletoNetCore;
using BoletoNetCore.Assinaturas;
using BoletoNetCore.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoletoNetCore.WebAPI.Controllers
{
    /// <summary>
    /// Assinaturas recorrentes (multi-banco).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssinaturasController : ControllerBase
    {
        private readonly MetodosUteis _metodosUteis = new();
        private readonly IConfiguration _configuration;

        public AssinaturasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Cria assinatura recorrente no banco informado.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AssinaturaItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarAssinatura(
            [FromBody] CriarAssinaturaRequest request,
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi)
        {
            try
            {
                if (request == null)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Dados da assinatura não informados.",
                        "/api/Assinaturas");
                    return BadRequest(err);
                }

                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        "/api/Assinaturas");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta assinaturas.",
                        "/api/Assinaturas");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var retorno = await rest.CriarAssinatura(request);
                return Ok(retorno);
            }
            catch (ArgumentException ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.BadRequest,
                    "Requisição inválida",
                    ex.Message,
                    "/api/Assinaturas");
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
        /// Lista assinaturas do banco informado.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ListaAssinaturasResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarAssinaturas(
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi,
            [FromQuery] int? offset = null,
            [FromQuery] int? limit = null,
            [FromQuery] string? customer = null,
            [FromQuery] string? status = null,
            [FromQuery] string? externalReference = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        "/api/Assinaturas");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta assinaturas.",
                        "/api/Assinaturas");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var filtros = new ListaAssinaturasFiltros
                {
                    Offset = offset,
                    Limit = limit,
                    Customer = customer,
                    Status = status,
                    ExternalReference = externalReference,
                };

                var retorno = await rest.ListarAssinaturas(filtros);
                return Ok(retorno);
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
        /// Obtém assinatura por id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssinaturaItemResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterAssinatura(
            string id,
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        $"/api/Assinaturas/{id}");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta assinaturas.",
                        $"/api/Assinaturas/{id}");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var retorno = await rest.ObterAssinatura(id);
                return Ok(retorno);
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
        /// Cancela assinatura por id.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CancelarAssinatura(
            string id,
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveApi))
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Chave da API não informada.",
                        $"/api/Assinaturas/{id}");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta assinaturas.",
                        $"/api/Assinaturas/{id}");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                await rest.CancelarAssinatura(id);
                return NoContent();
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
