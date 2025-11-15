# Dependency Inventory - MvcMusicStore

**Document Version**: 1.0  
**Date**: November 15, 2025  
**Project**: MvcMusicStore .NET Framework to .NET 9 Migration  
**Current Framework**: .NET Framework 4.8  
**Target Framework**: .NET 9

## Purpose

This inventory catalogs all dependencies in the MvcMusicStore seed project to guide migration planning. It identifies which dependencies will require replacement, updating, or removal during the .NET 9 migration.

---

## NuGet Package Dependencies

### Core Framework

|Package|Version|Purpose|Migration Strategy|
|---|---|---|---|
|Microsoft.AspNet.Mvc|5.2.9|MVC framework|Replace with ASP.NET Core MVC (included in SDK)|
|Microsoft.AspNet.Razor|3.2.9|Razor view engine|Replace with ASP.NET Core Razor (included in SDK)|
|Microsoft.AspNet.WebPages|3.2.9|Web Pages infrastructure|Replace with ASP.NET Core equivalents|
|Microsoft.AspNet.Web.Optimization|1.1.3|Bundling and minification|Replace with modern build tools or built-in bundling|
|Microsoft.Web.Infrastructure|2.0.0|Core web infrastructure|Not needed in ASP.NET Core|

**Migration Impact**: High - Core framework replacement is the essence of Phase 4

### Authentication & Identity

|Package|Version|Purpose|Migration Strategy|
|---|---|---|---|
|Microsoft.AspNet.Identity.Core|2.2.4|Identity core framework|Replace with Microsoft.AspNetCore.Identity|
|Microsoft.AspNet.Identity.EntityFramework|2.2.4|Identity EF integration|Replace with Microsoft.AspNetCore.Identity.EntityFrameworkCore|
|Microsoft.AspNet.Identity.Owin|2.2.4|Identity OWIN integration|Not needed - ASP.NET Core uses native middleware|
|Microsoft.Owin|4.2.3|OWIN core|Replace with ASP.NET Core middleware pipeline|
|Microsoft.Owin.Host.SystemWeb|4.2.3|OWIN IIS hosting|Not needed - ASP.NET Core has Kestrel|
|Microsoft.Owin.Security|4.2.3|OWIN security|Replace with ASP.NET Core authentication|
|Microsoft.Owin.Security.Cookies|4.2.3|Cookie authentication|Replace with ASP.NET Core cookie authentication|
|Microsoft.Owin.Security.OAuth|3.0.1|OAuth middleware|Replace with ASP.NET Core OAuth handlers|
|Owin|1.0|OWIN abstractions|Not needed in ASP.NET Core|

**Migration Impact**: High - Complete authentication pipeline rewrite required

**Note**: External OAuth providers (Facebook, Google, Microsoft, Twitter) are not currently implemented but packages are present in v1 reference. Will be added in Phase 5.

### Data Access

|Package|Version|Purpose|Migration Strategy|
|---|---|---|---|
|EntityFramework|6.5.1|ORM framework|Replace with Microsoft.EntityFrameworkCore|
|EntityFramework.SqlServer|(implicit)|SQL Server provider|Replace with Microsoft.EntityFrameworkCore.SqlServer|

**Migration Impact**: High - EF6 to EF Core requires migration script updates and pattern changes

**Key Considerations**:

- Current uses DropCreateDatabaseIfModelChanges initializer
- Connection strings in Web.config move to appsettings.json
- DbContext constructor patterns change
- Lazy loading behavior differences

### Client-Side Libraries

|Package|Version|Purpose|Migration Strategy|
|---|---|---|---|
|jQuery|3.7.0|JavaScript library|Keep current version - modern and compatible|
|jQuery.Validation|1.19.5|Client-side validation|Keep - verify with ASP.NET Core validation|
|Microsoft.jQuery.Unobtrusive.Validation|3.2.11|Unobtrusive validation|Replace with ASP.NET Core validation scripts|
|bootstrap|5.2.3|UI framework|Keep - already modern version|
|Modernizr|2.8.3|Feature detection|Reassess - may be unnecessary for modern browsers|

**Migration Impact**: Low to Medium - Most client libraries remain compatible

**Phase 1a Action**: Verify AJAX cart removal functionality works in modern browsers

### Build & Optimization

|Package|Version|Purpose|Migration Strategy|
|---|---|---|---|
|Antlr|3.5.0.2|Parser for bundling|Remove - not needed with modern tooling|
|WebGrease|1.6.0|CSS/JS optimization|Remove - use modern build tools|
|Newtonsoft.Json|13.0.3|JSON serialization|Consider keeping for compatibility; ASP.NET Core uses System.Text.Json by default|

