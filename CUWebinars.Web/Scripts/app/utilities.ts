/// <reference path="../typings/jquery/jquery.d.ts" />

module Common {

    declare var $;

    export class Utilities {

        getRelativePath(): string {
            return $.url().attr('relative');
        }

        getMainPath(pathToCheck : string) : string  {

            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        }

        relativePathStartsWith(stringToCheck : string) : boolean {
            var path = this.getRelativePath();
            var len = stringToCheck.length;

            if (path.slice(0, len) === stringToCheck)
                return true;
            else {
                return false;
            }
        }

        setPath(): string {
            var fullUrl = $.url().attr('source');
            var relativePath = this.getRelativePath();
            var indexOfAccount = location.href.indexOf(relativePath);
            return fullUrl.substr(0, indexOfAccount - 1);
        }

        isValidEmailAddress(emailAddress:string) : boolean {
            var pattern = new RegExp('/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i');
            return pattern.test(emailAddress);
        }
    };
}