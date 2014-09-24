/// <reference path="../typings/jquery/jquery.d.ts" />
var Common;
(function (Common) {
    var Utilities = (function () {
        function Utilities() {
        }
        Utilities.prototype.getMainPath = function (pathToCheck) {
            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        };

        Utilities.prototype.setPath = function () {
            var indexOfHome = location.href.indexOf('Account');
            var path = '';

            if (indexOfHome > -1)
                path = location.href.substr(0, location.href.indexOf('Account') - 1);
            else
                path = location.href;

            //  IE is a rubbish browser!
            if (path === '')
                path = $(location).attr('href');
            return path;
        };
        return Utilities;
    })();
    Common.Utilities = Utilities;
    ;
})(Common || (Common = {}));
//# sourceMappingURL=utilities.js.map
