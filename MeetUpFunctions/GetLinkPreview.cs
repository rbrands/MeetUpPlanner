using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using MSiccDev.Libs.LinkTools.LinkPreview;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Functions
{
    public class GetLinkPreview
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private LinkPreviewService _linkPreviewRepository;

        public GetLinkPreview(ILogger<GetLinkPreview> logger, ServerSettingsRepository serverSettingsRepository, LinkPreviewService linkPreviewRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _linkPreviewRepository = linkPreviewRepository;
        }


        [FunctionName("GetLinkPreview")]
        [OpenApiOperation(Summary = "Gets the LinkPreview for given URL",
                          Description = "Retrieves the link preview infos.")]
        [OpenApiRequestBody("application/json", typeof(LinkPreview), Description = "LinkPreview with url to be evaluated.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(LinkPreview))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function GetLinkPreview processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            LinkPreview linkPreview = JsonConvert.DeserializeObject<LinkPreview>(requestBody);

            LinkPreviewRequest previewRequest = new LinkPreviewRequest(linkPreview.Url);

            LinkPreviewRequest previewResponse = await _linkPreviewRepository.GetLinkDataAsync(previewRequest, false, true, false, true);
            linkPreview.Title = System.Web.HttpUtility.HtmlDecode(previewResponse.Result.Title);
            linkPreview.Description = System.Web.HttpUtility.HtmlDecode(previewResponse.Result.Description);
            linkPreview.ImageUrl = previewResponse.Result.ImageUrl;
            linkPreview.Url = previewResponse.Result.Url;
            linkPreview.CanoncialUrl = previewResponse.Result.CanoncialUrl;
            linkPreview.Success = previewResponse.IsSuccess;
            if (!linkPreview.Success && null != previewResponse.Error)
            { 
                linkPreview.Message = previewResponse.Error.Message;
            }

            return new OkObjectResult(linkPreview);
        }
    }
}