**Migration Impact**: Low - Build process modernization is straightforward

---

## System Assembly References

From `MvcMusicStore.csproj`:

**Core System Assemblies**:

- Microsoft.CSharp - Dynamic language support
- System - Core types
- System.Core - LINQ and extensions
- System.ComponentModel.DataAnnotations - Validation (mostly compatible)
- System.Xml - XML processing
- System.Xml.Linq - LINQ to XML

**Data Access**:

- System.Data - ADO.NET
- System.Data.DataSetExtensions - Dataset LINQ

**Web Framework** (Major Migration Concern):

- System.Web - Core ASP.NET Framework (must replace entirely)
- System.Web.Abstractions - Testable web abstractions
- System.Web.ApplicationServices - Membership services
- System.Web.DynamicData - Dynamic data controls
- System.Web.Entity - EF web integration
- System.Web.Extensions - AJAX extensions
- System.Web.Helpers - Helper utilities
- System.Web.Mvc - MVC framework
- System.Web.Optimization - Bundling/minification
- System.Web.Razor - Razor engine
- System.Web.Routing - URL routing
- System.Web.Services - Web services
- System.Web.WebPages - Web Pages framework
- System.Web.WebPages.Deployment - Deployment utilities

**HTTP and Configuration**:

- System.Net.Http - HTTP client (compatible)
- System.Net.Http.WebRequest - WebRequest HTTP
- System.Configuration - Configuration system (replace with Options pattern)

**Legacy/Rarely Used**:

- System.Drawing - Graphics (avoid in web apps)
- System.EnterpriseServices - COM+ integration (likely unused)
- System.Security - Security APIs

**Migration Impact**: All `System.Web.*` assemblies are Framework-specific and must be replaced with `Microsoft.AspNetCore.*` equivalents.

---

## Database Configuration

### Connection Strings

From `Web.config`:

```xml
<connectionStrings>
  <add name="MusicStoreEntities" 
       connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MvcMusicStoreV2;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
  <add name="IdentityConnection"
       connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MvcMusicStoreUsersV2;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Current Setup**:

- Two separate LocalDB databases
- System.Data.SqlClient provider

**Migration Changes**:

- Move to appsettings.json
- Replace System.Data.SqlClient with Microsoft.Data.SqlClient
- Phase 2: Migrate to Azure SQL Database
- Consider consolidating into single database or using separate contexts with single connection string

### Database Contexts

1. **MusicStoreEntities** - Main application data
    
    - Albums, Artists, Genres
    - Shopping Cart items
    - Orders and OrderDetails
2. **ApplicationDbContext** - ASP.NET Identity
    
    - Users, Roles, Claims
    - Separate database for security isolation

**Initializers**:

- SampleData: DropCreateDatabaseIfModelChanges (seeds music catalog)
- IdentityInitializer: DropCreateDatabaseIfModelChanges (creates admin user)

**Migration Note**: EF Core uses different initialization patterns - move to explicit migrations

---

## Application Configuration

### Web.config Settings

**Application Settings**:

```xml
<appSettings>
  <add key="webpages:Version" value="3.0.0.0" />
  <add key="webpages:Enabled" value="false" />
  <add key="ClientValidationEnabled" value="true" />
  <add key="UnobtrusiveJavaScriptEnabled" value="true" />
</appSettings>
```

**System.Web Settings**:

```xml
<system.web>
  <compilation debug="true" targetFramework="4.8" />
  <httpRuntime targetFramework="4.8" />
