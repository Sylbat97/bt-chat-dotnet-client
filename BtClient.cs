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
        private const char COMMAND_PREFIX = '!';

        public string Name { get; set; }
        private SocketIO socket;

        private bool running = true;

        public BtClient(string name)
        {
            this.Name = name;
        }

        public async Task Start()
        {
            Console.WriteLine(Environment.OSVersion);
            Console.OutputEncoding = Encoding.UTF8;
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));

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

            socket.On("users", response =>
            {
                response.GetValue<List<string>>().ForEach(Console.WriteLine);
            });

            socket.On("errors", response =>
            {
                Console.Error.WriteLine(response.GetValue<string>());
            });

            await socket.ConnectAsync();

            WaitForInput();
        }

        private void WaitForInput()
        {
            while (running)
            {
                string input = GetMessage();
                if (input.StartsWith(COMMAND_PREFIX) && input.Length > 1)
                {
                    SendCommand(input.Substring(1));
                }
                else
                {
                    SendMessage(input);
                }
            }
        }

        private void SendMessage(string message)
        {
            SendEvent("message", message);
        }

        private void SendCommand(string command) 
        {
            switch (command)
            {
                case "exit":
                    running = false;
                    break;
                case "help":
                    DisplayHelp();
                    break;
                default:
                    SendEvent("command", command);
                    break;
            }
        }

        private async void SendEvent(string eventName, string content) 
        {
            await socket.EmitAsync(eventName, content);
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
            Console.WriteLine("Connected to BT Server");
            Console.WriteLine($"Commands prefix: '{COMMAND_PREFIX}'. Type {COMMAND_PREFIX}help to see the list of available commands.");
            await socket.EmitAsync("name", this.Name);
        }

        private void DisplayHelp() 
        {
            Console.WriteLine("Supported commands");
            DisplayCommandHelp("exit", "Terminates the session and exits the client");
            DisplayCommandHelp("help", "Displays the available commands");
            DisplayCommandHelp("users", "Lists the users that are logged in");
        }

        private void DisplayCommandHelp(string command, string description)
        {
            Console.WriteLine($"{COMMAND_PREFIX}{command}\t{description}");
        }
    }
}
