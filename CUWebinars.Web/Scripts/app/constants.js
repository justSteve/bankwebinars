var constants = function(){

    var billingAddressFields = '#RegisterFields_BillingAddress',
        city = '_City',
        country = '_Country',
        confirmDeleteShippingAddressdialog = '#ConfirmDeleteShippingAddressdialog',
        formPostContentType = 'application/x-www-form-urlencoded',
        jsonContentType = 'application/json; charset=utf-8',
        jsonDataType = 'json',
        htmlDataType = 'html',
        phone = '_Phone',
        shippingAddressFields = '#RegisterFields_ShippingAddress',
        shippingAddressContainer = '#ShippingAddressContainer',
        state = '_State',
        addShippingAddressLink = '#AddShippingAddressLink',
        hideAddShippingAddressLink = '#HideAddShippingAddressLink',
        streetAddress = '_StreetAddress',
        streetAddress2 = '_StreetAddress2',
        typeofAddressBilling = 'Billing',
        typeofAddressShipping = 'Shipping',
        zip = '_Zip';


    return {
        AddShippingAddressLink : addShippingAddressLink,
        BillingAddressFields: billingAddressFields,
        City: city,
        ConfirmDeleteShippingAddressdialog: confirmDeleteShippingAddressdialog,
        Country: country,
        FormPostContentType: formPostContentType,
        JsonContentType: jsonContentType,
        JsonDataType: jsonDataType,
        HideAddShippingAddressLink: hideAddShippingAddressLink,
        HtmlDataType: htmlDataType,
        Phone: phone,
        ShippingAddressContainer : shippingAddressContainer,
        ShippingAddressFields: shippingAddressFields,
        State : state,
        StreetAddress: streetAddress,
        StreetAddress2: streetAddress2,
        TypeofAddressBilling: typeofAddressBilling,
        TypeofAddressShipping: typeofAddressShipping,
        Zip : zip
    };

}();

var commonFuncs = function() {

    var fail = function(jqXHR, textStatus, errorThrown) {

        if (jqXHR.statusCode().status === 403) {
            alert('Session expired. Please login again to continue.');
            window.location.href = '/Account/Login';
        } else if (jqXHR.statusCode().status === 0 && errorThrown === '' && textStatus === 'error') {
            return; // do nothing
        } else {
            //Rollbar.error( jqXHR.statusCode().status + ' nError: ' + jqXHR.statusCode().statusText);
            alert('An error occurred: ')+ textStatus + errorThrown; 
        };
    };

    return {
        failCallBack: fail
    }
}();