/// <reference path="../Constants.js" />
/// <reference path="../utilities.js" />

if (MEMBERSHIPNOTIFICATIONS === null || typeof MEMBERSHIPNOTIFICATIONS === 'undefined')
    var MEMBERSHIPNOTIFICATIONS = {}; // create namespace - object to holds all references and methods.
var MN = MEMBERSHIPNOTIFICATIONS; // alias for code brevity

//  document.ready function
$(function () {

    MN.primeDomVariables();
    MN.wireUpHandlers();
});


(function(ns) {

    ns.passwordResetInitialized = false;
    ns.passwordConfirmResetInitialized = false;
    ns.jsonDataForImportedOrder;

    ns.primeDomVariables = function () {
        MN.inputFormFields = $('#InputFormFields');
        MN.impersonateUserButton = $('#ImpersonateUserButton');
        MN.createUserButton = $('#CreateUserButton');
        MN.accountsBrandLink = MN.impersonateUserButton.parent().parent().prev();
        MN.resetPasswordButton = $('#ResetPasswordButton');
        MN.confirmPasswordButton = $('#ConfirmPasswordButton');
        MN.getImportOrderFieldsButton = $('#GetImportOrderFieldsButton');
        MN.getManualResetPasswordFieldsButton = $('#GetManualResetPasswordFieldsButton');
        MN.getImportOrderFieldsFromCsvButton = $('#GetImportOrderFieldsFromCsvButton');
        MN.getBatchPwdResetHtmlButton = $('#BatchPwdResetHtmlButton');
    };

    ns.wireUpHandlers = function() {

        MN.impersonateUserButton.on('click', MN.impersonateUserButtonClicked);
        MN.createUserButton.on('click', MN.createUserButtonClicked);
        MN.resetPasswordButton.on('click', MN.resetPasswordButtonClicked);
        MN.confirmPasswordButton.on('click', MN.confirmPasswordButtonClicked);
        MN.getImportOrderFieldsButton.on('click', MN.getImportOrderFieldsButtonClicked);
        MN.getManualResetPasswordFieldsButton.on('click', MN.getManualResetPasswordFieldsButtonClicked);
        MN.getImportOrderFieldsFromCsvButton.on('click', MN.getImportOrderFieldsFromCsvButtonClicked);
        MN.getBatchPwdResetHtmlButton.on('click', MN.getBatchPwdResetHtmlButtonClicked);
    };

    ns.impersonateUserButtonClicked = function(e) {

        e.preventDefault();

        var url = '/Admin/LogInAsUser';

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function() {
                // this is where we append a loading image
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function(result) {
            MN.inputFormFields.html(result);
            $('#loadSpinner1').remove();
        }).fail(function(jqXHR, textStatus, errorThrown) {

            // errorThrown has error message, or "timeout" in case of timeout.

            Rollbar.error({ 'LogInAsUser AJAX error: ': { 'errorThrown': errorThrown } });

            $('#loadSpinner1').remove();
        });
    };

    ns.createUserButtonClicked = function(e) {

        e.preventDefault();

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/CreateAUser',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function() {
                // this is where we append a loading image
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (result) {

            MN.inputFormFields.html(result);
            MN.initializeCreateUserFields();
            $('#loadSpinner1');

        }).fail(function(jqXHR, textStatus, errorThrown) {
            // Request failed. Show error message to user. 
            // errorThrown has error message, or "timeout" in case of timeout.

            var err = new Error('CreateUserButton AJAX error: ' + errorThrown);
            //NREUM.noticeError(err);
            var i = 0;
            $('#loadSpinner1');
        });

    };

    ns.initializePasswordResetFields = function() {

        var passwordResetStatusLabel = $('#PasswordResetStatus');
        var resetPasswordForm = $('#ResetPasswordForm');

        passwordResetStatusLabel.text('');

        if (!MN.passwordResetInitialized) {
            resetPasswordForm.submit(function (e) {
                e.preventDefault();
            });

            $('#NormalResetPasswordButton').on('click', function(e) {

                e.preventDefault();

                var self = this;

                var token = resetPasswordForm.find('input[name=__RequestVerificationToken]').val();
                var headers = {};
                headers['__RequestVerificationToken'] = token;

                var model = {
                    email: $.trim($('#ResetPassEmail').val())
                };

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: '/Admin/ResetPassword',
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(model),
                    headers: headers,
                    beforeSend: function() {
                        $(self).after('<span id="loadSpinner2">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                        $('#resetResult').remove();
                    }
                }).done(function(data) {

                    if (data.Result === 'Success') {
                        $(self).after('<span id="resetResult">&nbsp;<span class="label label-success"><span> Password reset email has been sent!</span></span></span>');
                    } else if (data.Result === 'Fail') {
                        $(self).after('<span id="resetResult">&nbsp;<span class="label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;<span>Password reset failed!</span></span></span>');
                    }

                    $('#loadSpinner2').remove();
                });
            });

            MN.passwordResetInitialized = true;
        }
    };

    ns.resetPasswordButtonClicked = function(e) {

        e.preventDefault();

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/PasswordResetOperation',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function() {
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function(result) {

            MN.inputFormFields.html(result);
            MN.initializePasswordResetFields();
            $('#loadSpinner1').remove();
        });
    };

    ns.confirmPasswordButtonClicked = function (e) {

        e.preventDefault();

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/GetPasswordResetConfirmFields',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (result) {

            MN.inputFormFields.html(result);
            MN.initializePasswordResetConfirmFields();
            $('#loadSpinner1').remove();
        });
    };

    ns.getImportOrderFieldsButtonClicked = function(e) {

        e.preventDefault();

        if (MN.jsonDataForImportedOrder) {
            $('#OrderSucceeded').remove();
            MN.inputFormFields.append('<button id="ImportOrderButton" class =" btn btn-success">Import Order</button>');
            MN.inputFormFields.append('<textarea id="JsonPayloadTextArea" rows="40" cols="100" style="width:100%;margin-top:10px"></textarea>');
            $('#JsonPayloadTextArea').val(MN.jsonDataForImportedOrder);
            MN.addImportOrderButtonClick();
        } else {
            $.ajax({
                type: 'GET',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/GetJsonTextArea',
                dataType: constants.HtmlDataType,
                data: null,
                beforeSend: function() {
                    // this is where we append a loading image
                    if ($('#ImportOrderButton').length > 0)
                        $('#ImportOrderButton').off('click');
                    jsonPayload = $('#JsonPayload').text();
                    MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function(result) {

                MN.inputFormFields.html(result);

                MN.addImportOrderButtonClick();

                $('#loadSpinner1').remove();
            });
        }
    };

    ns.getManualResetPasswordFieldsButtonClicked = function(e) {

        e.preventDefault();

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/ManualPasswordReset',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function() {
                if ($('#ImportOrderButton').length > 0)
                    $('#ImportOrderButton').off('click');
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function(result) {

            MN.inputFormFields.html(result);

            $('#ManualResetPasswordButton').on('click', function(args) {

                args.preventDefault();

                var self = this;

                $.ajax({
                    type: 'POST',
                    contentType: constants.FormPostContentType,
                    cache: false,
                    url: '/Admin/ManualPasswordReset',
                    dataType: constants.JsonDataType,
                    data: $('#ManualResetPasswordForm').serialize(),
                    beforeSend: function() {
                        // this is where we append a loading image
                        $(self).after('<span id="loadSpinner2">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                    }
                }).done(function(response) {

                    if (response.Result === 'Success') {
                        $('#OperationMessage').html('<span class="label label-success">&nbsp;&nbsp;User password has been changed.</span>');
                    } else if (response.Result === 'Fail') {
                        $('#OperationMessage').html('<span class="label label-important"><strong>&nbsp;&nbsp;There was an error at the server. The new user has not been created.</strong></span>');
                    }

                    $('#loadSpinner2').remove();
                });
            });

            $('#loadSpinner1').remove();
        });
    };

    ns.getImportOrderFieldsFromCsvButtonClicked = function (e) {

        e.preventDefault();

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/ReadCsvAndReturnJson',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                if ($('#ImportOrderButton').length > 0)
                    $('#ImportOrderButton').off('click');
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (result) {

            var jsonData = JSON.parse(result);

            $.ajax({
                type: 'POST',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Api/Order',
                dataType: constants.HtmlDataType,
                data: jsonData,
                beforeSend: function () {
                }
            }).done(function (result) {

                var resultAsJson = JSON.parse(result);

                MN.inputFormFields.html('<span id="OrderSucceeded" class="label label-success">' + resultAsJson.Result + '</span>');
                $('#loadSpinner1').remove();

            }).fail(function (result) {
                var resultAsJson = JSON.parse(result.responseText);
                MN.inputFormFields.html('<span id="OrderSucceeded" class="label label-important">' + resultAsJson.Result + '</span>');
                $('#loadSpinner1').remove();
            });
        });
    };

    ns.addImportOrderButtonClick = function () {

        $('#ImportOrderButton').on('click', function (e) {

            e.preventDefault();

            var self = this;

            var jsonPayload = $('#JsonPayloadTextArea').val();
            var queryString = '?';

            $.each($.parseJSON(jsonPayload), function (idx, value) {
                queryString += idx + '=' + value + '&';
            });

            $.ajax({
                type: 'GET',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Order/CreateOrder' + queryString,
                dataType: constants.JsonDataType,
                //data: $('#JsonPayloadTextArea').val(),
                beforeSend: function () {
                    $(self).after('<span id="loadSpinner2">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function (result) {

                var id = parseInt(result.Result, 10); // this is base 10 (2nd param)

                if (id > 0) {
                    MN.inputFormFields.html('<span id="OrderSucceeded" class="label label-success">Success! Order Id: ' + id + '</span>');
                } else {
                    MN.inputFormFields.html('<span id="OrderFailed" class="label label-important">There was an error at the server and the order was not imported.</span>');
                }

                $('#loadSpinner2').remove();

            }).fail(function (result) {

                var id = parseInt(result.Result, 10); // this is base 10 (2nd param)

                if (id < 1) {
                    MN.inputFormFields.html('<span id="OrderFailed" class="label label-important">There was an error at the server and the order was not imported.</span>');
                }
                $('#loadSpinner2').remove();
            });
        });
    };

    ns.initializeCreateUserFields = function() {

        var shippingAddressContainer = $(constants.ShippingAddressContainer);
        var addShippingAddressLink = $(constants.AddShippingAddressLink);
        var hideAddShippingAddressLink = $(constants.HideAddShippingAddressLink);

        shippingAddressContainer.hide();
        hideAddShippingAddressLink.hide();

        addShippingAddressLink.on('click', function(e) {

            e.preventDefault();

            if (!hideAddShippingAddressLink.is(':visible')) {

                shippingAddressContainer.slideDown(800, function() {
                    addShippingAddressLink.fadeOut(400, function() {
                        hideAddShippingAddressLink.fadeIn(400);
                    });
                });
            }
        });

        hideAddShippingAddressLink.on('click', function(e) {

            e.preventDefault();

            shippingAddressContainer.slideUp(800, function() {
                hideAddShippingAddressLink.fadeOut(400, function() {
                    addShippingAddressLink.fadeIn(400);
                });
            });
        });

        $('form').submit(function() {

            if (!hideAddShippingAddressLink.is(':visible')) {

                var billingStreetAddress = $(constants.BillingAddressFields.concat(constants.StreetAddress)).val();
                var billingStreetAddress2 = $(constants.BillingAddressFields.concat(constants.StreetAddress2)).val();
                var billingCity = $(constants.BillingAddressFields.concat(constants.City)).val();
                var billingState = $(constants.BillingAddressFields.concat(constants.State)).val();
                var billingZip = $(constants.BillingAddressFields.concat(constants.Zip)).val();
                var billingCountry = $(constants.BillingAddressFields.concat(constants.Country)).val();
                var billingPhone = $(constants.BillingAddressFields.concat(constants.Phone)).val();

                $(constants.ShippingAddressFields.concat(constants.StreetAddress)).val(billingStreetAddress);
                $(constants.ShippingAddressFields.concat(constants.StreetAddress2)).val(billingStreetAddress2);
                $(constants.ShippingAddressFields.concat(constants.City)).val(billingCity);
                $(constants.ShippingAddressFields.concat(constants.State)).val(billingState);
                $(constants.ShippingAddressFields.concat(constants.Zip)).val(billingZip);
                $(constants.ShippingAddressFields.concat(constants.Country)).val(billingCountry);
                $(constants.ShippingAddressFields.concat(constants.Phone)).val(billingPhone);
            }
        });

        $('#QuickRegisterUserButton').on('click', function(e) {

            e.preventDefault();

            var self = this;

            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                cache: false,
                url: '/Admin/CreateAUser',
                dataType: constants.HtmlDataType,
                data: null,
                beforeSend: function() {
                    // this is where we append a loading image
                    $(self).after('<span id="loadSpinner2">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
                }
            }).done(function(data) {
                var response = JSON.parse(data);
                if (response.Result === 'Success') {
                    $('#OperationMessage').html('&nbsp;The new user has been added with username: <strong>' + response.email + '</strong> and password: <strong>' + response.password + '</strong>');
                } else if (response.Result === 'Fail') {
                    $('#OperationMessage').html('<span class="label label-important"><strong>&nbsp;&nbsp;There was an error at the server. The new user has not been created.</strong></span>');
                }
                $('#loadSpinner2').remove();
            });
        });
    };

    ns.initializePasswordResetConfirmFields = function() {

        $('#PasswordResetVerifyStatus').text('');

        if (!MN.passwordConfirmResetInitialized) {
            $('#PasswordConfirmResetForm').submit(function(e) {
                e.preventDefault();
            });

            $('#PasswordResetVerifyButton').on('click', function() {

                var model = {
                    email: $.trim($('#Email').val()),
                    password: $.trim($('#Password').val()),
                    key: '',
                    verificationKey: $.trim($('#verificationKey').val())
                };

                $.ajax({
                    type: 'POST',
                    contentType: constants.JsonContentType,
                    cache: false,
                    url: '/Admin/FirePasswordResetEvent',
                    dataType: constants.JsonDataType,
                    data: JSON.stringify(model),
                    beforeSend: function() {
                        // this is where we append a loading image
                        $('#WaitIndicator').show();
                    }
                }).done(function(data) {
                    //var response = JSON.parse(data);
                    if (data.ChangePasswordSucceeded) {
                        $('#PasswordResetVerifyStatus').text('   Operation succeeded.');
                    }

                }).always(function(data) {
                    $('#WaitIndicator').hide();
                });
            });

            MN.passwordConfirmResetInitialized = true;
        }
    };

    ns.getBatchPwdResetHtmlButtonClicked = function(e) {

        e.preventDefault();

        var url = '/Admin/BatchPasswordReset';

        $.ajax({
            type: 'GET',
            cache: true,
            url: url,
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                MN.accountsBrandLink.after('<span id="loadSpinner1">&nbsp;<i class="icon-spinner icon-spin"></i></span>');
            }
        }).done(function (data, textStatus, jqXHR) {
            MN.inputFormFields.html(data);
            $('#loadSpinner1').remove();

            $('#ResetBatchButton').on('click', MN.ResetBatchButtonClicked);

        }).fail(commonFuncs.failCallBack);
    }

    ns.ResetBatchButtonClicked = function(e) {

        e.preventDefault();

        var batchPasswordResetForm = $('#BatchPasswordResetForm');
        var url = batchPasswordResetForm.attr('action');
        var token = batchPasswordResetForm.find('input[name=__RequestVerificationToken]').val();

        var headers = {};
        headers['__RequestVerificationToken'] = token;

        var payload = {
            UserEmails: $('#usersTextArea').val()
        };

        $.ajax({
            type: 'POST',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.JsonDataType,
            data: JSON.stringify(payload),
            headers: headers,
            beforeSend: function () {

            }
        }).done(function (data, textStatus, jqXHR) {
            if (data.Result === 'Success') {

            } else {

            }
        });


    };

})(MN);
