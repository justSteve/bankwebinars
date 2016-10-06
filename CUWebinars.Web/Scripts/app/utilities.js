/// <reference path="../typings/jquery/jquery.d.ts" />
//  This script is for url manipulation/interpretation
//  It uses the purl library - https://github.com/allmarkedup/purl
var Common;
(function (Common) {
    var Utilities = (function () {
        function Utilities() {
            this.sourceAttribute = 'source';
            this.relativeAttribute = 'relative';
        }
        Utilities.prototype.getFullPath = function () {
            return $.url().attr(this.sourceAttribute);
        };

        Utilities.prototype.getRelativePath = function () {
            return $.url().attr(this.relativeAttribute);
        };

        Utilities.prototype.getMainPath = function (pathToCheck) {
            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        };

        Utilities.prototype.goToUrl = function (actionMethod) {
            window.location.href = this.setPathToBaseUrl() + actionMethod;
        };

        Utilities.prototype.openInNewWindow = function (actionMethod, title) {
            window.open(this.setPathToBaseUrl() + actionMethod, title);
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

        Utilities.prototype.setPathToBaseUrl = function () {
            var fullUrl = $.url().attr(this.sourceAttribute);
            var relativePath = this.getRelativePath();
            var indexOfRelativePath = fullUrl.indexOf(relativePath);
            return fullUrl.substr(0, indexOfRelativePath);
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
