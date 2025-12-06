# Smoke Test Procedures - Phase 1 Baseline

**Last Updated**: December 06, 2025

**This is a baseline measurement, not a quality assessment.**

- We record what the application does today (including any quirks or limitations)
- We use this baseline to detect regressions during migration phases
- We do NOT fix seed project issues - we preserve current behavior during migration

**Why this matters**: Successful migration means "works the same in .NET 9 on Azure as it did in Framework 4.8 locally" - not "works perfectly." This baseline tells us what "the same" means.

**Test Data State**: This procedure assumes the application is running with the original sample dataset included in the seed project.

------

## Test Accounts

This is a **portfolio demonstration project** with publicly available source code.
These credentials are for local testing only and are documented here for convenience.

**Admin Account** (auto-created by seed data):
- Email: `admin@musicstore.com`
- Password: `Admin123!`

**Visitor Account** (create manually via `/Account/Register`):
- Email: `visitor@test.com`  
- Password: `Visitor@123456`

**Note**: In production environments, test credentials would be in Azure Key Vault 
or environment variables, never committed to source control. This project demonstrates
proper cloud secrets management in Phase 2 (Azure deployment) and Phase 5 
(Managed Identity integration).

------

## Prerequisites

- [ ] **Fresh database state verified** (see Appendix A: Database Reset Procedure)
- [ ] Application running locally on IIS Express
- [ ] **Chrome browser** with DevTools (F12) available for console monitoring

------

## Page Coverage

Test all 12 pages in the following order:

1. Home Page
2. Store Index
3. Store Browse (Rock genre)
4. Store Details (Album ID 1)
5. Shopping Cart
6. Checkout (authenticated)
7. Checkout Complete
8. Admin - StoreManager (authenticated, admin role)
9. Create a New Album (authenticated, admin role)
10. Register Page
11. Login Page
12. Store Manager Page (authenticated, visitor role)

------

## Test Execution Instructions

### For Each Page:

1. **Navigate** to the page URL
2. **Visual Check**: Record what displays - all elements, any quirks, missing items
3. **Console Check**: Open F12 → Check both Console tab and Issues tab, capture any errors, warnings, or issues present
4. **Document Behavior**: Record observations in the Results section (see template below)
5. **Screenshot**: Optional - take if it helps document unusual behavior

### Documentation Status:

Use these labels when recording results:

- **✅ Documented**: Page behavior captured completely
  - Record what you see, including any console warnings or visual quirks
  - Example: "Page loads, shows 5 albums, jQuery deprecation warning in console"
- **⚠️ Needs Investigation**: Behavior seems wrong but unclear if it's seed project quirk or real problem
  - Example: "Album art missing on details page - is this expected?"
  - Example: "Form validation message shows 'undefined' - seed project bug or environmental?"
- **🚫 Blocked**: Cannot document page behavior due to blocker
  - Example: "Page returns 500 error - cannot proceed"
  - Example: "Login completely broken - cannot test authenticated flows"

### Critical Distinction:

**Console errors ARE NOT test failures** - they're observations to document.

If you see:

- Console warning about jQuery → Document it as current behavior
- Missing image icon → Document it as current behavior
- Layout slightly misaligned → Document it as current behavior

Only mark **Blocked** if you literally cannot see the page or complete the user flow.

------

## 1. Home Page

**URL**: `/` or `/Home/Index` **Authentication Required**: No

### Expected Elements

- [ ] Site logo/header displays
- [ ] Featured albums section visible with minimum 5 albums shown
- [ ] Genre navigation menu in left sidebar
- [ ] All album images load (broken image icons count as "documented behavior")
- [ ] Footer
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 2. Store Index

**URL**: `/Store` or `/Store/Index` **Authentication Required**: No

### Expected Elements

- [ ] Page title "Browse Genres" or similar heading
- [ ] Complete list of all music genres
- [ ] All genre links are clickable (cursor changes, not disabled)
- [ ] Consistent styling and layout with site theme
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 3. Store Browse

