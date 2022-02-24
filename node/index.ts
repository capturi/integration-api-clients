import { randomBytes } from "crypto";

import { readFile } from "fs/promises";
import { FormData, fetch } from "undici";

const apiToken = "your-api-token";
const apiURL = "https://integrations.capturi.ai/v1/";

const randomId = () => randomBytes(20).toString("hex");

(async () => {
  try {
    const ourOwnInternalCallId = randomId();

    const call: CreateConversationRequest = {
      externalId: ourOwnInternalCallId,
      numberOfSpeakers: 1,
      phoneNumber: "11223344",
      title: "Demo call",
      labels: [],
      dateTime: new Date(),
      outcome: "Success",
      outcomeReason: "We have a great product",
      agentId: randomId(),
      agentName: "Agent Schmidt",
      agentEmail: "agent-schmidt@capturi.com",
    };

    const rawResponse = await fetch(`${apiURL}conversation`, {
      headers: {
        Authorization: apiToken,
      },
      method: "POST",
      body: JSON.stringify(call),
    });

    if (rawResponse.status >= 300) {
      const { status, statusText } = rawResponse;
      throw Error(`${status}: ${statusText} - ${await rawResponse.text()}`);
    }

    const response = (await rawResponse.json()) as { UID: string };
    console.log("Conversation id: ", response.UID);

    const file = await readFile("./test-recording.wav");
    const form = new FormData();
    form.append("data", file);

    const rawFileResponse = await fetch(
      `${apiURL}audio/external${ourOwnInternalCallId}`,
      {
        headers: {
          Authorization: apiToken,
        },
        method: "POST",
        body: form,
      }
    );

    if (rawFileResponse.status >= 300) {
      const { status, statusText } = rawFileResponse;
      throw Error(`${status}: ${statusText} - ${await rawFileResponse.text()}`);
    }
  } catch (error) {
    console.error(error);
  }
})();

type CreateConversationRequest = {
  externalId: string;
  title: string;
  numberOfSpeakers: 1 | 2;
  dateTime: Date;
  caseId?: string;
  labels: string[];
  phoneNumber: string;
  agentId: string;
  agentEmail?: string;
  agentName: string;
  channel?: "callback" | "phone";
  direction?: "inbound" | "outbound" | "internal";
  nps?: number;
  outcome?: string;
  outcomeReason?: string;
  score?: string;
  timeToAnswer?: number;
};
