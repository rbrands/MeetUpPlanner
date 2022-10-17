using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Shared
{
    public class StravaSegmentChallenge : CosmosDBEntity
    {
        public class Participant
        {
            [JsonPropertyName("athleteId")]
            public ulong AthleteId { get; set; }
            [JsonPropertyName("athleteSex")]
            public string AthleteSex { get; set; }
            [JsonPropertyName("athleteName")]
            public string AthleteName { get; set; }
            [JsonPropertyName("profileImage")]
            public string ProfileImage { get; set; }
            [JsonPropertyName("stravaAuthorizationIsPending")]
            public bool StravaAuthorizationIsPending { get; set; }

            [JsonPropertyName("rank")]
            public int Rank { get; set; }
            [JsonPropertyName("totalPoints")]
            public double TotalPoints { get; set; }
            public int SegmentCounter { get; set; }
            public string GetAhtleteLink()
            {
                string link = $"https://www.strava.com/athletes/{AthleteId}";
                return link;
            }

        }
        public class Segment
        {
            [JsonPropertyName("segmentId")]
            public ulong SegmentId { get; set; }
            [JsonPropertyName("segmentName")]
            public string SegmentName { get; set; }
            [JsonPropertyName("distance")]
            public double Distance { get; set; }
            [JsonPropertyName("averageGrade")]
            public double AverageGrade { get; set; }
            [JsonPropertyName("maximumGrade")]
            public double MaximumGrade { get; set; }
            [JsonPropertyName("elevation")]
            public double Elevation { get; set; }
            [JsonPropertyName("climbCategory")]
            public long ClimbCategory { get; set; }
            [JsonPropertyName("city")]
            public string City { get; set; }
            // Optional link to be used as header
            [JsonPropertyName("imageLink")]
            public string ImageLink { get; set; }
            // Optional description for segment
            [JsonPropertyName("description")]
            public string Description { get; set; }
            // Optional link to a recommendated route that includes the segment
            [JsonPropertyName("routeRecommendation")]
            public string RouteRecommendation { get; set; }
            // Comma-separated list of tags to filter segments for presentation. E.g. "scuderia,dsd"
            [JsonPropertyName("tags")]
            public string Tags { get; set; }
            // Comma-separated list of labels to be used for display when presenting the results. E.g. "Sprint,Cat1" 
            [JsonPropertyName("labels")]
            public string Labels { get; set; }
            [JsonPropertyName("scope")]
            public string Scope { get; set; }
            public string GetSegmentLink()
            {
                string segmentLink = $"https://www.strava.com/segments/{SegmentId}";
                return segmentLink;
            }
            public string GetClimbCategoryLabel()
            {
                string[] climbCategories = { "-", "4", "3", "2", "1", "HC" };
                return climbCategories[ClimbCategory];
            }
            public string GetDistanceAsText()
            {
                string distanceFormat = String.Empty;
                if (Distance > 1000)
                {
                    double distanceKm = Distance / 1000.0;
                    distanceFormat = String.Format("{0:N2}km", distanceKm);
                }
                else
                {
                    distanceFormat = String.Format("{0:N0}m", Distance);
                }
                return distanceFormat;
            }
            public string[] GetTags()
            {
                string[] tagsAsArray = { };
                char[] charSeparators = new char[] { ',', ';' };
                if (!String.IsNullOrEmpty(Tags))
                {
                    tagsAsArray = this.Tags.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                }
                return tagsAsArray;
            }
            public string[] GetLabels()
            {
                string[] labelsAsArray = { };
                if (!String.IsNullOrEmpty(Labels))
                {
                    labelsAsArray = this.Labels.Split(',');
                }
                return labelsAsArray;
            }
        }
        [JsonPropertyName("challengeTitle"), Required(ErrorMessage = "Bitte einen Titel für die Challenge angeben."), MaxLength(252, ErrorMessage = "Titel zu lang")]
        public string ChallengeTitle { get; set; }
        [JsonPropertyName("imageLink")]
        public string ImageLink { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("urlTitle")]
        [RegularExpression("[a-z0-9-_]*", ErrorMessage = "Bitte nur Kleinbuchstaben und Zahlen für den Titel-Link eingeben.")]
        [MaxLength(160, ErrorMessage = "Url-Titel zu lang")]
        public string UrlTitle { get; set; }
        [JsonPropertyName("startDateUTC")]
        public DateTime StartDateUTC { get; set; } = DateTime.Now.Date.AddDays(7.0).ToUniversalTime();
        [JsonPropertyName("endDateUTC")]
        public DateTime EndDateUTC { get; set; } = DateTime.Now.Date.AddDays(37.0).ToUniversalTime();
        [JsonPropertyName("isPublicVisible")]
        public bool IsPublicVisible { get; set; } = true;
        [JsonPropertyName("invitationRequired")]
        public bool InvitationRequired { get; set; } = false;
        [JsonPropertyName("registrationIsOpen")]
        public bool RegistrationIsOpen { get; set; } = true;
        [JsonPropertyName("invitationLink")]
        [RegularExpression("[a-z0-9-_]*", ErrorMessage = "Bitte nur Kleinbuchstaben und Zahlen für den Einladunges-Link eingeben.")]
        [MaxLength(160, ErrorMessage = "Einladungslink zu lang")]
        public string InvitationLink { get; set; }
        [JsonPropertyName("segments")]
        public IDictionary<ulong, Segment> Segments { get; set; } = new Dictionary<ulong, Segment>();
        [JsonPropertyName("participants")]
        public IDictionary<ulong, Participant> Participants { get; set; } = new Dictionary<ulong, Participant>();
        [JsonPropertyName("participantsFemale")]
        public IDictionary<ulong, Participant> ParticipantsFemale { get; set; } = new Dictionary<ulong, Participant>();
        [JsonPropertyName("pointLookup")]
        public double[] PointLookup { get; set; } = new double[]
        {
            100.0, 90.0, 81.5, 74.0, 67.0, 60.5, 55.0, 49.5, 45.0, 40.5, 37.0, 33.5, 30.0, 27.5, 24.5, 22.5, 20.0, 18.5,
            16.5, 15.0, 13.5, 12.0, 11.0, 10.0, 9.0, 8.0, 7.5, 6.5, 6.0, 5.5
        };
        public double MapRankingToPoints(int rank)
        {
            if (rank < 1 || rank > PointLookup.Length)
            {
                return 0.0;
            }
            else
            {
                return PointLookup[rank - 1];
            }
        }

        public string GetUrlFriendlyTitle()
        {
            string urlFriendlyTitle = null;
            if (!String.IsNullOrEmpty(ChallengeTitle))
            {
                string titleLowerCase = ChallengeTitle.ToLowerInvariant();
                StringBuilder sb = new StringBuilder();
                int charCounter = 0;
                foreach (char c in titleLowerCase)
                {
                    if (++charCounter > 160)
                    {
                        // url not longer than 160 chars
                        break;
                    }
                    switch (c)
                    {
                        case '\u00F6':
                        case '\u00D6':
                            sb.Append("oe");
                            break;
                        case '\u00FC':
                        case '\u00DC':
                            sb.Append("ue");
                            break;
                        case '\u00E4':
                        case '\u00C4':
                            sb.Append("ae");
                            break;
                        case '\u00DF':
                            sb.Append("ss");
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'g':
                        case 'h':
                        case 'i':
                        case 'j':
                        case 'k':
                        case 'l':
                        case 'm':
                        case 'n':
                        case 'o':
                        case 'p':
                        case 'q':
                        case 'r':
                        case 's':
                        case 't':
                        case 'u':
                        case 'v':
                        case 'w':
                        case 'x':
                        case 'y':
                        case 'z':
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            sb.Append(c);
                            break;
                        default:
                            sb.Append('-');
                            break;
                    }
                }
                urlFriendlyTitle = sb.ToString().Trim('-');
            }
            return urlFriendlyTitle;
        }
        public string GetInvitationLink() => $"/registerfor/{GetUrl()}/withtoken/{InvitationLink}";
        public string GetTitleWithDate() => $"{ChallengeTitle} {StartDateUTC.ToLocalTime().ToString("dd.MM.")} - {EndDateUTC.ToLocalTime().ToString("dd.MM.yyyy")}";
        public string GetUrl() => $"{UrlTitle ?? Id}";
    }
}
