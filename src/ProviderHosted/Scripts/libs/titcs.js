var TITcs = {
    SharePoint: {}
};


/*
 (function(TITcs) {
    "use strict";

    var MODULE = (function() {


        return {
        };

    })();

    TITcs.NAMESPACE = MODULE;

})(TITcs || (TITcs = {}));
 */
 (function(TITcs) {
    "use strict";

    var log = (function() {

        var write = function(type, source, message) {

            var output = "[" + type + " - " + source + "] ";

            if (typeof message === "object") {
                output += JSON.stringify(message);
            } else {
                output += message;
            }

            if (console.log) {
                console.log(output);
            } else {
                alert(output);
            }
        };

        var debug = function(source, message) {
            
            write("DEBUG", source, message);

        };

        var error = function(source, message) {
            
            write("ERROR", source, message);

        };

        var info = function(source, message) {
            
            write("INFO", source, message);

        };

        return {
            debug: debug,
            error: error,
            info: info
        };

    })();

    TITcs.Log = log;

})(TITcs || (TITcs = {}));
 
(function(TITcs) {

    var http = (function() {

        var get = function(url, data, cache) {

            TITcs.Log.debug("TITcs.Http.get", { url: url, data: data, cache: cache });

            return http({
                url: url,
                data: data,
                type: 'GET',
                cache: cache
            });

        };

        var post = function (url, data) {

            TITcs.Log.debug("TITcs.Http.get", { url: url, data: data });

            return http({
                url: url,
                data: data,
                type: 'POST'
            });

        };

        var put = function(url, data) {

            TITcs.Log.debug("TITcs.Http.put", { url: url, data: data });

            return http({
                url: url,
                data: data,
                type: 'PUT'
            });

        };

        var del = function(url, data) {

            TITcs.Log.debug("TITcs.Http.del", { url: url, data: data });

            return http({
                url: url,
                data: data,
                type: 'DELETE'
            });

        };

        var http = function (options) {

            if (options.cache === undefined) {
                options.cache = false;
            }

            TITcs.Log.debug("TITcs.Http.http", options.url);

            var defer = $.Deferred();

            $.ajax({
                url: options.url,
                type: options.type,
                data: options.data,
                dataType: 'json',
                cache: options.cache

            }).then(function(data) {

                TITcs.Log.debug("TITcs.Http.done", data);

                defer.resolve(data);

            }, function(jqXHR) {

                var response = jqXHR.responseJSON;

                var result = {
                    success: false
                };

                if (response.exception) {
                    result.exception = response.exception;
                }

                if (response.rule) {
                    result.rule = response.rule;
                }

                if (response.data) {
                    result.data = response.data;
                }

                TITcs.Log.debug("TITcs.Http.fail", result);

                defer.resolve(result);

            });

            return defer;

        };

        return {
            get: get,
            post: post,
            put: put,
            del: del
        };

    })();

    TITcs.Http = http;

})(TITcs || (TITcs = {}));





(function(TITcs) {

    var utils = (function() {

        var isIE = function() {
            
            var isBrowserIE = false;

            if ((navigator.userAgent.indexOf("MSIE") !== -1) || (!!document.documentMode)) //IF IE > 10
            {
                isBrowserIE = true;
            }

            TITcs.Log.info("TITcs.Utils.isIE", "Is Internet Explorer: " + isBrowserIE);

            return isBrowserIE;
        };

        return {
            isIE: isIE()
        };

    })();

    TITcs.Utils = utils;

})(TITcs || (TITcs = {}));