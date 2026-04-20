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

### Create Conversation (v1) — Legacy

> **Note:** v1 is maintained for backward compatibility. Use v2 or v3 for new integrations.

- **Path:** `POST /v1/conversation`
- **Content-Type:** `application/json`

#### Request Body

```json
{
  "externalId": "1234",
  "numberOfSpeakers": 1,
  "phoneNumber": "+4522446688",
  "title": "Call with Donald Duck",
  "labels": ["campaign1", "new-product2"],
  "datetime": "2022-02-25T12:00:00Z",
  "outcome": "Declined",
  "outcomeReason": "Competing product won",
  "caseId": "123",
  "agentId": "1234",
  "agentName": "Agent Schmidt",
  "agentEmail": "schmidt@capturi.com",
  "agentAudioChannel": "left",
  "hasConsent": true
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `externalId` | string | ✅ | Unique ID from your system |
| `numberOfSpeakers` | int | ✅ | 1 = mono, 2 = stereo |
| `phoneNumber` | string | ✅ | Customer phone number |
| `title` | string | ✅ | Conversation subject/title |
| `labels` | string[] | ❌ | Classification labels |
| `datetime` | string (ISO 8601) | ✅ | When the conversation occurred |
| `outcome` | string | ❌ | Maps to customProp1 |
| `outcomeReason` | string | ❌ | Maps to customProp2 |
| `caseId` | string | ❌ | Maps to customProp3 |
| `agentId` | string | ✅ | Agent identifier in your system |
| `agentName` | string | ✅ | Agent display name |
| `agentEmail` | string | ✅ | Agent email address |
| `agentAudioChannel` | string | ❌ | `"left"` (default) or `"right"` |
| `hasConsent` | bool | ❌ | Defaults to `true`. When `false`, conversation is analysed but the recording is deleted and insights are anonymised. |

#### Response

```json
{
  "UID": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

### Create Conversation (v2) — Recommended

- **Path:** `POST /v2/conversation`
- **Content-Type:** `application/json`

#### Request Body

```json
{
  "agentEmail": "schmidt@capturi.com",
  "agentId": "1234",
  "agentName": "Agent Schmidt",
  "audioChannels": "Stereo",
  "customer": "+4522446688",
  "customerCompany": "Capturi",
  "dateTime": "2024-01-15T10:00:00Z",
  "externalIdentity": "unique-id-from-your-system",
  "hasConsent": true,
  "labels": ["Inbound", "callback"],
  "salesPersonAudioChannel": 1,
  "status": "closed",
  "subject": "Customer service queue 1",
  "team": "Team 1",
  "customProp1": "Some custom data",
  "customProp2": "string",
  "customProp3": "string",
  "customProp4": "string",
  "customProp5": "string",
  "customProp6": "string",
  "customProp7": "string",
  "customProp8": "string",
  "customProp9": "string",
  "customProp10": "string",
  "customNumberProp1": 0,
  "customNumberProp2": 0,
  "customNumberProp3": 0,
  "customNumberProp4": 0,
  "customNumberProp5": 0,
  "customNumberProp6": 0,
  "customNumberProp7": 0,
  "customNumberProp8": 0,
  "customNumberProp9": 0,
  "cutAudio": [
    {
      "startSeconds": 10,
      "durationSeconds": 25
    }
  ]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `agentEmail` | string | ✅ | Agent email address |
| `agentId` | string | ✅ | Agent identifier in your system |
| `agentName` | string | ✅ | Agent display name |
| `audioChannels` | string | ❌ | `"Mono1Speaker"`, `"Mono2Speaker"`, or `"Stereo"` (default) |
| `customer` | string | ✅ | Customer identifier (e.g. phone number) |
| `customerCompany` | string | ❌ | Customer company name |
| `dateTime` | string (ISO 8601) | ✅ | When the conversation occurred |
| `externalIdentity` | string | ✅ | Unique ID from your system — must be unique per organization |
| `hasConsent` | bool | ❌ | Defaults to `true`. When `false`, conversation is analysed but the recording is deleted and insights are anonymised. |
| `labels` | string[] | ❌ | Classification labels |
| `salesPersonAudioChannel` | int | ❌ | `1` for left channel (default), `2` for right channel |
| `status` | string | ❌ | Conversation status |
| `subject` | string | ✅ | Conversation subject |
| `team` | string | ❌ | Team name — team will be created in Capturi if it doesn't exist |
| `customProp1`–`customProp10` | string | ❌ | Custom string properties (must be configured in Capturi) |
| `customNumberProp1`–`customNumberProp9` | float | ❌ | Custom number properties |
| `cutAudio` | object[] | ❌ | Audio segments to keep (trims the rest) |
| `cutAudio[].startSeconds` | int | ❌ | Segment start time in seconds |
| `cutAudio[].durationSeconds` | int | ❌ | Segment duration in seconds |

#### Response

```json
{
  "UID": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### Changes from v1 to v2

| v1 Field | v2 Field |
|----------|----------|
| `externalId` | `externalIdentity` |
| `numberOfSpeakers` | `audioChannels` |
| `phoneNumber` | `customer` |
| `title` | `subject` |
| `outcome` | `customProp1` |
| `outcomeReason` | `customProp2` |
| `caseId` | `customProp3` |
| `teamId` + `teamName` (v1 had no team) | `team` |

**Added in v2:** `salesPersonAudioChannel`, `customerCompany`, `customProp4`–`customProp10`, `customNumberProp1`–`customNumberProp9`, `cutAudio`, `team`, `status`

---

### Create Conversation (v3)

Identical to v2 request body, but returns lowercase JSON field names and has stricter validation.

- **Path:** `POST /v3/conversation`
- **Content-Type:** `application/json`

#### Request Body

Same as v2 (see above). All required fields are validated:
- `externalIdentity` must not be empty
- `agentId` must not be empty
- `agentName` must not be empty
- `agentEmail` must not be empty
- `customer` must not be empty
- `dateTime` must not be empty
- `subject` must not be empty

#### Response

```json
{
  "uid": "550e8400-e29b-41d4-a716-446655440000"
}
```

> **Note:** v3 returns `"uid"` (lowercase) instead of `"UID"` (uppercase) returned by v1/v2.

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
