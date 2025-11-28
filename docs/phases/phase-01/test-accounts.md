# Test Accounts - MVC Music Store

**Last Updated**: November 28, 2025

**Purpose**: Document test account credentials for smoke testing and development. These accounts must be created once before running the smoke test procedures.

**Security**: This file is excluded from git via `.gitignore`. Never commit credentials to version control.

------

## Account Overview

| Role    | Username/Email       | Purpose                     | Required For                            |
| ------- | -------------------- | --------------------------- | --------------------------------------- |
| Admin   | admin@musicstore.com | Store management operations | Tests 8-9 (StoreManager, Create Album)  |
| Visitor | visitor@test.com     | Customer checkout flows     | Tests 6-7 (Checkout, Checkout Complete) |

------

## Account Sources

- **Admin account**: Auto-created by identity initializer (`Models/IdentityInitializer.cs`)
- **Visitor account**: Manually registered via UI (one-time setup)

This hybrid approach reflects the seed project's design: admin users are infrastructure, visitor accounts simulate real user registration flows.

------

## Admin Account

**Email**: `admin@musicstore.com`  
**Password**: `Admin123!`  
**Role**: Administrator

(Password meets policy: 1 uppercase, 1 lowercase, 1 number, 1 special char)

**Permissions**:

- Access to `/StoreManager` (album CRUD operations)
- Create, edit, delete albums
- All visitor permissions

**Used In Tests**:

- Test 8: Admin - StoreManager
- Test 9: Create a New Album

**Note**: This account is created by `Models/IdentityInitializer.cs` (see `Seed()` method) during database initialization. No manual setup required.

### Verifying Admin Account Seeding

The admin account is created automatically during database initialization.

**To verify the account exists:**

1. Start application in IIS Express
2. Navigate to `/Account/Login`
3. Login with `admin@musicstore.com` / `Admin123!`
4. Navigate to `/StoreManager` (should display album list, not access denied)

------

## Visitor Account

**Email**: `visitor@test.com`  
**Password**: `Visitor@123456`  
**Role**: Registered User (no admin privileges)

(Password meets policy: 1 uppercase, 1 lowercase, 1 number, 1 special char)

**Permissions**:

- Browse store and albums
- Add items to cart
- Complete checkout process
- View order confirmation

**Used In Tests**:

- Test 6: Checkout (authenticated)
- Test 7: Checkout Complete
- Test 10: Store Manager Page (authorization check - should be denied)

**Note**: This account must be created manually via the registration UI (one-time setup).

------

## Creating the Visitor Account

The visitor account simulates a real user registration flow and must be created manually before your first smoke test run.

**Setup Steps**:

1. Start the application in IIS Express
2. Navigate to `/Account/Register`
3. Register with:
   - Email: `visitor@test.com`
   - Password: `Visitor@123456`
   - Confirm Password: `Visitor@123456`
4. No additional configuration needed (standard user role assigned automatically)

------

## Password Policy

The MVC Music Store uses ASP.NET Identity 2.x default password requirements:

- **Minimum length**: 6 characters
- **Required characters**: At least one uppercase, one lowercase, one number, one special character
- **Example valid passwords**: `Test@123456`, `Admin123!`, `Visitor@123456`

------

## Account Maintenance

### When to Recreate Accounts

Recreate test accounts if:

- Database is dropped and reseeded (admin auto-recreates, visitor needs manual re-registration)
- Account credentials are compromised
- Password policy changes in future phases
- Testing requires a fresh user state (no order history)

### Verification Steps

After setup, verify both accounts work correctly:

1. **Admin Account**:
   - Login with `admin@musicstore.com` / `Admin123!`
   - Navigate to `/StoreManager`
   - Verify page loads (should see album list, not access denied)
2. **Visitor Account**:
   - Login with `visitor@test.com` / `Visitor@123456`
   - Navigate to `/StoreManager`
   - Verify access is denied (should redirect to login)
   - Add an album to cart
   - Navigate to `/Checkout/AddressAndPayment`
   - Verify checkout form displays


------

 **Last Verified**: [Add date after you verify setup works]