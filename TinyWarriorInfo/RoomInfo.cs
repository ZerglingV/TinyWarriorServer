using System;
using System.Collections;

namespace TinyWarriorInfo
{
        [Serializable]
        public class RoomInfo
        {
                public string RoomName { get; set; }
                public bool IsStart { get; set; } // is game started
                public string OwnerAddress { get; set; } // room creator's ip address
                public Hashtable GuestsAddressAndName { get; set; } // room guess' name and ip address
                public int MaxPlayerNumber { get; set; }
        }
}