**URL**: `/Store/Browse?genre=Rock` **Authentication Required**: No **Test Parameter**: Use "Rock" genre consistently

### Expected Elements

- [ ] Genre name "Rock" displayed as page heading
- [ ] List of albums in Rock genre 
- [ ] Album titles and artists visible for each album
- [ ] Album artwork/thumbnails display (broken images count as "documented behavior")
- [ ] Album title and image links are clickable
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 4. Store Details

**URL**: `/Store/Details/1` **Authentication Required**: No **Test Parameter**: Use Album ID 1 consistently

### Expected Elements

- [ ] Album title displayed prominently as heading
- [ ] Album artwork displays (note: seed project uses thumbnail-only, not full-size image)
- [ ] Artist name visible
- [ ] Genre visible
- [ ] Price formatted correctly as currency (X.XX)
- [ ] "Add to cart" button visible and enabled (not grayed out)
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 5. Shopping Cart

**URL**: `/ShoppingCart` **Authentication Required**: No **Test Setup**: Add 1-2 albums to cart before testing this page

### Expected Elements

- [ ] Cart items table or list displays
- [ ] Each item shows: album name, price, quantity
- [ ] "Remove item" link/button present per item
- [ ] Subtotal per line item displays correctly
- [ ] Cart total displayed prominently (sum of all items)
- [ ] "Checkout" button visible and enabled
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

### Behavior

- [ ] "Checkout" click when NOT logged in: Login page will be displayed
- [ ] "Checkout" click when logged in: Checkout page will be displayed

------

## 6. Checkout

**URL**: `/Checkout/AddressAndPayment` **Authentication Required**: YES **Test Setup**:

1. Login with admin account
2. Ensure cart has at least 1 item
3. Navigate to checkout

### Expected Elements

- [ ] Page title "Address and Payment"
- [ ] Checkout form displays with all required fields:
  - [ ] First Name
  - [ ] Last Name
  - [ ] Address
  - [ ] City
  - [ ] State/Province
  - [ ] Postal Code
  - [ ] Country
  - [ ] Phone Number
  - [ ] Email
  - [ ] Promo Code field
- [ ] "Submit Order" button visible and enabled
- [ ] Form validation indicators present (required field markers)
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

### Behavior

- [ ] "Submit Order" click when fields are empty causes "required" indicators to be displayed on all fields. Ex: "First Name is required"
- [ ] "Submit Order" click when fields have been entered with valid data causes navigation to Checkout Complete page

------

## 7. Checkout Complete

**URL**: `/Checkout/Complete` (after successful checkout) **Authentication Required**: YES **Test Setup**:

1. Login with visitor test account 
2. Ensure cart has at least 1 item
3. Navigate to checkout
4. Complete checkout form and click "Submit Order"

### Expected Elements

- [ ] Page title "Checkout Complete"
- [ ] Order number displayed
- [ ] "How about shopping for some more music in our store" displayed and "store" is link
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 8. Admin (StoreManager)

**URL**: `/StoreManager` **Authentication Required**: YES (Admin role required) **Test Setup**: Login with admin test account 

### Expected Elements

- [ ] Page title "Index" (Known Limitation: not "Store Manager")
- [ ] Table or list of all albums displays
- [ ] Album table includes columns: Title, Artist, Genre, Price (minimum)
- [ ] "Create New" button/link visible
- [ ] Each album row has action links:
  - [ ] "Edit" link
  - [ ] "Details" link
  - [ ] "Delete" link
- [ ] No pagination controls (Known Limitation)
- [ ] Search/filter box (if implemented)
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

------

## 9. Create a New Album

**URL**: `/StoreManager/Create` **Authentication Required**: YES (Admin role required) **Test Setup**:

1. Starting on Admin Page (so you must be logged in as an admin)
2. Click on "Create New" Link (above the Album List)

### Expected Elements

- [ ] Page title "Create"
- [ ] Create Album form displays with all required fields:
  - [ ] GenreId as populated Dropdown List
  - [ ] ArtistId as populated Dropdown List
  - [ ] Title
  - [ ] Price
  - [ ] Album Art URL
