# Seeds

Arquivos JSON para popular o banco de dados DynamoDB.

## Como utilizar

Cada arquivo representa uma tabela. O nome do arquivo corresponde ao nome base da tabela, sem o prefixo de serviço e
ambiente. Por exemplo, `users.json` popula a tabela `auth-dev-users` no ambiente `dev`.

A ordem de processamento não é determinística, pois depende da ordem de leitura do sistema de arquivos.

Os arquivos são utilizados em três contextos:

- inicialização do LocalStack via [scripts de setup local](../scripts/localstack)
- deploy local via [script Powershell](../scripts/deploy-function.ps1)
- pipeline de deploy

Arquivos com sufixo `-test` (ex: `users-test.json`) são ignorados pela pipeline de deploy e são voltados ao ambiente de
testes ou desenvolvimento.
