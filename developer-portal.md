# Developer Portal

The Developer Portal is a self-service web dashboard for inspecting your integration traffic against the Capturi Integrations API. Use it to watch requests live, debug errors, browse the conversations you've created, and ask an AI assistant for help building your integration.

Access is granted through short-lived **magic links** — no separate portal login to manage.

## Base URL

```
https://integrations.capturi.ai
```

## Quick start

1. **Create a magic link** with your API token (see [below](#create-a-magic-link)).
2. **Open the returned URL** in a browser. It signs you straight into the portal for your organisation.
3. **Explore** the dashboard, request log, conversations, errors, and AI assistant.

The link is valid for 24 hours. When it expires, create a new one.

---

## Create a Magic Link

**`POST /v1/dev/magic-link`**

Authenticated with your normal API token (see [readme.md](readme.md#authentication)). The link inherits the organisation scope of the token that created it, so portal data is limited to that organisation.

### Request Body

```json
{
  "name": "Jane Developer",
  "email": "jane@partner.example",
  "reasonForAccess": "Debugging conversation upload integration",
  "singleUse": false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `name` | string | ✅ | Name of the person the link is for |
| `email` | string | ✅ | Email of the person the link is for |
| `reasonForAccess` | string | ✅ | Why access is being requested (recorded for audit) |
| `singleUse` | bool | ❌ | When `true`, the link can only be used once. Defaults to `false`. |

### Response

```json
{
  "url": "https://integrations.capturi.ai/dev/8f3c…<64-char token>",
  "expiresAt": "2026-07-09T12:00:00Z"
}
```

| Field | Type | Description |
|-------|------|-------------|
| `url` | string | The magic link. Open in a browser to enter the portal. |
| `expiresAt` | string (ISO 8601) | When the link expires (24 hours after creation). |

### Notes

- **Expiry:** links expire 24 hours after creation.
- **Rate limit:** up to **5 links per hour per API token**. Exceeding this returns `429`.
- **Treat the URL as a secret** — anyone with it can view your organisation's portal data until it expires. The token is shown only once, in the response, and cannot be retrieved again.

| Status Code | Description |
|-------------|-------------|
| 200 | Success — link created |
| 400 | Bad Request — missing `name`, `email`, or `reasonForAccess` |
| 429 | Too Many Requests — hourly link limit reached |

---

## Using the Portal

Open the magic-link URL to land on the dashboard at `/dev/{token}`. All views are scoped to your organisation.

### Landing page

`GET /dev` is a public landing page where you can paste a magic-link token if you have one but not the full URL.

### Dashboard views

| View | What it shows |
|------|---------------|
| **Dashboard** | At-a-glance statistics (request volume, success rate, average duration) with an AI summary. |
| **Requests** | Every API request your integration has made, with method, endpoint, status, and duration. Open any request for full detail (headers, body, response). |
| **Conversations** | The conversations your organisation has created, searchable by external ID or Capturi UID. |
| **Errors** | Requests that failed (status ≥ 400), for quick debugging. |
| **Live feed** | A real-time stream of incoming requests as they happen. |
| **Docs** | The API documentation, embedded in the portal. |

---

## Portal JSON API

The portal also exposes a read-only JSON API under the same magic-link token, useful for programmatic access or building your own tooling. All endpoints are authenticated by the `{token}` path segment.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/dev/{token}/api/requests` | List request logs (paginated; filter by `method`, `status`, `errors_only`) |
| GET | `/dev/{token}/api/requests/{id}` | Full detail for a single request |
| GET | `/dev/{token}/api/errors` | List failed requests (status ≥ 400) |
| GET | `/dev/{token}/api/stats` | Dashboard statistics (`period` = `1h`, `24h`, `7d`, `30d`) |
| GET | `/dev/{token}/api/conversations` | List conversations (paginated) |
| GET | `/dev/{token}/api/conversations/{externalId}` | Get a conversation by external ID or UID |

Pagination uses `limit` / `offset` for request logs and errors, and `limit` / `continuationToken` for conversations.

---

## AI Assistant

The portal includes an AI assistant, purpose-built to help you integrate with the Capturi Integrations API. It is available when AI features are enabled for the environment.

### Dashboard summary

The dashboard shows an AI-generated summary of your recent integration activity — request volume, success rate, and anything notable in your traffic — written in plain language. Summaries are cached and can be refreshed on demand.

### Chat assistant

An interactive chat assistant that can:

- **Answer integration questions** about the API, endpoints, and payloads.
- **Inspect your recent requests** — it can look at your actual request log to help explain what happened and why a call failed.
- **Generate code examples** for creating conversations, uploading audio, and calling the API in various languages.
- **Help you build integration clients** for Puzzel from scratch.

The assistant is scoped to your organisation and rate-limited (30 messages per hour). It is focused solely on Capturi API integration and will not answer off-topic questions.

---

## Support

- **Email:** integrations@capturi.com
- **GitHub Issues:** Create an issue on this repository
