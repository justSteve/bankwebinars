if (LOGGING === null || typeof LOGGING === 'undefined')
    var LOGGING = {}; // object to holds all references and methods.

var L = LOGGING; // create shortcut alias

L.clientLogger = function () {

    var errorLevel = 'error';
    var _logNote = '';
    var infoLevel = 'log';
    var SendErrRpt = function (level, id1, object) {

        var data = {
            level: level,
            id: id1,
            object: object
        };

        $.post('/Cart/Logger', data,
            function (response,
                status,
                xhr) {

                window.Cookies.set('Log', response);
            });
    };
    var log = function (logLevel, id, obj) {

        var logObj = {};

        logObj[id] = obj;

        //SendErrRpt(logLevel, id, obj);
        switch (logLevel) {
            case errorLevel:

                if (typeof $zopim !== 'undefined') {
                    $zopim && $zopim(function () {

                        $zopim.livechat.addTags(id);

                        $zopim.livechat.bubble.setTitle('Get Help Here!');
                    });
                }
                break;
            case infoLevel:
                //Rollbar.info(logObj);

                if (typeof $zopim !== 'undefined') {
                    $zopim(function () {

                        $zopim.livechat.addTags(id);

                        //$zopim.livechat.bubble.setTitle('Get Help Here!');
                    });
                }
                break;
        }
    };
    var logNote = function (logNote, id, obj) {

        var logObj = {};
        logObj[id] = obj;

        if (typeof $zopim !== 'undefined') {
            $zopim && $zopim(function () {

                $zopim.livechat.appendNotes(logNote);
                $zopim.livechat.bubble.setTitle('Question? Get Help Here!');
            });

        }
    };

    var error = function (id, obj) {
        log(errorLevel, id, obj);
    };
    var appendNotes = function (id, obj) {
        //alert("hit");
        //logNote(_logNote, id, obj);
    };

    var info = function (id, obj) {
        log(infoLevel, id, obj);
    };


    return {
        appendNotes: appendNotes,
        errorLevel: errorLevel,
        infoLevel: infoLevel,
        log: log,
        error: error,
        info: info
    };
}();

