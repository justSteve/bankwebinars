/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />


module OrderRegistration {

    declare var $;

    //  todo: duplicate in Registration module
    export class Constants {
        static BillingAddressFields: string = '#RegisterFields_BillingAddress';
        static City: string = '_City';
        static Country: string = '_Country';
        static ConfirmDeleteShippingAddressdialog: string = '#ConfirmDeleteShippingAddressdialog';
        static FormPostContentType: string = 'application/x-www-form-urlencoded';
        static JsonContentType: string = 'application/json; charset=utf-8';
        static JsonDataType: string = 'json';
        static HtmlDataType: string = 'html';
        static Phone: string = '_Phone';
        static ShippingAddressFields: string = '#RegisterFields_ShippingAddress';
        static ShippingAddressContainer: string = '#ShippingAddressContainer';
        static State: string = '_State';
        static AddShippingAddressLink: string = '#AddShippingAddressLink';
        static HideAddShippingAddressLink: string = '#HideAddShippingAddressLink';
        static StreetAddress: string = '_StreetAddress';
        static StreetAddress2: string = '_StreetAddress2';
        static TypeofAddressBilling: string = 'Billing';
        static TypeofAddressShipping: string = 'Shipping';
        static Zip: string = '_Zip';
    };

    export class StateManager {

        private addressVerified: boolean;
        private discount: string;
        private wait4Emails: string;
        private notificationsTesting: boolean;
        private orderId: number;
        private orderRowID: number;
        private checkoutInProcess: boolean;
        private idWebinar: number;
        private isUserLoggedIn: boolean;
        private userHasDiscount: boolean;
        private confirmOrderForm: JQuery;
        private cancelOrderForm: JQuery;
        private shippingAddressRequired: boolean;

        constructor() { }

        getAddressVerified(): boolean {
            return this.addressVerified;
        }

        getCancelOrderForm(): JQuery {
            return this.cancelOrderForm;
        }

        getCheckoutInProcess(): boolean {
            return this.checkoutInProcess;
        }

        getConfirmOrderForm(): JQuery {
            return this.confirmOrderForm;
        }

        getIsUserLoggedIn(): boolean {
            return this.isUserLoggedIn;
        }
        getUserHasDiscount(): boolean {
            return this.userHasDiscount;
        }

        getNotificationsTesting(): boolean {
            return this.notificationsTesting;
        }

        getOrderId(): number {
            return this.orderId;
        }

        getOrderRowId(): number {
            return this.orderRowID;
        }

        getShippingAddressRequired(): boolean {
            return this.shippingAddressRequired;
        }

        getWebinarId(): number {
            return this.idWebinar;
        }

        setAddressVerified(verified: boolean): void {
            this.addressVerified = verified;
        }


        setCancelOrderForm(form: JQuery): void {
            this.cancelOrderForm = form;
        }

        setCheckoutInProcess(checkoutInProcess: boolean): void {
            this.checkoutInProcess = checkoutInProcess;
        }

        setConfirmOrderForm(form: JQuery): void {
            this.confirmOrderForm = form;
        }

        setIsUserLoggedIn(val: boolean): void {
            this.isUserLoggedIn = val;
        }

        setUserHasDiscount(val: boolean): void {
            this.userHasDiscount = val;
        }

        setNotificationsTesting(val: boolean): void {
            this.notificationsTesting = val;
        }

        setOrderId(num: number): void {
            this.orderId = num;
        }

        setOrderRowId(num: number): void {
            this.orderRowID = num;
        }

        setShippingAddressRequired(val: boolean): void {
            this.shippingAddressRequired = val;
        }

        setWebinarId(id: number): void {
            this.idWebinar = id;
        }

        BuildPreRegPrice(oEvent : JQueryEventObject, orderRowId : number): void {
            //permits a 'preReg' pricing scheme to handle
            //computation of discounts and addl locations prior
            //to stepping to confirmation.
            //alert("hit");
            var $form = $("#BuildPrice");

            oEvent.preventDefault();
            $.ajax({
                url: '/cart/CheckoutDisplayRowPrice/' + orderRowId,
                type: "POST",
                //data: $form.serialize(),
                success: function(data) {
                    $("#regTypeID_" + data.orderRowID).prop('checked', true);
                },
                error: function() {

                },
                complete: function() {
                }
            });
        }

        CheckIfAddLocShouldHide(optionID: string): void {
            //don't show AdditionalEmails when RegType
            // can't support them. (ex: recorded only)

            var self = this;

            $.ajax({
                url: "/cart/CheckIfAddLocShouldHide?optionID=" + optionID,
                type: "GET",
                cache: false,
                dataType: Constants.JsonDataType,

                beforeSend: function() {
                    //console.log('beforeSend CheckIfAddLocShouldHide');
                    // no loading image needed
                }
            }).done(function(data) {
                //console.log('done CheckIfAddLocShouldHide');
                if (data.shouldShow === 'Yes') {
                    //console.log('show CheckIfAddLocShouldHide');
                    $('#displayAddLoc').show('slow');
                } else if (data.shouldShow === 'No') {
                    //console.log('hide  CheckIfAddLocShouldHide');
                    $('#displayAddLoc').hide(1000);
                    $('#collectAdditionalLocations').empty();
                }

                if (data.shippingDetailsRqrd === 'Yes') {
                    self.shippingAddressRequired = true;
                } else {
                    self.shippingAddressRequired = false;
                }
            }).fail(function(data) {
                //console.log('CheckIfAddLocShouldHide failed!!! ');
            });
        }

        PlaceOrder(): void {
            $("#Step2_BillMeFormReferred").val($("#tbReferred").val());
            this.confirmOrderForm.submit();
        }

        SetCartState(cartType: string): void {

            if (cartType === 'affiliate') {
                $("#findUserTab").hide();
                $("#confirmationTabForAffiliate").hide();
            } else if (cartType === 'admin') {

            } else {
                $("#signUpTab").hide();
                $("#contactInfoTab").hide();
                $("#confirmationTab").hide();
            }

            //$("#connectionsCount").val(0);
            $('#collectAdditionalLocation').html('');
            
            $('#AddToCart').attr({ disabled: false, value: 'Sign Up' });
            $('#AddToCart1').attr({ disabled: false, value: 'Sign Up' });            
        }

    };
}

//function setPath() {
//    var indexOfHome = location.href.indexOf('Account');
//    var path = '';

//    if (indexOfHome > -1)
//        path = location.href.substr(0, location.href.indexOf('Account') - 1);
//    else
//        path = location.href;

//    //  IE is a rubbish browser!
//    if (path === '')
//        path = $(location).attr('href');
//    return path;
//}

//function isValidEmailAddress(emailAddress : string) {
//    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
//    return pattern.test(emailAddress);
//};