using System.Threading.Tasks;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace bt_chat_client
{
    class BtClient
    {
        public string Name { get; set; }
        private SocketIO socket;
        public BtClient(string name)
        {
            this.Name = name;
        }

        public async Task Start()
        {
            Console.WriteLine(Environment.OSVersion);
            Console.OutputEncoding = Encoding.UTF8;
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            var uri = new Uri("https://vps.sylbat.be:8000");

            socket = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "io" }                },
                ConnectionTimeout = TimeSpan.FromSeconds(10),
                EnabledSslProtocols = 0
            });

            socket.OnConnected += Socket_OnConnected;

            socket.On("message", response =>
            {
                var message = response.GetValue<Message>();
                Console.WriteLine(message);
            });

            socket.On("history", response =>
            {
                var messages = response.GetValue<List<Message>>();
                messages.ForEach(message =>
                {
                    Console.WriteLine(message);
                });
            });

            await socket.ConnectAsync();

            WaitForInput();
        }

        private void WaitForInput()
        {
            string input = GetMessage();
            while (input != "exit")
            {
                SendMessage(input);
                input = GetMessage();
            }
        }

        private async void SendMessage(string message)
        {
            await socket.EmitAsync("message", message);
        }

        private string GetMessage()
        {
            string message = "";
            while (string.IsNullOrWhiteSpace(message))
            {
                message = Console.ReadLine();
            }
            return message;
        }

        private async void Socket_OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("connected");
            await socket.EmitAsync("name", this.Name);
        }

    }
}
