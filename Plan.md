# Smart Food Planner — Trello Sprint Plan
**CSE 325 | 5 Weeks | Team: Ernesto (Backend), Abraham (Frontend), Adam (Auth/Frontend), Alan (AI/Backend), Daniel (Frontend/Databases)**

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
- **Daniel** — Frontend / Databases

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
  Scaffold a new .NET 8 Blazor Server app called `Smart Food Planner`. Push to the group GitHub repo. Define the branch strategy: `main` is protected; each story gets its own feature branch (e.g., `feature/ingredients-crud`). Add all 5 members as collaborators.
- **Checklist**:
  - [ ] `dotnet new blazor -n SmartFoodPlanner --interactivity Server --auth Individual`
  - [ ] Push to GitHub repo `gazalem/semicolonsquad` (or new repo)
  - [ ] Add all team members as collaborators (Ernesto, Abraham, Adam, Alan, Daniel)
  - [ ] Add `.gitignore` for .NET (Visual Studio template)
  - [ ] Add `appsettings.Development.json` to `.gitignore` (for API keys later)
  - [ ] Document branch naming in README: `feature/card-name`

---

### Card 1.2 — Configure EF Core + SQLite Database
- **Assigned to**: Ernesto + Daniel
- **Labels**: Backend
- **Description**:
  Install EF Core with SQLite for local development. Create `AppDbContext.cs`. Add connection string to `appsettings.json`. Run the first migration (empty schema) to verify everything is wired correctly. Ernesto leads the setup; Daniel verifies the DB configuration and assists with the migration.
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
- **Assigned to**: Abraham + Daniel
- **Labels**: Frontend
- **Description**:
  Set the visual identity for Smart Food Planner. Define CSS variables for the color palette (food-themed: greens and warm tones), font family, and spacing. Update `MainLayout.razor` and `NavMenu.razor` with the brand. Abraham leads the CSS and layout; Daniel assists with the component structure and responsive scaffolding. The app should look intentional from day one.
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
- **Assigned to**: Alan + Daniel (all members participate)
- **Labels**: DevOps
- **Description**:
  Create the Trello board "Smart Food Planner — CSE 325". Create all sprint lists, add all members, define labels, and import every card from this plan. Paste the Trello link into `projectProposal_en.md`. Alan leads the board creation; Daniel imports and organizes the sprint cards.
- **Checklist**:
  - [ ] Create Trello board and set to "Team Visible"
  - [ ] Create all 8 lists (5 sprints + In Progress + In Review + Done)
  - [ ] Add Ernesto, Abraham, Adam, Alan, Daniel as members
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

### Card 2.4 — Ingredient Model & Migration ✅ DONE
- **Assigned to**: Ernesto + Daniel
- **Labels**: Backend
- **Description**:
  Create `Models/Ingredient.cs`. Fields: Id (int), UserId (string, FK to IdentityUser), Name (string, required), Quantity (decimal, nullable), Unit (string, nullable, e.g. "cups"), CreatedAt (DateTime). Add `DbSet<Ingredient>` to `AppDbContext`. Ernesto defines the model; Daniel runs the migration and validates the schema.
  > **Implementation note**: model was named `Models/UserIngredient.cs` (not `Ingredient.cs`) and `Quantity`/`Unit` are `string?` rather than `decimal?`, but all required fields (Id, UserId FK, Name, Quantity, Unit, CreatedAt) are present and correctly wired.
- **Checklist**:
  - [x] Create `Models/Ingredient.cs` with all fields *(as `UserIngredient.cs`)*
  - [x] Add `DbSet<Ingredient> Ingredients` to `AppDbContext` *(as `DbSet<UserIngredient> UserIngredients`, `Data/ApplicationDbContext.cs:9`)*
  - [x] Run: `dotnet ef migrations add AddIngredient && dotnet ef database update`
  - [x] Verify `Ingredients` table in DB with correct columns *(as `UserIngredients`, incl. FK + unique index on `(UserId, Name)`)*

---

