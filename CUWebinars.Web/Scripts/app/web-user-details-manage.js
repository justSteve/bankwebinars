$(function () {
    var shippingAddressContainer = $(constants.ShippingAddressContainer);
    var addShippingAddressLink = $(constants.AddShippingAddressLink);
    var hideAddShippingAddressLink = $(constants.HideAddShippingAddressLink);
    var utilities = new Common.Utilities();

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

    addShippingAddressLink.hide();
    
    hideAddShippingAddressLink.on('click', function (e) {
        e.preventDefault();

        $(constants.ConfirmDeleteShippingAddressdialog).dialog("open");

    });

    $('#editUserForm').on('submit', function (e) {

        e.preventDefault();

        if (!hideAddShippingAddressLink.is(':visible')) {
            var prefixBilling = '#EditFields_BillingAddress';

            var billingStreetAddress = $(prefixBilling.concat(constants.StreetAddress)).val();
            var billingStreetAddress2 = $(prefixBilling.concat(constants.StreetAddress2)).val();
            var billingCity = $(prefixBilling.concat(constants.City)).val();
            var billingState = $(prefixBilling.concat(constants.State)).val();
            var billingZip = $(prefixBilling.concat(constants.Zip)).val();
            var billingCountry = $(prefixBilling.concat(constants.Country)).val();
            var billingPhone = $(prefixBilling.concat(constants.Phone)).val();

            var prefixShipping = '#EditFields_ShippingAddress';

            $(prefixShipping.concat(constants.StreetAddress)).val(billingStreetAddress);
            $(prefixShipping.concat(constants.StreetAddress2)).val(billingStreetAddress2);
            $(prefixShipping.concat(constants.City)).val(billingCity);
            $(prefixShipping.concat(constants.State)).val(billingState);
            $(prefixShipping.concat(constants.Zip)).val(billingZip);
            $(prefixShipping.concat(constants.Country)).val(billingCountry);
            $(prefixShipping.concat(constants.Phone)).val(billingPhone);
        }

        var payload = $(this).serialize();
        var url = $(this).attr('action');

        if ($(this).valid()) {

            $.ajax({
                type: 'POST',
                contentType: constants.FormPostContentType,
                cache: false,
                url: url,
                dataType: constants.JsonDataType,
                data: payload,
                beforeSend: function () {
                    $('#postalSpinner').remove();
                    submitButton.after('<span id="postalSpinner">&nbsp;<span class="label label-info"><i class="icon-spinner icon-spin"></i>&nbsp;Updating details...</span></span>');
                }
            }).done(function(data) {
                if (data.Result === 'Success') {
                    
                    // returnUrl is set in the Razor View
                    if (returnUrl) {
                        $('#postalSpinner').html('&nbsp;<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Transferring you back now ...</span>');
                        utilities.goToUrl(returnUrl);
                    } else {
                        $('#postalSpinner').html('&nbsp;<span class="label label-success">&nbsp;<i class="icon icon-thumbs-up"></i>&nbsp;Operation succeeded!</span>');
                    }
                }
                
            });
        }

    });

    // jQueryUi dialog for confirmation to delete Shipping Details
    $(constants.ConfirmDeleteShippingAddressdialog).dialog({
        autoOpen: false,
        resizable: false,
        height: 260,
        modal: true,
        show: 'fade',
        hide: 'fade',
        buttons: [
            {
                text: "Proceed",
                click: function () {

                    shippingAddressContainer.slideUp(800, function () {
                        hideAddShippingAddressLink.fadeOut(400, function () {
                            addShippingAddressLink.fadeIn(400);
                        });
                        
                    });

                    $(this).dialog("close");
                    return true;
                }
            },
            {
                text: "Cancel",
                click: function () {
                    $(this).dialog("close");
                    return false;
                }
            }]
        }
    );

    var submitButton = $('input[type="submit"]');
});