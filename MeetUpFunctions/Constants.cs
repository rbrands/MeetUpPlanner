using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Functions
{
    public static class Constants
    {
        public const string HEADER_KEYWORD = "x-meetup-keyword";
        public const string HEADER_TENANT = "x-meetup-tenant";
        public const string HEADER_TENANT_URL = "x-meetup-tenant-url";

        public const string KEY_SERVER_SETTINGS = "serversettings";
        public const string KEY_CLIENT_SETTINGS = "clientsettings";
        
        public const string DEFAULT_KEYWORD_USER = "UserKeyword";
        public const string DEFAULT_KEYWORD_ADMIN = "AdminKeyword";
        public const int DEFAULT_AUTO_DELETE_DAYS = 28;
        public const string DEFAULT_TITLE = "MeetUp-Planner";
        public const string DEFAULT_LINK = "https://robert-brands.com";
        public const string DEFAULT_LINK_TITLE = "https://robert-brands.com";
        public const string DEFAULT_DISCLAIMER = "Disclaimer";
        public const string DEFAULT_GUEST_DISCLAIMER = "Guest Disclaimer";

        public const string VERSION = "2021-02-06";
        public const int ADMINOVERBOOKFACTOR = 2;

        public const int LOG_TTL = 30 * 24 * 3600; // 30 days TTL for Log items

        public const int SUBSCRIPTION_TTL = 7 * 24 * 3600; // 7 days TTL for notification subscriptions

        public const string INVITE_GUEST_KEY_CONFIG = "InviteGuestKey";

        public const string NOTIFICATION_KEY_CONFIG = "NotificationKey";
    }
}
