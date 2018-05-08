using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChat_Model
{
    public class ChatRoom
    {
        /// <summary>
        /// 房间id
        /// </summary>
        public string RoomId { get; set; }
        // 房间名称
        public string RoomName { get; set; }

        // 用户集合
        public List<UserGroup> Users { get; set; }

        public ChatRoom()
        {
            Users = new List<UserGroup>();
        }
    }
}
