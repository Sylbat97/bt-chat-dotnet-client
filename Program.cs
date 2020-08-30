using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;

namespace bt_chat_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("pseudo : ");
            string name = Console.ReadLine();
            var client = new BtClient(name);
            await client.Start();
        }
    }
}
