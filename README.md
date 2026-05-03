# SupportTickets

API REST para controle de chamados internos, desenvolvida em ASP.NET Core 8 com PostgreSQL, autenticação JWT e arquitetura em camadas.

[![Deploy with Vercel](https://githubusercontent.com)](https://vercel.app)
![.NET](https://shields.io)
![PostgreSQL](https://shields.io)
![Render](https://shields.io)

---

## 🚀 Demonstração - Sistema em produção
**Acesse agora:** [https://support-tickets-frontend.vercel.app](https://support-tickets-frontend.vercel.app)
*(Nota: Por estar no plano gratuito, a API pode levar cerca de 50s para "acordar" no primeiro acesso).*

Acessar como usuário:
Login: usuariodemo@email.com
Senha: usuariodemo123

Acessar como atendente:
Login: atendentedemo@email.com
Senha: atendentedemo123

Acessar como administrador:
Login: admin@supporttickets.local
Senha: Admin@123

## Sumario

- [Visao Geral](#visao-geral)
- [Funcionalidades](#funcionalidades)
- [Arquitetura](#arquitetura)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Tecnologias](#tecnologias)
- [Como Rodar o Projeto](#como-rodar-o-projeto)
- [Configuracao de Ambiente](#configuracao-de-ambiente)
- [Como Testar com Swagger](#como-testar-com-swagger)
- [Usuarios Seed](#usuarios-seed)
- [Principais Endpoints](#principais-endpoints)
- [Testes](#testes)
- [Regras de Negocio Importantes](#regras-de-negocio-importantes)
- [Decisoes Tecnicas](#decisoes-tecnicas)
- [Melhorias Futuras](#melhorias-futuras)

## Visao Geral

O SupportTickets foi criado para ser um sistema real de controle de atendimentos internos, com foco em:

- abertura e acompanhamento de chamados;
- controle de status e historico;
- SLA por prioridade;
- autenticação e autorização por perfil;


## Funcionalidades

- cadastro de setores;
- cadastro de prioridades com tempo estimado em horas;
- cadastro de usuarios com perfis `Admin`, `Atendente` e `Usuario`;
- autenticacao com JWT;
- abertura de chamados por usuarios autenticados;
- inicio de atendimento por `Admin` e `Atendente`;
- finalizacao de atendimento com registro de solucao;
- cancelamento de chamados respeitando as regras de fluxo;
- listagem de chamados com setor, prioridade, status, tempo total e indicacao de atraso de SLA;
- historico de status para auditoria.

## Arquitetura

O projeto segue uma arquitetura em camadas:

- `Domain`: entidades, enums e regras centrais do negocio.
- `Application`: DTOs, validações, interfaces e servicos de caso de uso.
- `Infrastructure`: persistencia com EF Core, PostgreSQL, autenticação e repositórios.
- `Api`: controllers, middleware global, Swagger, autenticação e exposição HTTP.

Essa separacao ajuda a:

- evitar regra de negócio espalhada em controller;
- facilitar manutencao;
- deixar o projeto mais legível para avaliação técnica;
- permitir evolução sem reescrever tudo.

## Estrutura do Projeto

```text
SupportTickets
├─ src
│  ├─ Api
│  ├─ Application
│  ├─ Domain
│  └─ Infrastructure
├─ database
├─ docker
├─ tests
├─ .env.example
├─ .gitignore
├─ NuGet.Config
├─ README.md
└─ SupportTickets.sln
```

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- FluentValidation
- Swagger / OpenAPI
- xUnit
- Docker / Docker Compose

## Como Rodar o Projeto

Existem duas formas principais:

1. com Docker;
2. localmente com .NET + PostgreSQL instalados na maquina.

### Opcao 1 - Rodar com Docker

Requisitos:

- Docker Desktop instalado e em execucao.

Passos:

1. Extraia ou clone o projeto.
2. Na raiz do projeto, crie um `.env` a partir do `.env.example`.
3. Rode:

```bash
docker compose --env-file .env -f docker/docker-compose.yml up --build
```

Depois disso, a aplicacao deve ficar disponivel em:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- PostgreSQL: `localhost:5432`

Para parar os containers:

```bash
docker compose -f docker/docker-compose.yml down
```

Para remover tambem o volume do banco e recriar tudo do zero:

```bash
docker compose -f docker/docker-compose.yml down -v
docker compose -f docker/docker-compose.yml up --build
```

### Opcao 2 - Rodar localmente com .NET

Requisitos:

- .NET 8 SDK
- PostgreSQL instalado e em execucao

Passos:

1. Extraia ou clone o projeto.
2. Crie o arquivo `.env` a partir do `.env.example`.
3. Ajuste no `.env` a connection string para usar `Host=localhost`.
4. Crie o banco `supporttickets` no PostgreSQL.
5. Execute os scripts:
   - `database/create.sql`
   - `database/seed.sql`
6. Restaure os pacotes:

```bash
dotnet restore SupportTickets.sln --configfile NuGet.Config
```

7. Rode a API:

```bash
dotnet run --project src/Api/Api.csproj
```

Ao iniciar, o terminal vai mostrar a URL da aplicacao. Em ambiente local de desenvolvimento ela costuma subir em algo como:

- `http://localhost:5124`
- Swagger: `http://localhost:5124/swagger`

## Configuracao de Ambiente

O projeto usa variaveis de ambiente para:

- conexao com banco;
- configuracao JWT;
- ambiente de execucao;
- URL da API.

Use o arquivo [.env.example](.env.example) como base para criar o `.env`.


### Variaveis mais importantes

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__SecretKey`
- `Jwt__ExpirationMinutes`

### Importante sobre a connection string

Se voce for rodar com Docker, deixe:

```env
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=supporttickets;Username=supporttickets;Password=SupportTickets@123
```

Se voce for rodar localmente com PostgreSQL na propria maquina, troque para:

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=supporttickets;Username=supporttickets;Password=SupportTickets@123
```

## Como Testar com Swagger

1. Suba a aplicacao.
2. Abra `/swagger` no navegador.
3. Teste primeiro o endpoint:

```http
POST /api/auth/login
```

Body:

```json
{
  "email": "admin@supporttickets.local",
  "senha": "Admin@123"
}
```

4. Copie o token retornado.
5. Clique em `Authorize` no topo da tela do Swagger.
6. Cole:

```text
Bearer SEU_TOKEN
```

7. Depois disso, teste os endpoints protegidos.

Fluxo recomendado de teste:

1. `POST /api/auth/login`
2. `GET /api/setores`
3. `GET /api/prioridades`
4. `POST /api/chamados`
5. `GET /api/chamados`
6. `POST /api/chamados/{id}/iniciar`
7. `POST /api/chamados/{id}/finalizar`
8. `GET /api/chamados/{id}`

## Usuarios Seed

Os scripts iniciais criam estes usuarios:

- `admin@supporttickets.local` / `Admin@123`
- `atendente@supporttickets.local` / `Atendente@123`
- `usuario@supporttickets.local` / `Usuario@123`

Esses usuarios existem para facilitar os testes logo apos subir o ambiente.

Obs: Ao testar no Swagger, digite os campos manualmente para evitar bad request. Copiar e colar pode inserir caracteres sujos no Post.

## Principais Endpoints

### Autenticacao

- `POST /api/auth/login`
- `POST /api/auth/register` - apenas `Admin`

### Setores

- `GET /api/setores`
- `GET /api/setores/{id}`
- `POST /api/setores` - apenas `Admin`
- `PUT /api/setores/{id}` - apenas `Admin`
- `DELETE /api/setores/{id}` - apenas `Admin`

### Prioridades

- `GET /api/prioridades`
- `GET /api/prioridades/{id}`
- `POST /api/prioridades` - apenas `Admin`
- `PUT /api/prioridades/{id}` - apenas `Admin`
- `DELETE /api/prioridades/{id}` - apenas `Admin`

### Chamados

- `GET /api/chamados`
- `GET /api/chamados/{id}`
- `POST /api/chamados`
- `POST /api/chamados/{id}/iniciar` - `Admin` e `Atendente`
- `POST /api/chamados/{id}/finalizar` - `Admin` e `Atendente`
- `POST /api/chamados/{id}/cancelar`
- `PUT /api/chamados/{id}/prioridade` - apenas `Admin`

### Filtros disponiveis em `GET /api/chamados`

- `status`
- `setorId`
- `prioridadeId`
- `apenasAtrasados`
- `ordenarPor`

Valores aceitos em `ordenarPor`:

- `criadoEm_asc`
- `criadoEm_desc`
- `prioridade_asc`
- `prioridade_desc`
- `status_asc`
- `status_desc`

## Testes

Os testes unitarios ficam em:

- [tests/Domain.Tests/ChamadoTests.cs](tests/Domain.Tests/ChamadoTests.cs)

Para executar os testes:

```bash
dotnet test SupportTickets.sln
```

Regras cobertas inicialmente:

- criacao de chamado com status inicial correto;
- bloqueio de reinicio de chamado finalizado;
- bloqueio de finalizacao sem atendimento iniciado;
- bloqueio de cancelamento de chamado finalizado.

## Regras de Negocio Importantes

- um chamado sempre nasce com status `Aberto`;
- nao existe chamado sem usuario, setor e prioridade validos;
- so e possivel iniciar chamado `Aberto`;
- um chamado pode ter apenas um atendimento;
- so e possivel finalizar chamado que foi iniciado;
- finalizacao exige solucao;
- nao e possivel cancelar chamado `Finalizado`;
- toda mudanca de status entra no historico;
- o SLA vem da prioridade;
- a API respeita autorizacao por perfil.

## Decisoes Tecnicas

- historico de status guarda o novo status, o usuario responsavel e a data da alteracao;
- o campo `status_anterior` foi deixado como possivel evolucao futura;
- a persistencia foi feita com EF Core, mas o banco tambem possui scripts SQL completos para facilitar avaliacao;
- o primeiro `Admin` entra via seed para simplificar o funcionamento do sistema;
- a API usa um formato padrao de resposta com `success`, `message`, `data` e `errors`.

## Arquivos Importantes

- [src/Api/Program.cs](src/Api/Program.cs)
- [src/Api/Controllers/AuthController.cs](src/Api/Controllers/AuthController.cs)
- [src/Application/Services/ChamadoService.cs](src/Application/Services/ChamadoService.cs)
- [src/Domain/Entities/Chamado.cs](src/Domain/Entities/Chamado.cs)
- [src/Infrastructure/Data/AppDbContext.cs](src/Infrastructure/Data/AppDbContext.cs)
- [database/create.sql](database/create.sql)
- [database/seed.sql](database/seed.sql)
- [docker/docker-compose.yml](docker/docker-compose.yml)
- [Desenvolvimento.md](Desenvolvimento.md)

## Melhorias Futuras

- testes de integracao para endpoints criticos;
- refresh token;
- paginacao na listagem de chamados;
- logs estruturados;
- observabilidade com correlation id;
- migrations automatizadas com EF Core, se o projeto evoluir;
- pipeline CI para build e testes.
