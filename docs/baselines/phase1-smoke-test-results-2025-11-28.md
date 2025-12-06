# Phase 1 Smoke Test Results

**Test Date**: November 29, 2025  
**Documentation Date**: December 4, 2025  
**Tester**: Steve LeVesconte  
**Environment**: Windows 11, Chrome 142.0.7444.176 (Official Build) (64-bit), IIS Express, LocalDB  
**Build**: [a0dc430](https://github.com/steveLeVesconte/dotnet48-to-dotnet9-mvc/commit/a0dc430c7817675b256549dfff987160f4fa7b22)  
**Test Data State**: Fresh seed data from original seed project

## Baseline Documentation Status

**Overall**: ✅ COMPLETE

**Summary**:
- Pages Documented: **13 / 13** (including bonus error page)
- Pages Needing Investigation: **0**
- Pages Blocked: **0**
- Known Limitations Captured: **10** (layout quirks, no pagination, generic titles, etc.)
- Pre-baseline Fixes Applied: **2** (script ordering for cart removal, checkout validation)
- Phase 4 Regression Test Scenarios Identified: **13**

**Baseline Status**: All pages successfully documented. Application ready for Phase 1 automated testing (Playwright/xUnit).

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
- **No pagination - all albums display on single page**
- Each album shows: title, artist thumbnail
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


### 4. Store Details Page - ✅ Documented

**URL**: `/Store/Details/1`
**Authentication Required**: No
**Test Parameter**: Album ID 1

**Observations**:
- Album title displayed as page heading
- Artist name, genre, and price visible
- Price format: X.XX (no currency symbol)
- Album artwork: placeholder thumbnail (100x75) - no full-size image
- "Add to cart" displays in gray box
- Shared layout elements same as previous pages (non-clickable logo, narrow footer)

**Console Output**:
- Console tab: No errors or warnings

**Classification**: ✅ Documented

**Technical Note**: "Add to cart" implemented as anchor (`<a>`) inside `<p class="button">` 
rather than actual button element. Hover target is text link only, not entire gray box. 
Non-standard HTML pattern but functional. Preserve during migration.

**Phase 4 Implication**: Verify "Add to cart" maintains same visual appearance and clickable behavior. 
Test cart increment when clicked. Verify price format remains X.XX (no currency symbol).

---

### 5. Shopping Cart - ✅ Documented

**URL**: `/ShoppingCart`
**Authentication Required**: No
**Test Setup**: Added 2 albums to cart before testing

**Observations**:
- Cart displays items in table format (album name, price, quantity per line)
- Each line item shows: album title, price, quantity, subtotal
- "Remove from cart" link present for each item
- Cart total displayed prominently (sum of all line items)
- "Checkout" button visible and enabled (remains enabled even with empty cart)
- Empty cart state: Shows "Your cart is empty" message with $0.00 total
- Shared layout elements same as previous pages (non-clickable logo, narrow footer)

**Checkout Button Behavior** (tested both scenarios):
- When NOT logged in: Redirects to `/Account/Login` with return URL
- When logged in: Navigates to `/Checkout/AddressAndPayment`
- Works in both empty cart and populated cart states

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

**Technical Note**: Cart removal uses AJAX POST to `/ShoppingCart/RemoveFromCart` with 
jQuery animations for UI feedback. Original seed project had broken script ordering 
(AJAX code executed before jQuery loaded). Fixed by moving cart removal script to 
`@section Scripts` pattern - resolved before baseline documentation. This fix restores 
original MVC3 tutorial functionality using modern MVC script section pattern.

**Known Limitation**: Checkout button remains enabled when cart is empty. Clicking 
checkout with empty cart navigates to checkout page (no client-side validation). 
Seed project design - no cart item count check before allowing checkout navigation.

**Phase 4 Implication**: 
- Verify AJAX cart removal still works (fadeOut animation, dynamic total updates)
- Verify checkout navigation: unauthenticated redirects to login, authenticated proceeds
- Verify empty cart checkout behavior preserved (button enabled, navigation allowed)
- Test header cart count badge updates correctly after removal

---

### 6. Checkout (Address and Payment) - ✅ Documented

**URL**: `/Checkout/AddressAndPayment`
**Authentication Required**: YES
**Test Setup**: Login with test account, add item to cart

**Observations**:
- Page displays checkout form with all expected fields
- All form fields present: First Name, Last Name, Address, City, State, Postal Code, Country, Phone, Email
- Promo Code field visible
- "Submit Order" button enabled
- Client-side validation working (tested by submitting empty form)
- Validation messages display without server round-trip
- Required field indicators visible

**Console Output**:
- Console tab: No errors or warnings

**Classification**: ✅ Documented

**Technical Note**: Original seed project had script ordering issue - jQuery validation 
scripts loaded before jQuery core. Fixed by using `@section Scripts` with proper bundle 
order. Client-side validation now works correctly. Pre-baseline fix (seed project bug).

**Phase 4 Implication**: Verify ASP.NET Core validation helpers work correctly. 
Test both client-side (immediate feedback) and server-side (if JavaScript disabled) 
validation. Verify promo code validation logic.

---

### 7. Checkout Complete - ✅ Documented

**URL**: ``/Checkout/Complete` (after successful checkout)
**Authentication Required**: YES 
**Test Setup**: Login with test account, add item to cart, fill out Address and Payment Page, click "Submit Order"

**Observations**:

- Page title "Checkout Complete"
- Order number displayed
- "How about shopping for some more music in our store" displayed and "store" is link

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

---

### 8. Admin (Store Manager) 

**URL**: /StoreManager
**Authentication Required**: YES 
**Test Setup**: Login with Admin and click on "Admin" on the menu

**Observations**:
- Album list displays (462 albums visible on page) (no pagination)
- Page title shows "Index" instead of "Store Manager" (KNOWN LIMITATION per seed docs)
- Noticed: Create/Edit/Delete links work but Delete shows confirmation page, not modal

- "Create New" button/link visible

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

***

### 9. Create New (Store Manager) 

**URL**: ``/StoreManager/Create
**Authentication Required**: YES 
**Test Setup**: On Store Manager Page, click on "Create New"

**Observations**:

- Page title "Create"
- Create Album form displays with all required fields:
  - GenreId as populated Dropdown List
  - ArtistId as populated Dropdown List
  - Title
  - Price
  - Album Art URL
  - "Back to List" link visible and enabled

- "Create" button visible and enabled
- "Back to List" link visible and enabled
- **Create Button Behavior** (tested both scenarios):
  - When not all required fields are filled - required message displayed and required fields turn pink. Only Title and Price are "required". The dropdown list fields have no blank option, so they always default to filled.
  - When all required fields are filled in: Navigates to `/StoreManager` and the newly added album is displayed at the bottom of the list.

 - **Back To List Link Behavior** 
   - Navigates to `/StoreManager` without saving (discards form data).

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

---

### 10. Register Page 

**URL**: ``/Account/Register`
**Authentication Required**: NO
**Test Setup**: Must not be logged in. Click on "Register" on the menu

**Observations**:

- Page title "Register"
- Register form displays with these fields:
  - Email
  - Password
  - Confirm Password
- "Register" button visible and enabled
- **Register Button Behavior**: 
  - When not all required fields are filled - required message displayed an required fields turn pink.  
  - When an invalid email (not conforming to email address standards) is entered, The message "**The Email field is not a valid e-mail address**" is displayed and the email address is displayed as pink.
  - When the Password and Confirm Password do not match, The message "**The password and confirmation password do not match.**" 
  - When the Password and Confirm Password match but are only 3 characters long, The message **The Password must be at least 6 characters long**
  - When all required fields are filled and other validation rules are satisfied  in: Navigates to `/ (home page) and "Hello, emailAddress!" is displayed in the menu header` and "Log off" is displayed in the header menu.

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

---

### 11. Login Page 

**URL**: /Account/Login
**Authentication Required**: NO
**Test Setup**: Must not be logged in. Click on "Log in" on the header menu.

**Observations**:

- Page title "Log in"
- Login form displays with these fields:
  - Email
  - Password
  - Remember me? checkbox
- "Register as New User" link is visible and enabled
- "Log in" button visible and enabled
- **Login Button Behavior** (tested both scenarios):
  - When not all required fields are filled - required message displayed an required fields turn pink.  
  - When an invalid email (not conforming to email address standards) is entered, The message "**The Email field is not a valid e-mail address**" is displayed and the email address is displayed as pink.  Note: this happens just by typing in an invalid email address.  Clicking the "Log in" button is not required to see this message.
  - When the Email and Password are not found in the database (so, not a valid user), The message "**Invalid login attempt.**" 
  - When invalid credentials entered 20 times, no lockout occurred.
  - When a valid Password and Email entered,  Navigates to `/ (home page) and "Hello, emailAddress!" is displayed in the menu header` and "Log off" is displayed in the header menu.

**Console Output**:

- Console tab: No errors or warnings

**Classification**: ✅ Documented

---

### 12. Store Manager Page (Authorization Check) - ✅ Documented

**URL**: `/StoreManager`
**Authentication Required**: Admin role
**Test Setup**: Click "Admin" in header menu under different authentication states

**Observations**:
- When not authenticated: Redirects to `/Account/Login`
- When authenticated as visitor role (visitor@test.com): Redirects to `/Account/Login`
- When authenticated as admin role (admin@musicstore.com): Displays Store Manager page

**Tested Scenarios**:
- ✅ Not logged in → redirects to login
- ✅ Logged in as visitor role → redirects to login  
- ✅ Logged in as admin role → displays Store Manager page

**Console Output**:
- Console tab: No errors or warnings

**Classification**: ✅ Documented

**Phase 4 Implication**: Verify ASP.NET Core `[Authorize(Roles = "Administrator")]` attribute works identically. Test all three authentication states.

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

## Known Limitations Summary

Seed project quirks documented and preserved for migration baseline:

1. **Layout**: Footer doesn't extend full width (inside col-md-10)
2. **Navigation**: Site logo/title not clickable (missing home link)
3. **Store Index**: Page title shows "index" instead of meaningful title
4. **Store Index**: Duplicate genre lists (sidebar + main content redundant)
5. **Store Browse**: No pagination (187 Rock albums on single page)
6. **Details Page**: "Add to cart" is text link, not button element
7. **Shopping Cart**: Checkout button enabled even with empty cart
8. **Store Manager**: Page title shows "Index" instead of "Store Manager"
9. **Store Manager**: No pagination (462 albums on single page)
10. **Login**: No account lockout after failed attempts

**Pre-Baseline Fixes** (restoring original tutorial functionality):
- Fixed script ordering for cart removal AJAX (jQuery loaded before cart scripts)
- Fixed script ordering for checkout validation (jQuery validation after jQuery core)

These fixes restore broken seed project features to working state and become part of baseline.

---