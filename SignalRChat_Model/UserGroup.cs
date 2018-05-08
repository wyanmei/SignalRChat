using System.Collections.Generic;

namespace SignalRChat_Model
{
    /// <summary>
    /// 用户组信息
    /// </summary>
   public class UserGroup
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserConnectionId { get; set; }

        /// <summary>
        /// 用户的连接集合
        /// </summary>
        public List<Connection> Connections { get; set; }

        /// <summary>
        /// 用户房间集合，一个用户可以加入多个房间
        /// </summary>
        public List<ChatRoom> Rooms { get; set; }

        public UserGroup()
        {
            Connections = new List<Connection>();
            Rooms = new List<ChatRoom>();
        }
    }
}
