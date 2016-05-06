(function() {
    'use strict';

    TITcs.Http.get('/services/avatarservice.sps/get').then(function (response) {

        $('#serviceDone').text(JSON.stringify(response));

    });

    TITcs.Http.get('/services/avatar1service.sps/fail').then(function (response) {

        $('#serviceFail').text(JSON.stringify(response));

    });

    TITcs.Http.get('/services/avatarservice.sps/error').then(function (response) {

        $('#serviceError').text(JSON.stringify(response));

    });

    TITcs.Http.get('/services/avatarservice.sps/businessrule').then(function (response) {

        $('#serviceBusinessRule').text(JSON.stringify(response));

    });

    $("#isIE").text(TITcs.Utils.isIE);


})();