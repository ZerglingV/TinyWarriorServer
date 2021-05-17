using System.Net.Sockets;
using System.Threading;

namespace TinyWarriorInfo
{
        public class ClientInfo
        {
                public string PlayerName { get; set; } = "Player";
                public Socket Socket { get; set; }
                public Thread ReceiveThread { get; set; }
        }
}
