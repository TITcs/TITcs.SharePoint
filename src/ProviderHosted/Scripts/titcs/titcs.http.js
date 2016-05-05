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




