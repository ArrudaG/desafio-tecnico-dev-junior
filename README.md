# Encontre um Professor

Aplicação full-stack para busca de professores disponíveis com base em idioma, dias da semana, faixa de horário e duração da aula.

---

## Tecnologias

- **Backend:** .NET 8 (C# 12), xUnit, Moq
- **Frontend:** React 18, TypeScript, Vite, Axios

---

## Como Executar

### 1. Backend (API C#)
```bash
cd backend/EncontreProfessor.Api
dotnet run
```

A API rodará em ```http://localhost:5248``` e para acessar o Swagger ```http://localhost:5248/swagger```.

### 2. Testes Automatizados
```bash
dotnet test
```

### 3. Frontend (React)
```bash
cd frontend
npm install
npm run dev
```

Acesse o endereço que aparecer no terminal (ex: ```http://localhost:5173```)

## Principais implementações

- **Regras de negócio:** Estão no serviço ```BuscadorProfessorService``` (filtro de idiomas, validação de sobreposição de horários e bloqueios de agenda).
- **Validação:** DTOs e tratamento de requisições inválidas (HTTP 400).
- **Frontend:** Formulário responsivo cobrindo os 4 estados de interface (Carregamento, Resultado Vazio, Erros de Validação e Falha de Conexão).