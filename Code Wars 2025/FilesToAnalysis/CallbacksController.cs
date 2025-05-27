using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartHome_BE.Core.Dtos;
using SmartHome_BE.Core.Services;
using SmartHome_BE.Database;

namespace SmartHome_BE.Controllers
{
    public class CallbackResultDto
    {
        public int code { get; set; }
    }

    [ApiController]
    [EnableCors("AllowCors"), Route("[controller]")]
    public class CallbacksController : ControllerBase
    {
        private readonly ISmartHomeDbContext _smartHomeDbContext;
        private readonly IAqaraDevicesService _aqaraDevicesService;

        public CallbacksController(ISmartHomeDbContext smartHomeDbContext, IAqaraDevicesService aqaraDevicesService)
        {
            _smartHomeDbContext = smartHomeDbContext;
            _aqaraDevicesService = aqaraDevicesService;
        }

        [HttpPost]
        public async Task<IActionResult> SendActivity(object activityResponseDto)
        {
            _smartHomeDbContext.Logs.Add(new Common.Entities.LogEntity
            {
                Data = JsonConvert.SerializeObject(activityResponseDto.ToString()),
            });
            await _smartHomeDbContext.SaveChangesAsync();
            //AqaraResponseSubscribeDto
            return Ok(new CallbackResultDto());
        }

        [HttpGet]
        public async Task<IActionResult> GetSendActivity()
        {
            var result = await _aqaraDevicesService.SubscribeMessagePush();

            int a = 0;
            var result2 = await _aqaraDevicesService.SubscribeMessagePushKK("kkoa", a);
            //AqaraResponseSubscribeDto
            return Ok(result);
        }
    }
}