### Card 2.5 — IngredientService (Backend) ✅ DONE
- **Assigned to**: Ernesto + Daniel
- **Labels**: Backend
- **Description**:
  Create `Services/IIngredientService.cs` interface and `Services/IngredientService.cs` implementation. Always filter by `userId` so users can only see their own data. Ernesto writes the service; Daniel reviews the query logic and assists with registration in `Program.cs`.
  > **Implementation note**: method signatures differ slightly from the spec (`AddIngredientAsync`/`UpdateIngredientAsync` take `userId` as an explicit parameter, and `UpdateIngredientAsync`/`DeleteIngredientAsync` return `bool` instead of `void`/using an `id` positional param) — this is arguably safer since it forces every call site to pass `userId` explicitly, and all queries are correctly scoped.
- **Methods to implement**:
  ```csharp
  Task<List<Ingredient>> GetIngredientsAsync(string userId);
  Task AddIngredientAsync(Ingredient ingredient);
  Task UpdateIngredientAsync(Ingredient ingredient);
  Task DeleteIngredientAsync(int id, string userId);
  ```
- **Checklist**:
  - [x] Create `IIngredientService` interface
  - [x] Implement all 4 methods in `IngredientService`
  - [x] `DeleteIngredientAsync` verifies the ingredient belongs to `userId` before deleting (`Services/IngredientService.cs:35-44`)
  - [x] Register: `builder.Services.AddScoped<IIngredientService, IngredientService>()` (`Program.cs:61`)

---

