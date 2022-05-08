using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MQTTPublishSubscribeNetCore
{
    class Publisher
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
            client.UseConnectedHandler(x =>
            {
                Console.WriteLine("Conexión al brocker satisfactoria");
            });

            client.UseDisconnectedHandler(x =>
            {
                Console.WriteLine("Desconexión al broker satisfactoria");
            });

            await client.ConnectAsync(options);

            //Para cuando se haya establecido conexión con el broker
            Console.WriteLine("Por favor indica la clave para publicar el mensaje");
            Console.ReadLine();

            await PublishMessageAsync(client);

            //Cuando se haya publicado el mensaje, se desconectará el cliente
            await client.DisconnectAsync(); 
        }

        private static async Task PublishMessageAsync(IMqttClient client)
        {
            string messagePayload = "Hola!";
            //Construcción del mensaje
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("PruebaIS3")
                .WithPayload(messagePayload).
                WithAtLeastOnceQoS()
                .Build();
            //Si el cliente esta conectado se publicará el mensjae
            if (client.IsConnected)
            {
                await client.PublishAsync(message);
                Console.WriteLine($"Mensaje publicado - {messagePayload}");
            }
        }
    }
}

