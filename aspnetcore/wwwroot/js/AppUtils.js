(function (window, $) {
    var _utils = {
        /**
         * 手机号
         */
        checkTel: function (text) {
            return /^1[345789]\d{9}$/.test(text);
        },

        /**
         * 银行卡号
         */
        checkCardNo: function (text) {
            return /^\d{16,19}$/.test(text);
        },

        /**
         * 身份证号
         */
        checkIdcard: function (text) {
            return /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/.test(text);
        },

        /**
         * 6-10位非纯数字或字母
         */
        checkPassword: function (text) {
            return /^(?![^A-Za-z]+$)(?![^0-9]+$)[\x21-x7e]{6,10}$/.test(text);
        },

        /**
         * 6位数字
         */
        checkMobileCode: function (text) {
            return /^\d{6}$/.test(text);
        },

        /**
         * 金额输入格式
         */
        checkAmount: function (text) {
            return /^[0-9]+(.[0-9]{1,2})?$/.test(text);
        },

        /**
         * 姓名 2-6位汉字
         */
        checkName: function (text) {
            return /^[\u4E00-\u9FA5]{2,10}$/
        },

        /**
         * 短信发送倒计时
         */
        countdown: function (that, timespan) {
            if (!timespan) timespan = 60;
            that.SMSDisabled = true;
            var interval = window.setInterval(function () {
                if ((that.TimeSpan--) <= 0) {
                    window.clearInterval(interval);
                    that.TimeSpan = timespan;
                    that.SMSDisabled = false;
                }
            }, 1000);
        },

        /**
         * url参数
         */
        queryString: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return decodeURI(r[2]); return null;
        },

        /**
         * 解析url
         */
        parseUrl: function (url) {
            var reg = /^(?:([A-Za-z]+):)?(\/{0,3})([0-9.\-A-Za-z]+)(?::(\d+))?(?:\/([^?#]*))?(?:\?([^#]*))?(?:#(.*))?$/;
            var result = reg.exec(url);
            var names = ['url', 'scheme', 'slash', 'host', 'port', 'path', 'query', 'hash'];
            var urlObj = {};
            for (var i = 0; i < names.length; i++) {
                urlObj[names[i]] = result[i] || '';
            }

            return urlObj;
        },

        /**
         * localStorage占用空间
         */
        localStorageSpace: function (unit) {
            var allStrings = '';
            for (var key in window.localStorage) {
                if (window.localStorage.hasOwnProperty(key)) {
                    allStrings += window.localStorage[key];
                }
            }
            var size_kb = allStrings ? 3 + ((allStrings.length * 16) / (8 * 1024)) : 0;
            var size;
            unit = (unit || 'KB').toUpperCase();
            switch (unit) {
                case 'MB': size = size_kb / 1024; break;
                case 'KB': size = size_kb; break;
                default: size = size_kb; break;
            }

            return size.toFixed(2) + unit;
        }
    }

    window.APPUtils = window.AppUtils = _utils;
})(window, jQuery);