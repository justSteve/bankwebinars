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
        }

        isValidEmailAddress(emailAddress:string) : boolean {
            var pattern = new RegExp('/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i');
            return pattern.test(emailAddress);
        }
    };
}