package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/google/uuid"
	"io"
	"log"
	"mime/multipart"
	"net/http"
	"net/url"
	"os"
	"path/filepath"
	"time"
)

const apiURL = "https://integrations.capturi.ai/v1/"

func main() {
	//get api token from environment
	apiToken := os.Getenv("API-TOKEN")
	if apiToken == "" {
		log.Fatal("No api token found")
	}

	//setup base url
	base, err := url.Parse(apiURL)
	if err != nil {
		log.Fatalf("Failed to parse url. Error: %v", err)
	}

	//Create conversation
	createConversationRequest := CreateConversationRequest{
		ExternalID:       uuid.NewString(),
		NumberOfSpeakers: 1,
		PhoneNumber:      "11223344",
		Title:            "Demo call",
		Labels:           nil,
		DateTime:         time.Now(),
		Outcome:          "Success",
		OutcomeReason:    "We have a great product",
		AgentID:          uuid.NewString(),
		AgentName:        "Agent Schmidt",
		AgentEmail:       "agent-schmidt@capturi.com",
	}

	createConversationURL := *base
	createConversationURL.Path += "conversation"
	//body
	body := new(bytes.Buffer)
	err = json.NewEncoder(body).Encode(createConversationRequest)
	if err != nil {
		log.Fatalf("Failed to encode create conversation request. Error: %v", err)
	}

	req, err := http.NewRequest("POST", createConversationURL.String(), body)
	if err != nil {
		log.Fatalf("Failed to create http request. Error: %v", err)
	}

	//Add authorization header.
	req.Header.Add("Authorization", apiToken)

	httpClient := http.Client{Timeout: 30 * time.Second}
	resp, err := httpClient.Do(req)
	if err != nil {
		log.Fatalf("Failed to send http request. Error: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode >= 300 {
		//Response body is an error message. Let's log that.
		respBody, err := io.ReadAll(resp.Body)
		if err != nil {
			log.Fatalf("Failed to read response body. Error: %v", err)
		}
		log.Printf("Response: %s", string(respBody))
		log.Fatalf("Http request return with statuscode not ok, statuscode: %d", resp.StatusCode)
	}

	//Get conversation uid from response.
	var conversationCreatedResponse ConversationCreatedResponse
	err = json.NewDecoder(resp.Body).Decode(&conversationCreatedResponse)
	if err != nil {
		log.Fatalf("Failed to decode response. Error: %v", err)
	}

	log.Printf("created a new conversation with uid: %s, and external id: %s", conversationCreatedResponse.UID, createConversationRequest.ExternalID)

	//Let's add some audio to the conversation.

	file, _ := os.Open("test-recording.wav")
	defer file.Close()

	body = &bytes.Buffer{}
	writer := multipart.NewWriter(body)
	part, _ := writer.CreateFormFile("data", filepath.Base(file.Name()))
	_, err = io.Copy(part, file)
	if err != nil {
		log.Fatalf("Failed to create multipart file. Error: %v", err)
	}
	writer.Close()

	uploadAudioURL := *base
	uploadAudioURL.Path += fmt.Sprintf("audio/%s", conversationCreatedResponse.UID)

	req, err = http.NewRequest("POST", uploadAudioURL.String(), body)
	if err != nil {
		log.Fatalf("Failed to create upload audio request. Error: %v", err)
	}

	req.Header.Add("Content-Type", writer.FormDataContentType())

	//Add authorization header.
	req.Header.Add("Authorization", apiToken)

	resp, err = httpClient.Do(req)
	if err != nil {
		log.Fatalf("Failed to send http request. Error: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode >= 300 {
		//Response body is an error message. Let's log that.
		respBody, err := io.ReadAll(resp.Body)
		if err != nil {
			log.Fatalf("Failed to read response body. Error: %v", err)
		}
		log.Printf("Response: %s", string(respBody))
		log.Fatalf("Http request return with statuscode not ok, statuscode: %d", resp.StatusCode)
	}

	log.Printf("Audio uploaded with statuscode : %d", resp.StatusCode)

}

type CreateConversationRequest struct {
	ExternalID       string    `json:"externalId"`
	NumberOfSpeakers int       `json:"numberOfSpeakers"`
	PhoneNumber      string    `json:"phoneNumber"`
	Title            string    `json:"title"`
	Labels           []string  `json:"labels"`
	DateTime         time.Time `json:"datetime"`
	Outcome          string    `json:"outcome"`
	OutcomeReason    string    `json:"outcomeReason"`
	AgentID          string    `json:"agentId"`
	AgentName        string    `json:"agentName"`
	AgentEmail       string    `json:"agentEmail"`
	Nps              float32   `json:"nps"`
	Score            string    `json:"score"`
	Direction        string    `json:"direction"`
	TimeToAnswer     int32     `json:"timeToAnswer"`
	Channel          string    `json:"channel"`
	CaseID           string    `json:"caseId"`
}

type ConversationCreatedResponse struct {
	UID string
}
