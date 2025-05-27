using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Wars_2025
{
    public class AqaraResponseSubscribeDto
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public interface IAqaraDevicesService
    {
        Task AssignAccessKey();

        Task AddLightSensorMesaure();

        Task<AqaraResponseSubscribeDto> SubscribeMessagePush();

        Task PlayScene(string scene);
    }
}