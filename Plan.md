# Smart Food Planner — Trello Sprint Plan
**CSE 325 | 5 Weeks | Team: Ernesto (Backend), Abraham (Frontend), Adam (Auth/Frontend), Alan (AI/Backend)**

---

## Trello Board Setup

### Lists to Create (in this order)
1. **Sprint 1 · Foundation** — Week 1
2. **Sprint 2 · Auth & Ingredients** — Week 2
3. **Sprint 3 · AI & Meal Plans** — Week 3
4. **Sprint 4 · Cookbook & Features** — Week 4
5. **Sprint 5 · Deploy & QA** — Week 5
6. **In Progress**
7. **In Review** ← PR open, awaiting review
8. **Done**

### Labels
| Color | Label | Used for |
|-------|-------|---------|
| Blue | Backend | Database, services, models |
| Green | Frontend | Pages, components, CSS |
| Yellow | Auth | Authentication & authorization |
| Red | AI/API | AI integration, external APIs |
| Black | DevOps | CI/CD, deployment, infrastructure |
| Purple | QA | Testing, validation, accessibility |

### Team Members (add in Trello)
- **Ernesto** — Backend
- **Abraham** — Frontend
- **Adam** — Auth / Frontend
- **Alan** — AI / APIs / Backend

---

## Recommended Project Architecture

Agree on this folder structure before Sprint 1 starts. Simple, clean, follows .NET conventions:

```
SmartFoodPlanner/
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
├── Models/
│   ├── Ingredient.cs
│   ├── MealPlan.cs
│   ├── Recipe.cs
│   ├── RecipeStep.cs
│   └── Favorite.cs
├── Services/
│   ├── IIngredientService.cs + IngredientService.cs
│   ├── IMealPlanService.cs + MealPlanService.cs
│   ├── IAIService.cs + ClaudeAIService.cs
│   └── IFavoriteService.cs + FavoriteService.cs
├── Components/
│   └── Layout/
│       ├── MainLayout.razor
│       └── NavMenu.razor
└── Pages/
    ├── Auth/
    │   └── Login.razor          ← OAuth entry point (no Register page needed)
    ├── Ingredients.razor
    ├── MealPlan.razor
    ├── RecipeDetail.razor
    ├── Cookbook.razor
    ├── Favorites.razor
    ├── PastPlans.razor
    └── SharedRecipe.razor   ← public, no auth required
```

**Best practices to follow (simple rules for everyone):**
- Every page injects a service interface (not DbContext directly)
- Services always filter queries by `userId` — never return another user's data
- API keys always in environment variables — never committed to GitHub
- Use `async/await` everywhere that touches the DB or external APIs
- Register services as `Scoped` in `Program.cs`

---

## SPRINT 1 — Foundation & Setup
**Week 1 | Goal: Working scaffold, base layout, and database ready to go**

---

### Card 1.1 — Create Blazor Server Project & GitHub Repo
- **Assigned to**: Alan + Adam
- **Labels**: DevOps
- **Description**:
  Scaffold a new .NET 8 Blazor Server app called `Smart Food Planner`. Push to the group GitHub repo. Define the branch strategy: `main` is protected; each story gets its own feature branch (e.g., `feature/ingredients-crud`). Add all 4 members as collaborators.
- **Checklist**:
  - [ ] `dotnet new blazorserver -n SmartFoodPlanner --auth Individual`
  - [ ] Push to GitHub repo `gazalem/semicolonsquad` (or new repo)
  - [ ] Add all team members as collaborators
  - [ ] Add `.gitignore` for .NET (Visual Studio template)
  - [ ] Add `appsettings.Development.json` to `.gitignore` (for API keys later)
  - [ ] Document branch naming in README: `feature/card-name`

---

