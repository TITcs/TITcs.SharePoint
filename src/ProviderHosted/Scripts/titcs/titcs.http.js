TITcs.Http = (function () {

    var get = function (url, data, cache) {

        return http({
            url: url,
            data: data,
            type: 'GET',
            cache: cache
        });

    },
    post = function (url, data) {

        return http({
            url: url,
            data: data,
            type: 'POST'
        });

    },
    put = function (url, data) {

        return http({
            url: url,
            data: data,
            type: 'PUT'
        });

    },
    del = function (url, data) {

        return http({
            url: url,
            data: data,
            type: 'DELETE'
        });

    };

    function http(options) {

        if (options.cache === undefined) {
            options.cache = false;
        }

        console.log("Url: " + options.url);

        return $.ajax({
            url: options.url,
            type: options.type,
            data: options.data,
            dataType:'json',
            cache: options.cache,
            statusCode: {
                401: function () {
                    window.location = "/login.aspx";
                }
            },
            error: function (a) {

                if (a.status === 401) {
                    window.location = "/login.aspx";
                    return;
                }

                //var response = $.parseJSON(a.responseText);

                alert(a.responseText);

            }

        }).always(function(a) {
            console.log(a);
        });

    };

    return {
        get: get,
        post: post,
        put: put,
        del: del
    };

})();





