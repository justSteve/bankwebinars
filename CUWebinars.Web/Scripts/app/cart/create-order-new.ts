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

    export class PageObject {
        private discount: string;
        private wait4Emails: string;
        private orderRowID: number;
        private checkoutInProcess: boolean;
        private isUserLogged: boolean;
        private whichStep: string;

        getCheckoutInProcess(): boolean {
            return this.checkoutInProcess;
        }

        getIsUserLogged(): boolean {
            return this.isUserLogged;
        }

        getOrderRowId(): number {
            return this.orderRowID;
        }

        getWhichStep(): string {
            return this.whichStep;
        }

        setCheckoutInProcess(val : boolean): void {
            this.checkoutInProcess = val;
        }

        setIsUserLogged(val : boolean): void {
            this.isUserLogged = val;
        }

        setOrderRowId(num : number): void {
            this.orderRowID = num;
        }

        setWhichStep(step : string): void {
            this.whichStep = step;
        }

    };

    export class StateManager {

        private discount: string;
        private wait4Emails: string;
        private orderRowID: number;
        private checkoutInProcess: boolean;
        private isUserLogged: boolean;
        private whichStep: string;

        constructor() { }

        getCheckoutInProcess(): boolean {
            return this.checkoutInProcess;
        }

        getIsUserLogged(): boolean {
            return this.isUserLogged;
        }

        getOrderRowId(): number {
            return this.orderRowID;
        }

        getWhichStep(): string {
            return this.whichStep;
        }

        setCheckoutInProcess(val: boolean): void {
            this.checkoutInProcess = val;
        }

        setIsUserLogged(val: boolean): void {
            this.isUserLogged = val;
        }

        setOrderRowId(num: number): void {
            this.orderRowID = num;
        }

        setWhichStep(step: string): void {
            this.whichStep = step;
        }

        BuildPreRegPrice(oEvent : JQueryEventObject, orderRowId : number): void {
            //permits a 'preReg' pricing scheme to handle
            //computation of discounts and addl locations prior
            //to stepping to confirmation.

            var $form = $("#BuildPrice");

            oEvent.preventDefault();
            $("#ProgressDialogBS").modal('show');
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
                    $("#ProgressDialogBS").modal('hide');
                }
            });
        }

        CheckIfAddLocShouldHide(optionID: string): void {
            //don't show AdditionalEmails when RegType
            // can't support them. (ex: recorded only)

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
                    $("#displayAddLoc").show('slow');
                } else if (data.shouldShow === 'No') {
                    //console.log('hide  CheckIfAddLocShouldHide');
                    $("#displayAddLoc").hide(1000);
                }
            }).fail(function(data) {
                //console.log('CheckIfAddLocShouldHide failed!!! ');
            });
        }

        SetCartState(): void {
            switch (this.whichStep) {
            case "Step0":
                //console.log("Step0");
                $("#connectionsCount").val(0);
                $('#collectAdditionalLocation').html('');
                $("#confirmationTab").hide();
                $("#signUpTab").hide();
                //$("#contactInfoTab").hide();
                $('#AddToCart').attr({ disabled: false, value: 'SignUp' });
                $('#AddToCart1').attr({ disabled: false, value: 'SignUp' });

                break;

            case "Step1":
                //console.log("Step1");

                $("#confirmation").hide();
                $("#signUp").hide();
                $("#contactInfo").show();
                break;

            case "Step2":
                $("#signUpTab").hide();
                $("#signUp").hide();

                $("#confirmationTab").tab('show');
                $("#confirmationTab").addClass('active');
                $("#confirmation").addClass('active');
                $("#confirmation").show();
                //$.get("/cart/checkoutConfirm/" + orderRowID)
                //    .success(function (dataConfirm) {
                //        $('#confirmation').html(dataConfirm);
                //    })
                //.done($("#confirmation").show())
                $("#confirmationTab").trigger('click');

                //console.log("Step2");

                break;

            case "Registered":
                //console.log("state is registered");
                $("#confirmationTab a").text('Order Summary');
                $("#signUpTab a").text('Connection Info');

                var self = this;

                $.get("/cart/checkoutConfirm/" + self.getOrderRowId())
                    .success(function(dataConfirm) {
                        $('#confirmation').html(dataConfirm);
                    });
                $('#AddToCart').hide();
                $('#AddToCart1').hide();
                break;
            }
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