using SignalRChat_Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChat_Model
{
    /// <summary>
    /// 聊天记录实体类
    /// </summary>
    public class ChatHistory
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 发送方id
        /// </summary>
        public string frmId { get; set; }
        /// <summary>
        /// 接收方id
        /// </summary>
        public string toId { get; set; }
        /// <summary>
        /// 房间id
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 聊天类型，0公共聊天，1私聊，2群聊
        /// </summary>
        public ChatType ChatType { get; set; }
        /// <summary>
        /// 聊天内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 消息时间
        /// </summary>
        public DateTime msgTime { get; set; }
    }
}