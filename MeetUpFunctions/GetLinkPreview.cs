using System;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Functions
{
    public class GetLinkPreview
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;

        public GetLinkPreview(ILogger<GetLinkPreview> logger, ServerSettingsRepository serverSettingsRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
        }


        [FunctionName("GetLinkPreview")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function GetLinkPreview processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            LinkPreview linkPreview = JsonConvert.DeserializeObject<LinkPreview>(requestBody);

            try
            {
                var web = new HtmlAgilityPack.HtmlWeb();
                var doc = await web.LoadFromWebAsync(linkPreview.Url.ToString());

                linkPreview.Title = doc.DocumentNode.SelectSingleNode("//head/title")?.InnerText;
                linkPreview.Description = doc.DocumentNode.SelectSingleNode("//meta[@name='description']")?.GetAttributeValue("content", string.Empty);

                if (linkPreview.ImageUrl == null)
                {
                    var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']") ??
                                    doc.DocumentNode.SelectSingleNode("//meta[@name='twitter:image']");
                    linkPreview.ImageUrl = new Uri(imageNode?.GetAttributeValue("content", string.Empty));
                }

                linkPreview.Success = true;
            }
            catch (Exception ex)
            {
                linkPreview.Success = false;
                linkPreview.Message = ex.Message;
                _logger.LogError(ex, $"GetLinkPreview() failed. Message {ex.Message}");
            }

            return new OkObjectResult(linkPreview);
        }

    }

}
