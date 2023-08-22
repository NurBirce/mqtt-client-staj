using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MqttClientStaj.Forms;
using MqttClientStaj.Models;
using System.Threading;

namespace MqttClientStaj.MQTT
{
    class Mqtt
    {
        IMqttClient mqttClient;
        FormMain frm;
        

        public Mqtt(FormMain formMain)
        {
            frm = formMain;
        }

        public async Task connect_client()
        {
            var mqttFactory = new MqttFactory();

            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("test.mosquitto.org").Build();

            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            Console.WriteLine("The MQTT client is connected.");

            Console.WriteLine(response);
        }

        public async Task disconnect()
        {
            var mqttFactory = new MqttFactory();

            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
        }

        
        public async Task Handle_Received_Application_Message()
        {
            var mqttFactory = new MqttFactory();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                try
                {
                    Console.WriteLine("Received application message.");
                    Console.WriteLine("topic: " + e.ApplicationMessage.Topic);
                    Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

                    var topic = e.ApplicationMessage.Topic.Split('/');
                    var devicType = topic[2];
                    var id = topic[topic.Length - 1];

                    var strArrPayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    if (devicType == "dig")
                    {
                        frm.updateDigitalDevices(id, strArrPayload);
                    }
                    else
                    {
                        frm.updateAnalogDevices(id, strArrPayload);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception in RECIEVE: " + ex.ToString());
                }
                
                return Task.CompletedTask;
            };
        }

        public void subscribe(string topic)
        {
            var mqttSubscribeOptions = new MqttTopicFilterBuilder()
                    .WithTopic("karatal2023fatmaproje/s/" + topic)
                    .WithAtLeastOnceQoS()
                    .Build();

            mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine(" MQTT application message is subscribed.");
        }
        public void subscribeAnalog(string topic)
        {
            subscribe("ana/"+topic);
        }
        public void subscribeDigital(string topic)
        {
            subscribe("dig/" + topic);
        }

        public async Task initiliaze()
        {
            await connect_client();

            foreach (var d in frm.systemState.analogDeviceList)
            {
                subscribeAnalog(d.Topic);
            }
            foreach (var d in frm.systemState.digitalDeviceList)
            {
                subscribeDigital(d.Topic);
            }
            
            Handle_Received_Application_Message();
        }

        public async Task Publish(string msg, string topic)
        {
            var mqttFactory = new MqttFactory();

            var applicationMessage = new MqttApplicationMessageBuilder()
                 .WithTopic("karatal2023fatmaproje/c/" + topic)
                 .WithPayload(msg)
                 .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                 .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine("MQTT application message is published.");
        }
        public async Task PublishAnalog(string msg, string topic)
        {
            Publish(msg, "ana/" + topic);
        }
        public async Task PublishDigital(string msg, string topic)
        {
            Publish(msg, "dig/" + topic);
        }

    }
}
