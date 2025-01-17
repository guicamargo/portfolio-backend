### **To-Do List para o Backend**
---

#### **1. Configuração Inicial do Projeto**
- [ ] Criar um novo projeto ASP.NET Core:  
  ```bash
  dotnet new webapi -n MLPortfolioBackend
  ```  
- [ ] Configurar a estrutura inicial de pastas no padrão DDD:  
  ```
  MLPortfolioBackend/
  ├── Application/         # Camada de aplicação (DTOs, interfaces de serviços, validações)
  ├── Domain/              # Camada de domínio (entidades, contratos, lógica de negócio)
  ├── Infrastructure/      # Camada de infraestrutura (acesso a dados, integração com serviços externos)
  ├── API/                 # Camada de apresentação (controllers, endpoints)
  └── Tests/               # Testes unitários e de integração
  ```  

---

#### **2. Configuração do Banco de Dados**
- [ ] Adicionar suporte ao PostgreSQL:  
  ```bash
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
  ```
- [ ] Configurar a string de conexão no `appsettings.json`:  
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mlapp;Username=admin;Password=password"
  }
  ```
- [ ] Criar o `DbContext` na camada **Infrastructure** e registrar no container de dependências.  
- [ ] Implementar migrations e aplicar ao banco:  
  ```bash
  dotnet ef migrations add InitialMigration
  dotnet ef database update
  ```  

---

#### **3. Autenticação e Autorização**
- **Autenticação com JWT**  
  - [ ] Adicionar suporte a JWT:  
    ```bash
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    ```
  - [ ] Configurar middleware de autenticação e autorização.  
  - [ ] Criar endpoints para registro e login.  
  - [ ] Gerar e validar tokens JWT.  

- **Autenticação com Google OAuth**  
  - [ ] Configurar autenticação externa com Google:  
    ```bash
    dotnet add package Microsoft.AspNetCore.Authentication.Google
    ```  
  - [ ] Criar endpoints para login com Google e integração com JWT.  

---

#### **4. Padrões de Projeto**
- [ ] Implementar injeção de dependências para todos os serviços, repositórios e validadores.  
- [ ] Criar repositórios base para abstrair o acesso a dados.  
- [ ] Utilizar o padrão **Factory** para criar objetos complexos.  
- [ ] Seguir princípios do SOLID em toda a aplicação.  

---

#### **5. Criação dos Endpoints**
- [ ] **Autenticação**  
  - [ ] Endpoint para login interno.  
  - [ ] Endpoint para login via Google OAuth.  
  - [ ] Endpoint para refresh de token.  

- [ ] **Usuários**  
  - [ ] Endpoint para criar/atualizar informações do usuário.  
  - [ ] Endpoint para listar usuários (apenas para admins).  

- [ ] **Integração com Serviço de ML**  
  - [ ] Endpoint para enviar dados de entrada para predição.  
  - [ ] Receber resposta do serviço Flask e retornar ao cliente.  

- [ ] **Cursos e Certificados**  
  - [ ] Endpoint para adicionar/editar certificados.  
  - [ ] Endpoint para listar cursos/certificados do usuário.  

---

#### **6. Integração com o Serviço de ML**
- [ ] Criar um cliente HTTP dedicado para se comunicar com o serviço de ML.  
- [ ] Configurar retry policy com **Polly** para lidar com falhas.  
  ```bash
  dotnet add package Polly
  ```  
- [ ] Garantir validação de dados de entrada e saída antes de enviar/receber do serviço Flask.  

---

#### **7. Testes**
- [ ] Escrever testes unitários para lógica de negócio na camada **Domain**.  
- [ ] Escrever testes de integração para endpoints utilizando **xUnit** ou **NUnit**:  
  ```bash
  dotnet add package xunit
  ```  

---

#### **8. Documentação**
- [ ] Adicionar documentação com Swagger:  
  ```bash
  dotnet add package Swashbuckle.AspNetCore
  ```
- [ ] Configurar rotas documentadas para cada endpoint.  
- [ ] Adicionar exemplos de entrada e saída para cada endpoint.  

---

#### **9. Orquestração e Deploy**
- [ ] Criar um arquivo `Dockerfile` para o backend:  
  ```dockerfile
  FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
  WORKDIR /app
  EXPOSE 5001

  FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
  WORKDIR /src
  COPY . .
  RUN dotnet restore "MLPortfolioBackend.csproj"
  RUN dotnet build "MLPortfolioBackend.csproj" -c Release -o /app/build

  FROM build AS publish
  RUN dotnet publish "MLPortfolioBackend.csproj" -c Release -o /app/publish

  FROM base AS final
  WORKDIR /app
  COPY --from=publish /app/publish .
  ENTRYPOINT ["dotnet", "MLPortfolioBackend.dll"]
  ```
- [ ] Configurar o backend no `docker-compose.yml` para rodar junto com o banco e o serviço de ML:  
  ```yaml
  backend:
    build: ./backend
    ports:
      - "5001:5001"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=mlapp;Username=admin;Password=password
  ```
- [ ] Deploy para serviços como **Render** ou **Railway**, configurando variáveis de ambiente.  

---

#### **10. Melhorias Finais**
- [ ] Implementar logs centralizados com **Serilog**.  
- [ ] Configurar monitoramento com ferramentas como **Elastic APM** ou **Application Insights** (opcional).  
- [ ] Garantir que todas as dependências e configurações sensíveis estão em variáveis de ambiente.  

--- 
