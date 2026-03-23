Objective

Ensure that External (Guest) users from Azure Entra ID cannot access the Internal UAM Portal.

Currently, both Internal users (Members) and External users (Guests) are able to authenticate and enter the portal. This behavior must be corrected.

The system must allow only Internal users to access the UAM Portal.

User Type Validation

Azure Entra ID provides a property called:

userType

Possible values:

userType	Meaning
Member	Internal organization user
Guest	External user invited to tenant

For the UAM Internal Portal:

Allow: userType = Member
Block: userType = Guest
Validation Rule

After successful authentication, the application must validate the userType.

Logic:

If userType == "Guest"
    Deny access to UAM portal
Else
    Allow access

External users should not be able to enter the system UI or access any pages.

Implementation Guidance

The validation should happen after authentication but before granting application access.

Possible approaches:

Validate userType from ID token claims, if available.
If the claim is not available, retrieve it from Microsoft Graph API.

Example Graph API call:

GET https://graph.microsoft.com/v1.0/me

Relevant response field:

userType
Access Denial Behavior

If the authenticated user is a Guest:

The system should:

Stop the login flow
Redirect to an Access Denied page

Example message:

Access Denied

This portal is restricted to internal organization users only.
Important Constraints

The following must NOT be modified:

Authentication mechanism
OAuth / OpenID configuration
Existing controllers
Existing services
Application architecture
Database schema
Authorization policies

Only post-authentication validation logic should be introduced.

Portal Access Model
Portal	Allowed Users
UAM Internal Portal	Internal users only (Member)
CAP Portal	Internal + External users

This ensures that:

Internal administrative systems remain restricted
External collaboration portals remain accessible
Expected Result

After implementing this rule:

Internal users (Member) can log in normally.
External users (Guest) are blocked from accessing the UAM portal.
The authentication system remains unchanged.
No existing functionality or architecture is affected.