using SignalRChat_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class SignalRContext
    {
        public List<UserGroup> Users { get; set; }

        public List<Connection> Connections { get; set; }

        public List<ChatRoom> Rooms { get; set; }

        public SignalRContext()
        {
            Users = new List<UserGroup>();
            Connections = new List<Connection>();
            Rooms = new List<ChatRoom>();
        }
    }
}