- [ ] "Create" button visible and enabled
- [ ] "Back to List" link visible and enabled
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

### Behavior

- [ ] "Create" click when Title and Price fields are empty causes "required" indicators to be displayed on all fields. Ex: "An Album Title is required"
- [ ] Enter the following data and then click "Create":
  - "Pop" from GenreId dropdown
  - AC/DC from ArtistId dropdown
  - Title: "ZZZ"
  - Price: 99.99
  - Album Art URL: "/Content/Images/placeholder.gif" The Store Manager page will be displayed and the new album will be the last item in the list.

------

## 10. Register Page

**URL**: `/Account/Register` **Authentication Required**: No (test while logged out)

### Expected Elements

- [ ] Page title "Register" or "Create Account"
- [ ] Email input field with label
- [ ] Password input field with label (masked)
- [ ] Confirm password input field with label (masked)
- [ ] "Register" submit button
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

### Behavior

- [ ] "Register" click when all fields are empty causes "required" indicators to be displayed on all fields. Ex: "The Email field is required"
- [ ] "Register" click after a single character has been entered in the Password field causes the following messages to be displayed:
  - The Email field is required.
  - The Password must be at least 6 characters long.
  - The password and confirmation password do not match.
- [ ] Enter the following data and then click "Register":
  - an email address that conforms to standards - suggested "TestUser-<hour><min>@Test.com" Example: "TestUser0848@Test.com" (enter it on the form here) Test Email: [       ]
  - Password: "Test@123456"
  - Confirm Password: "Test@123456"
  - [ ] The Home page will be displayed.
  - [ ] Hello. [your entered email] will appear in the header menu
  - [ ] Log off will appear in the header menu

------

## 11. Login Page

**URL**: `/Account/Login` **Authentication Required**: No (test while logged out)

### Expected Elements

- [ ] Page title "Log in"
- [ ] Email input field with label
- [ ] Password input field with label (masked input)
- [ ] "Remember me" checkbox (if implemented)
- [ ] "Log in" submit button
- [ ] "Register as new user" link visible
- [ ] "Forgot password" link missing (Known Limitation)
- [ ] **Console state documented** (capture any errors or warnings present)
- [ ] **Layout observations documented** (note any quirks or visual issues)
- [ ] **Current behavior captured** (what does this page actually do today?)

### Behavior

- [ ] "Login" click when all fields are empty causes "required" indicators to be displayed on all fields. Ex: "The Email field is required"
- [ ] "Log In" click after a single character has been entered in the Email field causes the following message to be displayed:
  - The Email field is not a valid e-mail address.
- [ ] Enter the email and password used in the Register test and then click "Login":
  - [ ] The Home page will be displayed.
  - [ ] Hello. [your entered email] will appear in the header menu
  - [ ] Log off will appear in the header menu

------

## 12. Store Manager Page (when not admin role)

**URL**: `/StoreManager` **Authentication Required**: No (test while logged out)

### Expected Behavior

- [ ] Login Page will be displayed

------

## Results Documentation

### Results Documentation Philosophy

Your results file should answer: "What does the seed project do today?"

**Good baseline documentation**:

- Records behavior accurately (console warnings, quirks, visual issues)
- Distinguishes seed project characteristics from environmental problems
- Flags only true blockers (cannot document behavior at all)
- Provides enough detail for Phase 4 regression testing

**Bad baseline documentation**:

- Treats console warnings as failures
- Lists visual quirks as "issues to fix"
- Judges quality instead of documenting behavior
- Provides pass/fail without context

Remember: If the page loads and you can interact with it, you have successfully documented the baseline - even if that baseline includes quirks.

------

After completing smoke tests, document your findings:

1. **Create results file**: `/docs/baselines/phase1-smoke-test-results-YYYY-MM-DD.md`
2. **Include**:
   - Test metadata (date, tester, environment details, git commit SHA)
   - Documentation status for each of the 12 pages
   - Screenshots attached to any unusual behavior (stored in issue tracker, not git)
   - Detailed observations with classifications
