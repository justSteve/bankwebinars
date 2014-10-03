/// <reference path="../typings/jquery/jquery.d.ts" />
var Common;
(function (Common) {
    var Utilities = (function () {
        function Utilities() {
        }
        Utilities.prototype.getRelativePath = function () {
            return $.url().attr('relative');
        };

        Utilities.prototype.getMainPath = function (pathToCheck) {
            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        };

        Utilities.prototype.relativePathStartsWith = function (stringToCheck) {
            var path = this.getRelativePath();
            var len = stringToCheck.length;

            if (path.slice(0, len) === stringToCheck)
                return true;
            else {
                return false;
            }
        };

        Utilities.prototype.setPath = function () {
            var fullUrl = $.url().attr('source');
            var indexOfAccount = location.href.indexOf('Account');
            return fullUrl.substr(0, indexOfAccount - 1);
        };

        Utilities.prototype.isValidEmailAddress = function (emailAddress) {
            var pattern = new RegExp('/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i');
            return pattern.test(emailAddress);
        };
        return Utilities;
    })();
    Common.Utilities = Utilities;
    ;
})(Common || (Common = {}));
//# sourceMappingURL=utilities.js.map
