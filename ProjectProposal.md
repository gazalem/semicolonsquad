# Project Proposal — Smart Food Planner: AI Meal Planner

**CSE 325: .NET Software Development | BYU-Idaho**

---

## 1. Title

**Smart Food Planner** — AI-Powered Weekly Meal Planner

---

## 2. Overview

Smart Food Planner solves the daily friction of deciding what to cook with the ingredients already at home. The user enters their available groceries, and an AI model generates a personalized 7-day meal plan composed exclusively of 30-minute meals. Users can save favorite recipes to a personal cookbook and share them with others.

**Problem addressed**: Most people waste food and time deciding what to cook because recipe apps require shopping trips for ingredients they don't have. Smart Food Planner inverts this by working with what the user already owns.

**Value added**: Reduces food waste, saves money, and removes decision fatigue around meal planning.

---

## 3. Scope

### Included
- User registration, login, and authenticated sessions
- Pantry management: add, edit, and delete available ingredients
- AI-generated weekly meal plan (7 days × 1 meal suggestion per day) using the user's ingredients
- Recipe detail view: ingredient list, step-by-step instructions, and estimated time
- Favorites: mark/unmark any recipe and view a favorites list
- Personal cookbook: browse all recipes the user has generated or saved
- Basic share functionality: copy-shareable link to a recipe

### Excluded (out of scope for this semester)
- Nutritional information or calorie tracking
- Grocery shopping list generation
- Real-time collaboration or multi-user households
- Native mobile app (Blazor web only)
- Social feed or community browsing of other users' cookbooks

---

## 4. Functional Requirements

| # | User Story |
|---|-----------|
| FR-01 | As a new user, I can register with email and password so I have a personal account. |
| FR-02 | As a returning user, I can log in and log out securely. |
| FR-03 | As an authenticated user, I can add, edit, and delete ingredients in my pantry. |
| FR-04 | As an authenticated user, I can request an AI-generated 7-day meal plan based on my current pantry. |
| FR-05 | As an authenticated user, I can view the full recipe for any suggested meal (ingredients + steps). |
| FR-06 | As an authenticated user, I can mark a recipe as a favorite. |
| FR-07 | As an authenticated user, I can view all my favorited recipes in my cookbook. |
| FR-08 | As an authenticated user, I can get a shareable link to any recipe. |
| FR-09 | As an authenticated user, I can view my past generated meal plans. |
| FR-10 | As any user, I can access a shared recipe link without logging in (read-only). |

---

## 5. Non-Functional Requirements

| Category | Requirement |
|----------|-------------|
| **Security** | Passwords hashed via ASP.NET Identity; JWT or cookie-based auth sessions; HTTPS enforced in production. |
| **Performance** | AI generation calls are async; loading states shown to the user; no blocking UI during API calls. |
| **Accessibility** | WCAG 2.1 Level AA compliance; semantic HTML; keyboard navigable; validated with Lighthouse. |
| **Usability** | Responsive design for mobile and desktop; consistent branding (color scheme, typography, layout). |
| **Reliability** | Graceful error handling when the AI service is unavailable (fallback message shown to user). |
| **Deployment** | Deployed to Azure App Service (free or student tier); CI/CD via GitHub Actions. |

---

## 6. Project Links

> *To be completed by the group lead once created.*

- **GitHub Repository**: `https://github.com/gazalem/semicolonsquad.git`
- **Trello Project Board**: `https://trello.com/b/jkVL3NTW/cse325-group-project`
- **Deployed Application**: `[insert link after deployment]`
