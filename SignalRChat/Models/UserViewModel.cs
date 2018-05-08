using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Nick { get; set; }
        public string Password  { get; set; }
    }
}