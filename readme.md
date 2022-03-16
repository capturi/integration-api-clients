# Capturi Integrations demo clients

This repository contains demo applications for capturi integrations api. 

Swagger docs are available at  https://integrations.capturi.ai/swagger/index.html

To get an api token or if you have technical questions contact us at integrations@capturi.com, you are also welcome to create an issue on github.


# API Documentation

## Auth

The token must be provided in either the "Authorization" header (recommended, as it won't show up i etc. firewall logs), or as a query parameter. ```?api-token=```

## Endpoints

The base url is https://integrations.capturi.ai

### Create conversation

External-id should be a unique id for each call/conversation

* Path: /v1/conversation
* Method: POST
* Content-Type: json
* Model:  
   ```
  { 
    externalId: "string",
    numberOfSpeakers: int,
    phoneNumber: "string",
    title: "string",
    labels: ["string", "array"],
    datetime: datetime,
    outcome: "string",
    outcomeReason: "string",
    agentId : "string",
    agentName: "string",
    agentEmail: "string",
    nps: float,
    score: "string",
    direction: "string",
    timeToAnswer: int,
    channel: "string",
    caseId: "string"  
  }
  ```
* Returns: Capturi conversation id.

### Audio
 
* Path: /v1/audio/{capturi-conversation-id}
* Method: POST / MultiPart file upload
* Model: File must be added to the form in a key called "data"
* Returns: OK

* Path: /v1/audio/external/{external-id} //external-id, must be the same as used in create conversation endpoint.
* Method: POST / MultiPart file upload
* Model: File must be added to the form in a key called "data"
* Returns: OK
