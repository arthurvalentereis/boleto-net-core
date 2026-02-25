using BoletoNetCore;
using BoletoNetCore.Clientes;
using BoletoNetCore.WebAPI.Extensions;
using BoletoNetCore.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoletoNetCore.WebAPI.Controllers
{
    /// <summary>
    /// Listagem de clientes (multi-banco).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly MetodosUteis _metodosUteis = new MetodosUteis();

        /// <summary>
        /// Lista clientes do banco informado (paginação e filtros opcionais).
        /// </summary>
        /// <remarks>
        /// ## Tipo de banco emissor
        /// - Asaas = 461 (implementado; retorna clientes da API Asaas - GET /v3/customers)
        /// - Itaú = 341, Sicredi = 748 (retornam lista vazia)
        ///
        /// ## Filtros (query string, todos opcionais)
        /// - offset: elemento inicial da lista
        /// - limit: número de elementos por página (máx. 100)
        /// - name: filtrar por nome
        /// - email: filtrar por e-mail
        /// - cpfCnpj: filtrar por CPF ou CNPJ
        /// - groupName: filtrar por grupo
        /// - externalReference: filtrar pelo identificador do seu sistema
        /// </remarks>
        /// <param name="tipoBancoEmissor">Código do banco (ex.: 461 para Asaas).</param>
        /// <param name="chaveApi">Chave de API do banco (obrigatória).</param>
        /// <param name="offset">Elemento inicial da lista (filtro opcional).</param>
        /// <param name="limit">Número de elementos (máx. 100, filtro opcional).</param>
        /// <param name="name">Filtrar por nome (filtro opcional).</param>
        /// <param name="email">Filtrar por e-mail (filtro opcional).</param>
        /// <param name="cpfCnpj">Filtrar por CPF ou CNPJ (filtro opcional).</param>
        /// <param name="groupName">Filtrar por grupo (filtro opcional).</param>
        /// <param name="externalReference">Filtrar pelo identificador do seu sistema (filtro opcional).</param>
        [HttpGet]
        [ProducesResponseType(typeof(ListaClientesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] int tipoBancoEmissor,
            [FromQuery] string chaveApi,
            [FromQuery] int? offset = null,
            [FromQuery] int? limit = null,
            [FromQuery] string? name = null,
            [FromQuery] string? email = null,
            [FromQuery] string? cpfCnpj = null,
            [FromQuery] string? groupName = null,
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
                        "/api/Clientes");
                    return BadRequest(err);
                }

                if (limit.HasValue && limit.Value > 100)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "O parâmetro limit não pode ser maior que 100.",
                        "/api/Clientes");
                    return BadRequest(err);
                }

                var banco = Banco.Instancia(_metodosUteis.RetornarBancoEmissor(tipoBancoEmissor));
                if (banco is not IBancoOnlineRest rest)
                {
                    var err = _metodosUteis.RetornarErroPersonalizado(
                        (int)HttpStatusCode.BadRequest,
                        "Requisição inválida",
                        "Banco não suporta listagem de clientes.",
                        "/api/Clientes");
                    return BadRequest(err);
                }

                rest.ChaveApi = chaveApi;
                await rest.GerarToken();

                var filtros = new ListaClientesFiltros
                {
                    Offset = offset,
                    Limit = limit,
                    Name = name,
                    Email = email,
                    CpfCnpj = cpfCnpj,
                    GroupName = groupName,
                    ExternalReference = externalReference
                };

                var response = await rest.ListarClientes(filtros);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var retorno = _metodosUteis.RetornarErroPersonalizado(
                    (int)HttpStatusCode.BadRequest,
                    "Requisição inválida",
                    ex.Message,
                    "/api/Clientes");
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