### Card 1.2 — Configure EF Core + SQLite Database
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Install EF Core with SQLite for local development. Create `AppDbContext.cs`. Add connection string to `appsettings.json`. Run the first migration (empty schema) to verify everything is wired correctly.
- **Checklist**:
  - [ ] Install packages: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Tools`
  - [ ] Create `Data/AppDbContext.cs`
  - [ ] Add connection string to `appsettings.json`: `"DefaultConnection": "Data Source=smartfoodplanner.db"`
  - [ ] Register in `Program.cs`: `builder.Services.AddDbContext<AppDbContext>(...)`
  - [ ] Run `dotnet ef migrations add InitialCreate && dotnet ef database update`
  - [ ] Verify `smartfoodplanner.db` file is created on app startup

---

### Card 1.3 — Configure ASP.NET Identity + Google OAuth
- **Assigned to**: Adam
- **Labels**: Auth, Backend
- **Description**:
  Add ASP.NET Identity (manages the local user record in the DB) and wire it to Google OAuth (handles the actual login — no password stored). Users will click "Sign in with Google" and Identity creates their account automatically on first login.

  **Before coding**: Adam must create a Google OAuth app in Google Cloud Console to get a `ClientId` and `ClientSecret`.
- **One-time setup (Google Cloud Console)**:
  1. Go to console.cloud.google.com → Create a new project
  2. APIs & Services → Credentials → Create Credentials → OAuth 2.0 Client ID
  3. Application type: **Web application**
  4. Authorized redirect URI (dev): `https://localhost:{port}/signin-google`
  5. Authorized redirect URI (prod): `https://{your-azure-url}/signin-google`
  6. Copy the `Client ID` and `Client Secret`
- **Checklist**:
  - [ ] Install packages:
    - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
    - `Microsoft.AspNetCore.Authentication.Google`
  - [ ] Update `AppDbContext` to extend `IdentityDbContext<IdentityUser>`
  - [ ] Configure in `Program.cs`:
    ```csharp
    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication()
        .AddGoogle(options => {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        });
    ```
  - [ ] Add to `appsettings.Development.json` (this file is gitignored):
    ```json
    {
      "Authentication": {
        "Google": {
          "ClientId": "YOUR_CLIENT_ID",
          "ClientSecret": "YOUR_CLIENT_SECRET"
        }
      }
    }
    ```
  - [ ] Run migration: `dotnet ef migrations add AddIdentity`
  - [ ] Verify `AspNetUsers` table exists in the DB

---

### Card 1.4 — Base Layout & Smart Food Planner Branding
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Set the visual identity for Smart Food Planner. Define CSS variables for the color palette (food-themed: greens and warm tones), font family, and spacing. Update `MainLayout.razor` and `NavMenu.razor` with the brand. The app should look intentional from day one.
- **Suggested palette**:
  - Primary: `#2D6A4F` (forest green)
  - Accent: `#F4A261` (warm orange)
  - Background: `#FAFAF8` (off-white)
  - Text: `#1A1A1A`
- **Checklist**:
  - [ ] Add CSS variables to `wwwroot/css/app.css`: `--color-primary`, `--color-accent`, `--color-bg`, `--font-body`
  - [ ] Update `NavMenu.razor` with brand name "Smart Food Planner" and placeholder nav links
  - [ ] Update `MainLayout.razor` with consistent header/content/footer structure
  - [ ] Verify renders correctly on 375px (mobile) and 1280px (desktop)

---

### Card 1.5 — GitHub Actions CI Pipeline
- **Assigned to**: Alan
- **Labels**: DevOps
- **Description**:
  Create a CI workflow that runs `dotnet build` on every push and pull request. This catches compilation errors before they get merged. Simple and fast — no tests needed yet.
- **Checklist**:
  - [ ] Create `.github/workflows/ci.yml`
  - [ ] Trigger on: `push` and `pull_request` to any branch
  - [ ] Steps: `checkout` → `setup-dotnet` → `dotnet build`
  - [ ] Push and verify the green checkmark appears on GitHub

---

### Card 1.6 — Set Up Trello Board & Import All Cards
- **Assigned to**: Alan (all members participate)
- **Labels**: DevOps
- **Description**:
  Create the Trello board "Smart Food Planner — CSE 325". Create all sprint lists, add all members, define labels, and import every card from this plan. Paste the Trello link into `projectProposal_en.md`.
- **Checklist**:
  - [ ] Create Trello board and set to "Team Visible"
  - [ ] Create all 8 lists (5 sprints + In Progress + In Review + Done)
  - [ ] Add Ernesto, Abraham, Adam, Alan as members
  - [ ] Create all labels (Backend, Frontend, Auth, AI/API, DevOps, QA)
  - [ ] Add all cards from this plan to their respective sprint lists
  - [ ] Paste board URL into `projectProposal_en.md` → Project Links section

---

## SPRINT 2 — Authentication & Ingredients CRUD
**Week 2 | Goal: Users can register, log in, and manage ingredients**

---

### Card 2.1 — OAuth Callback Handler & External Login Flow
- **Assigned to**: Adam
- **Labels**: Auth, Backend
- **Description**:
  With Google OAuth there is no registration form — the first time a user signs in with their Google account, ASP.NET Identity automatically creates their local user record. This card wires up the callback that handles the OAuth response and creates (or finds) the Identity user.

  Create a Razor Page (not a Blazor page) at `Areas/Identity/Pages/Account/ExternalLoginCallback.cshtml` — or use the built-in scaffolded version. The key logic: call `SignInManager.ExternalLoginSignInAsync()`, and if the user doesn't exist yet, call `UserManager.CreateAsync()` with the claims from the provider.
