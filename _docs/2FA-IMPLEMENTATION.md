# Implementação de Autenticação de Dois Fatores (2FA)

## ✅ Fluxo Correto de Autenticação

### 1. **Login Inicial (sem código 2FA)**
**Endpoint:** `POST /api/autenticacao/login`

**Request:**
```json
{
  "email": "usuario@email.com",
  "password": "Senha@123"
}
```

**Response (Se 2FA estiver ativo):**
```json
{
  "isSuccess": true,
  "data": {
    "requiresTwoFactor": true,
    "message": "Código de verificação enviado para seu email.",
    "token": null,
    "refreshToken": null,
    "expiraEm": 0
  }
}
```

**O código é enviado automaticamente para o email/telefone do usuário!**

---

### 2. **Login com Código 2FA**
**Endpoint:** `POST /api/autenticacao/login` (mesmo endpoint!)

**Request:**
```json
{
  "email": "usuario@email.com",
  "password": "Senha@123",
  "twoFactorCode": "123456"
}
```

**Response (Sucesso):**
```json
{
  "isSuccess": true,
  "data": {
    "requiresTwoFactor": false,
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123...",
    "expiraEm": 3600,
    "message": null
  }
}
```

**Response (Código Inválido):**
```json
{
  "isSuccess": false,
  "errors": [
    {
      "key": "Login",
      "value": "Código inválido. Verifique o código e tente novamente ou solicite um novo envio."
    }
  ]
}
```

---

## 📋 Fluxo Completo no Frontend

### Passo 1: Login Inicial
```javascript
// POST /api/autenticacao/login
const response = await login({
  email: "usuario@email.com",
  password: "Senha@123"
});

if (response.data.requiresTwoFactor) {
  // Mostrar tela de código 2FA
  showTwoFactorCodeScreen();
  alert(response.data.message); // "Código enviado para seu email"
} else {
  // Login normal (2FA desativado)
  saveToken(response.data.token);
  redirectToDashboard();
}
```

### Passo 2: Validar Código 2FA
```javascript
// POST /api/autenticacao/login (com código)
const response = await login({
  email: "usuario@email.com",
  password: "Senha@123",
  twoFactorCode: "123456" // código digitado pelo usuário
});

if (response.isSuccess) {
  saveToken(response.data.token);
  redirectToDashboard();
} else {
  showError("Código inválido"); // MSG03
}
```

---

## 🎯 Endpoints Disponíveis

### ✅ **Login (Principal)**
`POST /api/autenticacao/login`
- Sem `twoFactorCode`: Envia código e retorna `requiresTwoFactor: true`
- Com `twoFactorCode`: Valida e retorna token JWT

### 📤 **Reenviar Código (Opcional)**
`POST /api/autenticacao/enviar-codigo-2fa`

**Quando usar?** Apenas se o usuário clicar em "Reenviar código"

**Request:**
```json
{
  "email": "usuario@email.com",
  "metodoEnvio": "Email"
}
```

### ❌ **Verificar Código (NÃO USAR)**
~~`POST /api/autenticacao/verificar-codigo-2fa`~~

**Este endpoint foi mantido mas NÃO deve ser usado no fluxo normal.**  
Use sempre o endpoint de login com `twoFactorCode`.

---

## � Como Testar

### Teste 1: Login com 2FA Ativo
```http
POST /api/autenticacao/login
{
  "email": "teste@email.com",
  "password": "Senha@123"
}
```
**Resultado:** Código enviado, `requiresTwoFactor: true`

### Teste 2: Ver código no console
O código aparecerá no console da aplicação (em dev).

### Teste 3: Login com código
```http
POST /api/autenticacao/login
{
  "email": "teste@email.com",
  "password": "Senha@123",
  "twoFactorCode": "123456"
}
```
**Resultado:** Token JWT retornado

---

## ✅ Requisitos Atendidos

- [x] 2FA ativo por padrão em novas contas
- [x] Código de 6 dígitos gerado automaticamente
- [x] Tempo limite de 5 minutos (padrão Identity)
- [x] Envio automático no primeiro login
- [x] Validação com mensagem personalizada (MSG03)
- [x] Fluxo unificado (um único endpoint de login)

---

## 📝 Próximos Passos (Melhorias)

### Controle de Reenvio (MSG04)
- Limite de 3 reenvios em 10 minutos
- Bloqueio temporário de 5 minutos

### Integração Real
- Email: SendGrid, AWS SES
- SMS: Twilio, AWS SNS
