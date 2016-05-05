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
 