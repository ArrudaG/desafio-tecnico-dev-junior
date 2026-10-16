# Uso de IA

## 1. Ferramenta
- **Gemini**

## 2. Como a IA foi utilizada

A ferramenta foi utilizada para gerar a estrutura base inicial do projeto (backend e frontend), além de atuar como suporte para esclarecimento de dúvidas técnicas e orientação na resolução de erros.

- **Estruturação do Backend (.NET 8):** Geração dos comandos de CLI para criação da solução e projeto Web API.
- **Estruturação do Frontend (React + TypeScript):** Criação da estrutura inicial da aplicação, tipos e integração com Axios, sobre a qual realizei ajustes e correções manuais.
- **Suporte Técnico e Aprendizado:** Sempre que surgiam dúvidas sobre erros de compilação ou sobre como implementar determinada lógica, consultei a ferramenta para me orientar e ensinar como resolver o problema, realizando a aplicação e os ajustes de forma manual.

Exemplo de comandos gerados para montagem da estrutura:

```bash
# 1. Criar o arquivo de solução (.sln) na raiz do repositório
dotnet new sln -n EncontreProfessor

# 2. Criar a pasta 'backend' e o projeto Web API (.NET 8) dentro dela
dotnet new webapi -o backend/EncontreProfessor.Api

# 3. Adicionar o projeto da API na solução (.sln)
dotnet sln add backend/EncontreProfessor.Api/EncontreProfessor.Api.csproj
```

## 3. Caso em que a IA errou e como foi resolvido.

### O Erro:

Ao criar o global.json ele me sugeriu para colocar como versão uma que não é a que tenho instalado e com isso os comandos ```dotnet build``` e ```dotnet clean``` falharam

### Como resolvi:

Percebi que o erro era causado analisando a mensagem do terminal que indicava que a versão instalada era diferente da versão que o arquivo fazia referência, corrigi ajustando o global.json para uma versão genérica, ```"version": "8.0.*"```, e executei o comando ```dotnet restore``` para restaurar os pacotes e dependências.