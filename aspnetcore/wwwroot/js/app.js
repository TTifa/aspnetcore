(function (window, $) {
    // 遮罩层对象
    var _mask = {
        show: function () { },
        hide: function () { }
    };
    var _user = {
        uid: 0,
        workstatus: 0,
        username: '',
        token: ''
    };
    var _dialog = {
        alert: function (msg) { alert(msg); },
        confirm: function (msg, ok, no) {
            if (confirm(msg)) {
                if (typeof (ok) == "function")
                    ok();
            }
            else if (typeof (no) == "function")
                no();
        }
    };
    var _client = {
        /**
         * *  请求接口
         * @param {string} api 请求接口
         * @param {object} args 请求参数
         * @param {function} cd 回调
         * @param {string} loading 显示loading
         * @return void
         */
        Post: function (api, args, cb, loading) {
            if (typeof (args) != 'object') {
                _dialog.alert('params error');
            }
            (loading || loading == '') && _mask.show(loading);
            $.ajax({
                type: 'POST',
                url: api,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(args),
                success: function (data) {
                    _mask.hide();

                    cb(data);
                },
                error: function (xmlRequest, textStatus, errorThrown) {
                    _dialog.alert('network error!');
                    _mask.hide();
                    console.log(textStatus);
                }
            });
        },
        Get: function (api, args, cb, loading) {
            if (typeof (args) != 'object') {
                _dialog.alert('params error');
            }
            api += '?' + this.getParamStr(args);
            (loading || loading == '') && _mask.show(loading);
            $.ajax({
                type: 'GET',
                url: api,
                dataType: 'json',
                success: function (data) {
                    _mask.hide();

                    cb(data);
                },
                error: function (xmlRequest, textStatus, errorThrown) {
                    _dialog.alert('network error!');
                    _mask.hide();
                    console.log(textStatus);
                }
            });
        },

        getParamStr: function (dic) {
            var sdic = Object.keys(dic).sort();
            var str = '';
            for (var ki in sdic) {
                str += sdic[ki] + "=" + dic[sdic[ki]] + "&";
            }
            return str.substring(0, str.length - 1);
        },

        urlParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return decodeURI(r[2]); return null;
        },

        /**
         * 设置请求遮罩层显示/隐藏
         *  @param show Function 显示遮罩层方法
         *  @param hide Function 隐藏遮罩层方法
         */
        setMask: function (show, hide) {
            if (!(typeof show === "function" && typeof hide === "function")) {
                console.log("mask event must be a function.");
                return this;
            }

            _mask = { show: show, hide: hide }

            return this;
        },

        /**
         * 设置对话框
         */
        setDialog: function (_alert, _confirm) {
            if (typeof _alert != "function") {
                console.log("dialog event must be a function.");
                return this;
            }

            _dialog = {
                alert: _alert,
                confirm: (typeof _confirm === "function") ? _confirm : confirm,
            };

            return this;
        },

        /**
         * 设置登陆用户信息
         */
        setUser: function (uid, username, token) {
            _user.uid = uid;
            _user.username = username;
            _user.token = token;

            return this;
        },
        getUserId: function () {
            return _user.uid;
        },
        getUserName: function () {
            return _user.username;
        },
        getToken: function () {
            return _user.token;
        }
    }
    window.APIClient = window.ApiClient = _client;
})(window, jQuery);

/*
(function (window, $) {
    ApiClient.setMask($.showLoading, $.hideLoading);
    ApiClient.setDialog($.alert, $.confirm);
    
    var account = localStorage.getItem('Logined_Worker');
    if (account) {
        account = JSON.parse(account);
        APIClient.setUser(account.Uid, account.UserName, account.Token);
    } else {
        var path = window.location.pathname.toLowerCase();
        var noLogin = false;
        for (var i = 0; i < config.NoLoginPage.length; i++) {
            if (config.NoLoginPage[i] == path) {
                noLogin = true;
                break;
            }
        }
        if (!noLogin)
            window.location.href = '/login.html';
    }
})(window, jQuery);
*/