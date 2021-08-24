using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Helpers;

namespace graphdaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = LoadAppSettings();
            if (config == null)
            {
                Console.WriteLine("Invalid appsettings.json file.");
                return;
            }
            var client = GetAuthenticatedGraphClient(config);

            // Get current date to be used in get events filter
            DateTime utcTimeNow = DateTime.UtcNow;
            // Get current date minus 30 days to be used in get events filter
            DateTime utcTime = DateTime.UtcNow.AddDays(-30);

            // List all events from user in appsettings.json targetUserId key filtering for events booked to start for the last 30 days
            var requestUserEvents = client.Users[config["targetUserId"]].Events
                .Request()
                .Filter("start/dateTime ge '" + utcTime + "' and start/dateTime le '" + utcTimeNow + "'")
                .GetAsync();

            var results = requestUserEvents.Result;

            // for each event we are going to get the respective onlineMeeting. Unfortunately API doesn't allow you to get all onlineMeetings for a given user,
            // this is why we are using events to get onlineMeetings. You must always filter which onlineMeeting object you want to get. It is not possible to go in the
            // onlineMeetings root and get the list of all meetings even in Beta endpoint which is the case of this demo
            foreach (var item in results)
            {
                // Checks if the user is the organizer of the event. Right now, the API doesn't support filtering by organizer so this is why we must check it here
                if(item.IsOrganizer == true)
                {
                    var requestOnlineMeeting = client.Users[config["targetUserId"]].OnlineMeetings
                        .Request()
                        .Filter("JoinWebUrl eq '" + item.OnlineMeeting.JoinUrl + "'")
                        .GetAsync();

                    var onlineMeetingResult = requestOnlineMeeting.Result;

                    // For each onlineMeeting found, we print the results including the MeetingAttendance report if any exists. The report become avaiable after meeting ends.
                    foreach (var item2 in onlineMeetingResult)
                    {
                        var meetingAttendanceReport = client.Users[config["targetUserId"]].OnlineMeetings[item2.Id].MeetingAttendanceReport
                            .Request()
                            .GetAsync().Result;

                        Console.WriteLine("");
                        Console.WriteLine("Subject : " + item.Subject);
                        Console.WriteLine("onlineMeeting: " + item.OnlineMeeting.JoinUrl);
                        Console.WriteLine("Attendance Report: " + JsonSerializer.Serialize(meetingAttendanceReport));
                    }
                }
            }
        }

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", false, true)
                                .Build();

                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["applicationSecret"]) ||
                    string.IsNullOrEmpty(config["tenantId"]) ||
                    string.IsNullOrEmpty(config["targetUserId"]))
                {
                return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var tenantId = config["tenantId"];
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithClientSecret(clientSecret)
                                                    .Build();
            return MsalAuthenticationProvider.GetInstance(cca, scopes.ToArray());
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            return new GraphServiceClient(authenticationProvider);
        }
    }
}
