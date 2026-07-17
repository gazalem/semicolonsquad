# .Net secrets

From the project folder (the one containing the `.csproj`), they should run:
```bash
dotnet user-secrets init
```

Then add the Client ID:
```bash
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
```
And add the Client Secret:
```bash
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

For example:
```bash
dotnet user-secrets set "Authentication:Google:ClientId" "1234567890-abcdefghijklmnopqrstuvwxyz.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "GOCSPX-xxxxxxxxxxxxxxxxxxxxxxxx"
```

They can verify that everything was saved correctly with:
```bash
dotnet user-secrets list
```

