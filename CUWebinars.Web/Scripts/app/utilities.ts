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
    };
}