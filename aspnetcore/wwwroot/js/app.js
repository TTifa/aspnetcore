(function (window, $) {
    var apiHost = 'http://localhost:10001';
    // 遮罩层对象
    var _mask = {
        show: function () { },
        hide: function () { }
    };
    var _user = {
        uid: 0,
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
            api = apiHost + api;
            console.log(api);
            $.ajax({
                type: 'POST',
                url: api,
                dataType: 'json',
                //跨域携带cookie
                //xhrFields: {
                //    withCredentials: true
                //},
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(args),
                beforeSend: function (request) {
                    console.log(_user.token);
                    if (_user.token)
                        request.setRequestHeader('access_token', _user.token);
                },
                success: function (data) {
                    _mask.hide();
                    if (data.status == 2) {
                        location.href = '/Account/SignIn';
                        return;
                    }
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
            api = apiHost + api;
            console.log(api);
            $.ajax({
                type: 'GET',
                url: api,
                dataType: 'json',
                xhrFields: {
                    withCredentials: true
                },
                beforeSend: function (request) {
                    console.log(_user.token);
                    if (_user.token)
                        request.setRequestHeader('access_token', _user.token);
                },
                success: function (data) {
                    _mask.hide();
                    if (data.status == 2) {
                        location.href = '/Account/SignIn';
                        return;
                    }
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

(function (window, $) {
    var loginuser = localStorage.getItem('LoginedUser');
    if (loginuser) {
        loginuser = JSON.parse(loginuser);
        APIClient.setUser(loginuser.Uid, loginuser.Username, loginuser.Token);
    }
})(window, jQuery);