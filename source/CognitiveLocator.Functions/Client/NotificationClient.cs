﻿using Microsoft.Azure.NotificationHubs;

namespace CognitiveLocator.Functions.Client
{
    public class NotificationClient
    {
        public static NotificationClient Instance = new NotificationClient();

        public NotificationHubClient Hub { get; set; }

        private NotificationClient()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(Settings.NotificationAccessSignature, Settings.NotificationHubName);
        }
    }
}