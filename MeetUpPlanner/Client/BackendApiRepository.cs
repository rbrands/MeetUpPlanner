using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using MeetUpPlanner.Shared;
using MeetUpPlanner.Client.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Core;
using System.Text;

namespace MeetUpPlanner.Client
{
    public class BackendApiRepository
    {
        private HttpClient _http;
        private const string HEADER_TENANT = "x-meetup-tenant";
        private const string HEADER_KEYWORD = "x-meetup-keyword";
        private AppState _appState;
        public BackendApiRepository(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }
        private void PrepareHttpClient()
        {
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            if (null != _appState.Tenant.TenantKey)
            {
               _http.DefaultRequestHeaders.Add(HEADER_TENANT, _appState.Tenant.TenantKey);
            }
            if (!_http.DefaultRequestHeaders.Contains(HEADER_KEYWORD))
            {
                _http.DefaultRequestHeaders.Add(HEADER_KEYWORD, _appState.KeyWord);
            }
        }


        public async Task<String> GetServerVersion()
        {
            this.PrepareHttpClient();
            String response = await _http.GetStringAsync($"Util/version");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }
        public async Task<String> GetFunctionVersion()
        {
            this.PrepareHttpClient();
            String response = await _http.GetStringAsync($"Util/functionsVersion");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }

        public async Task<StravaSegmentChallenge> GetChallengeByTitle(string challengeTitle)
        {
            this.PrepareHttpClient();

            //StravaSegmentChallenge response = await _http.GetFromJsonAsync<StravaSegmentChallenge>($"api/info/getchallengebytitle?challengeTitle={challengeTitle}");
            StravaSegmentChallenge challenge = null;
            var response = await _http.GetAsync($"api/info/getchallengebytitle?challengeTitle={challengeTitle}");
            if (response.IsSuccessStatusCode)
            {
                if ((response.Content.Headers.ContentLength ?? 0) > 0)
                {
                    challenge = await response.Content.ReadFromJsonAsync<StravaSegmentChallenge>();
                }
            }
            else
            {
                throw new Exception($"Http Fehlercode - {response.StatusCode.ToString()}");
            }

            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return challenge;
        }
        public async Task<ContentWithChaptersItem> GetContentWithChapters(string contentKey)
        {
            this.PrepareHttpClient();

            ContentWithChaptersItem content = null;
            var response = await _http.GetAsync($"api/info/getcontentwithchapters/{contentKey}");
            if (response.IsSuccessStatusCode)
            {
                if ((response.Content.Headers.ContentLength ?? 0) > 0)
                {
                    content = await response.Content.ReadFromJsonAsync<ContentWithChaptersItem>();
                }
            }
            else
            {
                throw new Exception($"Http Fehlercode - {response.StatusCode.ToString()}");
            }

            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return content;
        }
        public async Task WriteContentWithChapters(ContentWithChaptersItem content)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<ContentWithChaptersItem>($"api/info/writecontentwithchaptersitem", content);
            response.EnsureSuccessStatusCode();
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return;

        }

        public async Task<WinterpokalTeam> GetWinterpokalTeam(string teamId)
        {
            this.PrepareHttpClient();

            WinterpokalTeam team = null;
            var response = await _http.GetAsync($"api/winterpokal/getteam/{teamId}");
            if (response.IsSuccessStatusCode)
            {
                if ((response.Content.Headers.ContentLength ?? 0) > 0)
                {
                    team = await response.Content.ReadFromJsonAsync<WinterpokalTeam>();
                }
            }
            else
            {
                throw new Exception($"Http Fehlercode - {response.StatusCode.ToString()}");
            }

            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return team;
        }

        public async Task<BlobAccessSignature> GetBlobAccessSignatureForPNGImageUpload()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<BlobAccessSignature>($"/Util/GetBlobAccessSignatureForPNGImageUpload");
        }

        public async Task<string> GetWebcalToken()
        {
            this.PrepareHttpClient();
            return await _http.GetStringAsync($"api/webcal/getwebcaltoken");
        }

        public async Task<string> UploadImage(IBrowserFile picture, string title, string label = null)
        {
            BlobAccessSignature sas = await GetBlobAccessSignatureForPNGImageUpload();
            BlobClientOptions clientOptions = new BlobClientOptions();
            clientOptions.Retry.MaxRetries = 0;
            BlobClient blobClient = new BlobClient(sas.Sas, clientOptions);

            using var stream = picture.OpenReadStream(maxAllowedSize: 2048000);
            BlobUploadOptions options = new BlobUploadOptions();
            options.HttpHeaders = new BlobHttpHeaders() { ContentType = picture.ContentType };
            options.Metadata = new Dictionary<string, string>
            {
                { "Label", GetUrlFriendlyTitle(label) ?? String.Empty },
                { "Title", GetUrlFriendlyTitle(title) ?? String.Empty },
                { "Filename", GetUrlFriendlyTitle(picture.Name) ?? String.Empty }
            };
            
            await blobClient.UploadAsync(stream, options);
            return sas.PublicLink; 
        }
        public static string GetUrlFriendlyTitle(string title)
        {
            string urlFriendlyTitle = null;
            if (!String.IsNullOrEmpty(title))
            {
                string titleLowerCase = title.ToLowerInvariant();
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

    }
}
