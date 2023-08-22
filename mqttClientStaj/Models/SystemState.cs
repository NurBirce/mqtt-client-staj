using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClientStaj.Models
{
    internal class SystemState
    {
        public List<Device<float>> analogDeviceList { get; set; }
        public List<Device<bool>> digitalDeviceList { get; set; }

        public SystemState()
        {
            analogDeviceList = new List<Device<float>>();
            digitalDeviceList = new List<Device<bool>>();
        }
    }
}
