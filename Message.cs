using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;

namespace bt_chat_client
{
    public class Message
    {
        public String Author {get; set;}
        public String Content {get; set;}
    }
}
