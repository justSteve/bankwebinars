$(function () {
    var shippingAddressContainer = $(constants.ShippingAddressContainer);
    var addShippingAddressLink = $(constants.AddShippingAddressLink);
    var hideAddShippingAddressLink = $(constants.HideAddShippingAddressLink);       

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
        
    addShippingAddressLink.hide()
    
    hideAddShippingAddressLink.on('click', function (e) {
        e.preventDefault();

        $(constants.ConfirmDeleteShippingAddressdialog).dialog("open");

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

});