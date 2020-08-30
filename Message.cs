using System;

namespace bt_chat_client
{
    public class Message
    {
        public String Author { get; set; }
        public String Content { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return String.Format("{0} [{1}] {2}", Time.ToString("dd/MM/yyyy HH:mm:ss"), Author, Content);
        }
    }
}
