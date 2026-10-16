# Arquitetura

## Fluxo de dados

`SnapshotFactory.Build()` gera famílias, atendimentos e benefícios fictícios com semente fixa. `Validate()` verifica IDs únicos, territórios válidos, tamanho de família e referências entre registros. As visões são agregadas por território e para o total. A API mantém um snapshot em memória; a exportação grava o mesmo objeto em `site/data/snapshot.json`.

O navegador lê o JSON estático. Filtros de território usam visões agregadas previamente; busca e status de cadastro filtram somente a tabela de famílias. Assim, os indicadores continuam representando todo o território selecionado, não apenas a página visível.

## Limites

- **Core:** modelo, geração, validação e indicadores. Não depende de HTTP ou navegador.
- **Api:** endpoints de consulta e exportação. Não persiste dados nem recebe alterações.
- **Site:** apresentação e interação. Nenhum segredo, autenticação ou chamada a serviço privado.
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
