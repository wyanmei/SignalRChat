using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat_Model;
using SignalRChat_Common;
using System.Threading.Tasks;
using System.Text;
using SignalRChat_Model.Enum;

namespace SignalRChat.Hubs
{

    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        #region 全局对象
        protected static List<UserInfo> userInfoList = new List<UserInfo>();
        protected static SignalRContext DbContext = new SignalRContext();
        protected static List<ChatHistory> chatHistoryList = new List<ChatHistory>();
        #endregion

        #region 连接
      
        /// <summary>
        /// 客户端重连接时
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            AddUserGroup();//添加用户组
            UpdateAllRoomList();//更新房间列表

            return base.OnConnected();
        }
        /// <summary>
        /// 断线
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region 公共聊天

        /// <summary>
        /// 公共聊天
        /// </summary>
        /// <param name="message"></param>
        /// <param name="name"></param>
        public void PublicSendMsg(string message, string userId)
        {
            var user = userInfoList.FirstOrDefault(x => x.UserID == userId);
            Clients.All.sendPublicMessage(user.UserID, user.UserName, message);
            AddChatHistory(ChatType.PubChat,user.UserName, message, user.UserID,"");//添加历史记录
        }
        #endregion

        #region 私聊
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="sendName">发送名称</param>
        /// <param name="userId">用户id</param>
        /// <param name="message">消息</param>
        public void SendPrivateMsg(string sendName, string userId, string message)
        {
            var toUser = userInfoList.FirstOrDefault(x => x.UserID == userId);//接收用户信息
            var fromUser = userInfoList.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);//发送用户信息
            if (toUser != null && fromUser != null)
            {
                Clients.Caller.showMsgToPages(fromUser.UserID, sendName, message);
                if (fromUser.UserID != userId)//判断是否是自己给自己发消息
                {
                    Clients.Client(toUser.ConnectionId).remindMsg(fromUser.UserID, fromUser.UserName,message);
                }
                AddChatHistory(ChatType.PriChat, sendName, message, fromUser.UserID, userId, "");
            }
        }
        #endregion

        #region 群聊
        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="roomName"></param>
        public void CreateRoom(string roomName)
        {
            var room = DbContext.Rooms.Find(x => x.RoomName == roomName);
            if (room == null)
            {
                var rom = new ChatRoom
                {
                    RoomName = roomName,
                    RoomId = Guid.NewGuid().ToString().ToUpper()
                };
                DbContext.Rooms.Add(rom);//加入房间列表
                UpdateAllRoomList();//更新房间列表
                Clients.Client(Context.ConnectionId).showGroupMsg("success");
            }
            else
            {
                Clients.Client(Context.ConnectionId).showGroupMsg("error");
            }
        }

        /// <summary>
        ///加入聊天室
        /// </summary>
        public void JoinRoom(string roomId,string current_Id)
        {
            // 查询聊天室，
            var room = DbContext.Rooms.Find(x => x.RoomId == roomId.Trim());
            var u = userInfoList.Find(x => x.UserID == current_Id);
            if (room != null)
            {
                //检测该用户是否存在在该房间
                var isExistUser = room.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
                if (isExistUser == null)
                {
                    var user = DbContext.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
                    user.Rooms.Add(room);//用户信息中加入房间信息
                    room.Users.Add(user);//房间信息中加入用户信息
                    Groups.Add(Context.ConnectionId, room.RoomName);//添加到组中
                    Clients.Group(room.RoomName, new string[0]).showSysGroupMsg(u.UserName);
                }
            }
            else
            {
                Clients.Client(Context.ConnectionId).showMessage("该群组不存在");
            }
        }

        /// <summary>
        /// 给指定房间内的所有用户发消息
        /// </summary>
        /// <param name="room">房间名</param>
        /// <param name="message">消息</param>
        public void SendMessageByRoom(string roomId, string current_Id, string message)
        {
            var room = DbContext.Rooms.FirstOrDefault(x=>x.RoomId==roomId);
            var user = userInfoList.Find(x => x.UserID == current_Id);
            if (room != null && user != null)
            {
                Clients.Group(room.RoomName, new string[0]).showGroupByRoomMsg(user.UserName,room.RoomId, message);
                AddChatHistory(ChatType.GroChat, user.UserName, message, user.UserID, "", room.RoomId);
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        public void RemoveRoom(string roomId)
        {
            var room = DbContext.Rooms.Find(x => x.RoomId == roomId);
            if (room != null)
            {
                var user = DbContext.Users.Find(x => x.UserConnectionId == Context.ConnectionId);
                room.Users.Remove(user);//从房间里移除该用户
                if (room.Users.Count <= 0)
                {
                    DbContext.Rooms.Remove(room);//如果房间里没人了，删除该房间
                }
                Groups.Remove(Context.ConnectionId, room.RoomName);
                UpdateAllRoomList();//更新房间列表
                Clients.Client(Context.ConnectionId).removeRoom();
            }
            else
            {
                Clients.Client(Context.ConnectionId).showMessage("该房间不存在");
            }
        }

        #endregion

        #region  method
        /// <summary>
        /// 获取所有在线用户集合
        /// </summary>
        public void GetAllOnlineUser()
        {
           var uList = JsonHelper.ToJsonString(userInfoList);
            Clients.All.showUserList(uList);
        }
        /// <summary>
        /// 更新群组列表方法
        /// </summary>
        public void UpdateAllRoomList()
        {
            var room = DbContext.Rooms.Select(x => new { x.RoomId, x.RoomName }).ToList();
            var data = JsonHelper.ToJsonString(room);
            Clients.All.showAllRoomList(data);
        }
        /// <summary>
        /// 添加在线人员
        /// </summary>
        public void AddOnlineUser(string nickName, string password,string userId)
        {
            var user = userInfoList.FirstOrDefault(x => x.UserID == userId);
            if (user==null)
            {
                //添加在线人员
                userInfoList.Add(new UserInfo
                {
                    ConnectionId = Context.ConnectionId,
                    UserID = userId,//随机用户id
                    UserName = nickName,
                    Password = password,
                    LoginTime = DateTime.Now.ToString()
                });
                Clients.All.showJoinMessage(nickName);
            }
            else
            {
                user.ConnectionId = Context.ConnectionId;
            }
        }

        /// <summary>
        /// 添加用户组
        /// </summary>
        public void AddUserGroup()
        {
            // 查询用户
            var user = DbContext.Users.FirstOrDefault(u => u.UserConnectionId == Context.ConnectionId);
            if (user == null)
            {
                user = new UserGroup
                {
                    UserConnectionId = Context.ConnectionId,
                };
                DbContext.Users.Add(user);
            }
        }

        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <param name="chatType">消息类型0公共聊天，1好友，2群</param>
        /// <param name="toId">接收者id</param>
        /// <param name="frmId">发送方id</param>
        /// <param name="roomId">房间id</param>
        public void GetChatHistory(int chatType =(int)ChatType.PubChat,string toId="", string frmId="",string roomId="")
        {
            var list = chatHistoryList;
            var type = (ChatType)chatType;
            switch (type)
            {
                case ChatType.PubChat:
                    list = chatHistoryList.Where(x => x.ChatType == type).ToList();
                    break;
                case ChatType.PriChat:
                    //自己发送给对方的，和对方发给自己的数据集合
                    list = chatHistoryList.Where(x => x.ChatType == type && ((x.toId == toId && x.frmId == frmId) || (x.toId == frmId && x.frmId == toId))).ToList();
                    break;
                case ChatType.GroChat:
                    list = chatHistoryList.Where(x => x.ChatType == type && x.RoomId == roomId).ToList();
                    break;
                default:
                    list = new List<ChatHistory>();
                    break;
            }
            var data = JsonHelper.ToJsonString(list);
            var user = userInfoList.FirstOrDefault(x=>x.UserID== frmId);
            var conid = Context.ConnectionId;
            if (user != null)
            {
                conid = user.ConnectionId;
            }
            Clients.Client(conid).initChatHistoryData(data, chatType);
        }
        /// <summary>
        /// 添加历史记录数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <param name="chatType">0公共聊天，1私聊，2群聊</param>
        public void AddChatHistory(ChatType chatType = 0,string userName="", string message="", string frmId="",string toId="",string roomId="")
        {
            ChatHistory history = new ChatHistory()
            {
                Hid = Guid.NewGuid().ToString().ToUpper(),
                ChatType = chatType,
                Message = message,
                UserName = userName,
                frmId = frmId,
                toId = toId,
                RoomId = roomId
            };
            chatHistoryList.Add(history);
        }
      
        #endregion
    }
}