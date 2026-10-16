# Arquitetura

## Fluxo de dados

`SnapshotFactory.Build()` gera famílias, atendimentos e benefícios fictícios com semente fixa. `Validate()` verifica IDs únicos, territórios válidos, tamanho de família e referências entre registros. As visões são agregadas por território e para o total. A API mantém um snapshot em memória; a exportação grava o JSON localmente em `artifacts/snapshot.json` para validação técnica. Ele não é incluído na página pública.

O site público é uma página estática de portfólio com uma captura do dashboard original e um link para o sistema oficial. Não carrega a API ou o dataset sintético.

## Limites

- **Core:** modelo, geração, validação e indicadores. Não depende de HTTP ou navegador.
- **Api:** endpoints de consulta e exportação. Não persiste dados nem recebe alterações.
- **Site:** captura e contexto do projeto. Nenhum segredo, autenticação ou chamada a serviço privado.
- **Tests:** contratos de determinismo, integridade e reconciliação.

## Endpoints

| Método | Rota | Resposta |
| --- | --- | --- |
| GET | `/health` | Saúde e origem sintética |
| GET | `/api/v1/context` | Versão, período, territórios e avisos |
| GET | `/api/v1/overview` | Indicadores e série mensal; `territory` opcional |
| GET | `/api/v1/households` | Famílias; `territory` opcional |
| GET | `/api/v1/households/{id}` | Família, atendimentos e benefícios associados |

Território desconhecido retorna HTTP 400; família inexistente retorna HTTP 404. Não há endpoints de escrita.
