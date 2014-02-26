var constants = function(){

    var billingAddressFields = "#RegisterFields_BillingAddress",
        city = "_City",
        country = "_Country",
        confirmDeleteShippingAddressdialog = "#ConfirmDeleteShippingAddressdialog",
        formPostContentType = "application/x-www-form-urlencoded",
        phone = "_Phone",
        shippingAddressFields = "#RegisterFields_ShippingAddress",
        shippingAddressContainer = "#ShippingAddressContainer",
        state = "_State",
        addShippingAddressLink = "#AddShippingAddressLink",
        hideAddShippingAddressLink = "#HideAddShippingAddressLink",
        streetAddress = "_StreetAddress",
        streetAddress2 = "_StreetAddress2",
        zip = "_Zip";


    return {
        AddShippingAddressLink : addShippingAddressLink,
        BillingAddressFields: billingAddressFields,
        City: city,
        ConfirmDeleteShippingAddressdialog: confirmDeleteShippingAddressdialog,
        Country: country,
        FormPostContentType: formPostContentType,
        HideAddShippingAddressLink: hideAddShippingAddressLink,
        Phone: phone,
        ShippingAddressContainer : shippingAddressContainer,
        ShippingAddressFields: shippingAddressFields,
        State : state,
        StreetAddress: streetAddress,
        StreetAddress2: streetAddress2,
        Zip : zip
    };

}();