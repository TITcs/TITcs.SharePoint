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
            isIE: isIE
        };

    })();

    TITcs.Utils = utils;

})(TITcs || (TITcs = {}));