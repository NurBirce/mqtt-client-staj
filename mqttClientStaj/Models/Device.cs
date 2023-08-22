using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClientStaj.Models
{
    class Device<T>
    {
        public Device()
        {
        }
        internal Device(string name)
        {
            this.Name = name;
        }

        public Device(string topic, string name)
        {
            this.Name = name;
            Topic = topic;
        }

        public Device(string topic, string name, T value)
        {
            this.Name = name;
            Topic = topic;
            Value = value;
        }

        public string Name { get; set; }

        public string Topic { get; set; }

        public T Value { get; set; }
    }
}