3. **Commit** results file with descriptive message

### Results Template Structure

```markdown
# Phase 1 Smoke Test Results

**Date**: YYYY-MM-DD
**Tester**: [Your Name]
**Environment**: Local IIS Express
**Build/Commit**: [Git SHA]
**Test Date**: [Date]
**Test Data State**: [Fresh seed data from original seed projec/ enhanced seed data with users and orders / Other]

## Baseline Documentation Status

**Overall**: ✅ COMPLETE | ⚠️ NEEDS INVESTIGATION | 🚫 BLOCKED

**Summary**:
- Pages Documented: X / 12
- Pages Needing Investigation: X
- Pages Blocked: X
- Regression Risks Identified: X (behaviors to verify in Phase 4)
- Known Limitations Captured: X (quirks to preserve)

## Page Results

### 1. Home Page - ✅ Documented

**Observations**:
- All expected elements display correctly
- 3 featured albums shown (IDs 1, 5, 7)
- Console: 1 warning present (jQuery Migrate deprecation - KNOWN LIMITATION)
- Genre nav shows 10 genres with album counts

**Regression Risks**: 
- Featured album count (verify count=3 post-migration)

---

### 2. Store Index - ✅ Documented

**Observations**:
- Genre list complete (10 genres)
- Album counts display correctly per genre
- Minor alignment issue on genre names (text slightly off-center - KNOWN LIMITATION)

**Regression Risks**:
- Genre count accuracy (verify all 10 genres present post-migration)

---

### 8. Admin (StoreManager) - ⚠️ Needs Investigation

**Observations**:
- Album list displays (50 albums total)
- Page title shows "Index" instead of "Store Manager" (KNOWN LIMITATION per seed docs)
- Noticed: Create/Edit/Delete links work but Delete shows confirmation page, not modal

**Needs Investigation**:
- Delete confirmation: Is full page navigation expected, or should this be modal/AJAX?
- **Resolution needed before Phase 2**: Review seed project source to confirm intended behavior

---

### X. [Example Blocked Page] - 🚫 Blocked

**Blocker**: Login page throws NullReferenceException when clicking "Log in" button (regardless of credentials)

**Console Error**:
```

Uncaught TypeError: Cannot read property 'submit' of null at login.js:15

```
**Impact**: Cannot test any authenticated flows (Checkout, Admin pages)

**Next Steps**: 
1. Review seed project GitHub issues for known login bugs
2. Check local environment (IIS Express, .NET Framework version)
3. If seed project bug: Document decision to fix vs use different seed project

---

[Continue for all 12 pages...]

## Detailed Observations

### Observation #1: [Descriptive Title]

**Page**: [URL where observed]
**Classification**: [Regression Risk | Known Limitation | Blocker]
**Browser**: [Chrome/Edge + version]

**Description**: [What you observed]

**Current Behavior**:
1. [What happens step by step]
2. [Include any console messages]
3. [Note any visual quirks]

**Why This Matters**:
- If Regression Risk: [Explain why this must work the same post-migration]
- If Known Limitation: [Explain why we're preserving this quirk]
- If Blocker: [Explain why this prevents baseline documentation]

**Console Output** (if relevant):
```

[Paste exact console messages]

```
**Screenshot**: [Optional - attach if helpful for documentation]

**Phase 4 Test Implication**: [What test case does this inform?]
```

### Baseline Observations Classification

When documenting behaviors in your results file, use these categories:

#### ⚠️ Regression Risk (Track These - They Become Test Cases)

Behaviors that MUST be preserved during migration:

- Form validation patterns (client-side vs server-side)
- Error message text and formatting
- Page load sequences and redirects
- Data display formats (currency, dates, counts)

**Example observations**:

- "Checkout accepts empty fields - validation is server-side only"
- "Genre page shows empty list when no albums - not an error message"
- "Cart total updates on page reload, not dynamically"

**Why track these**: Phase 4 tests must verify these behaviors persist post-migration.

------

### Documentation Scope: What to Include vs Skip

**Document these (they affect functionality or could cause regressions)**:

