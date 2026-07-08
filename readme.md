# Capturi Integrations API

This repository contains documentation and demo clients for the Capturi integrations API.

Swagger docs are available at https://integrations.capturi.ai/swagger/index.html

To get an API token or if you have technical questions, contact us at integrations@capturi.com. You are also welcome to create an issue on GitHub.

## Authentication

The API token must be provided in **one** of the following ways:

- **Authorization header** (recommended — avoids token appearing in firewall logs):
  ```
  Authorization: <your-api-token>
  ```
- **Query parameter**:
  ```
  ?api-token=<your-api-token>
  ```

## Base URL

```
https://integrations.capturi.ai
```

---

## Developer Portal

The fastest way to explore and debug your integration is the **Developer Portal** — a self-service web dashboard where you can watch your API requests live, inspect errors, browse the conversations you've created, and ask an AI assistant for integration help (including generating client code).

Access is granted through short-lived **magic links**: create one with your API token, open the returned URL in a browser, and you're in.

```
POST /v1/dev/magic-link
```

See [developer-portal.md](developer-portal.md) for full documentation, including the magic link endpoint, the portal views, the read-only JSON API, and the AI assistant.

---

## Endpoints

### Conversations & Audio Upload

See [conversation.md](conversation.md) for full documentation on conversation creation and audio upload. Use the **v3** endpoint for new integrations; v1 and v2 are deprecated.

---

### Webhook (Custom Data)

For integrations where the data format can't be changed to match the Capturi conversation model. Capturi creates a custom handler for your data.

- **Path:** `POST /v1/webhook`
- **Content-Type:** `application/json`
- **Body:** Any valid JSON object

Capturi will save the raw data and process it asynchronously with a customer-specific handler.

> **Note:** Requires a webhook configuration for your organization. Contact Capturi to set this up.

---

### Case Management

See [case.md](case.md) for full case/email API documentation.

### SCIM (User Provisioning)

See [scim.md](scim.md) for Azure AD SCIM provisioning setup.

---

## Demo Clients

- **Go**: [`go/`](go/) — Creates a conversation via the v3 endpoint and uploads audio (audio upload uses `/v1/audio`)
- **.NET/C#**: [`dotnet-csharp/`](dotnet-csharp/) — Creates a conversation via the v3 endpoint and uploads audio (audio upload uses `/v1/audio`)

## Support

- **Email:** integrations@capturi.com
- **GitHub Issues:** Create an issue on this repository
- **Swagger:** https://integrations.capturi.ai/swagger/index.html
