# 🌡️ Temperatura_Api

Este projeto é uma API RESTful desenvolvida em ASP.NET Core 6, que consome dados meteorológicos da Brasil API e armazena informações sobre cidades e condições climáticas em um banco de dados SQL Server. A API inclui funcionalidades para consultar dados sobre cidades, condições climáticas de cidades e aeroportos, e testar a conexão com o banco de dados.

## 📚 Boas Práticas Utilizadas

1. **Injeção de Dependência**: Utiliza-se a injeção de dependência para injetar `IDbConnection` e `HttpClient`, promovendo a modularidade e facilitando o teste de componentes. 🛠️

2. **Tratamento de Erros**: Implementa tratamento robusto de erros com blocos `try-catch` e log de erros para rastreamento e solução de problemas. 🐞

3. **Transações**: Utiliza transações ao realizar inserções múltiplas no banco de dados para garantir a consistência dos dados. 🔄

4. **Desacoplamento de Código**: Métodos como `LogError` são isolados para manter o código principal limpo e focado na lógica de negócios. ✨

5. **Nomenclatura em Inglês**: As nomenclaturas são mantidas em inglês para consistência e compatibilidade com padrões internacionais e bibliotecas. 🌐

6. **Documentação da API**: A documentação da API é gerada com Swagger/OpenAPI, permitindo fácil integração e uso da API. 📖

7. **Uso de Async/Await**: Utiliza operações assíncronas para melhorar o desempenho e a escalabilidade do sistema. 🚀

## 🏗️ Design Pattern Utilizado

- **Repository Pattern**: O uso do `IDbConnection` para interações com o banco de dados e o encapsulamento das operações de banco de dados em métodos específicos ajuda a aplicar o Repository Pattern. Isso promove uma separação clara entre a lógica de acesso a dados e a lógica de aplicação. 🏛️

## ⚙️ Funcionalidades

### Métodos do Controller

- **`GetAllCitiesWithCodes`**: Recupera uma lista de cidades com códigos postais da Brasil API, armazena esses dados no banco de dados e retorna a lista de cidades. 🌆

- **`GetWeatherByCity`**: Recupera dados climáticos para uma cidade específica da Brasil API e os armazena no banco de dados, retornando as informações climáticas. 🌦️

- **`GetWeatherByAirport`**: Recupera dados climáticos para um aeroporto específico da Brasil API e armazena as informações no banco de dados, retornando os dados climáticos. ✈️

- **`TestConnection`**: Testa a conexão com o banco de dados e retorna um status de sucesso ou erro. 🔗

### Models

- **`CityDto`**: Representa a estrutura dos dados de uma cidade. 🏙️
- **`Clima`**: Representa as condições climáticas para uma data específica. 🌡️
- **`WeatherCityDto`**: Representa os dados climáticos de uma cidade. 🌆🌦️
- **`WeatherAirportDto`**: Representa os dados climáticos de um aeroporto. 🛩️🌫️
- **`WeatherForecast`**: Representa a previsão do tempo com mínima e máxima de temperatura. 📅🌡️

## 🔗 Links Úteis

- [Diagrama do Banco de Dados](https://dbdiagram.io/d/Diagrama-de-dados-Teste-AeC-66a058038b4bb5230e2ab3a4)
- [Documentação do DB](https://dbdocs.io/kemerssonvinicius/Doc-DB-Teste-AeC)

## 🧪 Testando a API com Swagger

1. **Executar a Aplicação**: Inicie sua aplicação ASP.NET Core. A API estará disponível em `https://localhost:{port}` (substitua `{port}` pelo número da porta que seu aplicativo está usando).

2. **Acessar Swagger UI**: Abra um navegador e acesse `https://localhost:{port}/swagger`. Esta página exibe a documentação gerada automaticamente da API.

3. **Testar Endpoints**:
   - **`GetAllCitiesWithCodes`**:
     - Selecione o endpoint `GET /cidades-com-codigo`.
     - Clique no botão "Try it out".
     - Clique em "Execute" para enviar a solicitação.
     - Visualize a resposta e o status HTTP retornado.

   - **`GetWeatherByCity`**:
     - Selecione o endpoint `GET /cidade/{cityCode}`.
     - No campo `{cityCode}`, insira um código de cidade válido (por exemplo, `SP`).
     - Clique em "Try it out".
     - Clique em "Execute" para enviar a solicitação.
     - Visualize a resposta e o status HTTP retornado.

   - **`GetWeatherByAirport`**:
     - Selecione o endpoint `GET /aeroporto/{icaoCode}`.
     - No campo `{icaoCode}`, insira um código de aeroporto válido (por exemplo, `SBGR`).
     - Clique em "Try it out".
     - Clique em "Execute" para enviar a solicitação.
     - Visualize a resposta e o status HTTP retornado.

   - **`TestConnection`**:
     - Selecione o endpoint `GET /test-Conexao`.
     - Clique em "Try it out".
     - Clique em "Execute" para enviar a solicitação.
     - Visualize a resposta e o status HTTP retornado.


### configurando o ambiente:

### Docker
**Certifique-se de estar no caminho correto para executar os comandos:**

primeiro:
docker build -t temperatura_api .
Isso criará uma imagem Docker chamada temperatura_api.

Certifique-se de que seu contêiner SQL Server esteja configurado e em execução:
``docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=senha@123d" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest``

**Configure o Contêiner da API para Conectar ao SQL Server**

Para garantir que a API possa se conectar ao SQL Server, você precisará definir as variáveis de ambiente de conexão ao iniciar o contêiner da API.

``docker run -d -p 80:80 --name temperatura_api --restart unless-stopped \
  -e "ConnectionStrings:DefaultConnection=Server=host.docker.internal;Database=SeuBancoDeDados;User Id=sa;Password=senha@123d;" \
  temperatura_api``

  ### disponibilizei um arquivo .db chamado create.db com os creates necessários para criar as tabelas e banco. ###

  **Nota: Deixei uma função retornando erro propositalmente para que a persistencia de erros no log seja visualizada, porém pode-se notar que ela segue a mesma estrutura que as demais partes do projeto, sendo isso parte da avaliação de todo o escopo do projeto.**