✅ Console errors/warnings (all of them - part of baseline)
✅ Missing functionality (no pagination, no password reset, etc.)
✅ Non-standard implementations that work (anchor styled as button)
✅ Layout quirks with technical causes (footer width, clickable areas)
✅ Validation behavior (client-side vs server-side patterns)
✅ Authentication/authorization behavior
✅ Data display patterns (placeholder images, missing fields)

**Skip these (CSS polish and minor visual inconsistencies)**:

❌ Hover color inconsistencies (unless it affects accessibility)
❌ Font size variations
❌ Minor spacing/alignment issues
❌ Missing currency symbols (unless it causes confusion)
❌ Cosmetic styling differences between similar elements

**Decision principle**: Document if it could break differently during migration or if 
a hiring manager would notice the quirk. Skip if it's pure CSS polish.

**Why this matters**: Over-documenting trivial UI details looks like nitpicking to 
hiring managers. Under-documenting functional quirks risks missing regressions. 
This balance reflects professional judgment.

**Example - Document**:
- "Add to cart button is `<a>` inside `<p class="button">` - hover target is text only, 
  not entire gray box. Non-standard but functional."

**Example - Skip**:
- "Featured album links don't change color on hover like menu items do"

---

#### 📝 Known Limitation (Document and Preserve)

Seed project quirks that are intentional or acceptable:

- Console warnings (jQuery versions, deprecated APIs)
- Visual inconsistencies (minor styling issues)
- Missing features noted in seed project docs
- Simplified implementations (no pagination, basic search)

**Example observations**:

- "Console warning: jQuery Migrate deprecation message - present in seed project"
- "Album details page uses thumbnail only, not full-size art"
- "StoreManager page title shows 'Index' not 'Store Manager'"
- "No 'forgot password' link on login page"

**Why document these**: Prevents "fixing" them during migration and introducing scope creep.

------

#### 🚨 Blocker (Investigate Before Migration Begins)

Issues that prevent establishing baseline:

- Application won't run or crashes
- Database connection failures
- Core user flows completely broken (login fails for all users)
- Pages return 500 errors consistently

**Example observations**:

- "Login page throws NullReferenceException for all test accounts"
- "Application fails to start - missing web.config connection string"
- "Database seed script fails - cannot populate test data"

**Why these are different**: You can't migrate what you can't measure. These must be resolved to establish baseline.

**Note**: A quirky implementation is NOT a blocker. "Login form doesn't validate empty fields client-side" is a known limitation. "Login button does nothing and console shows JavaScript crash" is a blocker.

------

## Common Baseline Documentation Scenarios

### Scenario 1: Page Loads But Has Console Errors

**What you see**: Home page displays correctly, but console shows jQuery warning

**How to document**:

- Status: ✅ Documented
- Classification: Known Limitation
- Note: "Console warning present - jQuery Migrate deprecation message"

**What NOT to do**: Mark page as failed or blocked

------

### Scenario 2: Visual Element Looks Wrong

**What you see**: Album art appears as small thumbnail instead of large image

**How to document**:

- Status: ✅ Documented
- Classification: Known Limitation (check seed project - might be intentional)
- Note: "Album details uses thumbnail only - no large image available"

**What NOT to do**: Assume this is a bug without checking seed project design

------

### Scenario 3: Feature Seems Missing

**What you see**: Login page has no "forgot password" link

**How to document**:

- Status: ✅ Documented
- Classification: Known Limitation
- Note: "No password recovery feature - absent in seed project by design"

**What NOT to do**: Treat missing features as failures (seed project scope is limited)

------

### Scenario 4: Something Actually Broken

**What you see**: Clicking "Add to Cart" throws exception, page crashes

**How to document**:

- Status: 🚫 Blocked
- Classification: Blocker
- Note: "Add to Cart throws NullReferenceException - cannot complete checkout flow testing"

**What to do next**: Investigate if this is environmental (your setup) or seed project bug

------

### Decision Framework: Is This a Blocker?

**Ask yourself**: "Can I document what the page currently does?"

