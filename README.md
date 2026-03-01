# Fiap.Mechanics.Auth

Repositório do projeto destinado à geração de tokens JWT para o ecossistema da Oficina Mecânica da FIAP.

Este projeto consulta as tabelas do projeto **Fiap.Mechanics**, portanto, é necessário executar primeiro para rodar as migrations e garantir a estrutura do banco de dados.

## Definição do ambiente

- SDK: .NET 8.0
- Banco de dados: MSSQL (via [Fiap.Mechanics](https://github.com/FIAP-POS-TECH-13SOAT-MECHANICS/Mechanics-13soat))
- Provedor de Segredos: AWS Secrets Manager

## Pré-requisitos

Para rodar o projeto localmente, é mandatório estar logado e configurado no AWS CLI para que a aplicação consiga recuperar as chaves de assinatura do JWT:

```bash
aws configure
```

## Execução do projeto

Certifique-se de que o banco de dados do projeto `Fiap.Mechanics` está ativo e com as migrations aplicadas.

Inicie o projeto via CLI:

```bash
dotnet run --project src/Mechanics.Auth.Api
```

Após o processo concluir, o projeto estará disponível nas seguintes URLs:

http://localhost:5001