- **Checklist**:
  - [ ] Scaffold the Identity external login pages:
    ```bash
    dotnet aspnet-codegenerator identity -dc AppDbContext --files "Account.ExternalLogin"
    ```
  - [ ] In the callback, map the Google `email` claim to the new `IdentityUser`
  - [ ] On first login: create the user + add external login record (`UserManager.AddLoginAsync`)
  - [ ] On subsequent logins: find existing user by external login and sign in
  - [ ] Redirect to `/ingredients` after successful login
  - [ ] Redirect to `/login` with error message if Google login is denied or fails

---

### Card 2.2 — Login Page & Logout
- **Assigned to**: Adam
- **Labels**: Auth, Frontend
- **Description**:
  Build `Pages/Auth/Login.razor`. There is no email/password form — the page shows a single "Sign in with Google" button. Clicking it initiates the OAuth redirect via `SignInManager.GetExternalAuthenticationSchemesAsync()`. Add a Logout button to `NavMenu.razor`.
- **Checklist**:
  - [ ] Create `Pages/Auth/Login.razor` with route `@page "/login"`
  - [ ] Page shows the app logo, a tagline, and a "Sign in with Google" button
  - [ ] Button triggers OAuth redirect:
    ```csharp
    var redirectUrl = NavigationManager.BaseUri + "signin-google";
    var properties = SignInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
    // Use NavigationManager.NavigateTo with the challenge URL
    ```
  - [ ] No email/password fields anywhere
  - [ ] Add "Sign out" link in `NavMenu.razor` that calls `SignInManager.SignOutAsync()` then redirects to `/`
  - [ ] Test: clicking "Sign in with Google" opens Google's login screen

---

### Card 2.3 — Protected Routes (Auth Guards)
- **Assigned to**: Adam
- **Labels**: Auth
- **Description**:
  All pages except Login and SharedRecipe require authentication. Add `[Authorize]` to protected pages. Configure `Routes.razor` to redirect unauthenticated users to `/login`. There is no separate Register page — the Login page is the only entry point.
- **Checklist**:
  - [ ] Add `@attribute [Authorize]` to: Ingredients, MealPlan, Cookbook, Favorites, PastPlans
  - [ ] In `Routes.razor`, set `NotAuthorized` to redirect to `/login`
  - [ ] NavMenu shows "Sign in with Google" link when logged out
  - [ ] NavMenu shows user's Google profile name/email and "Sign out" when logged in
  - [ ] Test: navigate to `/ingredients` while logged out → redirected to `/login`
  - [ ] Test: complete Google OAuth → land on `/ingredients`

---

### Card 2.4 — Ingredient Model & Migration
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Create `Models/Ingredient.cs`. Fields: Id (int), UserId (string, FK to IdentityUser), Name (string, required), Quantity (decimal, nullable), Unit (string, nullable, e.g. "cups"), CreatedAt (DateTime). Add `DbSet<Ingredient>` to `AppDbContext`. Run migration.
- **Checklist**:
  - [ ] Create `Models/Ingredient.cs` with all fields
  - [ ] Add `DbSet<Ingredient> Ingredients` to `AppDbContext`
  - [ ] Run: `dotnet ef migrations add AddIngredient && dotnet ef database update`
  - [ ] Verify `Ingredients` table in DB with correct columns

---

### Card 2.5 — IngredientService (Backend)
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Create `Services/IIngredientService.cs` interface and `Services/IngredientService.cs` implementation. Always filter by `userId` so users can only see their own data. Register in `Program.cs`.
- **Methods to implement**:
  ```csharp
  Task<List<Ingredient>> GetIngredientsAsync(string userId);
  Task AddIngredientAsync(Ingredient ingredient);
  Task UpdateIngredientAsync(Ingredient ingredient);
  Task DeleteIngredientAsync(int id, string userId);
  ```
- **Checklist**:
  - [ ] Create `IIngredientService` interface
  - [ ] Implement all 4 methods in `IngredientService`
  - [ ] `DeleteIngredientAsync` verifies the ingredient belongs to `userId` before deleting
  - [ ] Register: `builder.Services.AddScoped<IIngredientService, IngredientService>()`

---

