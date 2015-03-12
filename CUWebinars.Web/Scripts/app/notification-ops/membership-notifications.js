var passwordResetInitialized = false;
var passwordConfirmResetInitialized = false;
var jsonDataForImportedOrder;

$(function () {

    $('#WaitIndicator').hide();

    $('#ImpersonateUserButton').on('click', function () {

        var url = '/Admin/LogInAsUser';

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: url,
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (result) {
            $('#InputFormFields').html(result);

        }).fail(function (jqXHR, textStatus, errorThrown) {
            
            // errorThrown has error message, or "timeout" in case of timeout.
            
            var err = new Error('LogInAsUser AJAX error: ' + errorThrown);
            //NREUM.noticeError(err);
            var i = 0;

        }).always(function () {
            $('#WaitIndicator').hide();
        });
    });

    $('#CreateUserButton').on('click', function () {

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/CreateAUser',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (result) {
            $('#InputFormFields').html(result);
            InitializeCreateUserFields();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // Request failed. Show error message to user. 
            // errorThrown has error message, or "timeout" in case of timeout.
            
            var err = new Error('CreateUserButton AJAX error: ' + errorThrown);
            //NREUM.noticeError(err);
            var i = 0;

        }).always(function () {
            $('#WaitIndicator').hide();
        });
    });

    $('#ResetPasswordButton').on('click', function () {

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/PasswordResetOperation',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (result) {

            $('#InputFormFields').html(result);
            InitializePasswordResetFields();
        }).always(function () {
            $('#WaitIndicator').hide();
        });
    });

    $('#ConfirmPasswordButton').on('click', function () {
        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/GetPasswordResetConfirmFields',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (result) {

            $('#InputFormFields').html(result);
            InitializePasswordResetConfirmFields();
        }).always(function () {
            $('#WaitIndicator').hide();
        });
    });

    $('#GetImportOrderFieldsButton').on('click', function () {

        if (jsonDataForImportedOrder) {
            $('#OrderSucceeded').remove();
            $('#InputFormFields').append('<button id="ImportOrderButton" class =" btn btn-success">Import Order</button>');
            $('#InputFormFields').append('<textarea id="JsonPayloadTextArea" rows="40" cols="100" style="width:100%;margin-top:10px"></textarea>');
            $('#JsonPayloadTextArea').val(jsonDataForImportedOrder);
            addImportOrderButtonClick();
        } else {
            $.ajax({
                type: 'GET',
                contentType: constants.JsonContentType,
                cache: false,
                url: '/Admin/GetJsonTextArea',
                dataType: constants.HtmlDataType,
                data: null,
                beforeSend: function () {
                    // this is where we append a loading image
                    if ($('#ImportOrderButton').length > 0)
                        $('#ImportOrderButton').off('click');
                    jsonPayload = $('#JsonPayload').text();
                    $('#WaitIndicator').show();
                }
            }).done(function (result) {

                $('#InputFormFields').html(result);

                addImportOrderButtonClick();

            }).always(function () {
                $('#WaitIndicator').hide();
            });
        }
    });

    $('#GetImportOrderFieldsFromCsvButton').on('click', function () {

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
                $('#WaitIndicator').show();
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
                    // this is where we append a loading image
                    $('#WaitIndicator').show();
                }
            }).done(function (result) {

                var resultAsJson = JSON.parse(result);

                $('#InputFormFields').html('<span id="OrderSucceeded" class="label label-success">' + resultAsJson.Result + '</span>');

            }).fail(function (result) {
                var resultAsJson = JSON.parse(result.responseText);
                $('#InputFormFields').html('<span id="OrderSucceeded" class="label label-important">' + resultAsJson.Result + '</span>');
            }).always(function (result) {
                $('#WaitIndicator').hide();
            });

            //addImportOrderButtonClick();

        });
    });

    $('#GetManualResetPasswordFieldsButton').on('click', function () {

        $.ajax({
            type: 'GET',
            contentType: constants.JsonContentType,
            cache: false,
            url: '/Admin/ManualPasswordReset',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                if ($('#ImportOrderButton').length > 0)
                    $('#ImportOrderButton').off('click');
                $('#WaitIndicator').show();
            }
        }).done(function (result) {

            $('#InputFormFields').html(result);

            $('#ManualResetPasswordButton').on('click', function (args) {

                args.preventDefault();

                $.ajax({
                    type: 'POST',
                    contentType: constants.FormPostContentType,
                    cache: false,
                    url: '/Admin/ManualPasswordReset',
                    dataType: constants.JsonDataType,
                    data: $('#ManualResetPasswordForm').serialize(),
                    beforeSend: function () {
                        // this is where we append a loading image
                        $('#WaitIndicator').show();
                    }
                }).done(function (response) {

                    if (response.Result === 'Success') {
                        $('#OperationMessage').html('<span class="label label-success">&nbsp;&nbsp;User password has been changed.</span>');
                    } else if (response.Result === 'Fail') {
                        $('#OperationMessage').html('<span class="label label-important"><strong>&nbsp;&nbsp;There was an error at the server. The new user has not been created.</strong></span>');
                    }
                }).always(function (data) {
                    $('#WaitIndicator').hide();
                });
            });

        }).always(function (result) {
            $('#WaitIndicator').hide();
        });
    });

});