</system.web>
```

**Assembly Binding Redirects**: 15 redirect entries for version resolution

**Entity Framework Configuration**: LocalDB provider configuration

**Migration Strategy**:

- Convert to appsettings.json / appsettings.Development.json
- Remove binding redirects (SDK handles this)
- Move compilation settings to .csproj
- EF configuration moves to DbContext OnConfiguring or dependency injection

---

## Application Startup & Configuration

### Global.asax.cs

Current initialization sequence:

```csharp
protected void Application_Start()
{
    Database.SetInitializer(new SampleData());
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
    Database.SetInitializer(new IdentityInitializer());
}
```

**Migration Target**: Program.cs / Startup.cs pattern

- Replace with middleware pipeline configuration
- Move to dependency injection-based initialization
- Update routing to endpoint routing

### OWIN Startup (Startup.cs)

Current configuration:

- Cookie authentication
- Per-request context creation for Identity
- 2-hour cookie expiration with sliding expiration

**Migration Target**: ASP.NET Core authentication middleware

- Configure in Program.cs
- Use built-in authentication services

### Bundle Configuration (BundleConfig.cs)

Current bundles:

- jQuery
- jQuery validation
- Modernizr
- Bootstrap
- Site CSS

**Migration Options**:

1. ASP.NET Core bundling (link tags)
2. Modern build tool (webpack, Vite)
3. CDN references

---

## Controllers & Routes

### Controller Inventory

|Controller|Authorization|Key Dependencies|Migration Concerns|
|---|---|---|---|
|HomeController|None|EF queries|Update to async patterns|
|StoreController|None|EF queries|Update to async patterns|
|StoreManagerController|[Authorize(Roles = "Admin")]|EF CRUD|Policy-based auth, async|
|ShoppingCartController|None (CartSummary uses [ChildActionOnly])|Session, HttpContext|Session configuration, ViewComponent for child action|
|CheckoutController|[Authorize]|Session, HttpContext|Session configuration, async|
|AccountController|Mixed|Identity, OWIN|Complete Identity rewrite|

**Key Migration Issues**:

- [ChildActionOnly] → ViewComponents
- Direct HttpContext.Session access → Configured session middleware
- Synchronous database operations → async/await patterns
- OWIN authentication → ASP.NET Core authentication

---

## Session State

**Current Implementation**:

- In-memory session
- Shopping cart identified by session
- Session key: "CartId"
- Anonymous cart migration to user cart on login

**Migration Phases**:

- Phase 1-3: Keep in-memory session (single instance)
- Phase 2: Document session behavior for Azure deployment
- Phase 7: Implement distributed caching (Azure Redis Cache)

**Critical Code Locations**:

- ShoppingCart.GetCart(HttpContextBase)
- ShoppingCart.GetCartId(HttpContextBase)
- AccountController.MigrateShoppingCart()

---

## Views & Client-Side Assets

### Razor Views

Total: 15 views across 6 folders

**Shared**:

- _Layout.cshtml
- _NavPartial.cshtml
- Error.cshtml

**Migration Impact**: Low - Razor syntax mostly compatible, update tag helpers

### JavaScript Assets

**Custom Scripts**:

- None explicitly listed (may exist in Content/Scripts)

**Third-Party Scripts**:

- jQuery 3.7.0 (standard, slim, minified)
- jQuery Validation 1.19.5
- jQuery Unobtrusive Validation
- Bootstrap 5.2.3 (bundle, ESM variants)
- Modernizr 2.8.3

**Migration Impact**: Low - Modern versions already in use

### CSS Assets

**Frameworks**:

- Bootstrap 5.2.3 (including RTL, utilities, grid, reboot)
- Custom Site.css

**Images**:

- Logo, placeholder, showcase, screenshot

**Migration Impact**: Low - CSS is framework-agnostic

---

## Development Environment

### Project Configuration

From `.csproj`:

- **ToolsVersion**: 15.0 (Visual Studio 2015+)
- **Target Framework**: .NET Framework 4.8
- **Project Type GUID**: Web Application
- **IIS Express**: Port 44363 (HTTPS)
- **Build Output**: bin/

**Migration Changes**:

- SDK-style project format
- .NET 9 target framework
- Simplified .csproj (remove explicit file listings)

### Required Tools

**Current**:

- Visual Studio 2015 or later
- .NET Framework 4.8 SDK
- LocalDB (SQL Server)
- IIS Express

**Post-Migration**:

- Visual Studio 2022, VS Code, or Rider
- .NET 9 SDK
- SQL Server or Azure SQL Database
- Kestrel web server (cross-platform)

---

## Security Features

### Current Implementation

**Authentication**:

- Cookie-based authentication via OWIN
- Email + password
- Session-based cart identification

**Authorization**:

- Role-based (Admin, Visitor)
- Admin role required for StoreManager
- Checkout requires authenticated user

**Password Policy** (IdentityConfig.cs):

```csharp
RequiredLength = 6
RequireNonLetterOrDigit = false
RequireDigit = false
RequireLowercase = false
RequireUppercase = false
```

**Anti-Forgery**:

- [ValidateAntiForgeryToken] on POST actions

**Migration Considerations**:

- Phase 4: Strengthen password requirements
- Phase 5: Add external OAuth providers
- Phase 5: Implement Azure Key Vault for secrets
- Consider adding rate limiting, HTTPS redirection

---

## Testing Infrastructure

**Current State**: No test project included in provided files

**Planned (Phase 1)**:

- xUnit test framework
- Integration tests for critical workflows:
    - Anonymous browsing
    - Shopping cart operations
    - User registration and login
    - Checkout process
    - Admin CRUD operations

**Migration Impact**: Tests must be updated for ASP.NET Core:

- TestServer for integration tests
- Update mocking strategies
- Async test patterns

---

## Missing or Unused Features

### Not Currently Implemented

1. **Email Services**: Registration/password reset emails
2. **External OAuth**: Facebook, Google, Microsoft, Twitter (packages present but not configured)
3. **Payment Processing**: Only promo code validation ("FREE")
4. **File Upload**: Album art URL is text field, not upload
5. **Logging**: No structured logging framework
6. **Health Checks**: No health endpoints
7. **API Endpoints**: Pure MVC, no Web API controllers

### Commented Out / Disabled

From reference documents:

- External OAuth provider configurations in Startup.Auth.cs (v1)

---

## Risk Assessment by Phase

### Phase 1 (Foundation) - Low Risk

- xUnit adoption - straightforward
- Application Insights - additive only
- No breaking changes to codebase

### Phase 2 (Azure Migration) - Medium Risk

- Database migration critical path
- Connection string management
- IaC learning curve
- Secrets management setup required

### Phase 3 (Azure Validation) - Low Risk

- Validation only
- No code changes

### Phase 4 (Platform Migration) - **CRITICAL HIGH RISK**

- All System.Web.* dependencies must be replaced
- OWIN → ASP.NET Core middleware complete rewrite
- EF6 → EF Core breaking changes
- Identity 2.x → ASP.NET Core Identity schema differences
- Session state configuration changes
- Routing system differences
- [ChildActionOnly] → ViewComponents refactor

### Phase 5 (Modern Auth) - Medium Risk

- External OAuth adds complexity
- Azure Key Vault integration
- Secrets rotation strategy

### Phases 6-10 - Medium Risk

- Additive features primarily
- Performance optimization without breaking changes

---

## Critical Success Factors

### Must-Have Before Phase 4

1. **Comprehensive test coverage** - Safety net for breaking changes
2. **Performance baselines** - Prove no regression
3. **Database migration scripts** - Preserve data integrity
4. **Session behavior documentation** - Understand state management implications

### Must-Verify During Phase 4

1. **Identity schema compatibility** - Users can still log in
2. **Shopping cart persistence** - Anonymous carts survive
3. **Role-based authorization** - Admin access still works
4. **AJAX functionality** - Cart operations still work
5. **Database queries** - EF Core LINQ differences don't break logic

### Must-Not-Do

1. **Don't combine Phase 4 with other major changes** - Platform migration alone is complex enough
2. **Don't skip performance comparison** - Regression is unacceptable
3. **Don't assume test compatibility** - Rewrite tests as needed for ASP.NET Core patterns

---

## Dependency Change Tracking

This table will be updated as packages are replaced during migration:

|Original Package|Version|Replacement Package|Version|Phase|Status|
|---|---|---|---|---|---|
|EntityFramework|6.5.1|Microsoft.EntityFrameworkCore|TBD|4|Not Started|
|Microsoft.AspNet.Mvc|5.2.9|Microsoft.AspNetCore.Mvc|TBD|4|Not Started|
|Microsoft.AspNet.Identity.*|2.2.4|Microsoft.AspNetCore.Identity|TBD|4|Not Started|
|_(Add others as replaced)_||||||

---

## Quick Reference: Migration Cheat Sheet

**Most Impactful Namespace Changes**:

```
System.Web.Mvc                    → Microsoft.AspNetCore.Mvc
System.Data.Entity                → Microsoft.EntityFrameworkCore
Microsoft.AspNet.Identity         → Microsoft.AspNetCore.Identity
System.Web.Optimization           → (use build tools or TagHelpers)
```

**Most Impactful API Changes**:

```
[ChildActionOnly]                 → ViewComponent
Global.asax.cs                    → Program.cs / Startup.cs
Web.config                        → appsettings.json
DbContext.Database.SetInitializer → EF Core Migrations
HttpContext.GetOwinContext()      → HttpContext directly
```

**Most Common Gotchas**:

1. Session must be explicitly configured in ASP.NET Core
2. EF Core does not support DropCreateDatabase* initializers
3. ASP.NET Core Identity has different schema (migration required)
4. Routing uses endpoint routing pattern
5. Authentication middleware order matters in pipeline

---

## Document Maintenance

**Frozen After**: Phase 1 completion (baseline captured)

**Update Only For**:

- Discovered dependencies not in original inventory
- Corrections to version numbers or package names
- Additional migration notes based on actual experience

**Do Not Update For**:

- Replaced packages (use Dependency Change Tracking table)
- Post-migration versions (separate document in Phase 4+)

---

**Last Updated**: November 15, 2025  
**Document Status**: ✅ Ready for Phase 1 Baseline