### Card 2.6 — Ingredients Page UI
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Build `Pages/Ingredients.razor`. Inject `IIngredientService`. On load, fetch and display the current user's ingredients in a clean card or list layout. Include Add, Edit, and Delete actions. Use inline editing or a simple modal for Add/Edit.
- **Checklist**:
  - [ ] List all ingredients with Name, Quantity, Unit displayed
  - [ ] "Add Ingredient" button opens a form (modal or inline section at top)
  - [ ] Edit: click a pencil icon to make fields editable inline → Save button
  - [ ] Delete: click trash icon → confirmation dialog ("Delete this ingredient?")
  - [ ] Empty state: "No ingredients yet — add your first one!"
  - [ ] Page shows user's name or a "Welcome back!" greeting

---

### Card 2.7 — AI Provider Setup & Service Skeleton
- **Assigned to**: Alan
- **Labels**: AI/API
- **Description**:
  Choose and install the AI SDK (Claude API recommended — Anthropic.SDK for .NET). Store the API key in `appsettings.Development.json` (gitignored). Create `IAIService` interface and a stub implementation that returns dummy data so other developers can build against it before real AI is wired up.
- **Checklist**:
  - [ ] Choose provider and install NuGet package (e.g., `Anthropic.SDK`)
  - [ ] Add API key to `appsettings.Development.json` (never commit this file)
  - [ ] Create `Services/IAIService.cs`:
    ```csharp
    public interface IAIService {
        Task<MealPlanResponse> GenerateMealPlanAsync(List<string> ingredients);
    }
    ```
  - [ ] Create `MealPlanResponse` DTO (record or class) with 7 `RecipeDto` items
  - [ ] Create stub `ClaudeAIService` that returns hardcoded dummy data
  - [ ] Register: `builder.Services.AddScoped<IAIService, ClaudeAIService>()`
  - [ ] Share `MealPlanResponse` and `RecipeDto` classes with Ernesto (he'll need them for models)

---

## SPRINT 3 — AI Integration & Meal Plan
**Week 3 | Goal: AI generates a real 7-day meal plan; users can view recipe details**

---

### Card 3.1 — AI Prompt Engineering & Real Implementation
- **Assigned to**: Alan
- **Labels**: AI/API
- **Description**:
  Replace the stub `ClaudeAIService` with a real implementation. Design a prompt that sends the user's ingredients and requests a structured JSON response with 7 meals. Parse the JSON into typed C# objects.
- **Prompt structure**:
  ```
  System: "You are a helpful meal planning assistant. Always respond with valid JSON only."
  User: "I have these ingredients: {ingredients}. Generate a 7-day meal plan using only 30-minute meals.
  Return JSON with this structure:
  { "meals": [ { "day": 1, "name": "...", "estimatedMinutes": 25,
    "ingredients": ["..."], "steps": ["..."] } ] }"
  ```
- **Checklist**:
  - [ ] Replace stub implementation with real API call
  - [ ] System prompt sets JSON-only response mode
  - [ ] User prompt passes ingredients as a comma-separated list
  - [ ] Deserialize JSON response into `MealPlanResponse` DTO
  - [ ] Test with a real API call in debug mode — verify 7 meals are returned
  - [ ] Log request and response for debugging (no API key in logs)

---

### Card 3.2 — MealPlan & Recipe Database Models
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Create the data models to persist AI-generated meal plans. Coordinate with Alan on the DTO structure before writing models to ensure they align.
- **Models**:
  - `MealPlan`: Id, UserId, GeneratedAt, List\<Recipe\>
  - `Recipe`: Id, MealPlanId, DayNumber, Name, EstimatedMinutes, ShareToken (GUID string), List\<RecipeStep\>
  - `RecipeStep`: Id, RecipeId, StepNumber, Description
- **Checklist**:
  - [ ] Create all 3 model files in `Models/`
  - [ ] Set up EF relationships: MealPlan →(1:N)→ Recipe →(1:N)→ RecipeStep
  - [ ] Add `DbSet` entries in `AppDbContext` for all 3 models
  - [ ] Run: `dotnet ef migrations add AddMealPlanRecipes`
  - [ ] Verify all tables and foreign keys are created correctly

---

### Card 3.3 — MealPlanService (Backend)
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Create `Services/IMealPlanService.cs` and `MealPlanService.cs`. The save method maps Alan's DTO objects into EF entities. Queries always scope to `userId`.
- **Methods**:
  ```csharp
  Task<MealPlan> SaveMealPlanAsync(string userId, MealPlanResponse dto);
  Task<List<MealPlan>> GetMealPlansAsync(string userId);
  Task<MealPlan?> GetMealPlanByIdAsync(int id, string userId);
  Task<Recipe?> GetRecipeByIdAsync(int recipeId);
  Task<Recipe?> GetRecipeByTokenAsync(string shareToken);
  ```
- **Checklist**:
  - [ ] Implement all 5 methods
  - [ ] `SaveMealPlanAsync` maps DTO → Entity and generates a GUID `ShareToken` per recipe
  - [ ] Register as scoped in `Program.cs`
  - [ ] Test `SaveMealPlanAsync` in debug mode with real AI output

---

### Card 3.4 — Generate Meal Plan Page
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Build `Pages/MealPlan.razor`. The page has a "Generate This Week's Plan" button. While generating, show a loading spinner and disable the button. Once done, display 7 day cards in a grid (Mon–Sun), each showing the meal name and estimated time.
- **Checklist**:
  - [ ] "Generate" button: calls `IAIService.GenerateMealPlanAsync()` then saves via `IMealPlanService`
  - [ ] Loading state: spinner + "Cooking up your plan..." message + button disabled
  - [ ] Weekly grid: 7 cards labeled Day 1 (Monday) through Day 7 (Sunday)
  - [ ] Each card: meal name, estimated time badge (e.g. "25 min")
  - [ ] Click any day card → navigate to `/recipe/{recipeId}`
  - [ ] Error message banner if AI call fails (styled, not a browser alert)

---

### Card 3.5 — Recipe Detail Page
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Build `Pages/RecipeDetail.razor`. Takes `RecipeId` as a route parameter. Fetches the recipe and displays: name, estimated time, ingredient list (bulleted), and numbered step-by-step instructions. Include a Favorite button (wired in Sprint 4) and a Share button (wired in Sprint 4).
- **Checklist**:
  - [ ] Route: `@page "/recipe/{RecipeId:int}"`
  - [ ] Fetch recipe via `IMealPlanService.GetRecipeByIdAsync(RecipeId)`
  - [ ] Show: name, "⏱ X min" badge, ingredient list, numbered steps
  - [ ] Placeholder "★ Favorite" button (not wired yet — Sprint 4)
  - [ ] Placeholder "🔗 Share" button (not wired yet — Sprint 4)
  - [ ] Show "Recipe not found" if ID is invalid

---

### Card 3.6 — Past Meal Plans Page
- **Assigned to**: Ernesto
- **Labels**: Backend, Frontend
- **Description**:
  Build `Pages/PastPlans.razor`. Lists all meal plans the user has previously generated, sorted newest first. Each entry shows the generation date and links to that week's meal plan view.
- **Checklist**:
  - [ ] Fetch all meal plans via `IMealPlanService.GetMealPlansAsync(userId)`
  - [ ] Display: date generated, number of recipes (always 7)
  - [ ] Click on a plan → show that week's 7-day grid (reuse the MealPlan page or a detail view)
  - [ ] Empty state: "No plans yet — generate your first one!"

---

### Card 3.7 — AI Error Handling
- **Assigned to**: Alan
- **Labels**: AI/API, Backend
- **Description**:
  Wrap the AI service call in proper error handling. Cover three failure scenarios: timeout, API quota/auth error, and JSON parse failure. Each shows a user-friendly message in the UI. Log the full technical error server-side for debugging.
- **Checklist**:
  - [ ] Set HTTP timeout to 30 seconds on the API client
  - [ ] Catch `TimeoutException` → show: "The AI is taking too long. Please try again."
  - [ ] Catch HTTP 429 / 401 → show: "Service temporarily unavailable."
  - [ ] Catch `JsonException` → show: "Could not process the AI response. Please try again."
  - [ ] Log all exceptions with `ILogger` (not Console.WriteLine) for production visibility

---

## SPRINT 4 — Cookbook, Favorites & Sharing
**Week 4 | Goal: Complete all remaining functional requirements (FR-06 through FR-10)**

---

### Card 4.1 — Favorite Model & FavoriteService
- **Assigned to**: Ernesto
- **Labels**: Backend
- **Description**:
  Create `Models/Favorite.cs` (Id, UserId, RecipeId, CreatedAt). Run migration. Create `IFavoriteService` and `FavoriteService` with toggle, check, and list methods.
- **Methods**:
  ```csharp
  Task ToggleFavoriteAsync(string userId, int recipeId);
  Task<bool> IsFavoriteAsync(string userId, int recipeId);
  Task<List<Recipe>> GetFavoriteRecipesAsync(string userId);
  ```
- **Checklist**:
  - [ ] Create `Models/Favorite.cs` with a unique index on (UserId, RecipeId)
  - [ ] Run: `dotnet ef migrations add AddFavorites`
  - [ ] Implement `ToggleFavoriteAsync`: if exists → delete; if not → create
  - [ ] Register: `builder.Services.AddScoped<IFavoriteService, FavoriteService>()`

---

### Card 4.2 — Favorite Toggle on Recipe Detail
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Wire up the Favorite button on `RecipeDetail.razor`. On page load, check if the recipe is already favorited. Button shows "★ Favorited" (filled) or "☆ Add to Favorites" (outline). Click toggles the state instantly without a full page reload.
- **Checklist**:
  - [ ] On `OnInitializedAsync`, call `IFavoriteService.IsFavoriteAsync(userId, RecipeId)`
  - [ ] Button label and style change based on `isFavorited` bool
  - [ ] Click handler calls `ToggleFavoriteAsync` then flips `isFavorited`
  - [ ] Button only visible when user is authenticated (use `<AuthorizeView>`)

---

### Card 4.3 — Favorites Page
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Build `Pages/Favorites.razor`. Displays all recipes the user has starred, as cards in a grid. Each card shows the recipe name, estimated time, and a "View Recipe" link.
- **Checklist**:
  - [ ] Fetch favorites via `IFavoriteService.GetFavoriteRecipesAsync(userId)`
  - [ ] Display as a responsive grid of recipe cards
  - [ ] Card: recipe name, "⏱ X min", "View Recipe →" link
  - [ ] Empty state: "No favorites yet — star a recipe to save it here!"

---

### Card 4.4 — Cookbook Page
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Build `Pages/Cookbook.razor`. Shows every recipe ever generated by the user across all meal plans. Include a live search bar that filters recipes by name as the user types.
- **Checklist**:
  - [ ] Fetch all recipes from all of the user's meal plans
  - [ ] Search bar: bind to a string variable, filter the displayed list in real time
  - [ ] Grid of recipe cards (same component as Favorites if possible)
  - [ ] Empty state: "Your cookbook is empty — generate your first meal plan!"

---

### Card 4.5 — Shareable Recipe Link
- **Assigned to**: Alan
- **Labels**: AI/API, Backend
- **Description**:
  Each recipe already has a `ShareToken` GUID (added in Sprint 3). Build the public-facing page `Pages/SharedRecipe.razor` at route `/share/{token}`. No `[Authorize]` attribute — anyone with the link can view. Wire up the "Share" button on Recipe Detail to copy the URL to the clipboard.
- **Checklist**:
  - [ ] Create `Pages/SharedRecipe.razor` with route `@page "/share/{Token}"`
  - [ ] No `[Authorize]` on this page
  - [ ] Fetch recipe via `IMealPlanService.GetRecipeByTokenAsync(Token)`
  - [ ] Show read-only recipe detail (name, ingredients, steps — no Favorite/Share buttons)
  - [ ] Show "Recipe not found" if token is invalid
  - [ ] On `RecipeDetail.razor`: wire "Share" button to copy `https://{host}/share/{shareToken}` to clipboard using JS interop

---

### Card 4.6 — Navigation Updates
- **Assigned to**: Adam
- **Labels**: Frontend
- **Description**:
  Update `NavMenu.razor` to include all app pages. Show nav links only for authenticated users (except Login/Register). Highlight the currently active page.
- **Nav links (authenticated)**:
  - Ingredients → `/ingredients`
  - Generate Plan → `/mealplan`
  - Cookbook → `/cookbook`
  - Favorites → `/favorites`
  - Past Plans → `/pastplans`
  - Logout
- **Nav links (unauthenticated)**: "Sign in with Google" → `/login`
- **Checklist**:
  - [ ] All links present and correct in NavMenu
  - [ ] Active page link has a distinct style (bold or color change)
  - [ ] Authenticated-only links are inside `<AuthorizeView>`
  - [ ] Test on mobile — nav should not overflow horizontally

---

### Card 4.7 — Responsive Design Audit
- **Assigned to**: Adam
- **Labels**: Frontend, QA
- **Description**:
  Test every page at three breakpoints using browser DevTools: 375px (iPhone SE), 768px (iPad), 1280px (desktop). Fix layout issues — overflowing text, broken buttons, squished forms, or unreadable cards.
- **Checklist**:
  - [ ] Ingredients page: form and list stack cleanly on mobile
  - [ ] Meal plan grid: 1 column on mobile, 2 on tablet, 3-4 on desktop
  - [ ] Recipe detail: ingredients and steps are readable on 375px
  - [ ] Cookbook/Favorites: grid adjusts column count
  - [ ] NavMenu: collapses on mobile or uses a hamburger icon
  - [ ] All buttons are at least 44×44px touch target on mobile

---

## SPRINT 5 — Deploy, Polish & QA
**Week 5 | Goal: App live on Azure, all requirements met, ready to demo**

---

### Card 5.1 — Deploy to Azure App Service
- **Assigned to**: Alan
- **Labels**: DevOps
- **Description**:
  Create an Azure App Service (Free or Student tier). Create a `cd.yml` GitHub Actions workflow that publishes and deploys the app to Azure on every push to `main`.
- **Checklist**:
  - [ ] Create Azure App Service in Azure Portal (name: `smartfoodplanner` — no spaces, all lowercase)
  - [ ] Download the Publish Profile from Azure
  - [ ] Add Publish Profile to GitHub Secrets: `AZURE_WEBAPP_PUBLISH_PROFILE`
  - [ ] Create `.github/workflows/cd.yml`:
    - `dotnet publish -c Release`
    - Deploy to Azure using `azure/webapps-deploy` action
  - [ ] Trigger on push to `main`
  - [ ] Verify app loads at `https://smartfoodplanner.azurewebsites.net` (or your chosen URL)

---

### Card 5.2 — Production Database & Environment Variables
- **Assigned to**: Alan + Ernesto
- **Labels**: DevOps, Backend
- **Description**:
  Configure production secrets in Azure App Service (Application Settings). Apply EF migrations on startup. Verify auth and CRUD work in production.
- **Checklist**:
  - [ ] Add to Azure App Service → Configuration → Application Settings:
    - `ConnectionStrings__DefaultConnection` (SQLite path or Azure SQL string)
    - `AIService__ApiKey` (AI API key — never in code)
    - `Authentication__Google__ClientId` (from Google Cloud Console)
    - `Authentication__Google__ClientSecret` (from Google Cloud Console)
  - [ ] In Google Cloud Console, add the Azure URL to the OAuth app's authorized redirect URIs: `https://{your-azure-url}/signin-google`
  - [ ] In `Program.cs`, run migrations on startup:
    ```csharp
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
    ```
  - [ ] Test in production: click "Sign in with Google" → OAuth flow completes → land on `/ingredients`

---

### Card 5.3 — Accessibility Audit (WCAG 2.1 AA)
- **Assigned to**: Adam
- **Labels**: QA
- **Description**:
  Run Lighthouse on all 5 main pages in the deployed app. Fix any accessibility issues to reach a score ≥ 90. Common fixes: missing `alt`, icon-only buttons without labels, low color contrast, form inputs without `<label>`.
- **Checklist**:
  - [ ] Run Lighthouse on: Home, Ingredients, MealPlan, RecipeDetail, Cookbook
  - [ ] Target: Accessibility score ≥ 90 on all pages
  - [ ] Fix: add `alt=""` or `aria-label` to all images and icon buttons
  - [ ] Fix: verify color contrast ratio ≥ 4.5:1 for all body text
  - [ ] Fix: every `<input>` has a paired `<label>` element
  - [ ] Test keyboard navigation: Tab key reaches all interactive elements in logical order

---

### Card 5.4 — HTML Validation & Console Error Cleanup
- **Assigned to**: Adam
- **Labels**: QA
- **Description**:
  Paste each page's rendered HTML into the W3C Markup Validator. Fix any errors (bad nesting, missing closing tags, invalid attributes). Open browser DevTools and resolve all console errors.
- **Checklist**:
  - [ ] Validate all 5 main pages via W3C validator — zero errors
  - [ ] Zero red errors in browser console on page load
  - [ ] Any warnings reviewed and justified or fixed

---

### Card 5.5 — Empty States & UX Polish
- **Assigned to**: Abraham
- **Labels**: Frontend
- **Description**:
  Ensure every page that can show zero data has a helpful empty state message. Confirm all async operations show a loading spinner. Do a final visual pass for consistent spacing, typography, and button styles.
- **Checklist**:
  - [ ] Empty states on: Ingredients, Cookbook, Favorites, Past Plans, MealPlan (no plan generated yet)
  - [ ] Loading spinners on: Generate Plan button, all initial page loads
  - [ ] Consistent button styles: primary (filled, brand color), secondary (outlined)
  - [ ] Consistent page header: every page has a `<h1>` and optional subtitle
  - [ ] No orphaned styles or leftover scaffolding CSS from the template

---

### Card 5.6 — End-to-End QA Pass (All 10 FRs)
- **Assigned to**: Ernesto (leads) + all members test features they didn't build
- **Labels**: QA
- **Description**:
  Test every functional requirement in the live Azure production environment. Each developer tests features built by their teammates, not their own. Any bug found gets a Trello card immediately and fixed before the demo.
- **Checklist (test in production)**:
  - [ ] FR-01: Sign in with Google for the first time → account auto-created, lands on `/ingredients` ✓
  - [ ] FR-02: Sign out; click "Sign in with Google" again → returns to `/ingredients` ✓
  - [ ] FR-03: Add 5 ingredients; edit one; delete one ✓
  - [ ] FR-04: Click "Generate Plan" — see 7-day plan appear ✓
  - [ ] FR-05: Click any day card — see full recipe with steps ✓
  - [ ] FR-06: Mark a recipe as favorite; verify it fills/unfills ✓
  - [ ] FR-07: Go to Favorites — see the favorited recipe ✓
  - [ ] FR-08: Copy share link; open in an Incognito tab — see recipe ✓
  - [ ] FR-09: Go to Past Plans — see the generated plan listed ✓
  - [ ] FR-10: Open the share link without being logged in — recipe loads ✓

---

### Card 5.7 — User Documentation (Getting Started Guide)
- **Assigned to**: Abraham
- **Labels**: QA
- **Description**:
  Write a short "Getting Started" section in the GitHub repo README. Cover the 4 main user flows. Max 1 page — clear and simple. This satisfies the course's user documentation requirement.
- **Guide must cover**:
  1. How to sign in with Google (account is created automatically on first login)
  2. How to add your ingredients
  3. How to generate a meal plan
  4. How to favorite and share a recipe
- **Checklist**:
  - [ ] Add "Getting Started" section to README.md
  - [ ] Steps are numbered and clear
  - [ ] Include the deployed app URL at the top of the README

---

### Card 5.8 — Demo Video Recording (~5-7 min)
- **Assigned to**: All members
- **Labels**: QA
- **Description**:
  Each member records their segment using their own camera and presents the features they built. Edit into a single ~5-7 minute video. Upload to YouTube (unlisted or public). Paste link into Canvas submission.
- **Segments**:
  - **Adam** (~1.5 min): Sign in with Google, show that protected pages redirect when logged out, Sign out
  - **Ernesto** (~1.5 min): Add ingredients, edit, delete
  - **Alan** (~1.5 min): Generate meal plan, explain AI integration
  - **Abraham** (~1.5 min): Recipe detail, Favorites, Cookbook, share link
- **Checklist**:
  - [ ] Rehearse each segment before recording
  - [ ] Use the live Azure app (not localhost) for the recording
  - [ ] Record with camera on for each presenter
  - [ ] Combine segments into one video (iMovie, Clipchamp, DaVinci Resolve, etc.)
  - [ ] Upload to YouTube
  - [ ] Paste YouTube link in the Canvas submission document

---

### Card 5.9 — Canvas Submission Document
- **Assigned to**: Alan
- **Labels**: DevOps
- **Description**:
  Prepare and submit the final Canvas document with the three required links before the deadline.
- **Checklist**:
  - [ ] Link 1: Trello board URL (verify accessible without login or as "Team Visible")
  - [ ] Link 2: Azure deployed app URL (verify it loads)
  - [ ] Link 3: YouTube demo video URL (verify it plays)
  - [ ] Submit all three in a single document in Canvas before the deadline

---

## Summary: Who Owns What

| Area | Primary | Support |
|------|---------|---------|
| Project scaffold & GitHub setup | Alan | Adam |
| Database models & migrations | Ernesto | Alan |
| Backend service layer (CRUD) | Ernesto | Alan |
| Authentication (Google OAuth + Identity) | Adam | Alan |
| Auth guards & protected routes | Adam | — |
| AI service & prompt engineering | Alan | — |
| AI error handling | Alan | — |
| Azure deployment & CI/CD | Alan | Ernesto |
| Blazor pages & components | Abraham | Adam |
| Responsive design & CSS | Abraham | Adam |
| Accessibility audit & fixes | Adam | Abraham |
| QA / end-to-end testing | Ernesto (lead) | All |
| Demo video editing | Abraham | All |
| Canvas submission | Alan | — |

---

## Definition of Done (every card)

Before moving any card to **Done**, verify:
- [ ] Feature works in debug mode locally
- [ ] Feature works in the deployed Azure app
- [ ] Code pushed on a feature branch and merged via Pull Request
- [ ] PR reviewed and approved by at least one teammate
- [ ] No breaking changes introduced to other features
- [ ] Card moved to **Done** in Trello