function addImportOrderButtonClick() {
    $('#ImportOrderButton').on('click', function () {

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
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (result) {

            var id = parseInt(result.Result, 10); // this is base 10 (2nd param)

            if (id > 0) {
                $('#InputFormFields').html('<span id="OrderSucceeded" class="label label-success">Success! Order Id: ' + id + '</span>');
            } else {
                $('#InputFormFields').html('<span id="OrderFailed" class="label label-important">There was an error at the server and the order was not imported.</span>');
            }
        }).fail(function (result) {

            var id = parseInt(result.Result, 10); // this is base 10 (2nd param)

            if (id < 1) {
                $('#InputFormFields').html('<span id="OrderFailed" class="label label-important">There was an error at the server and the order was not imported.</span>');
            }
        }).always(function () {
            $('#WaitIndicator').hide();
        });
    });
};

function InitializeCreateUserFields() {
    var shippingAddressContainer = $(constants.ShippingAddressContainer);
    var addShippingAddressLink = $(constants.AddShippingAddressLink);
    var hideAddShippingAddressLink = $(constants.HideAddShippingAddressLink);

    shippingAddressContainer.hide();
    hideAddShippingAddressLink.hide();

    addShippingAddressLink.on('click', function (e) {
        e.preventDefault();

        if (!hideAddShippingAddressLink.is(':visible')) {

            shippingAddressContainer.slideDown(800, function () {
                addShippingAddressLink.fadeOut(400, function () {
                    hideAddShippingAddressLink.fadeIn(400);
                });
            });
        }
    });

    hideAddShippingAddressLink.on('click', function (e) {
        e.preventDefault();

        shippingAddressContainer.slideUp(800, function () {
            hideAddShippingAddressLink.fadeOut(400, function () {
                addShippingAddressLink.fadeIn(400);
            });
        });
    });

    $('form').submit(function () {

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

    $('#QuickRegisterUserButton').on('click', function () {

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/Admin/CreateAUser',
            dataType: constants.HtmlDataType,
            data: null,
            beforeSend: function () {
                // this is where we append a loading image
                $('#WaitIndicator').show();
            }
        }).done(function (data) {
            var response = JSON.parse(data);
            if (response.Result === 'Success') {
                $('#OperationMessage').html('&nbsp;The new user has been added with username: <strong>' + response.email + '</strong> and password: <strong>' + response.password + '</strong>');
            } else if (response.Result === 'Fail') {
                $('#OperationMessage').html('<span class="label label-important"><strong>&nbsp;&nbsp;There was an error at the server. The new user has not been created.</strong></span>');
            }
        }).always(function (data) {
            $('#WaitIndicator').hide();
        });
    });
}

function InitializePasswordResetFields() {

    $('#PasswordResetStatus').text('');

    if (!passwordResetInitialized) {
        $('#ResetPasswordForm').submit(function (e) {
            e.preventDefault();
        });

        $('#NormalResetPasswordButton').on('click', function () {

            var token = $('#ResetPasswordForm').find('input[name=__RequestVerificationToken]').val();
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
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#WaitIndicator').show();
                }
            }).done(function (data) {

                if (data.Status === 'Success') {
                    $('#PasswordResetStatus').text('   Operation Succeeded');
                }

            }).always(function (data) {
                $('#WaitIndicator').hide();
            });
        });

        passwordResetInitialized = true;
    }
}

function InitializePasswordResetConfirmFields() {

    $('#PasswordResetVerifyStatus').text('');

    if (!passwordConfirmResetInitialized) {
        $('#PasswordConfirmResetForm').submit(function (e) {
            e.preventDefault();
        });

        $('#PasswordResetVerifyButton').on('click', function () {

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
                beforeSend: function () {
                    // this is where we append a loading image
                    $('#WaitIndicator').show();
                }
            }).done(function (data) {
                //var response = JSON.parse(data);
                if (data.ChangePasswordSucceeded) {
                    $('#PasswordResetVerifyStatus').text('   Operation succeeded.');
                }

            }).always(function (data) {
                $('#WaitIndicator').hide();
            });
        });

        passwordConfirmResetInitialized = true;
    }
}