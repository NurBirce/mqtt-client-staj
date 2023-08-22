using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MqttManagement.Forms;
using MqttManagement.Models;
using System.Threading;

namespace MqttManagement.MQTT
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
                Console.WriteLine("Received application message.");
                Console.WriteLine("topic: " + e.ApplicationMessage.Topic);
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                var strArr = e.ApplicationMessage.Topic.Split('/');
                var id = strArr[strArr.Length - 1];

                var strArrPayload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                frm.updateDevices(id,strArrPayload);

                //frm.updateDeviceValue(id, Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

                return Task.CompletedTask;
            };
        }

        public void subscribe()
        {
                foreach (var d in frm.systemState.analogDeviceList)
                {
                    var mqttSubscribeOptions = new MqttTopicFilterBuilder()
                    .WithTopic("karatal2023fatmaproje/" + d.Topic)
                    .WithAtLeastOnceQoS()
                    .Build();

                    mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                    Console.WriteLine(" MQTT application message is subscribed.");
                    
                }
                foreach (var d in frm.systemState.digitalDeviceList)
                {
                    var mqttSubscribeOptions = new MqttTopicFilterBuilder()
                    .WithTopic("karatal2023fatmaproje/" + d.Topic)
                    .WithAtLeastOnceQoS()
                    .Build();

                    mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                    Console.WriteLine("MQTT application message is subscribed.");
                    
                }
        }

        public async Task initiliaze()
        {
            await connect_client();
            subscribe();

            Handle_Received_Application_Message();
        }

        public async Task Publish_Application_Message(string msg, string topic)
        {
            var mqttFactory = new MqttFactory();

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("karatal2023fatmaproje/" + topic)
                .WithPayload(msg)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine("MQTT application message is published.");

        }

      
    }
}
