# Cobranças – Contrato genérico (multi-banco)

Esta pasta contém **entidades normalizadas** para listagem de cobranças, usadas como linguagem única entre a API e os bancos.

## Estrutura

| Arquivo | Descrição |
|--------|-----------|
| **ListaCobrancasResponse** | Resposta genérica da listagem (HasMore, TotalCount, Limit, Offset, Data). |
| **CobrancaItemDto** | Item genérico de cobrança; cada banco mapeia seu retorno para este DTO. |
| **ListaCobrancasFiltros** | Filtros genéricos de entrada; cada banco traduz para os parâmetros da sua API. |

## Onde ficam os DTOs por banco

Os **request/response específicos de cada banco** ficam na pasta do banco (organização por banco), para deixar claro a qual integração pertencem:

- **Asaas:** pasta `BoletoNetCore/Banco/Asaas/Models/`
  - `AsaasPaymentListResponse` – resposta bruta de GET /v3/payments
  - `AsaasPaymentListItem` – item de cobrança do Asaas
- **Outros bancos:** ao implementar listagem, criar em `Banco/{Banco}/Models/` os DTOs específicos (ex.: `ItauPaymentListResponse`) e mapear para os tipos desta pasta.

## Fluxo

1. A API recebe filtros e chama `IBancoOnlineRest.ListarCobrancas(ListaCobrancasFiltros)`.
2. Cada banco monta o request da sua API (ex.: query string do Asaas), obtém o response específico (ex.: `AsaasPaymentListResponse`).
3. O banco mapeia para `ListaCobrancasResponse` e `CobrancaItemDto` e retorna para a API.
4. A API expõe apenas o contrato genérico (esta pasta).
