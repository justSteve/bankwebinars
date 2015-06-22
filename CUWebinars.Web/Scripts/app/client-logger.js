if (LOGGING === null || typeof LOGGING === 'undefined')
    var LOGGING = {}; // object to holds all references and methods.

var L = LOGGING; // create shortcut alias



L.clientLogger = function () {

    var errorLevel = 'error';
    var infoLevel = 'log';

    var log = function(logLevel, id, obj) {

        var logObj = {};
        logObj[id] = obj;

        switch (logLevel) {
            case errorLevel:
                Rollbar.error(logObj);
                break;
            case infoLevel:
                Rollbar.info(logObj);
                break;
        }
    };

    var error = function(id, obj) {
        log(errorLevel, id, obj);
    };

    var info = function (id, obj) {
        log(infoLevel, id, obj);
    };


    return{
        errorLevel: errorLevel,
        infoLevel: infoLevel,
        log: log,
        error: error,
        info: info
    };
}();

