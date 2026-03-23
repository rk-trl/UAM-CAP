# External User Onboarding Flow (UAMP + CAP) using Microsoft Entra ID

## Purpose

This document defines the external user onboarding and authentication flow used by the UAMP and CAP portals.

The system uses Microsoft Entra ID External Identities for authentication and Microsoft Graph API for guest user provisioning.

The goal is to allow external users (Gmail, Microsoft accounts, corporate domains) to access the CAP portal through invitation-based onboarding.

---

# Identity Platform

Authentication Provider
Microsoft Entra ID

External Identity Type
External Identities (Guest users)

API used for provisioning
Microsoft Graph API

Authentication Protocol
OpenID Connect (OIDC)

Login Endpoint
https://login.microsoftonline.com/

---

# System Components

UAMP Portal
Responsible for user onboarding, invitations, and role management.

CAP Portal
Responsible for user registration completion and application access.

Microsoft Entra ID
Responsible for authentication and identity provider federation.

Microsoft Graph API
Used by UAMP portal to create guest users in Entra.

---

# High Level Flow

Admin invites external user from UAMP portal.

UAMP creates an invitation record in the database.

UAMP calls Microsoft Graph API to create a guest user in Entra.

UAMP generates an invitation token.

UAMP sends an invitation email with link:

https://localhost:5002/register?token={invitationToken}

User clicks the link.

CAP portal validates the invitation token.

CAP portal redirects the user to Microsoft Entra login.

User authenticates using one of the allowed identity providers:

Microsoft account
Google account
Email OTP

After authentication Microsoft Entra redirects the user back to CAP.

CAP portal receives user claims.

CAP verifies that the authenticated email matches the invitation email.

If valid, CAP completes registration and creates a CAP user record.

CAP marks the invitation as completed.

User is logged into CAP portal.

---

# UAMP Portal Responsibilities

When an admin invites a user:

1. Create invitation record in database

Fields

InvitationId
Email
ApplicationId
RoleId
Token
Status (Pending / Completed)
CreatedDate

2. Create guest user in Entra using Microsoft Graph API.

Endpoint

POST https://graph.microsoft.com/v1.0/invitations

Example request

{
"invitedUserEmailAddress": "[user@gmail.com](mailto:user@gmail.com)",
"inviteRedirectUrl": "https://localhost:5002/register?token={token}",
"sendInvitationMessage": false
}

3. Store Entra invited user id returned from Graph.

4. Send invitation email containing

https://localhost:5002/register?token={token}

---

# CAP Portal Responsibilities

CAP handles invitation validation and authentication callback.

Initial endpoint

GET /register?token={token}

Flow

1. Validate invitation token exists.
2. Ensure status = Pending.
3. Ensure token not expired.

If invalid show error page.

If valid:

Store invitation context in session

Session["InvitationToken"]
Session["InvitationEmail"]

Then redirect to Entra authentication.

Use OpenID Connect Challenge().

---

# Authentication Configuration

CAP portal must authenticate using Microsoft Entra ID.

Appsettings configuration

"AzureAd": {
"Instance": "https://login.microsoftonline.com/",
"TenantId": "<tenant-id>",
"ClientId": "<client-id>",
"ClientSecret": "<client-secret>",
"CallbackPath": "/signin-oidc"
}

Use Microsoft.Identity.Web for authentication.

Remove any Azure AD B2C configuration.

Do not use b2clogin.com endpoints.

---

# Entra Callback Handling

After login Entra redirects to

/signin-oidc

CAP must extract the following claims

email
name
oid (Entra ObjectId)
tid (TenantId)

---

# Invitation Verification

CAP retrieves invitation from session.

Compare

Invitation.Email == AuthenticatedUserEmail

If mismatch

Return error

"This invitation was issued for a different email address."

---

# User Creation in CAP

If email matches invitation

Create CAP user

Fields

Email
Name
EntraObjectId (oid)
TenantId
RoleId
ApplicationId
CreatedDate

---

# Invitation Completion

Update invitation record

Status = Completed
RegisteredUserId
CompletedDate

---

# Final Login

After successful registration

Create CAP user session.

Assign application roles.

Redirect user to CAP dashboard.

---

# Security Rules

Invitation tokens must be single use.

Invitation must expire.

Authenticated email must match invitation email.

Store Entra ObjectId for future logins.

Always validate token status before authentication.

---

# Expected Behavior

Users cannot directly register in CAP portal.

Access is only allowed via invitation link.

External authentication must always be performed through Microsoft Entra ID.

---

# Scope for Implementation

Copilot should only implement missing pieces required for:

Invitation token validation
Graph API invitation call in UAMP
CAP authentication redirect
CAP callback claim processing
CAP user provisioning
Invitation completion logic

Existing portal features must remain unchanged.
