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

## Endpoints

### Conversations & Audio Upload

See [conversation.md](conversation.md) for full documentation on conversation creation (v1, v2, v3) and audio upload.

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

- **Go**: [`go/`](go/) — Creates a v1 conversation and uploads audio
- **.NET/C#**: [`dotnet-csharp/`](dotnet-csharp/) — Creates a v1 conversation and uploads audio

## Support

- **Email:** integrations@capturi.com
- **GitHub Issues:** Create an issue on this repository
- **Swagger:** https://integrations.capturi.ai/swagger/index.html
