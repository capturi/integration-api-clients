# Case Management API

REST API for creating, updating, and managing email cases in Capturi. Supports full CRUD operations with message threading and custom metadata fields.

## Implementation Notes

- Self-managed integration - you control data flow and error handling
- Custom fields support arbitrary key-value metadata (max 10 fields)
- All timestamps must be RFC3339 formatted
- HTML content is automatically stripped from message text
- Maximum 10 custom fields per case (alphanumeric keys only)

## API Endpoints

### Create or Update Case (PUT)
**Endpoint:** `PUT /v1/case`

Creates a new case or updates an existing one if it exists with the given CaseUid. Note that updating will overwrite all existing fields with the provided values.

### Add Message and Create Case (PUT)
**Endpoint:** `PUT /v1/case/messages`

Adds a new message to a case. If the case doesn't exist it will be created.

### Add Message to Existing Case (POST)
**Endpoint:** `POST /v1/case/messages`

Adds a new message to a case. If the case doesn't exist the api will return a 204.

### Patch Case (PATCH)
**Endpoint:** `PATCH /v1/case/{externalID}`

Patches an existing case. If any fields are missing the api will update the ones that are present. Patching will only overwrite fields that are present.

## Schema Requirements

### Required Fields
```typescript
{
  caseUid: string;        // Unique case identifier
  source: string;         // Source system identifier
  created: Date;          // ISO 8601 timestamp
  updated: Date;          // ISO 8601 timestamp  
  subject: string;        // Case subject (non-empty)
  messages: Message[];    // Minimum 1 message for PUT operations
}
```

### Optional Fields
```typescript
{
  inbox?: string;         // Target inbox/routing
  status?: string;        // Case lifecycle status
  tags?: string[];        // Classification labels (no duplicates)
  customFields?: Array<{  // Max 10 fields, alphanumeric keys only
    name: string;         // Regex: /^[a-zA-Z0-9]+$/
    value: string;
  }>;
  priority?: string;      // Business priority level
}
```

### Message Schema
```typescript
{
  messageUid: string;           // Unique message ID
  created: Date;                // ISO 8601 timestamp
  direction: "Inbound" | "Outbound";
  from: {
    name: string;
    email: string;
    id: string;
  };
  to: Array<{
    name: string;
    email: string;
    id?: string;
  }>;
  subject: string;
  text: string;                 // HTML automatically stripped
  type: string;                 // Message type identifier
  attachments?: string[];       // File reference IDs
}
```

## Request Examples

### Create or Update Case
```json
{
  "caseUid": "case-12345",
  "source": "email-system",
  "created": "2024-01-15T10:00:00Z",
  "updated": "2024-01-15T10:00:00Z",
  "subject": "Customer Support Request",
  "inbox": "support@company.com",
  "status": "open",
  "tags": ["support", "urgent"],
  "customFields": [
    {
      "name": "customerType",
      "value": "premium"
    },
    {
      "name": "productVersion",
      "value": "2.1.0"
    }
  ],
  "messages": [
    {
      "messageUid": "msg-001",
      "created": "2024-01-15T10:00:00Z",
      "type": "email",
      "direction": "Inbound",
      "from": {
        "name": "John Customer",
        "email": "john@customer.com",
        "id": "cust-123"
      },
      "to": [
        {
          "name": "Support Team",
          "email": "support@company.com",
          "id": "support-team"
        }
      ],
      "subject": "Help with login issues",
      "text": "I'm having trouble logging into my account...",
      "attachments": ["screenshot-login-error.png"]
    }
  ]
}
```

### Add Single Message to Case
```json
{
  "caseUid": "case-12345",
  "source": "email-system",
  "created": "2024-01-15T11:00:00Z",
  "updated": "2024-01-15T11:00:00Z",
  "subject": "Customer Support Request",
  "inbox": "support@company.com",
  "status": "in-progress",
  "tags": ["support"],
  "customFields": [
    {
      "name": "priority",
      "value": "high"
    }
  ],
  "message": {
    "messageUid": "msg-002",
    "created": "2024-01-15T11:00:00Z",
    "type": "email",
    "direction": "Outbound",
    "from": {
      "name": "Support Agent",
      "email": "agent@company.com",
      "id": "agent-456"
    },
    "to": [
      {
        "name": "John Customer",
        "email": "john@customer.com",
        "id": "cust-123"
      }
    ],
    "subject": "Re: Help with login issues",
    "text": "Hi John, I can help you with the login issue. Please try...",
    "attachments": []
  }
}
```

### Add Message to Existing Case
```json
{
  "caseUid": "case-12345",
  "messageUid": "msg-003",
  "created": "2024-01-15T12:00:00Z",
  "type": "email",
  "direction": "Inbound",
  "from": {
    "name": "John Customer",
    "email": "john@customer.com",
    "id": "cust-123"
  },
  "to": [
    {
      "name": "Support Agent",
      "email": "agent@company.com",
      "id": "agent-456"
    }
  ],
  "subject": "Re: Help with login issues",
  "text": "Thank you! That fixed the issue.",
  "attachments": []
}
```

### Patch Case
```json
{
  "status": "resolved",
  "updated": "2024-01-15T13:00:00Z",
  "customFields": [
    {
      "name": "resolutionTime",
      "value": "3 hours"
    },
    {
      "name": "satisfaction",
      "value": "5"
    }
  ],
  "onlyUpdateCustomFields": false
}
```

## Validation Requirements

Cases are validated with the following rules:
- **caseUid**: Required, cannot be null, empty, or whitespace
- **source**: Required, cannot be null, empty, or whitespace  
- **created**: Required, must be valid timestamp
- **updated**: Required, must be valid timestamp
- **subject**: Required, cannot be empty or whitespace
- **tags**: Cannot contain null values or duplicates, all tags must be non-empty
- **customFields**: Maximum 10 custom fields allowed, keys must match pattern `/^[a-zA-Z0-9]+$/`
- **messages**: Required for create/update operations, cannot be null or empty

## Custom Fields

Custom fields provide flexible metadata storage with the following constraints:
- Maximum 10 custom fields per case
- Field names must contain only alphanumeric characters
- Field values are stored as strings
- Custom fields can be used for filtering, reporting, and business logic

## Error Handling

| Status Code | Description |
|-------------|-------------|
| 400 | Bad Request - Invalid JSON, missing required fields, or validation errors |
| 500 | Internal Server Error - User creation failed or downstream service issues |

## Implementation Notes

- API schema may change without notice - implement defensive parsing
- Custom field keys are validated server-side with regex `/^[a-zA-Z0-9]+$/`
- HTML in message text is automatically stripped using server-side sanitization
- For support issues, reference this documentation or contact support directly