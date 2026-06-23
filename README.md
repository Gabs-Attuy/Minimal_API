# Minimal API - Sistema de Gerenciamento de Veículos

Projeto desenvolvido como desafio prático do Bootcamp Avanade - Back-end com .NET e IA, utilizando ASP.NET 8 Minimal API, Entity Framework Core e autenticação JWT.

O projeto foi estruturado com foco em boas práticas de arquitetura, segurança, testes automatizados e deploy em ambiente cloud (AWS EC2).

## Deploy

A aplicação está hospedada em uma instância EC2 na AWS com IP público fixo (Elastic IP).

* Swagger: http://3.219.27.134/swagger
* API REST disponível publicamente

## Funcionalidades

### Administradores

* Autenticação via JWT
* Cadastro de administradores
* Consulta de administradores
* Controle de acesso por perfil (Adm e Editor)

### Veículos

* Cadastro de veículos
* Consulta de veículos
* Atualização de veículos
* Exclusão de veículos

## Tecnologias Utilizadas

* .NET 8
* ASP.NET Minimal API
* Entity Framework Core
* MySQL
* JWT (JSON Web Token)
* MSTest
* Swagger/OpenAPI

## Segurança

As senhas dos administradores são armazenadas utilizando hash através do PasswordHasher do ASP.NET Identity, evitando o armazenamento de senhas em texto puro.

A API utiliza autenticação JWT e autorização baseada em perfis.

## Estrutura de Perfis

### Administrador (Adm)

Possui acesso completo ao sistema.

### Editor

Possui acesso de consulta e cadastro de veículos, mas não pode realizar operações administrativas.

## Usuário Inicial

Ao executar as migrations, um usuário administrador é criado automaticamente:

Email:

administrador@minimalapi.com

Senha:

123456

## Configuração do Banco de Dados

Configurar a string de conexão no arquivo appsettings.json:

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Database=minimal_api;Uid=root;Pwd=root;"
  }
}
```

## Executando o Projeto

### Restaurar dependências

```bash
dotnet restore
```

### Aplicar migrations

```bash
dotnet ef database update
```

### Executar a aplicação

```bash
dotnet run
```

## Swagger

Após iniciar a aplicação, acessar:

http://localhost:5201/swagger

## Executando os Testes

```bash
dotnet test
```

## Testes Implementados

### Entidades

* Testes de propriedades da entidade Administrador e Veiculo

### Serviços

* Inclusão de administradores e veiculos
* Buscas por ID
* Paginação de resultados
* Atualização de veiculos
* Exclusão de veiculos
* Verificação de hash de senha

### Requisições HTTP

* Login com sucesso
* Login inválido
* Acesso sem token
* Acesso com token válido
* Validação de permissões por perfil
* Validação de BadRequest para quebra da regra de negócio

## Melhorias Implementadas Além do Desafio

* Hash de senhas utilizando ASP.NET Identity
* Cobertura de testes automatizados para Administrador e Veiculo
* Documentação Swagger
* Seed inicial de administrador com hash da senha
* Correção de bug na paginação de resultados

## Autor

Gabriel Attuy