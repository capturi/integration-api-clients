# Conversation Management API

REST API for creating call conversations with metadata for speech-to-text processing in Capturi. Supports custom metadata fields and audio configuration options.

## Implementation Notes

- Self-managed integration - you handle data flow and error handling
- Custom properties support flexible metadata (10 text + 9 numeric fields)
- All timestamps must be RFC3339 formatted
- ExternalIdentity must be globally unique per organization
- Audio processing priority determined by conversation age (>24h = low priority)

## API Endpoints

### Create Conversation (POST)
**Endpoint:** `POST /v2/conversation`

Creates a new conversation with the provided metadata. The conversation will be processed for speech-to-text analysis if audio is uploaded subsequently.

## Schema Requirements

### Required Fields
```typescript
{
  externalIdentity: string;    // Globally unique conversation ID
  agentId: string;            // Agent identifier  
  agentName: string;          // Agent display name
  agentEmail: string;         // Agent email address
  dateTime: Date;             // ISO 8601 timestamp
  subject: string;            // Conversation topic (non-empty)
  customer: string;           // Customer identifier (non-empty)
}
```

### Optional Fields
```typescript
{
  customerCompany?: string;           // Customer organization
  labels?: string[];                 // Classification tags (no duplicates)
  status?: string;                   // Conversation lifecycle status
  hasConsent?: boolean;              // Recording consent flag
  audioChannels?: "Mono1Speaker" | "Mono2Speaker" | "Stereo";
  salesPersonAudioChannel?: 1 | 2;   // Agent audio channel (stereo only)
  team?: string;                     // Team assignment
  cutAudio?: Array<{                 // Audio segment extraction
    startSeconds: number;            // Start time (≥0)
    durationSeconds: number;         // Duration (≥1)
  }>;
}
```

### Custom Properties Schema
```typescript
{
  // Text properties (must be pre-configured in Capturi)
  customProp1?: string;
  customProp2?: string;
  // ... up to customProp10
  
  // Numeric properties
  customNumberProp1?: number;
  customNumberProp2?: number;  
  // ... up to customNumberProp9
}
```

## Audio Processing Configuration

| Audio Channel | Description | Use Case |
|---------------|-------------|----------|
| `Mono1Speaker` | Single channel, one speaker | Phone recordings, single participant |
| `Mono2Speaker` | Single channel, two speakers | Conference calls mixed to mono |
| `Stereo` | Two channels | Separate speaker channels |

**Sales Person Channel**: For stereo recordings, specify which channel (1 or 2) contains the agent audio. Default is channel 1.

**Cut Audio**: Extract specific time segments for focused analysis. Useful for analyzing key conversation moments without processing entire recordings.

## Request Example

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
    {
      "startSeconds": 120,
      "durationSeconds": 180
    },
    {
      "startSeconds": 600,
      "durationSeconds": 300
    }
  ],
  "customProp1": "enterprise-tier",
  "customProp2": "product-demo",
  "customProp3": "q1-2024",
  "customNumberProp1": 15000.0,
  "customNumberProp2": 0.85,
  "customNumberProp3": 3.5
}
```

## Validation Requirements

Conversations are validated with the following rules:
- **externalIdentity**: Required, cannot be null, empty, or whitespace. Must be unique across all conversations for your organization
- **agentId**: Required, cannot be empty
- **agentName**: Required, cannot be empty  
- **agentEmail**: Required, cannot be empty
- **dateTime**: Required, must be valid timestamp
- **subject**: Required, cannot be null, empty, or whitespace
- **customer**: Required, cannot be null, empty, or whitespace
- **labels**: Cannot contain null values or duplicates
- **status**: Required, cannot be null, empty, or whitespace
- **salesPersonAudioChannel**: Must be 1 or 2 if specified
- **cutAudio.startSeconds**: Must be 0 or greater
- **cutAudio.durationSeconds**: Must be 1 or greater

## Custom Properties

Custom properties provide flexible metadata storage:
- **Text Properties**: 10 custom text fields (customProp1-10)
- **Number Properties**: 9 custom numeric fields (customNumberProp1-9)  
- All custom properties must be configured in Capturi before use
- Contact support to configure custom properties for your organization

## Response Schema

```typescript
{
  uid: string;  // Capturi conversation UUID for subsequent operations
}
```

## Processing Pipeline

1. **Conversation Creation**: Creates metadata record and user/team mappings
2. **Audio Upload**: Use returned UID for audio file uploads via separate endpoint
3. **ASR Processing**: Automatic speech-to-text with configurable priority
4. **Analysis Available**: Results accessible via Capturi dashboard and APIs

**Priority Logic**: Conversations older than 24 hours are processed with `LowPriority`, others get `DefaultPriority`.

## Error Handling

| Status Code | Description |
|-------------|-------------|
| 400 | Bad Request - Invalid JSON, missing required fields, validation errors |
| 409 | Conflict - ExternalIdentity already exists (must be unique) |
| 500 | Internal Server Error - User creation failed or downstream service issues |

## Implementation Notes

- API schema may change without notice - implement defensive parsing
- Custom properties must be pre-configured in Capturi before use
- ExternalIdentity uniqueness is enforced at organization level
- For support issues, reference this documentation or contact support directly