# Teams Meetings Attendance Report
IdentityLabs

## Pre-reqs
]
 - Visual Studio or Visual Studio Code
 - Azure AD trial account
 - Office 365 account with Teams licensed user
 - Meetings that have started in the last 30 days with at least one user who joined the meeting for some moment

This scenario explores the access of attendance report in Teams Meetings API. This demo will list all events for a given user, for each event finds the onlineMeeting by joinWebUrl and then finaly gets the attendance report.

## Setup

1. Register an application on Azure AD with the following admin consent granted application permissions (https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#:~:text=%20Follow%20these%20steps%20to%20create%20the%20app,for%20your%20application.%20Users%20of%20your...%20More%20)

    - Calendars.Read > used to list user events
    - OnlineMeetings.Read > allow to access the onlineMeeting resource which contains the attendance report

2. Everytime you are going to use OnlineMeetings permission, you must allow the application to access Teams API by using an Application Access Policy. You can allow the application to access specific users or the entire tenant. Create and grant the policy using the following procedure: 

https://docs.microsoft.com/en-us/graph/cloud-communication-online-meeting-application-access-policy


3. Configure client id, tenant id, client secret and the target user's id in the appsettings.json
4. Open a terminal in the root directory and run the following command to demo: 

    dotnet run

5. The console should list the meetings for the last 30 days including the attendance report if any exists.