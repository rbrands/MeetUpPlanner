using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.IO;
using QRCoder;

using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UtilController : ControllerBase
    {
        private readonly MeetUpFunctions _meetUpFunctions;
        private readonly ILogger<UtilController> logger;
        const string serverVersion = "2021-02-06";
        string functionsVersion = "tbd";

        public UtilController(ILogger<UtilController> logger, MeetUpFunctions meetUpFunctions)
        {
            _meetUpFunctions = meetUpFunctions;
            this.logger = logger;
        }

        [HttpGet("version")]
        public String GetVersion()
        {
            logger.LogInformation("Server version returned: {serverVersion}", serverVersion);
            return serverVersion;
        }

        [HttpGet("functionsVersion")]
        public async Task<String> GetFunctionsVersion()
        {
            logger.LogInformation("Functions version returned: {functionsVersion}", functionsVersion);
            functionsVersion = await _meetUpFunctions.GetVersion();
            
            return functionsVersion;
        }

        [HttpGet("tenantclientsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTenantClientSettings([FromHeader(Name = "x-meetup-tenant-url")] string tenantUrl)
        {
            TenantClientSettings tenantClientSettings = await _meetUpFunctions.GetTenantClientSettings(tenantUrl);
            return Ok(tenantClientSettings);
        }

        [HttpGet("clientsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientSettings([FromHeader(Name = "x-meetup-tenant")] string tenant)
        {
            ClientSettings clientSettings = await _meetUpFunctions.GetClientSettings(tenant);
            return Ok(clientSettings);
        }

        [HttpGet("checkkeyword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckKeyword([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword)
        {
            KeywordCheck keywordCheck = await _meetUpFunctions.CheckKeyword(tenant, keyword);

            return Ok(keywordCheck);
        }

        [HttpGet("serversettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServerSettings([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string adminKeyword)
        {
            ServerSettings serverSettings = await _meetUpFunctions.GetServerSettings(tenant, adminKeyword);
            return Ok(serverSettings);
        }
        [HttpPost("writeserversettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteServerSettings([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string adminKeyword, [FromBody] ServerSettings serverSettings)
        {
            await _meetUpFunctions.WriteServerSettings(tenant, adminKeyword, serverSettings);
            return Ok();
        }
        [HttpPost("writesettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteClientSettings([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string adminKeyword, [FromBody] ClientSettings clientSettings)
        {
            await _meetUpFunctions.WriteClientSettings(tenant, adminKeyword, clientSettings);
            return Ok();
        }
        [HttpPost("writenotificationsubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteNotificationSubscription([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] NotificationSubscription subscription)
        {
            await _meetUpFunctions.WriteNotificationSubscription(tenant, keyword, subscription);
            return Ok();
        }
        [HttpGet("qrcode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public String GetQrCode([FromQuery] string link)
        {
            string imageUrl = "";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }
            return imageUrl;
        }
        [HttpGet("qrcodeimage")]
        public IActionResult GetQrCodeImage([FromQuery] string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            byte[] byteImage;

            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteImage = ms.ToArray();
                }
            }
            return File(byteImage, "image/png");
        }
    }
}
