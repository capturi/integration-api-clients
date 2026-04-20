# Conversation API

REST API for creating call conversations with metadata for speech-to-text processing in Capturi. Supports custom metadata fields and audio configuration options.

## Base URL

```
https://integrations.capturi.ai
```

## Authentication

See [readme.md](readme.md#authentication) for authentication details.

## Implementation Notes

- Custom properties support flexible metadata (10 text + 9 numeric fields)
- All timestamps must be ISO 8601 / RFC 3339 formatted
- `externalIdentity` must be unique per organization
- Audio processing priority is determined by conversation age (>24h = low priority)

---

## API Endpoints

### Create Conversation v1 (Legacy)

**`POST /v1/conversation`**

> **Note:** v1 is maintained for backward compatibility. Use v2 or v3 for new integrations.

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
| `numberOfSpeakers` | int | ✅ | `1` = Mono1Speaker, `2` = Mono2Speaker |
| `phoneNumber` | string | ✅ | Customer phone number |
| `title` | string | ✅ | Conversation subject/title |
| `labels` | string[] | ❌ | Classification labels |
| `datetime` | string (ISO 8601) | ✅ | When the conversation occurred |
| `outcome` | string | ❌ | Maps to `customProp1` internally |
| `outcomeReason` | string | ❌ | Maps to `customProp2` internally |
| `caseId` | string | ❌ | Maps to `customProp3` internally |
| `agentId` | string | ✅ | Agent identifier in your system |
| `agentName` | string | ✅ | Agent display name |
| `agentEmail` | string | ✅ | Agent email address |
| `agentAudioChannel` | string | ❌ | `"left"` (default) or `"right"` |
| `hasConsent` | bool | ❌ | Defaults to `true`. When `false`, recording is deleted after analysis and insights are anonymised. |

#### Response

```json
{
  "UID": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### Field Mapping (v1 → v2)

| v1 Field | v2 Field |
|----------|----------|
| `externalId` | `externalIdentity` |
| `numberOfSpeakers` | `audioChannels` |
| `phoneNumber` | `customer` |
| `title` | `subject` |
| `outcome` | `customProp1` |
| `outcomeReason` | `customProp2` |
| `caseId` | `customProp3` |

---

### Create Conversation v2 (Recommended)

**`POST /v2/conversation`**

Creates a new conversation with the provided metadata. Upload audio separately using the returned UID.

#### Required Fields

```json
{
  "externalIdentity": "call-12345-2024",
  "agentId": "agent-001",
  "agentName": "John Sales",
  "agentEmail": "john.sales@company.com",
  "dateTime": "2024-01-15T14:30:00Z",
  "subject": "Product demo call",
  "customer": "+45 22 44 66 88"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `externalIdentity` | string | ✅ | Unique ID from your system — must be unique per organization |
| `agentId` | string | ✅ | Agent identifier in your system |
| `agentName` | string | ✅ | Agent display name |
| `agentEmail` | string | ✅ | Agent email address |
| `dateTime` | string (ISO 8601) | ✅ | When the conversation occurred |
| `subject` | string | ✅ | Conversation subject |
| `customer` | string | ✅ | Customer identifier (e.g. phone number) |

#### Optional Fields

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `customerCompany` | string | — | Customer company name |
| `labels` | string[] | — | Classification labels |
| `status` | string | — | Conversation lifecycle status |
| `hasConsent` | bool | `true` | When `false`, recording is deleted after analysis and insights are anonymised. |
| `audioChannels` | string | `"Stereo"` | `"Mono1Speaker"`, `"Mono2Speaker"`, or `"Stereo"` |
| `salesPersonAudioChannel` | int | `1` | `1` for left channel, `2` for right channel |
| `team` | string | — | Team name — created in Capturi if it doesn't exist |
| `cutAudio` | object[] | — | Audio segments to extract (see below) |
| `customProp1`–`customProp10` | string | — | Custom text properties (must be configured in Capturi) |
| `customNumberProp1`–`customNumberProp9` | float | — | Custom numeric properties |

#### Cut Audio

Extract specific time segments for focused analysis:

```json
{
  "cutAudio": [
    { "startSeconds": 120, "durationSeconds": 180 },
    { "startSeconds": 600, "durationSeconds": 300 }
  ]
}
```

| Field | Type | Description |
|-------|------|-------------|
| `startSeconds` | int | Segment start time in seconds (≥ 0) |
| `durationSeconds` | int | Segment duration in seconds (≥ 1) |

#### Full Request Example

```json
{
  "externalIdentity": "call-12345-2024",
  "agentId": "agent-001",
  "agentName": "John Sales",
  "agentEmail": "john.sales@company.com",
  "dateTime": "2024-01-15T14:30:00Z",
  "subject": "Product demo call",
  "customer": "+45 22 44 66 88",
  "customerCompany": "ABC Corporation",
  "labels": ["inbound", "demo", "qualified-lead"],
  "status": "completed",
  "hasConsent": true,
  "audioChannels": "Stereo",
  "salesPersonAudioChannel": 1,
  "team": "Sales Team A",
  "cutAudio": [
    { "startSeconds": 120, "durationSeconds": 180 },
    { "startSeconds": 600, "durationSeconds": 300 }
  ],
  "customProp1": "enterprise-tier",
  "customProp2": "product-demo",
  "customProp3": "q1-2024",
  "customNumberProp1": 15000.0,
  "customNumberProp2": 0.85,
  "customNumberProp3": 3.5
}
```

#### Response

```json
{
  "UID": "550e8400-e29b-41d4-a716-446655440000"
}
```

> **Note:** The response field is uppercase `"UID"`. Use this value for subsequent audio uploads.

---

### Create Conversation v3

**`POST /v3/conversation`**

Identical request body to v2 with two differences:
1. **Stricter validation** — all required fields are validated before processing
2. **Lowercase response** — returns `"uid"` instead of `"UID"`

#### Validation Rules

All of these are enforced (returns 400 if any fail):
- `externalIdentity` — must not be empty
- `agentId` — must not be empty
- `agentName` — must not be empty
- `agentEmail` — must not be empty
- `customer` — must not be empty
- `dateTime` — must not be empty
- `subject` — must not be empty

#### Response

```json
{
  "uid": "550e8400-e29b-41d4-a716-446655440000"
}
```

> **Important:** v3 returns lowercase `"uid"`, unlike v1/v2 which return uppercase `"UID"`.

---

## Audio Channel Configuration

| Audio Channel | Description | Use Case |
|---------------|-------------|----------|
| `Mono1Speaker` | Single channel, one speaker | Phone recordings with single participant |
| `Mono2Speaker` | Single channel, two speakers | Conference calls mixed to mono |
| `Stereo` | Two channels (default) | Separate speaker channels |

For stereo recordings, `salesPersonAudioChannel` specifies which channel contains the agent audio:
- `1` = left channel (default)
- `2` = right channel

---

## Upload Audio

After creating a conversation, upload audio as a multipart form file.

### Upload by Capturi Conversation UID

**`POST /v1/audio/{conversation_uid}`**

Use the UID returned from the create conversation response.

### Upload by External ID

**`POST /v1/audio/external/{external_id}`**

Use the same external ID you provided when creating the conversation (`externalId` for v1, `externalIdentity` for v2/v3).

### Form Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `data` | file | ✅ | Audio file (binary) |
| `spokenLanguage` | string | ❌ | Language code, e.g. `da-DK`, `en-US` |
| `stereoConfig` | string | ❌ | `"AgentChannelOne"` or `"AgentChannelTwo"` |
| `monoConfig` | string | ❌ | `"AgentOnly"`, `"CustomerOnly"`, or `"TwoSpeakers"` |

**Content-Type:** `multipart/form-data`
**Max upload size:** 1 GB
**Response:** 200 OK on success

### Upload Audio via Webhook (Async)

For conversations created via the webhook endpoint (async flow).

**`POST /v1/audio/webhook`**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `data` | file | ✅ | Audio file (binary) |
| `externalId` | string | ✅ | Same external identity used when creating the conversation |

> **Note:** Requires a webhook configuration for your organization. Contact Capturi to set this up.

---

## Processing Pipeline

1. **Create Conversation** → Returns a UID
2. **Upload Audio** → Use the UID with the audio upload endpoints above
3. **ASR Processing** → Automatic speech-to-text (conversations >24h old get lower priority)
4. **Analysis Available** → Results accessible via Capturi dashboard

## Custom Properties

- **Text properties** (`customProp1`–`customProp10`): 10 custom text fields
- **Number properties** (`customNumberProp1`–`customNumberProp9`): 9 custom numeric fields
- All custom properties must be configured in Capturi before use
- Contact support to configure custom properties for your organization

## Error Handling

| Status Code | Description |
|-------------|-------------|
| 200 | Success — conversation created |
| 400 | Bad Request — invalid JSON, missing required fields, or validation errors |
| 409 | Conflict — `externalIdentity` already exists for this organization |
| 500 | Internal Server Error — downstream service issues |
