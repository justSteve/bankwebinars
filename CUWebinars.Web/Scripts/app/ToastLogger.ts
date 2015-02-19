/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/toastr/toastr.d.ts" />


module Common {
    declare var $;
    declare var toastr;

    export class Logger {

        logIt(message: string, data: string, source: string, showToast: boolean, toastType: string): void {
            if (showToast) {
                if (toastType === 'error') {
                    toastr.error(message);
                } else if (toastType === 'warning') {
                    toastr.warning(message);
                } else if (toastType === 'success') {
                    toastr.success(message);
                } else {
                    toastr.info(message);
                }
            }
        }

        getLogFn = (moduleId: string, fnName: string) => {

            fnName = fnName || 'log';

            switch (fnName.toLowerCase()) { // convert aliases
            case 'success':
                fnName = 'logSuccess';
                break;
            case 'error':
                fnName = 'logError';
                break;
            case 'warn':
                fnName = 'logWarning';
                break;
            case 'warning':
                fnName = 'logWarning';
                break;
            }

            var func = this[fnName];

            return (msg, data, showToast) => {
                func(msg, data, moduleId, (showToast === undefined) ? true : showToast);
            };
        }

        log = (message: string, data: string, source: string, showToast: boolean) => {
            this.logIt(message, data, source, showToast, 'info');
        }

        logWarning = (message: string, data: string, source: string, showToast: boolean) => {
            this.logIt(message, data, source, showToast, 'warning');
        }

        logSuccess = (message: string, data: string, source: string, showToast: boolean) => {
            this.logIt(message, data, source, showToast, 'success');
        }

        logError = (message: string, data: string, source: string, showToast: boolean) => {
            this.logIt(message, data, source, showToast, 'error');
        }

    };
}