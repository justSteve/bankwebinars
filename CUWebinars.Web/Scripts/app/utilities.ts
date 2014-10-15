/// <reference path="../typings/jquery/jquery.d.ts" />

//  This script is for url manipulation/interpretation
//  It uses the purl library - https://github.com/allmarkedup/purl

module Common {

    declare var $;

    export class Utilities {

        private sourceAttribute : string = 'source';
        private relativeAttribute: string = 'relative';

        getRelativePath(): string {
            return $.url().attr(this.relativeAttribute);
        }

        getMainPath(pathToCheck : string) : string  {

            if (pathToCheck.substr(pathToCheck.length - 1) === '/')
                return pathToCheck.substr(0, pathToCheck.length - 1);
            return pathToCheck;
        }

        goToUrl(actionMethod: string): void {
            window.location.href = this.setPathToBaseUrl() + actionMethod;
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

        setPathToBaseUrl(): string {
            var fullUrl = $.url().attr(this.sourceAttribute);
            var relativePath = this.getRelativePath();
            var indexOfRelativePath = fullUrl.indexOf(relativePath);
            return fullUrl.substr(0, indexOfRelativePath);
        }

        isValidEmailAddress(emailAddress:string) : boolean {
            var pattern = new RegExp('/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i');
            return pattern.test(emailAddress);
        }
    };
}