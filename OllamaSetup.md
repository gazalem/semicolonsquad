# Ollama Setup Guide

## 1. Install Ollama

### Windows
1. Go to [https://ollama.com/download](https://ollama.com/download) and click **Download for Windows**.
2. Run the `.exe` installer and follow the prompts.
3. Ollama runs as a background service automatically after installation.

### macOS
1. Go to [https://ollama.com/download](https://ollama.com/download) and click **Download for Mac**.
2. Open the `.dmg` file and drag Ollama to your Applications folder.
3. Launch Ollama from Applications — it will run in the menu bar.

**Or via Homebrew:**
```bash
brew install ollama
```

### Linux
Run the official install script in your terminal:
```bash
curl -fsSL https://ollama.com/install.sh | sh
```
Ollama will be installed and started as a systemd service.

---

## 2. Pull the gemma3:4b Model

Once Ollama is installed and running, open a terminal and run:

```bash
ollama pull gemma3:4b
```

This downloads the model (~3 GB). When it finishes you should see:

```
success
```

---

## 3. Verify It Works

Run a quick test:
```bash
ollama run gemma3:4b "Say hello!"
```

You should get a response from the model in your terminal.

---

## Troubleshooting

| Issue | Fix |
|-------|-----|
| `ollama: command not found` | Restart your terminal or re-run the installer |
| Connection refused on Windows | Open Task Manager and check that the Ollama service is running |
| Slow download | The model is ~3 GB — wait for it to complete before running |