- **YES, I can see the page and describe its behavior** → ✅ Documented (even if quirky)
- **NO, I need to investigate if this is expected** → ⚠️ Needs Investigation
- **NO, the page is completely broken** → 🚫 Blocked

**Example**:

- "Form doesn't validate empty fields" = Documented (might be server-side only)
- "Form submits but I get HTTP 500 error" = Blocked (cannot complete flow)

------

## Testing Tips

### Console Monitoring

- Keep DevTools console open during entire test session
- Clear console between pages (🚫 icon) for accurate per-page results
- Record both errors (red) and warnings (yellow) - they're all part of the baseline

### Screenshot Strategy

- Only capture screenshots if they help document behavior
- Use browser's full-page screenshot tool (DevTools → Ctrl+Shift+P → "screenshot")
- Attach screenshots to issue tracker entries, not in git repository

### Environment Consistency

- Test on the same machine/browser throughout Phase 1
- Note any environmental changes (Windows updates, IIS restarts) in results
- Retest if environment significantly changes

### When to Stop and Investigate

**Stop baseline documentation if**:

- 🚫 Multiple pages blocked (cannot document current behavior)
- 🚫 Core flows completely broken (login, database connection)
- 🚫 Application won't start or crashes immediately

**Continue documenting if**:

- ✅ Pages load with console warnings (document the warnings)
- ✅ Visual quirks present (document as known limitations)
- ✅ Features missing or simplified (document as seed project scope)

**Investigate before migration if**:

- ⚠️ Behavior seems wrong but you're unsure if it's intentional
- ⚠️ Same issue appears across multiple pages (might be environmental)
- ⚠️ Core business logic behaves unexpectedly (empty cart shows $0 vs error message)

**Remember**: Quirky but consistent behavior is documentable. Broken, inconsistent, or crashing behavior is a blocker.

------

## Key Reminders for Baseline Documentation

**What success looks like**:

- Every page has documented observations (even if they include quirks)
- Console warnings are captured, not treated as failures
- Known limitations are distinguished from blockers
- You have enough detail to write Phase 4 regression tests

**What success does NOT require**:

- Zero console errors
- Perfect visual presentation
- All features implemented
- Professional polish

**The goal**: Accurate measurement of current state, not quality judgment.

If you complete this smoke test and your summary shows "8 pages documented with 12 known limitations," you have succeeded - even though that sentence includes the word "limitations."

------

## See Also

- **Phase 1 Roadmap**: `/docs/roadmap.md` (context for this testing phase)

------

## Appendix A: Database Reset Procedure

**Prerequisites**:

- Application is stopped (close browser, stop IIS Express debugging in Visual Studio)
- SQL Server Management Studio (SSMS) installed

**Steps**:

1. Open SSMS and connect to: `(LocalDB)\MSSQLLocalDB`
2. In Object Explorer, expand **Databases**
3. Right-click `MvcMusicStore` → **Delete**
   - Check "Close existing connections" if available
   - Click OK
4. Right-click `MvcMusicStoreUsers` → **Delete**
   - Check "Close existing connections" if available
   - Click OK
5. Databases will be gone from Object Explorer
6. Start the application—databases auto-recreate with seed data

**Note**: After dropping the databases, the first time you start the MvcMusicStore application the app will pause to recreate the MvcMusicStore database on startup and again the first time you use login or register as it will need to create the MvcMusicStoreUsers database.

**Note**: We use SSMS GUI instead of T-SQL because LocalDB's connection handling makes `DROP DATABASE` scripts unreliable in development environments (Visual Studio keeps background connections open). The GUI handles this automatically.

**Why this matters**: Smoke test results must reflect seed project behavior in a known valid state. Reseeding the database ensures that the state is known and valid.

**When to drop databases**:

- ✅ Before Phase 1 baseline documentation (this smoke test)
- ✅ Before Phase 2 Azure migration (pre-migration snapshot)
- ✅ Before Phase 4 .NET 9 validation (clean comparison)
- ✅ Any time test results seem inconsistent

------

