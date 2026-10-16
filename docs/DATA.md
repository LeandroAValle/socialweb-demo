# Dicionário de dados e indicadores

Todos os registros são fictícios. IDs `FAM-`, `ATD-` e `BEN-` são gerados apenas para navegação. Os territórios Aurora, Horizonte e Vale são nomes de demonstração. Não há nomes, CPF, endereço, telefone ou outros identificadores de pessoas reais.

## Entidades

| Entidade | Campos principais | Relação |
| --- | --- | --- |
| Família | ID, território, número de integrantes, faixa de renda, completude e acompanhamento | Raiz do conjunto |
| Atendimento | ID, ID da família, mês, tipo, status | Muitos por família |
| Benefício | ID, ID da família, tipo, status | Zero ou um por família nesta amostra |

## Métricas

| Indicador | Definição |
| --- | --- |
| Famílias cadastradas | Contagem de famílias do território |
| Pessoas vinculadas | Soma dos integrantes dessas famílias |
| Atendimentos em aberto | Atendimentos vinculados com status `Em aberto` |
| Benefícios ativos | Benefícios vinculados com status `Ativo` |
| Qualidade do cadastro | `100 × famílias completas ÷ famílias`, arredondado a uma casa decimal |
| Atendimentos por mês | Contagem de atendimentos do mês, independente do status |

Período ilustrativo: abril a setembro de 2026. Uma mudança de território atualiza todas as métricas e a tabela. Busca e filtro de status afetam a lista de famílias; os indicadores continuam sendo os totais do território selecionado.
