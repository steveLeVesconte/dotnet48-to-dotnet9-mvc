# Phase 1 Smoke Test Results

**Date**: YYYY-MM-DD
**Tester**: [Your Name]
**Environment**: Local IIS Express
**Build/Commit**: [Git SHA]
**Test Date**: November 29, 2025
**Environment**: Windows 11, Chrome [version], IIS Express, LocalDB
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
- Site logo/title in header is not clickable (no link to home page - common UX pattern missing)
- 5 featured albums shown 
- Genre nav shows 15 genres with album counts
- All albums use single `placeholder.gif` (100x75) - no unique album art
- Jumbotron image missing alt text
- Footer does not extend full width - constrained to col-md-10 div

**Console Output**:
- Console tab: No errors or warnings

**Classification**: Known Limitation - layout quirks and missing UX patterns from seed project

**Technical Note**: Footer is inside `.col-md-10` in _Layout.cshtml (should be outside). 
Site logo/title in header is not wrapped in link to home page (standard UX pattern). 
Seed project adapted MVC3 tutorial to MVC5/Bootstrap 5 - likely missed these during conversion. 
Present across all pages using _Layout.cshtml. Not fixing during migration (preserve baseline behavior).

---

### 2. Store Index - ✅ Documented

**URL**: `/Store` or `/Store/Index`
**Authentication Required**: No

**Observations**:
- Page title shows "index" (not meaningful - should be "Browse Genres" or "Music Store")
- Main content shows "Select from 15 genres:" with full genre list
- Left sidebar also shows same 15 genres (site-wide navigation)
- Duplicate genre lists - body content became redundant when sidebar was added to _Layout.cshtml
- All genre links functional in both locations
- Shared layout elements same as previous pages (non-clickable logo, narrow footer)

**Console Output**:
- Console tab: No errors or warnings

**Classification**: Known Limitation - redundant page content and generic page title from seed project

**Technical Note**: Tutorial originally had Store Index as the genre selection page. Later step added 
genre navigation to _Layout.cshtml sidebar, making the Store Index body content redundant. 
Page title "index" is default ViewBag.Title value, never customized. 
Functionally correct but awkward UX - page could default to first genre or show featured content instead, 
and title should be more descriptive. Preserving as-is during migration (baseline behavior).

**Phase 4 Implication**: Verify both genre lists still render and link correctly post-migration. 
Verify page title remains "index" (preserve quirk). Consider UX improvements in Phase 8.

---

### 3. Store Browse Page - ✅ Documented

**URL**: `/Store/Browse?genre=Rock`
**Authentication Required**: No
**Test Parameter**: Rock genre

**Observations**:
- Genre heading "Rock" displays correctly
- 187 Rock albums shown (verified via SQL query)
- No pagination - all albums display on single page
- Each album shows: title, artist, price ($X.XX format), thumbnail
- Album title links navigate to Details page
- Shared layout elements same as previous pages (non-clickable logo, narrow footer)

**Console Output**:
- Console tab: No errors or warnings

**Classification**: ✅ Documented

**Technical Note**: No pagination implemented - all albums in genre load on single page. 
With 187 Rock albums, page is long but functional. Seed project limitation (tutorial scope). 
Preserving as-is during migration (baseline behavior).

**Phase 4 Implication**: Verify all 187 Rock albums still display post-migration. 
Page performance may differ with EF Core query patterns.

---




### 99 xxxxx Index - ✅ Documented

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

### 13. Error Page - ✅ Documented

**URL**: N/A (triggered by unhandled exception)
**Authentication Required**: No
**Test Method**: 
1. Injected `throw new Exception();` in HomeController.Index()
2. Tested both error modes by toggling `<customErrors>` in Web.config

**Observations - Development Mode** (`customErrors mode="Off"`):
- Yellow Screen of Death (YSOD) displays
- Shows detailed stack trace with file paths and line numbers
- Shows source code excerpt from controller
- Expected development behavior for debugging

**Observations - Production Mode** (`customErrors mode="On"`):
- Custom Error.cshtml view renders correctly
- Maintains site layout and navigation
- Shows user-friendly message: "We're sorry, we've hit an unexpected error"
- Provides link to return home
- No stack trace or technical details exposed

**Console Output**:
- Console tab: `Failed to load resource: the server responded with a status of 500 ()`
- This is expected - server returned HTTP 500 before rendering error page

**Classification**: ✅ Documented - error handling works as designed for both modes

**Technical Note**: HTTP 500 console error is expected behavior when unhandled exception occurs. 
Custom error page renders after the 500 response. This is standard ASP.NET error handling flow.

**Phase 4 Implication**: Verify ASP.NET Core follows same pattern:
- Development: Developer Exception Page with 500 status
- Production: Custom error page with 500 status
- Console should show same HTTP 500 failed resource message

---



---

### X. [Example Blocked Page] - 🚫 Blocked

**Blocker**: Login page throws NullReferenceException when clicking "Log in" button (regardless of credentials)

**Console Error**:

Uncaught TypeError: Cannot read property 'submit' of null at login.js:15

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

[Paste exact console messages]

**Screenshot**: [Optional - attach if helpful for documentation]

**Phase 4 Test Implication**: [What test case does this inform?]