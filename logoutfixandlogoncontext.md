Logout Fix and Login Landing Page
Objective

Resolve the Sign Out issue in the UAM Portal and introduce a Login Landing Page that allows users to initiate authentication.

The logout mechanism should correctly:

Sign the user out of the application
Terminate the Azure Entra authentication session
Redirect the user to a Login page with a Sign-In button

This feature must not modify or break the existing authentication architecture.

Fix Sign-Out Behavior

Currently, the Sign Out option in the UAM Portal is not functioning properly.

The logout action must perform the following steps.

Logout Flow
User clicks Logout
        ↓
Application clears local authentication session
        ↓
Trigger OpenID Connect SignOut
        ↓
Azure Entra Logout endpoint
        ↓
Redirect to application Login page
Azure Entra Logout Endpoint

The logout request should redirect to:

https://login.microsoftonline.com/{tenant-id}/oauth2/v2.0/logout

With redirect parameter:

post_logout_redirect_uri = /login

After logout, the user should be redirected to the Login Landing Page.

Login Landing Page

A new public landing page should be created.

Example route:

/login

This page will serve as the entry point for both UAM and CAP portals when users are not authenticated.

Login Page UI

The login page should contain:

Portal Branding

Include the organization logo.

Example:

ManpowerGroup Talent Solutions

Display the logo above the login button.

Sign-In Button

The page must display a Sign In button.

Example layout:

--------------------------------
        [ Logo ]

  Talent Solutions Portal

        [ Sign In ]

--------------------------------

Clicking Sign In should initiate the existing Azure Entra authentication flow.

Sign-In Behavior

When the user clicks Sign In:

Trigger existing OpenID Connect authentication
Redirect user to Microsoft login page
Authenticate using organization credentials
Return to portal after successful authentication

Users should authenticate using their internal organizational email accounts.

Example:

user@company.com
Login Page UI Guidelines

The login page should follow the same UI theme used in the portal.

Requirements:

Centered login card
Organization logo
Simple Sign-In button
Clean minimal design
Same font used across UAM and CAP portals

Example styling:

Centered container
Rounded login card
Soft shadow
Primary theme color button
Redirect Behavior
When user logs out
Redirect → /login
When user is not authenticated
Redirect → /login
After successful login
Redirect → portal home dashboard
Portal Access Model
Portal	Access
UAM Portal	Internal users only
CAP Portal	Internal + External users

The login page should reuse the existing authentication configuration and should not implement new authentication logic.

Important Constraints

The following must not be changed:

Azure Entra App Registration
OAuth / OpenID configuration
Authentication middleware
Existing controllers
Existing services
Database schema
Authorization policies

Allowed changes:

Logout UI behavior
Login landing page
Redirect logic
Sign-in button UI
Routing to login page
Expected Result

After implementation:

Logout works correctly in the UAM Portal.
Users are redirected to a Login Landing Page after logout.
The login page contains a Sign-In button with organization branding.
Clicking Sign-In initiates the existing Azure Entra authentication flow.
Users log in using their internal organization email accounts.
The authentication architecture remains unchanged.