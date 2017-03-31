//  Rename this file 'additional-locations' if we decide to be rid of additional-locations.js
var utilities = new Common.Utilities();
var addLocationsButton,
    AdditionalLocationSubmitButton,
    newLocationsContainer;

var additionalLocationEmailWrapper,
    breakSuffix,
    deleteItem,
    locationsSpanPrefix,
    numberOfAdditionalLocations;
deleteItem = function (event) {
    numberOfAdditionalLocations--;
    var trashClicked = event.currentTarget.id;
    var idx = trashClicked.substring(0, 1);
    var spanToRemove = locationsSpanPrefix + idx;

    $('#' + spanToRemove).hide(500, function () {
        $(this).remove();
    });

    $('#' + idx + breakSuffix).hide(500, function () {
        $(this).remove();
    });

    if (numberOfAdditionalLocations < 1) {
        $('#sumbitAdditionalLocationsButton').hide(300, function () {
            $(this).remove();
        });
    }
};

$(function () {
    primeDomVariables();
    wireUpHandlers();

    $("#additionalLocationsModalDialog").on('shown', function () {

        $("#AddInputsButton").click();
    });

});

function wireUpHandlers() {
    
    locationsSpanPrefix = 'LocationSpan-',
    breakSuffix = '-break';

    var modalFormOptions = {
        keyboard: true,
        backdrop: 'static',
        show: true
    };


    addLocationsButton.on('click', function (e) {
        
        e.preventDefault();

        $(this).append('<i id="loadModalSpinner" class="icon-spinner icon-spin"></i>');
        
        $('#additionalLocationsModalDialog > div.modal-body').load('/Cart/GetAdditionalLocationByOrderId/'
            + webinarId +'/0', function () {
            //+ webinarId +'/0' + (idUser || 0).toString(), function () {
            wireUpHandlersForModal();
            $('#AddInputsButton').focus();
            $('#additionalLocationsModalDialog').modal(modalFormOptions);
        });

    });

    $('#additionalLocationsModalDialog').on('shown', function () {
        $('#loadModalSpinner').remove();
    });

};

function primeDomVariables() {
    addLocationsButton = $('#AddLocationsButton');
    newLocationsContainer = $('#NewLocationsContainer'); // commented out in razor
}

function wireUpHandlersForModal() {
    
    var locationsCloned, locationsBakForCancel;
    var collectAdditionalLocations = $('#collectAdditionalLocations');

    //console.log(collectAdditionalLocations);
    //console.log(additionalLocationEmailWrapper.find('input[type="email"]:last'));
    var locations = collectAdditionalLocations.children();

    if (locations.length > 0) {
        locationsCloned = locations.clone();
        locationsBakForCancel = locations.clone();
    }

    collectAdditionalLocations.empty();

    additionalLocationEmailWrapper = $('#AdditionalLocationEmailWrapper');
    if (locationsCloned)
        additionalLocationEmailWrapper.append(locationsCloned);

    $('#AddInputsButton').on('click', function (e) {
        
        e.preventDefault();
        $(".AddLocCaption")
            .html("<b>Note:</b> Only use this entry for branches or other remote locations that you wish to attend. We will collect the primary (billing) email on the next screen.");

        if (numberOfAdditionalLocations === 0) {
            
            $('#AdditionalLocationEmailWrapper').after($('<button>',
            {
                id: 'sumbitAdditionalLocationsButton',
                text: 'done adding?',
                'class': 'btn btn-primary'
            }));

            $('#sumbitAdditionalLocationsButton').on('click', function () {

                collectAdditionalLocations.empty();
                var allValid = true;

                $.each(additionalLocationEmailWrapper.children(), function (i, val) {
                    if (typeof this.childNodes[0] !== "undefined") {
                        console.log("Index #" + i + ": " + this.childNodes[0].value);
                        if (utilities.isValidEmailAddress(this.childNodes[0].value)) {

                            $('#AddInputsButton').show();


                        } else {
                            allValid = false;
                            //additionalLocationEmailWrapper.children().remove();
                            alert("Non-valid email detected. Try again?");
                        }


                    }
                });
                if (allValid) {
                    collectAdditionalLocations.append(additionalLocationEmailWrapper.children());
                    
                    $('#additionalLocationsModalDialog').modal('hide');
                    $(this).remove();
                }

            });
        }

        var newId;

        if (numberOfAdditionalLocations === 0) {
            newId = 0;
        } else {
            // first get the last previous email input
            var lastInput = additionalLocationEmailWrapper.find('input[type="email"]:last');
            // get its id
            
            var lastInputId = lastInput.attr('id');
            var id = parseInt(lastInputId.charAt(lastInputId.length - 1));
            newId = id + 1;
        }
        additionalLocationEmailWrapper.append('<span id="' + locationsSpanPrefix + newId + '"><input id="AdditionalLocationEmail_' + newId + '" name="AdditionalLocations[' + newId + '].Email" type="email" placeholder="Enter email address" />&nbsp;<i class="icon-trash icon-white" style="cursor: pointer" id="' + newId + '-AdditionLocationEmail-delete"></i></span> <br id="' + newId + breakSuffix + '">');
        additionalLocationEmailWrapper.find('i#' + newId + '-AdditionLocationEmail-delete').on('click', deleteItem);
        $('#AdditionalLocationEmail_' + newId).focus();
        numberOfAdditionalLocations++;

    });

    numberOfAdditionalLocations = $('#AdditionalLocationEmailWrapper input[type="email"]').length;

    if (numberOfAdditionalLocations < 1) {
        $('#sumbitAdditionalLocationsButton').off('click');

    } else {

        $('#AdditionalLocationEmailWrapper').after($('<button>',
        {
            id: 'sumbitAdditionalLocationsButton',
            text: 'done adding?',
            'class': 'btn btn-primary'
        }));

        $('#sumbitAdditionalLocationsButton').on('click', function () {

            collectAdditionalLocations.empty();
            collectAdditionalLocations.append(additionalLocationEmailWrapper.children());

            $('#additionalLocationsModalDialog').modal('hide');
            $(this).remove();

            //Rollbar.info("aal-#3", { SubmitClicked: numberOfAdditionalLocations } );
        });
        var trashCans = additionalLocationEmailWrapper.find('i');

        $.each(trashCans, function (idx, i) {
            $(i).on('click', deleteItem);
        });
    }

    $('#closeButton, #additionalLocationsModalDialog > div > div.modal-header > button').on('click', function (e) {
        if (locationsBakForCancel) {

            $.each(locationsBakForCancel.find('i'), function (idx, i) {
                $(i).on('click', deleteItem);
            });
            collectAdditionalLocations.append(locationsBakForCancel);
        }
        //Rollbar.info({ "aal-#4": "CloseClicked" });
    });
}
