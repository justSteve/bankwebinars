/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/toastr/toastr.d.ts" />
var Common;
(function (Common) {
    var Logger = (function () {
        function Logger() {
            var _this = this;
            this.getLogFn = function (moduleId, fnName) {
                fnName = fnName || 'log';

                switch (fnName.toLowerCase()) {
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

                var func = _this[fnName];

                return function (msg, data, showToast) {
                    func(msg, data, moduleId, (showToast === undefined) ? true : showToast);
                };
            };
            this.log = function (message, data, source, showToast) {
                _this.logIt(message, data, source, showToast, 'info');
            };
            this.logWarning = function (message, data, source, showToast) {
                _this.logIt(message, data, source, showToast, 'warning');
            };
            this.logSuccess = function (message, data, source, showToast) {
                _this.logIt(message, data, source, showToast, 'success');
            };
            this.logError = function (message, data, source, showToast) {
                _this.logIt(message, data, source, showToast, 'error');
            };
        }
        Logger.prototype.logIt = function (message, data, source, showToast, toastType) {
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
        };
        return Logger;
    })();
    Common.Logger = Logger;
    ;
})(Common || (Common = {}));
//# sourceMappingURL=ToastLogger.js.map
