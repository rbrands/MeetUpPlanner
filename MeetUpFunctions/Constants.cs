﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Functions
{
    public static class Constants
    {
        public const string HEADER_KEYWORD = "x-meetup-keyword";
        
        public const string KEY_SERVER_SETTINGS = "serversettings";
        public const string KEY_CLIENT_SETTINGS = "clientsettings";
        
        public const string DEFAULT_KEYWORD_USER = "UserKeyword";
        public const string DEFAULT_KEYWORD_ADMIN = "AdminKeyword";
        public const int DEFAULT_AUTO_DELETE_DAYS = 28;
        public const string DEFAULT_TITLE = "MeetUp-Planner";
        public const string DEFAULT_LINK = "https://robert-brands.com";
        public const string DEFAULT_LINK_TITLE = "https://robert-brands.com";

        public const string VERSION = "2020-08-02";
        public const int ADMINOVERBOOKFACTOR = 2;

        public const int LOG_TTL = 30 * 24 * 3600; // 30 days TTL for Log items
    }
}
