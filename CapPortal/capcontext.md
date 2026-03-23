You are a senior .NET and Azure identity architect.

Refactor an existing ASP.NET Core CAP Portal application that currently uses Azure AD B2C authentication to instead use Microsoft Entra ID with External Identities (customer identity) authentication.

The goal is to support external users (Google, Microsoft accounts, or Email OTP) using Microsoft Entra External ID.

Do not use Azure AD B2C policies or b2clogin.com endpoints.

Use OpenID Connect with Microsoft.Identity.Web.

---

## ARCHITECTURE

Authentication Provider:
Microsoft Entra ID (External Identities)

Login Endpoint:
https://login.microsoftonline.com/

Authentication Library:
Microsoft.Identity.Web

---

## APP SETTINGS CONFIGURATION

Replace the AzureAdB2C configuration with:

"AzureAd": {
"Instance": "https://login.microsoftonline.com/",
"TenantId": "<tenant-id>",
"ClientId": "<client-id>",
"ClientSecret": "<client-secret>",
"CallbackPath": "/signin-oidc"
}

Remove the following B2C settings entirely:

SignUpSignInPolicyId
Domain
b2clogin.com references

---

## AUTHENTICATION SETUP

Configure authentication in Program.cs:

builder.Services
.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

Enable:

app.UseAuthentication();
app.UseAuthorization();

---

## INVITATION BASED REGISTRATION FLOW

Initial request will come from email invitation link:

https://localhost:5002/register?token={guid}

Example:

https://localhost:5002/register?token=0954a5a8-2673-4ff7-850d-7236bdbbe4e4

---

## STEP 1 – REGISTER ENDPOINT

Create controller endpoint:

GET /register?token={token}

Logic:

1. Validate token exists in database
2. Verify token is not expired
3. Verify invitation status = Pending

If invalid return error page.

If valid:

Store in session:

Session["InvitationToken"]
Session["InvitationEmail"]

Then redirect user to Microsoft Entra login using OpenID Connect Challenge().

---

## STEP 2 – USER AUTHENTICATION

User should authenticate using Microsoft Entra External Identity providers:

Google
Microsoft account
Email OTP

This is configured in Entra External Identities.

CAP portal only handles OpenID Connect authentication.

---

## STEP 3 – ENTRA CALLBACK

After authentication Entra redirects back to:

/signin-oidc

From the returned claims extract:

email
name
oid (object id)
tid (tenant id)

---

## STEP 4 – INVITATION VALIDATION

Retrieve invitation from session.

Compare:

InvitationEmail == AuthenticatedUserEmail

If mismatch:

Return error: "This invitation was sent to a different email address."

---

## STEP 5 – USER CREATION

If email matches:

Create CAP user in database.

Store:

Email
Name
EntraObjectId (oid)
TenantId (tid)
Role
ApplicationId
CreatedDate

---

## STEP 6 – INVITATION COMPLETION

Update invitation record:

Status = Completed
RegisteredUserId
CompletedDate

---

## STEP 7 – SIGN USER INTO CAP PORTAL

After successful registration:

Create application session
Assign roles
Redirect to CAP dashboard

---

## SECURITY REQUIREMENTS

Token must be single use.

Validate:

token exists
token not expired
status = pending

Email from Entra claim must match invitation email.

---

## EXPECTED PROJECT OUTPUT

Generate full implementation including:

Controllers
Services
Invitation validation service
User provisioning service
Authentication configuration
Session handling
Database entity models
Error pages

---

## DOCUMENTATION

Also generate a README.md explaining:

Authentication architecture
External identity login flow
Invitation based onboarding flow
Required Azure portal configuration

---

## IMPORTANT

Do not use Azure AD B2C policies.
Do not use b2clogin.com.
Use only Microsoft Entra ID OpenID Connect endpoints.

---

## FLOW SUMMARY

User receives invitation email
↓
User clicks link
/register?token=xxxx
↓
CAP validates token
↓
CAP stores invitation in session
↓
CAP redirects to Entra login
↓
User authenticates
↓
Entra redirects to /signin-oidc
↓
CAP reads user claims
↓
CAP validates invitation email
↓
CAP creates user
↓
Invitation marked completed
↓
User logged into CAP portal