### Card 2.6 — Ingredients Page UI
- **Assigned to**: Abraham (primary) + Daniel (support)
- **Labels**: Frontend
- **Description**:
  Build `Pages/Ingredients.razor`. Inject `IIngredientService`. On load, fetch and display the current user's ingredients in a clean card or list layout. Include Add, Edit, and Delete actions. Abraham leads the component and interaction design; Daniel assists with the form and layout markup.
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
  - [ ] Share `MealPlanResponse` and `RecipeDto` classes with Ernesto and Daniel (they'll need them for models)

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
  System: "You are a helpful meal planning assistant. Always respond with valid JSON only. No explanation, no markdown, just JSON."
  User: "I have these ingredients: {ingredients}. Create exactly 7 recipes, one for each day of the week (Monday through Sunday). Each recipe should be completable in 30 minutes or less.
  Return JSON with this structure:
  { "recipes": [ { "day": "Monday", "name": "...", "prepTime": "15 min", "cookTime": "30 min",
    "ingredients": ["..."], "instructions": "..." } ] }"
  ```
- **Checklist**:
  - [x] Replace stub implementation with real API call
  - [x] System prompt sets JSON-only response mode
  - [x] User prompt passes ingredients as a comma-separated list
  - [x] Deserialize JSON response into `MealPlanResponse` DTO
  - [ ] Test with a real API call in debug mode — verify 7 meals are returned
  - [x] Log request and response for debugging (no API key in logs)

---

### Card 3.2 — MealPlan & Recipe Database Models ✅ DONE
- **Assigned to**: Ernesto + Daniel
- **Labels**: Backend
- **Description**:
  Create the data models to persist AI-generated meal plans. Coordinate with Alan on the DTO structure before writing models to ensure they align. Ernesto defines the EF relationships; Daniel creates the model files and runs the migration.
  > **Implementation note**: `Recipe.Ingredients` is stored as a serialized JSON string column rather than a separate table — a reasonable simplification since ingredients are never queried independently of their recipe.
- **Models**:
  - `MealPlan`: Id, UserId, GeneratedAt, List\<Recipe\>
  - `Recipe`: Id, MealPlanId, Day (string, e.g. "Monday"), Name, PrepTime (string), CookTime (string), Instructions (string), ShareToken (GUID string)
- **Checklist**:
  - [x] Create 2 model files in `Models/`: `MealPlan.cs`, `Recipe.cs`
  - [x] Set up EF relationship: MealPlan →(1:N)→ Recipe (`Data/ApplicationDbContext.cs:45-49`, cascade delete)
  - [x] Add `DbSet` entries in `AppDbContext` for both models (`Data/ApplicationDbContext.cs:13,15`)
  - [x] Run: `dotnet ef migrations add AddMealPlanRecipes` (`Data/Migrations/20260707173257_AddMealPlanRecipes.cs`)
  - [x] Verify all tables and foreign keys are created correctly, incl. unique index on `ShareToken`

---

### Card 3.3 — MealPlanService (Backend) ✅ DONE
- **Assigned to**: Ernesto (primary) + Daniel (support)
- **Labels**: Backend
- **Description**:
  Create `Services/IMealPlanService.cs` and `MealPlanService.cs`. The save method maps Alan's DTO objects into EF entities. Queries always scope to `userId`. Ernesto writes the service methods; Daniel assists with the DTO-to-entity mapping logic and validates queries against the schema.
  > **Update**: `GetRecipeByTokenAsync(string shareToken)` was missing (blocking Card 4.5) — added while implementing Card 4.5. All 5 methods are now present.
- **Methods**:
  ```csharp
  Task<MealPlan> SaveMealPlanAsync(string userId, MealPlanResponse dto);
  Task<List<MealPlan>> GetMealPlansAsync(string userId);
  Task<MealPlan?> GetMealPlanByIdAsync(int id, string userId);
  Task<Recipe?> GetRecipeByIdAsync(int recipeId);
  Task<Recipe?> GetRecipeByTokenAsync(string shareToken);
  ```
- **Checklist**:
  - [x] Implement all 5 methods *(note: `GetRecipeByIdAsync` also takes an extra `userId` param not in the original spec, which is a good extra safety scope)*
  - [x] `SaveMealPlanAsync` maps DTO → Entity and generates a GUID `ShareToken` per recipe (`Services/MealPlanService.cs:10-30`)
  - [x] Register as scoped in `Program.cs:62`
  - [ ] Test `SaveMealPlanAsync` in debug mode with real AI output

---

### Card 3.4 — Generate Meal Plan Page
- **Assigned to**: Abraham (primary) + Daniel (support)
- **Labels**: Frontend
- **Description**:
  Build `Pages/MealPlan.razor`. The page has a "Generate This Week's Plan" button. While generating, show a loading spinner and disable the button. Once done, display 7 day cards in a grid (Mon–Sun), each showing the meal name and estimated time. Abraham leads the component; Daniel assists with the grid layout and card markup.
- **Checklist**:
  - [ ] "Generate" button: calls `IAIService.GenerateMealPlanAsync()` then saves via `IMealPlanService`
  - [ ] Loading state: spinner + "Cooking up your plan..." message + button disabled
  - [ ] Weekly grid: 7 cards labeled Day 1 (Monday) through Day 7 (Sunday)
  - [ ] Each card: meal name, estimated time badge (e.g. "25 min")
  - [ ] Click any day card → navigate to `/recipe/{recipeId}`
  - [ ] Error message banner if AI call fails (styled, not a browser alert)

---

### Card 3.5 — Recipe Detail Page
- **Assigned to**: Abraham (primary) + Daniel (support)
- **Labels**: Frontend
- **Description**:
  Build `Pages/RecipeDetail.razor`. Takes `RecipeId` as a route parameter. Fetches the recipe and displays: name, estimated time, ingredient list (bulleted), and numbered step-by-step instructions. Abraham leads the layout; Daniel wires the data-fetching logic and assists with the Favorite/Share placeholder buttons.
- **Checklist**:
  - [ ] Route: `@page "/recipe/{RecipeId:int}"`
  - [ ] Fetch recipe via `IMealPlanService.GetRecipeByIdAsync(RecipeId)`
  - [ ] Show: name, "⏱ X min" badge, ingredient list, numbered steps
  - [ ] Placeholder "★ Favorite" button (not wired yet — Sprint 4)
  - [ ] Placeholder "🔗 Share" button (not wired yet — Sprint 4)
  - [ ] Show "Recipe not found" if ID is invalid

---

### Card 3.6 — Past Meal Plans Page ❌ NOT DONE (built the wrong feature)
- **Assigned to**: Daniel
- **Labels**: Backend, Frontend
- **Description**:
  Build `Pages/PastPlans.razor`. Lists all meal plans the user has previously generated, sorted newest first. Each entry shows the generation date and links to that week's meal plan view. Daniel owns this card end-to-end — it draws on both his database querying skills (via `IMealPlanService`) and his frontend skills for the list UI.
  > **Gap found**: `Components/Pages/PastPlans.razor` does not call `IMealPlanService` at all. It's a standalone journal/notes form (Name, Date, Notes fields) backed by an in-memory `List<PastPlanEntry>` in `@code` — nothing is persisted to the database, so all entries vanish on page refresh. It does not fetch real AI-generated `MealPlan` records, does not show recipe counts, and does not link into the 7-day grid view. This card still needs to be rebuilt against `IMealPlanService.GetMealPlansAsync(userId)`.
- **Checklist**:
  - [ ] Fetch all meal plans via `IMealPlanService.GetMealPlansAsync(userId)`
  - [ ] Display: date generated, number of recipes (always 7)
  - [ ] Click on a plan → show that week's 7-day grid (reuse the MealPlan page or a detail view)
  - [x] Empty state: "No plans yet — generate your first one!" *(present, but for the wrong data source — "No past plans saved yet.")*

---

### Card 3.7 — AI Error Handling
- **Assigned to**: Alan
- **Labels**: AI/API, Backend
- **Description**:
  Wrap the AI service call in proper error handling. Cover three failure scenarios: timeout, API quota/auth error, and JSON parse failure. Each shows a user-friendly message in the UI. Log the full technical error server-side for debugging.
- **Checklist**:
  - [x] Set HTTP timeout to 30 seconds on the API client
  - [x] Catch `TimeoutException` → show: "The AI is taking too long. Please try again."
  - [x] Catch HTTP 429 / 401 → show: "Service temporarily unavailable."
  - [x] Catch `JsonException` → show: "Could not process the AI response. Please try again."
  - [x] Log all exceptions with `ILogger` (not Console.WriteLine) for production visibility

---

## SPRINT 4 — Cookbook, Favorites & Sharing
**Week 4 | Goal: Complete all remaining functional requirements (FR-06 through FR-10)**

---

### Card 4.1 — Favorite Model & FavoriteService
- **Assigned to**: Ernesto + Daniel
- **Labels**: Backend
- **Description**:
  Create `Models/Favorite.cs` (Id, UserId, RecipeId, CreatedAt). Run migration. Create `IFavoriteService` and `FavoriteService` with toggle, check, and list methods. Ernesto writes the service logic; Daniel creates the model, runs the migration, and validates the unique index.
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
- **Assigned to**: Daniel
- **Labels**: Frontend
- **Description**:
  Build `Pages/Favorites.razor`. Displays all recipes the user has starred, as cards in a grid. Each card shows the recipe name, estimated time, and a "View Recipe" link. Daniel owns this page end-to-end — it combines a straightforward DB query (via `IFavoriteService`) with a responsive frontend grid.
- **Checklist**:
  - [ ] Fetch favorites via `IFavoriteService.GetFavoriteRecipesAsync(userId)`
  - [ ] Display as a responsive grid of recipe cards
  - [ ] Card: recipe name, "⏱ X min", "View Recipe →" link
  - [ ] Empty state: "No favorites yet — star a recipe to save it here!"

---

### Card 4.4 — Cookbook Page
- **Assigned to**: Abraham (primary) + Daniel (support)
- **Labels**: Frontend
- **Description**:
  Build `Pages/Cookbook.razor`. Shows every recipe ever generated by the user across all meal plans. Include a live search bar that filters recipes by name as the user types. Abraham leads the search logic and overall layout; Daniel assists with the recipe card component (reusing work from the Favorites page).
- **Checklist**:
  - [ ] Fetch all recipes from all of the user's meal plans
  - [ ] Search bar: bind to a string variable, filter the displayed list in real time
  - [ ] Grid of recipe cards (same component as Favorites if possible)
  - [ ] Empty state: "Your cookbook is empty — generate your first meal plan!"

---

### Card 4.5 — Shareable Recipe Link ✅ DONE
- **Assigned to**: Alan
- **Labels**: AI/API, Backend
- **Description**:
  Each recipe already has a `ShareToken` GUID (added in Sprint 3). Build the public-facing page `Pages/SharedRecipe.razor` at route `/share/{token}`. No `[Authorize]` attribute — anyone with the link can view. Wire up the "Share" button on Recipe Detail to copy the URL to the clipboard.
  > **Implementation note**: extracted the ingredient/step-parsing logic shared by `RecipeDetail.razor` and `SharedRecipe.razor` into a new static `Services/RecipeTextFormatter.cs` to avoid duplicating it across both pages.
- **Checklist**:
  - [x] Create `Pages/SharedRecipe.razor` with route `@page "/share/{Token}"` (`Components/Pages/SharedRecipe.razor:1`)
  - [x] No `[Authorize]` on this page
  - [x] Fetch recipe via `IMealPlanService.GetRecipeByTokenAsync(Token)` (added to `IMealPlanService`/`MealPlanService`)
  - [x] Show read-only recipe detail (name, ingredients, steps — no Favorite/Share buttons)
  - [x] Show "Recipe not found" if token is invalid
  - [x] On `RecipeDetail.razor`: wire "Share" button to copy `https://{host}/share/{shareToken}` to clipboard using JS interop (`Components/Pages/RecipeDetail.razor:125-134`, `navigator.clipboard.writeText`)

---

### Card 4.6 — Navigation Updates ✅ DONE
- **Assigned to**: Adam + Daniel
- **Labels**: Frontend
- **Description**:
  Update `NavMenu.razor` to include all app pages. Show nav links only for authenticated users (except Login). Highlight the currently active page. Adam leads the auth-conditional logic; Daniel assists with the styling and active-link highlighting.
- **Nav links (authenticated)**:
  - Ingredients → `/ingredients`
  - Generate Plan → `/mealplan`
  - Cookbook → `/cookbook`
  - Favorites → `/favorites`
  - Past Plans → `/pastplans`
  - Logout
- **Nav links (unauthenticated)**: "Sign in with Google" → `/login`
- **Checklist**:
  - [x] All links present and correct in NavMenu (`Components/Layout/NavMenu.razor:19-48`)
  - [x] Active page link has a distinct style (`.nav-item ::deep a.active`, `NavMenu.razor.css:98-102`)
  - [x] Authenticated-only links are inside `<AuthorizeView>` (`NavMenu.razor:17-73`)
  - [x] Test on mobile — nav should not overflow horizontally (hamburger checkbox toggle collapses menu below 641px, `NavMenu.razor.css:109-143`)

---

### Card 4.7 — Responsive Design Audit ✅ DONE (CSS verified; manual DevTools pass still recommended)
- **Assigned to**: Adam + Daniel
- **Labels**: Frontend, QA
- **Description**:
  Test every page at three breakpoints using browser DevTools: 375px (iPhone SE), 768px (iPad), 1280px (desktop). Fix layout issues — overflowing text, broken buttons, squished forms, or unreadable cards. Adam covers auth-related pages; Daniel covers Favorites, Past Plans, and Cookbook pages.
  > **Implementation note**: verified by reading the CSS breakpoints (below), not by driving a browser — recommend an actual DevTools pass at 375/768/1280px before marking QA-complete, especially since Card 3.6's `PastPlans.razor` will likely be rebuilt (see above) and will need its own responsiveness check.
- **Checklist**:
  - [x] Ingredients page: form and list stack cleanly on mobile (`Ingredients.razor.css:27-30`, `.form-grid` collapses to 1 column ≤640px)
  - [x] Meal plan grid: 1 column on mobile, 2 on tablet, 3-4 on desktop (`MealPlan.razor.css:17-31`: 4 cols → 2 cols ≤900px → 1 col ≤520px)
  - [x] Recipe detail: ingredients and steps are readable on 375px (`RecipeDetail.razor.css:31-42`, layout collapses to 1 column ≤760px)
  - [x] Cookbook/Favorites/Past Plans: grid adjusts column count (Bootstrap `col-lg-*` in `Cookbook.razor`/`Favorites.razor`; `PastPlans.razor` also uses `col-lg-*` but only for the wrong feature — see Card 3.6)
  - [x] NavMenu: collapses on mobile or uses a hamburger icon (`NavMenu.razor.css:1-16,109-143`)
  - [x] All buttons are at least 44×44px touch target on mobile (`wwwroot/app.css:116-126`, global `@media (max-width: 640px)` rule)

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
- **Assigned to**: Alan + Ernesto + Daniel
- **Labels**: DevOps, Backend
- **Description**:
  Configure production secrets in Azure App Service (Application Settings). Apply EF migrations on startup. Alan manages the Azure configuration; Ernesto and Daniel verify all migrations run cleanly and the connection string is correct in the production environment.
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
- **Assigned to**: Adam + Daniel
- **Labels**: QA
- **Description**:
  Run Lighthouse on all 5 main pages in the deployed app. Fix any accessibility issues to reach a score ≥ 90. Adam covers auth/nav pages; Daniel covers Favorites, Cookbook, and Past Plans. Common fixes: missing `alt`, icon-only buttons without labels, low color contrast, form inputs without `<label>`.
- **Checklist**:
  - [ ] Run Lighthouse on: Home, Ingredients, MealPlan, RecipeDetail, Cookbook, Favorites, PastPlans
  - [ ] Target: Accessibility score ≥ 90 on all pages
  - [ ] Fix: add `alt=""` or `aria-label` to all images and icon buttons
  - [ ] Fix: verify color contrast ratio ≥ 4.5:1 for all body text
  - [ ] Fix: every `<input>` has a paired `<label>` element
  - [ ] Test keyboard navigation: Tab key reaches all interactive elements in logical order

---

### Card 5.4 — HTML Validation & Console Error Cleanup
- **Assigned to**: Adam + Daniel
- **Labels**: QA
- **Description**:
  Paste each page's rendered HTML into the W3C Markup Validator. Fix any errors (bad nesting, missing closing tags, invalid attributes). Open browser DevTools and resolve all console errors. Adam validates auth and nav pages; Daniel validates the pages he built (Favorites, PastPlans) plus Cookbook.
- **Checklist**:
  - [ ] Validate all main pages via W3C validator — zero errors
  - [ ] Zero red errors in browser console on page load
  - [ ] Any warnings reviewed and justified or fixed

---

### Card 5.5 — Empty States & UX Polish
- **Assigned to**: Abraham + Daniel
- **Labels**: Frontend
- **Description**:
  Ensure every page that can show zero data has a helpful empty state message. Confirm all async operations show a loading spinner. Abraham leads the visual polish pass; Daniel covers the Favorites and Past Plans empty states and verifies loading spinners on those pages.
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
- **Assigned to**: Abraham + Daniel
- **Labels**: QA
- **Description**:
  Write a short "Getting Started" section in the GitHub repo README. Cover the 4 main user flows. Max 1 page — clear and simple. This satisfies the course's user documentation requirement. Abraham writes the narrative; Daniel documents the Favorites and Past Plans flows.
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
  - **Adam** (~1 min): Sign in with Google, show that protected pages redirect when logged out, Sign out
  - **Ernesto** (~1 min): Add ingredients, edit, delete
  - **Alan** (~1 min): Generate meal plan, explain AI integration
  - **Abraham** (~1 min): Recipe detail, Cookbook, branding overview
  - **Daniel** (~1 min): Favorites page, Past Plans page, share link demo
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
| Database models & migrations | Ernesto | Daniel |
| Backend service layer (CRUD) | Ernesto | Daniel |
| Authentication (Google OAuth + Identity) | Adam | Alan |
| Auth guards & protected routes | Adam | — |
| AI service & prompt engineering | Alan | — |
| AI error handling | Alan | — |
| Azure deployment & CI/CD | Alan | Ernesto |
| Blazor pages & components | Abraham | Daniel, Adam |
| Responsive design & CSS | Abraham | Daniel, Adam |
| Favorites page | Daniel | — |
| Past Meal Plans page | Daniel | — |
| Accessibility audit & fixes | Adam | Daniel |
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
