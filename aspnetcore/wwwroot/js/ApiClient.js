(function (window, $) {
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
                if (typeof (ok) === 'function')
                    ok();
            }
            else if (typeof (no) === "function")
                no();
        }
    };
    var _client = {
        /**
         * *  请求接口
         * @param {string} api 请求接口
         * @param {object} args 请求参数
         * @param {function} cb 回调
         * @param {string} loading 显示loading
         * @return void
         */
        Request: function (api, args, cb, loading) {
            if (typeof (args) != 'object') {
                _dialog.alert('params error');
            }
            (loading || loading == '') && _mask.show(loading);
            $.ajax({
                type: 'post',
                url: api,
                dataType: 'json',
                data: subData,
                beforeSend: function (request) {
                    request.setRequestHeader("Token", _user.token);
                },
                success: function (data) {
                    _mask.hide();
                    // 未登录
                    if (data.status == 2) {
                        location.href = '/Account/SignIn';
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
        combineData: function (service, data) {
            var common = {};
            common.format = config.common.format;
            common.v = config.common.v;
            common.partner = config.common.partner;
            common.sec_id = config.common.sec_id;
            common.req_data = data;
            common.service = service;
            var str = this.getStr(common);
            var md5Str = md5(str + 111111).toUpperCase();
            common.sign = md5Str;

            return common;
        },
        getStr: function (dic) {
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

    var _config = {
        common: {
            format: "json",
            v: "1.0",
            partner: "2015000000000000",
            sec_id: "md5",
            url: "http://localhost/",
        },
        Api: {
            Login: '/Account/SignIn', //登录
        },
        NoLoginPage: ['/Account/SignIn']
    }

    window.config = _config;
})(window, jQuery);

(function (window, $) {
    var account = localStorage.getItem('LoginedUser');
    if (account) {
        account = JSON.parse(account);
        APIClient.setUser(account.Uid, account.Username, account.Token);
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
            window.location.href = '/Account/SignIn';
    }
})(window, jQuery);