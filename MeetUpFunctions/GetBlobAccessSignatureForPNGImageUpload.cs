using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetUpPlanner.Functions;
using MeetUpPlanner.Shared;


namespace MeetUpFunctions
{
    public class GetBlobAccessSignatureForPNGImageUpload
    {
        private readonly ILogger _logger;
        private BlobStorageRepository _blobRepository;

        public GetBlobAccessSignatureForPNGImageUpload(ILogger<GetBlobAccessSignatureForPNGImageUpload> logger, BlobStorageRepository blobRepository)
        {
            _logger = logger;
            _blobRepository = blobRepository;
        }

        [FunctionName("GetBlobAccessSignatureForPNGImageUpload")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetBlobAccessSignatureForPNGImageUpload")] HttpRequest req)
        {
            _logger.LogInformation($"GetBlobAccessSignatureForPNGImageUpload");

            BlobAccessSignature sas = _blobRepository.GetBlobAccessSignatureForPNGImageUpload();
            _logger.LogInformation($"SAS is {sas.Sas}");

            return new OkObjectResult(sas);
        }
    }
}
