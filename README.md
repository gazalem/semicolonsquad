# Smart Food Planner

**CSE 325 — Semester Project**

## Group Members

- Adam James Nielsen
- Alan Montoya
- Ernesto Quispe Cajan
- Daniel Nthengu
- Abraham Moreno-Sanchez

Group Leader: Ernesto Quispe

---

## AI Setup — Ollama (Local)

This project uses [Ollama](https://ollama.com) to run the `gemma3:4b` model locally.
No API key is required — the model runs on your machine.

### 1. Install Ollama

**macOS**

```bash
brew install ollama
```

Or download the desktop app from [ollama.com/download](https://ollama.com/download).

**Windows**

Download and run the installer from [ollama.com/download](https://ollama.com/download).
After install, Ollama runs as a background service automatically.

**Linux**

```bash
curl -fsSL https://ollama.com/install.sh | sh
```

### 2. Pull the model

```bash
ollama pull gemma3:4b
```

### 3. Start the Ollama server (Linux / Windows CLI only)

macOS and Windows desktop apps start the server automatically.
On Linux, run:

```bash
ollama serve
```

The server listens on `http://localhost:11434` by default.

### 4. Verify the setup

**Option A — VS Code REST Client (recommended)**

1. Install the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension in VS Code.
2. Open `Request.http` at the root of the project.
3. Click **Send Request** above `### TEST ENDPOINT` to confirm Ollama is running.
   You should receive a JSON response with a `"done": true` field.
4. Click **Send Request** above `### Request of Menu` to test the full meal plan
   generation with the structured JSON format schema.

**Option B — curl**

```bash
curl http://localhost:11434/api/generate -d '{
  "model": "gemma3:4b",
  "prompt": "Say hello in one sentence.",
  "stream": false
}'
```

---

## Running the App

```bash
dotnet run
```

> The AI service returns stub data by default (Card 2.7). Real Ollama calls are wired up in Sprint 3 (Card 3.1).

---

## Branch Naming

Feature branches follow the pattern `feature/card-name`, e.g. `AIService/Card2.7`.
`main` is the protected branch — all changes go through Pull Requests.
