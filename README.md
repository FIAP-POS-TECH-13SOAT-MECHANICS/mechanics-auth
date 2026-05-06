# Fiap.Mechanics.Auth

Repositório do projeto destinado à geração de tokens JWT para o ecossistema da Oficina Mecânica da FIAP.

Este projeto consulta as tabelas do projeto **Fiap.Mechanics**, portanto, é necessário executar primeiro para rodar as migrations e garantir a estrutura do banco de dados.

## Definição do ambiente

- SDK: .NET 10.0
- Banco de dados: DynamoDB (via [Fiap.Mechanics](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/Mechanics-13soat))
- Provedor de Segredos: AWS Secrets Manager

## Pré-requisitos

Para rodar o projeto localmente, é mandatório estar logado e configurado no AWS CLI para que a aplicação consiga recuperar as chaves de assinatura do JWT:

```powershell
aws configure
```

Execute os scripts do [repositório de infraestrutura](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/mechanics-infra) e do [repositório de banco de dados](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/mechanics-database) para provisionar o ambiente antes de executar a aplicação.

O comando abaixo inicia a API na porta 5050. Utilize uma aplicação como [Postman](https://www.postman.com/downloads) para testar.

```powershell
dotnet run --project .\src\Mechanics.Auth.Api
```

## Script de deploy

Execute o script Powershell para fazer deploy da aplicação na AWS.
Certifique-se de antes ter provisionado o ambiente usando os scripts do [repositório de infraestrutura](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/mechanics-infra).

```powershell
.\scripts\deploy-function dev
```

## Acessar via API Gateway

Obtenha a URL da usando o comando abaixo. Adapte o nome de acordo o ambiente.

```powershell
aws apigatewayv2 get-apis --query "Items[?Name=='fiap-mechanics-dev-api'].ApiEndpoint" --output text
```

## Endpoints disponíveis

Para realizar login, utilize o endpoint `POST /auth/login`.

```shell
curl --location 'http://localhost:5050/auth/login' \
--header 'Content-Type: application/json' \
--data '{
    "cpfNumber": "12345678909",
    "password": "5eCre+Key"
}'
```

A resposta contém o token de acesso e o de atualização.
O token de acesso possui uma validade de poucos minutos e pode ser renovado utilizando o token de atualização.

```json
{
    "accessToken": "...",
    "refreshToken": "...",
    "expirationDate": "2025-11-02T00:39:54.1120047+00:00"
}
```

Para renovar o token de acesso, utilize o endpoint `POST /auth/refresh`.

```shell
curl --location 'http://localhost:5050/auth/refresh' \
--header 'accept: application/json' \
--header 'Content-Type: application/json' \
--data '{
  "refreshToken": "..."
}'
```

O token de atualização é válido por 12 horas e é cancelado quando o usuário altera a senha.

## Usuários padrão

Utilize o endpoint `/auth/login` para gerar um token.
O token possui validade de poucos minutos, mas pode ser renovado.

Os seguintes logins podem ser utilizados para testes:

| CPF           | Senha       | Perfil        | Permissões                    |
|---------------|-------------|---------------|-------------------------------|
| `12345678909` | `5eCre+Key` | Administrador | Acesso completo ao sistema    |
| `98765432100` | `5eCre+Key` | Atendente     | Cadastrar clientes e veículos |
| `11144477735` | `5eCre+Key` | Mecânico      | Gerenciar produtos e serviços |

Os dados acima estão disponíveis nos seeds do DynamoDB. Consulte [Seeds](./seeds/README.md) para mais detalhes.

Qualquer funcionário autenticado pode criar e atualizar ordens de serviço.
Para mais detalhes, consulte [Autenticação e autorização](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/Mechanics-13soat/blob/main/docs/auth.md).

## Token para serviços

Este endpoint é destinado à comunicação entre microserviços.
Diferente do login de usuários, ele gera um token com a Role `SERVICE`, que permite que um serviço se identifique para
outro dentro do ecossistema.

```shell
curl --location 'http://localhost:5050/auth/service-token' \
--header 'Content-Type: application/json' \
--data '{
    "serviceName": "Mechanics.Orders"
}'
```

O token retornado possui uma validade curta e não gera um `refreshToken`.
Para mais informações, consulte [Integração entre microsserviços](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/Mechanics-13soat/blob/main/docs/integration.md).

## Diagrama desse projeto

![Infraestructure](./images/lambda.png)
