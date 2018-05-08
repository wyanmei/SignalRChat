using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    /// <summary>
    /// Author:wym
    /// </summary>
    public class ChatController : Controller
    {
        /// <summary>
        /// 聊天室主页
        /// </summary>
        /// <param name="nick">名称</param>
        /// <param name="pwd">密码</param>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        // GET: Chat
        public ActionResult Index(string nick,string pwd,string userid)
        {
            Random ran = new Random();
            UserViewModel model = new UserViewModel();
            model.Nick = nick;
            model.UserId = userid;
            model.Password = pwd;
            return View(model);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            ViewData["userid"] = Guid.NewGuid().ToString().ToUpper();
            return View();
        }
    }
}