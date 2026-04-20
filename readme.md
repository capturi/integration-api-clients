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

### Conversations

See [conversation.md](conversation.md) for full conversation API documentation (v1, v2, v3).

---

### Upload Audio

Audio is uploaded as a multipart form file in the `data` field.

#### Upload by Capturi Conversation UID

- **Path:** `POST /v1/audio/{conversation_uid}`
- **Content-Type:** `multipart/form-data`

#### Upload by External ID

- **Path:** `POST /v1/audio/external/{external_id}`
- **Content-Type:** `multipart/form-data`

The `external_id` must match the ID used when creating the conversation (`externalId` for v1, `externalIdentity` for v2/v3).

#### Form Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `data` | file | ✅ | Audio file (binary) |
| `spokenLanguage` | string | ❌ | Language code, e.g. `da-DK`, `en-US` |
| `stereoConfig` | string | ❌ | `"AgentChannelOne"` or `"AgentChannelTwo"` |
| `monoConfig` | string | ❌ | `"AgentOnly"`, `"CustomerOnly"`, or `"TwoSpeakers"` |

#### Response

- **200 OK** on success

**Max upload size:** 1 GB

---

### Upload Audio via Webhook (Async)

For conversations created via the webhook endpoint (async flow).

- **Path:** `POST /v1/audio/webhook`
- **Content-Type:** `multipart/form-data`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `data` | file | ✅ | Audio file (binary) |
| `externalId` | string | ✅ | Same external identity used when creating the conversation |

> **Note:** Requires a webhook configuration for your organization. Contact Capturi to set this up.

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
