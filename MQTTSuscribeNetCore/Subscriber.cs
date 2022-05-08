using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MQTTSuscribeNetCore
{
    class Subscriber
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            //Creación del cliente
            var client = mqttFactory.CreateMqttClient();
            //Creación de opciones de conexión
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCleanSession()
                .Build();
            client.UseConnectedHandler(async x =>
            {
                Console.WriteLine("Conexión al brocker satisfactoria");
                var topicFilter = new TopicFilterBuilder()
                        .WithTopic("PruebaIS3")    
                        .Build();
                await client.SubscribeAsync(topicFilter);
            });

            client.UseDisconnectedHandler(x =>
            {
                Console.WriteLine("Desconexión al broker satisfactoria");
            });

            client.UseApplicationMessageReceivedHandler(x =>
            {
                Console.WriteLine($"Mesanje recivido - {Encoding.UTF8.GetString(x.ApplicationMessage.Payload)}"); 
            });

            await client.ConnectAsync(options);

            Console.ReadLine();

            await client.DisconnectAsync();
        }
    }
}

