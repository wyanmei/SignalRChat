(function ($) {
    var index = {
        init: function () {
            var me = this;
            me.render();
            me.bind();
        },
        render: function () {
            var me = this;
            me.nick = $('#txtNick');/*昵称*/
            me.pwd = $('#txtPwd');/*密码*/
            me.userid = $("#userid");
            me.login = $('#btnLogin');

        },
        bind: function () {
            var me = this;
            me.login.on('click', function () {
                var nick = $.trim(me.nick.val());
                var pwd = $.trim(me.pwd.val());
                var userid = $.trim(me.userid.val());
                location.href = '/Chat/Index?nick=' + nick + '&pwd=' + pwd + '&userid=' + userid;
            });
        }
    };
    $(function () {
        index.init();
    });
})(jQuery)