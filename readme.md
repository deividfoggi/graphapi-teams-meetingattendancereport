# Application permission
IdentityLabs

## Pre-reqs

 - .net 5.0
 - Any code editor to adjust settings like VSCode
 - Azure AD trial account
 - Office 365 or Outlook.com account

This scenario explores the application permissions using the client credentials grant flow, such as an automation that will get mails from user's inbox.

## Setup

1. Register an application on Azure AD with the following admin consent granted application permissions (https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#:~:text=%20Follow%20these%20steps%20to%20create%20the%20app,for%20your%20application.%20Users%20of%20your...%20More%20)

    - Mail.Read

2. Configure client id, tenant id, client secret and the target user's id in the appsettings.json
3. Open a terminal in the root directory and run the following command to demo: 

    dotnet run

4. The console should list user's inbox email. Make sure the target user has some e-mails in the mailbox.