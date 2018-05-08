    // 引用自动生成的集线器代理
    var chat = $.connection.chatHub;
    $(function () {
        $('#hid_touserid').val('');
    $('#textMessage').focus();
        $('.js-tab-item').eq(0).siblings('.js-tab-item').hide();/*先隐藏除第一个以外的tab*/
        //tab切换
        $('.js-tab').on('click', function (e) {
        $(this).siblings('.js-tab').toggleClass('js-li-active');
    $(this).toggleClass('js-li-active');

            var id = $('.js-tab').filter('.js-li-active').attr('href').substr(1);
            $('#' + id).toggle();
            $('#' + id).siblings('.js-tab-item').toggle();
        });
        // 开始连接服务器
        $.connection.hub.start().done(function () {
        $('#btnSend').click(function () {
            var msg = $('#textMessage').val().trim();
            if (msg == "" || msg == undefined || msg == null) {
                alert("请输入聊天信息");
                $('#textMessage').focus();
            } else {
                // 调用服务器端集线器的Send方法
                chat.server.publicSendMsg(msg, current_userid);
                // 清空输入框信息并获取焦点
                $('#textMessage').val('').focus();
            }
        });
    //添加在线用户
    chat.server.addOnlineUser(current_uname, current_pwd, current_userid);

            //获取在线用户集合
            chat.server.getAllOnlineUser();
            //获取群组集合
            chat.server.updateAllRoomList();
        });

        //显示新用户加入消息
        chat.client.showJoinMessage = function (nickName) {
        $("#js-panel-content").append('<div class="js-time text-white text-center"><span>' + nickName + '加入了聊天</span></div>');
    }
        ///显示大厅消息
        chat.client.sendPublicMessage = function (userid, username, message) {
            var direction = userid === current_userid ? 'right' : 'left';
            var str = '<div class="col-sm-12"><div class="js-msg-' + direction + ' clearfix text-black"><div class="js-msg-img pull-' + direction + '">'
                + '<img src="/Images/avatar.jpg" width="50" height="50" /></div><div class="js-msg-text pull-' + direction + '"><div class="js-msg-nick text-' + direction + '">' + username + '</div>'
                + '<div class="js-msg-info bg-white">' + message + '</div></div></div></div>';
            $("#js-panel-content").append(str);
        };

        //显示好友列表
        chat.client.showUserList = function (data) {
            $("#friends").html('');
            var html = '<div class="js-fgname"><span class="js-lt text-gray">&gt;</span>在线好友</div> <div class="js-users">';
            var allUsers = $.parseJSON(data);
            if (allUsers != null) {
                for (var i = 0; i < allUsers.length; i++) {
                    html += '<div class="js-user clearfix" userid=' + allUsers[i].UserID + ' username=' + allUsers[i].UserName + '>' +
                        '<div class="js-li-img pull-left">' +
                        '<img src="/Images/avatar.jpg" alt="Alternate Text" width="40" height="40" />' +
                        '</div>' +
                        '<div class="js-li-nickname pull-left">' + allUsers[i].UserName + '</div>' +
                        ' <div class="js-li-signature pull-left text-gray"><span>(在线)</span>无个性不签名。。。</div>' +
                        '</div>';
                }
            }
            html += '</div>';
            $("#friends").html(html);

            //点击选择好友聊天事件
            $(".js-user").each(function () {
                $(this).click(function () {
                    var userid = $(this).attr('userid');
                    var username = $(this).attr('username');
                    var str = '<span style="font-weight:bold">  ' + username + '   </span>';
                    $("#hid_touserid").val(userid + '');
                    $(".js-friend-title").html("与" + str + "聊天");
                    //获取好友聊天记录
                    chat.server.getChatHistory(1, userid, current_userid, "");
                    $("#js-chat-friends").show();//好友窗口
                    $("#js-chat-panel").hide();//大厅窗口
                    $("#js-chat-groups").hide();//群窗口
                })
            });

            ////好友列表收缩
            $('.js-fgname').on('click', function () {
                var index = $(this).parent().index();
                $('.js-users').eq(index).slideToggle();
            });
        };

        //显示私聊消息
        chat.client.showMsgToPages = function (userId, sendname, message) {
        getPriMsgHtml(userId, sendname, message);
    }
        //接收私聊消息提醒
        chat.client.remindMsg = function (fromuserid, fromusername, message) {
            var touserid = $("#hid_touserid").val();
            if (touserid != fromuserid)//如果当前聊天的id不相同
            {
                if (confirm("您收到一条消息")) {
                    chat.server.getChatHistory(1, fromuserid, current_userid, "");
                    $("#hid_touserid").val(fromuserid + '');
                    $(".js-friend-title").html("与" + fromusername + "聊天");
                    $("#js-chat-friends").show();//好友窗口
                    $("#js-chat-panel").hide();//大厅窗口
                    $("#js-chat-groups").hide();//群窗口
                }
            } else {
                getPriMsgHtml(fromuserid, fromusername, message);
            }
        }
        //拼接聊天记录
        function getPriMsgHtml(userId, sendname, message) {
            var direction = userId === current_userid ? 'right' : 'left';
            var str = '<div class="col-sm-12"><div class="js-msg-' + direction + ' clearfix text-black"><div class="js-msg-img pull-' + direction + '">'
                + '<img src="/Images/avatar.jpg" width="50" height="50" /></div><div class="js-msg-text pull-' + direction + '"><div class="js-msg-nick text-' + direction + '">' + sendname + '</div>'
                + '<div class="js-msg-info bg-white">' + message + '</div></div></div></div>';
            $("#js-friend-content").append(str);
        }

        //显示群组列表
        chat.client.showAllRoomList = function (data) {
            if (data != null) {
                var jsondata = $.parseJSON(data);
                $("#grouplist").html('');
                var str = '<div class="js-group-name"><span class="js-lt text-gray">&gt;</span>全部群组</div> <div class="js-groups">';
                for (var i = 0; i < jsondata.length; i++) {
                    str += '<div class="js-room clearfix" roomid=' + jsondata[i].RoomId + ' roomname=' + jsondata[i].RoomName + '>' +
                        '<div class="js-li-img pull-left">' +
                        '<img src="/Images/6.jpg" alt="群头像" width="40" height="40" />' +
                        '</div>' +
                        '<div class="js-li-nickname pull-left">' + jsondata[i].RoomName + '</div>' +
                        '</div>';
                }
                str += '</div>';
                $("#grouplist").append(str);
            }

            //群组列表收缩
            $(".js-group-name").on('click', function () {
                var index = $(this).parent().index();
                $('.js-groups').eq(index).slideToggle();
            })

            //点击选择群组聊天
            $('.js-room').each(function () {
                $(this).click(function () {
                    var roomid = $(this).attr('roomid');
                    var roomname = $(this).attr('roomname');
                    if (roomid != null && roomid != "" && roomid != "undefined") {
                        $(".js-group-title").html(roomname);
                        chat.server.joinRoom(roomid, current_userid);//加入该房间
                        $("#hid_roomid").val(roomid);
                        $("#js-chat-panel").hide();
                        $("#js-chat-friends").hide();//好友窗口
                        $("#js-chat-groups").show();

                        chat.server.getChatHistory(2, "", current_userid, roomid);//获取该群组得聊天记录
                    } else {
                        alert("请选择一个群组聊天");
                    }
                })
            })
        }
        //加入群组聊天
        chat.client.showSysGroupMsg = function (username) {
            $("#js-group-content").append('<div class="js-time text-white text-center"><span>' + username + '加入了聊天</span></div>');
        }
        //显示提示消息
        chat.client.showMessage = function (message) {
            alert(message);
        }
        ///显示群组消息
        chat.client.showGroupByRoomMsg = function (username, roomid, message) {
            var hid_roomid = $("#hid_roomid").val();
            if (roomid == hid_roomid && hid_roomid != "" && hid_roomid != null && hid_roomid != "undefined") {
                var str = '';
                var direction = username === current_uname ? 'right' : 'left';
                var str = '<div class="col-sm-12"><div class="js-msg-' + direction + ' clearfix text-black"><div class="js-msg-img pull-' + direction + '">'
                    + '<img src="/Images/avatar.jpg" width="50" height="50" /></div><div class="js-msg-text pull-' + direction + '"><div class="js-msg-nick text-' + direction + '">' + username + '</div>'
                    + '<div class="js-msg-info bg-white">' + message + '</div></div></div></div>';
                $("#js-group-content").append(str);
            } else {
                alert("您接收到一条群消息");
            }
        }

        //退出群组
        chat.client.removeRoom = function () {
            showHome();
        }

        ///显示聊天记录
        chat.client.initChatHistoryData = function (data, chattype) {
            if (data != null) {
                var list = $.parseJSON(data);
                switch (chattype) {
                    case 0:  //公共聊天记录
                        $("#js-panel-content").html(getStrHtml(list));
                        break;
                    case 1://私聊
                        $("#js-friend-content").html(getStrHtml(list));
                        break;
                    case 2: //群组聊天记录
                        $("#js-group-content").html(getStrHtml(list));
                        break;
                    default:
                        break;
                }
            }
        }

        //拼接字符串
        function getStrHtml(list) {
            var str = '';
            for (var i = 0; i < list.length; i++) {
                var direction = list[i].frmId === current_userid ? 'right' : 'left';
                 str += '<div class="col-sm-12"><div class="js-msg-' + direction + ' clearfix text-black"><div class="js-msg-img pull-' + direction + '">'
                    + '<img src="/Images/avatar.jpg" width="50" height="50" /></div><div class="js-msg-text pull-' + direction + '"><div class="js-msg-nick text-' + direction + '">' + list[i].UserName + '</div>'
                    + '<div class="js-msg-info bg-white">' + list[i].Message + '</div></div></div></div>';
            }
            return str;
        }
    })


    //发送消息给指定好友
    $("#btnFriendSend").click(function () {
        var hid_userid = $("#hid_touserid").val();
        var txtmsg = $("#txtfriendMsg").val();
        if (hid_userid == null || hid_userid == "" || hid_userid == "undefined") {
        alert("请选择一个好友进行聊天");
    } else if (txtmsg == null || txtmsg == "" || txtmsg == "undefined") {
        alert("请输入聊天内容");
    $("#txtfriendMsg").focus();
        }
        else {
        $("#txtfriendMsg").val('');
    $("#txtfriendMsg").focus();
            chat.server.sendPrivateMsg(current_uname, hid_userid, txtmsg);
        }
    })
    ///关闭好友聊天窗口
    $("#js-user-close").click(function () {
        showHome();
    })

    ///显示主页面板
    function showHome() {
        $("#js-chat-panel").show();
    $("#js-chat-friends").hide();//好友窗口
        $("#js-chat-groups").hide();
        $("#js-friend-content").html('');//清空好友聊天内容
        $("#hid_roomid").val('');
        $("#hid_touserid").val('');
        chat.server.getChatHistory(0);//获取大厅历史记录
    }

    //退出群组聊天窗口
    $("#js-group-close").click(function () {
        var roomid = $("#hid_roomid").val();
        if (roomid != null && roomid != "" && roomid != "undefined") {
            chat.server.removeRoom(roomid);
        }
    })

    ///显示创建群组模态框
    $("#btnshowGroupView").click(function () {
        $("#group_errmsg").html("");
    $("#groupModal").modal();
    })
    //创建群组
    function creategroup() {
        var roomname = $("#newroom-name").val().trim();
        if (roomname == null || roomname == "" || roomname == "undefined") {
        $("#group_errmsg").html("请输入群组名称");
    $("#newroom-name").focus();
        } else {
        chat.server.createRoom(roomname);
    $("#newroom-name").val('');
        }
    }
    //显示创建群组消息提示
    chat.client.showGroupMsg = function (state) {
        if (state == "success") {
        $("#groupModal").modal('hide');
    } else {
        $("#group_errmsg").html("已存在相同名称的群组");
    }
    }
    ///发送群组消息
    $("#btnGroupdSend").click(function () {
        var msg = $("#txtgroupMsg").val();
        var roomid = $("#hid_roomid").val();
        if (msg != null && msg != "" && msg != "undefined") {
            if (roomid != "" && roomid != null && roomid != "undefined") {
        chat.server.sendMessageByRoom(roomid, current_userid, msg);
    $("#txtgroupMsg").val('');
                $("#txtgroupMsg").focus();
            } else {
        alert("房间id不存在")
    }
    } else {
        alert("请输入消息");
    }
    })