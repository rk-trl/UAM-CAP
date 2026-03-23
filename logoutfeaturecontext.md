Logout Feature Implementation – UAM Portal and CAP Portal
Objective

Implement a Logout feature for both portals:

UAM Internal Portal
CAP Portal

The logout functionality must sign the user out of the application and Azure Entra authentication session.

This implementation must reuse the existing authentication setup and must not introduce architectural changes.

Logout Button Placement

Both portals must provide a Logout option in the UI.

Recommended placement:

Sidebar Footer
Account & Settings
Help & Support
Sign Out

or

Header Profile Menu

Example:

RN  (User Profile)
   Profile
   Logout

The Logout action must be accessible globally from any page.

Logout Behavior

When the user clicks Logout:

The system must:

1. Sign the user out of the application session
2. Clear authentication cookies
3. Redirect to Azure Entra logout endpoint
4. Redirect the user back to the application login page
Azure Entra Logout Endpoint

Logout must trigger the OpenID Connect sign-out flow.

Example logout endpoint:

https://login.microsoftonline.com/{tenant-id}/oauth2/v2.0/logout

After logout, redirect the user to the application's login page.

Example:

post_logout_redirect_uri = https://yourportalurl/signin
Application Logout Flow

The logout process should follow this sequence:

User Clicks Logout
        ↓
Application clears session
        ↓
OpenID Connect SignOut triggered
        ↓
Azure Entra Logout
        ↓
Redirect back to Portal Login Page
Implementation Guidance

Logout should use the existing authentication middleware.

Example behavior:

Trigger OpenID Connect SignOut
Clear authentication cookies
Redirect user to login page

The implementation must reuse existing authentication configuration already present in the application.

UI Consistency

Both portals should use the same logout mechanism.

Portal	Logout Location
UAM Portal	Sidebar or Header
CAP Portal	Sidebar or Header

No difference in logout behavior should exist between the portals.

Important Constraints

Do not modify:

Authentication configuration
Azure Entra app registration
OAuth/OpenID settings
Existing controllers
Existing services
Authorization policies

Allowed modifications:

UI logout button
Logout route or endpoint
Triggering OpenID SignOut
Redirect behavior
Expected Result

After implementing logout:

Users can click Logout from both portals.
The system signs the user out of the application.
Azure Entra authentication session is terminated.
The user is redirected to the login screen.
No existing authentication functionality is broken.