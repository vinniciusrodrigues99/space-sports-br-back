# Fluxo de Autenticação 2FA - Diagrama

```
┌─────────────────────────────────────────────────────────────────────┐
│                    FLUXO DE AUTENTICAÇÃO 2FA                        │
└─────────────────────────────────────────────────────────────────────┘

┌──────────┐                                    ┌──────────┐
│ Frontend │                                    │ Backend  │
└────┬─────┘                                    └────┬─────┘
     │                                               │
     │  1. POST /api/autenticacao/login              │
     │  { email, password }                          │
     ├──────────────────────────────────────────────>│
     │                                               │
     │                                       ┌───────┴────────┐
     │                                       │ Valida credenciais
     │                                       │ 2FA ativo?      │
     │                                       └───────┬────────┘
     │                                               │
     │                                       ┌───────┴────────┐
     │                                       │ Gera código     │
     │                                       │ Envia Email/SMS │
     │                                       └───────┬────────┘
     │                                               │
     │  Response: { requiresTwoFactor: true,         │
     │             message: "Código enviado" }       │
     │<──────────────────────────────────────────────┤
     │                                               │
┌────┴─────┐                                         │
│ Exibe    │                                         │
│ tela de  │                                         │
│ código   │                                         │
└────┬─────┘                                         │
     │                                               │
     │  2. POST /api/autenticacao/login              │
     │  { email, password, twoFactorCode: "123456" } │
     ├──────────────────────────────────────────────>│
     │                                               │
     │                                       ┌───────┴────────┐
     │                                       │ Valida código   │
     │                                       │ Gera JWT token  │
     │                                       └───────┬────────┘
     │                                               │
     │  Response: { requiresTwoFactor: false,        │
     │             token: "eyJhbG...",                │
     │             refreshToken: "abc..." }          │
     │<──────────────────────────────────────────────┤
     │                                               │
┌────┴─────┐                                         │
│ Salva    │                                         │
│ token    │                                         │
│ Redireciona                                        │
└──────────┘                                         │


┌─────────────────────────────────────────────────────────────────────┐
│                    OPÇÃO: REENVIAR CÓDIGO                           │
└─────────────────────────────────────────────────────────────────────┘

┌──────────┐                                    ┌──────────┐
│ Frontend │                                    │ Backend  │
└────┬─────┘                                    └────┬─────┘
     │                                               │
     │  POST /api/autenticacao/enviar-codigo-2fa     │
     │  { email, metodoEnvio: "Email" }              │
     ├──────────────────────────────────────────────>│
     │                                               │
     │                                       ┌───────┴────────┐
     │                                       │ Gera novo código│
     │                                       │ Envia Email/SMS │
     │                                       │ Invalida anterior
     │                                       └───────┬────────┘
     │                                               │
     │  Response: { message: "Código reenviado",     │
     │             expiresAt: "..." }                │
     │<──────────────────────────────────────────────┤
     │                                               │
┌────┴─────┐                                         │
│ Reinicia │                                         │
│ timer    │                                         │
│ 5 min    │                                         │
└──────────┘                                         │
```

## Resumo do Fluxo

### ✅ Primeira Chamada (Login Inicial)
- **Input:** `{ email, password }`
- **Output:** `{ requiresTwoFactor: true, message: "Código enviado" }`
- **Backend:** Envia código automaticamente

### ✅ Segunda Chamada (Login com Código)
- **Input:** `{ email, password, twoFactorCode: "123456" }`
- **Output:** `{ requiresTwoFactor: false, token: "...", refreshToken: "..." }`
- **Backend:** Valida código e gera token JWT

### 🔄 Reenviar Código (Opcional)
- **Input:** `{ email, metodoEnvio: "Email" }`
- **Output:** `{ message: "Código reenviado", expiresAt: "..." }`
- **Backend:** Gera novo código e invalida o anterior
