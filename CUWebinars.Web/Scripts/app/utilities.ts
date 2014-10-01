/// <reference path="../typings/jquery/jquery.d.ts" />

module Common {

    declare var $;

    export class Utilities {

        getMainPath(pathToCheck : string) : string  {

            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        }

        setPath(): string {
            var fullUrl = $.url().attr('source');
            var indexOfAccount = location.href.indexOf('Account');
            return fullUrl.substr(0, indexOfAccount - 1);

        }

        isValidEmailAddress(emailAddress:string) : boolean {
            var pattern = new RegExp('/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i');
            return pattern.test(emailAddress);
        }
    };
}