// MANAGEWEBINARWEBINAR namespace
if (MANAGEWEBINAR === null || typeof MANAGEWEBINAR === 'undefined')
    var MANAGEWEBINAR = {};

var MW = MANAGEWEBINAR; // create shortcus to namespace

// jQuery doc.ready function
$(function () {

    MW.webinarSearchButton = $('#webinarSearchButton');
    MW.webinarSearchInput = $('#webinarSearchInput');
    MW.webinarCloneInput = $('#webinarCloneInput');
    MW.webinarCreateButton = $('#webinarCreateButton');
    MW.webinarCloneButton = $('#webinarCloneButton');
    MW.webinarContent = $('#webinarContent');
    MW.webinarSearchButton.on('click', MW.searchWebinar);
    MW.webinarCreateButton.on('click', MW.createWebinar);
    MW.webinarCloneButton.on('click', MW.cloneWebinar);

    MW.primeDomVariables(MW.edit);
    MW.wireUpHandlers(MW.edit);

    MW.ceuTextArea.cleditor(MW.optionsForEditors);
    MW.webinarDescriptionTextArea.cleditor(MW.optionsForEditors);
    MW.webinarLongDescriptionTextArea.cleditor(MW.optionsForEditors);

    var ceuEditor = MW.ceuTextArea.cleditor()[0];
    ceuEditor.disable(false);
    var webinarDescriptionEditor = MW.webinarDescriptionTextArea.cleditor()[0];
    webinarDescriptionEditor.disable(false);
    var webinarLongDescriptionEditor = MW.webinarLongDescriptionTextArea.cleditor()[0];
    webinarLongDescriptionEditor.disable(false);


    MW.webinarSearchInput.focus(); 
});

// self-invoking function.
(function (ns) {

    ns.primeDomVariables = function (operation) {

        if (operation === ns.edit || operation === ns.create || operation === ns.clone) {
            ns.statusSelectList = $('#Status');
            ns.ceuTextArea = $('#ceu');
            ns.webinarDescriptionTextArea = $('#Description');
            ns.webinarLongDescriptionTextArea = $('#DescriptionLong');
            ns.addAdditionalLocationsPriceButton = $('#AddAdditionalLocationsPriceButton');
            ns.addAdditionalLocationsPriceInput = $('#AddAdditionalLocationsPriceInput');
            ns.additionalLocationsPrices = $('#AdditionalLocationsPrices');
        } 

        if (operation === ns.create) {
            ns.submitCreatedDetailsButton = $('#submitCreatedDetailsButton');
            ns.createWebinarForm = $('#createWebinarForm');
        } else if (operation === ns.edit) {
            ns.submitEditedDetailsButton = $('#submitEditedDetailsButton');
            ns.editWebinarForm = $('#editWebinarForm');
        } else if (operation === ns.clone) {
            ns.cloneWebinarForm = $('#cloneWebinarForm');
            ns.submitCloneDetailsButton = $('#submitCloneDetailsButton');
        }
    };

    ns.wireUpHandlers = function(operation) {

        if (operation === ns.edit) {
            ns.submitEditedDetailsButton.on('click', ns.submitWebinarDetails);
        } else if (operation === ns.create) {
            ns.submitCreatedDetailsButton.on('click', ns.submitNewWebinarDetails);
        } else if (operation === ns.clone) {
            ns.submitCloneDetailsButton.on('click', ns.submitCloneWebinarDetails);
        }

        if (operation === ns.edit || operation === ns.clone || operation === ns.create) {
            ns.addAdditionalLocationsPriceButton.on('click', ns.popAdditionalLocationsPrice);
        }
    };

    ns.searchWebinar = function (e) {

        e.preventDefault();

        $('#feedbackLabel').remove();

        var searchString = $.trim(ns.webinarSearchInput.val());

        $(this).append('<span id="crunchingSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        ns.webinarContent.load('/webinar/edit/' + searchString, function(response, status, xhr) {

            if (status === 'error') {
                var statusCode = xhr['status'];

                switch(statusCode) {
                    case 404:
                    {
                        ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + xhr['statusText'] + '</span></span>');
                    }
                }
            } else {
                if (xhr['responseText'].toString().slice(0, 1) === '{') { // if json response, we have an error condition 
                    $(this).empty();
                    ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + JSON.parse(xhr['responseText'])['Msg'] + '</span></span>');
                } else {
                    ns.primeDomVariables(ns.edit);
                    ns.wireUpHandlers(ns.edit);

                    ns.ceuTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarDescriptionTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarLongDescriptionTextArea.cleditor(ns.optionsForEditors);

                    var ceuEditor = ns.ceuTextArea.cleditor()[0];
                    ceuEditor.disable(false);
                    var webinarDescriptionEditor = ns.webinarDescriptionTextArea.cleditor()[0];
                    webinarDescriptionEditor.disable(false);
                    var webinarLongDescriptionEditor = ns.webinarLongDescriptionTextArea.cleditor()[0];
                    webinarLongDescriptionEditor.disable(false);
                }
            }

            $('#crunchingSpinner').remove();
        });
    };

    ns.cloneWebinar = function (e) {

        e.preventDefault();

        $('#feedbackLabel').remove();

        var searchString = $.trim(ns.webinarCloneInput.val());

        $(this).append('<span id="crunchingSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        ns.webinarContent.load('/webinar/clone/' + searchString, function (response, status, xhr) {

            if (status === 'error') {
                var statusCode = xhr['status'];

                switch (statusCode) {
                    case 404:
                        {
                            ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + xhr['statusText'] + '</span></span>');
                        }
                }
            } else {
                if (xhr['responseText'].toString().slice(0, 1) === '{') { // if json response, we have an error condition 
                    $(this).empty();
                    ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + JSON.parse(xhr['responseText'])['Msg'] + '</span></span>');
                } else {
                    ns.primeDomVariables(ns.clone);
                    ns.wireUpHandlers(ns.clone);

                    ns.ceuTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarDescriptionTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarLongDescriptionTextArea.cleditor(ns.optionsForEditors);

                    var ceuEditor = ns.ceuTextArea.cleditor()[0];
                    ceuEditor.disable(false);
                    var webinarDescriptionEditor = ns.webinarDescriptionTextArea.cleditor()[0];
                    webinarDescriptionEditor.disable(false);
                    var webinarLongDescriptionEditor = ns.webinarLongDescriptionTextArea.cleditor()[0];
                    webinarLongDescriptionEditor.disable(false);
                }
            }

            $('#crunchingSpinner').remove();
        });
    };

    ns.createWebinar = function (e) {

        e.preventDefault();

        $('#feedbackLabel').remove();

        $(this).append('<span id="crunchingSpinner">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        ns.webinarContent.load('/webinar/create', function (response, status, xhr) {

            if (status === 'error') {
                var statusCode = xhr['status'];

                switch (statusCode) {
                    case 404:
                        {
                            ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + xhr['statusText'] + '</span></span>');
                        }
                }
            } else {
                if (xhr['responseText'].toString().slice(0, 1) === '{') { // if json response, we have an error condition 
                    $(this).empty();
                    ns.webinarSearchButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">' + JSON.parse(xhr['responseText'])['Msg'] + '</span></span>');
                } else {
                    ns.primeDomVariables(ns.create);
                    ns.wireUpHandlers(ns.create);
                    
                    ns.ceuTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarDescriptionTextArea.cleditor(ns.optionsForEditors);
                    ns.webinarLongDescriptionTextArea.cleditor(ns.optionsForEditors);

                    var ceuEditor = ns.ceuTextArea.cleditor()[0];
                    ceuEditor.disable(false);
                    var webinarDescriptionEditor = ns.webinarDescriptionTextArea.cleditor()[0];
                    webinarDescriptionEditor.disable(false);
                    var webinarLongDescriptionEditor = ns.webinarLongDescriptionTextArea.cleditor()[0];
                    webinarLongDescriptionEditor.disable(false);
                }
            }

            $('#crunchingSpinner').remove();
        });

    };

    ns.submitWebinarDetails = function(e) {

        e.preventDefault();

        ns.submitEditedDetailsButton.append('<span id="crunchingSpinnerOfSubmit">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        var payload = ns.editWebinarForm.serialize();

        var addedPayloadData = ns.nonFormInputs();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/webinar/edit',
            dataType: constants.JsonDataType,
            data: payload += addedPayloadData,
            beforeSend: function() {
                $('#feedbackLabel').remove();
                $('.webinar-form-error').remove();
                formProcessor.clearValidationSummary($('#EditWebinarValSummary'));
            }
        }).done(function (data) {

            if (data.Result === 'Success') { 
                ns.submitEditedDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-success">&nbsp;The operation has succeeded.</span></span>');
            } else if (data.Result === 'Fail') {
                ns.submitEditedDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">&nbsp;The operation has failed. Please contact the administrator for assistance.</span></span>');
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('EditWebinarValSummary', data);

                _.each(_.keys(data.data), function (key) {
                    var id = key.slice(key.indexOf('-') + 1);

                    if (key === 'fromfluent-RegTypesGroupsXref') {
                        $('#regTypeGroupsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-Status') {
                        $('#SelectedStatus').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-idPresenter') {
                        $('#SelectedPresenter').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-WebinarTopicXrefs') {
                        $('#topicsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    }

                    try {
                        $('#' + id).after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } catch (e) {
                        console.log(key);
                    }
                });

                $('html, body').animate({ scrollTop: '30px' });

            }

            $('#crunchingSpinnerOfSubmit').remove();
        });
    };

    ns.submitNewWebinarDetails = function(e) {
        
        e.preventDefault();

        ns.submitCreatedDetailsButton.append('<span id="crunchingSpinnerOfSubmit">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        var payload = ns.createWebinarForm.serialize();

        var addedPayloadData = ns.nonFormInputs();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/webinar/create',
            dataType: constants.JsonDataType,
            data: payload += addedPayloadData,
            beforeSend: function() {
                $('.webinar-form-error').remove();
                $('#feedbackLabel').remove();
                formProcessor.clearValidationSummary($('#AddWebinarValSummary'));
            }
        }).done(function (data) {

            if (data.Result === 'Success') {
                ns.submitCreatedDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-success">&nbsp;The operation has succeeded.</span></span>');
            } else if (data.Result === 'Fail') {
                ns.submitCreatedDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">&nbsp;The operation has failed. Please contact the administrator for assistance.</span></span>');
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('AddWebinarValSummary', data);

                _.each(_.keys(data.data), function (key) {
                    var id = key.slice(key.indexOf('-') + 1);

                    if (key === 'fromfluent-RegTypesGroupsXref') {
                        $('#regTypeGroupsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-Status') {
                        $('#SelectedStatus').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-idPresenter') {
                        $('#SelectedPresenter').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-WebinarTopicXrefs') {
                        $('#topicsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    }

                    try {
                        $('#' + id).after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } catch (e) {
                        console.log(key);
                    } 
                });
            }

            $('#crunchingSpinnerOfSubmit').remove();
        });
    };

    ns.submitCloneWebinarDetails = function (e) {
        
        e.preventDefault();

        ns.submitCloneDetailsButton.append('<span id="crunchingSpinnerOfSubmit">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        var payload = ns.cloneWebinarForm.serialize();

        var addedPayloadData = ns.nonFormInputs();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/webinar/clone',
            dataType: constants.JsonDataType,
            data: payload += addedPayloadData,
            beforeSend: function() {
                $('.webinar-form-error').remove();
                $('#feedbackLabel').remove();
                formProcessor.clearValidationSummary($('#CloneWebinarValSummary'));
            }
        }).done(function (data) {

            if (data.Result === 'Success') {
                ns.submitCloneDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-success">&nbsp;The operation has succeeded.</span></span>');
            } else if (data.Result === 'Fail') {
                ns.submitCloneDetailsButton.after('<span id="feedbackLabel">&nbsp;<span class="label label-important">&nbsp;The operation has failed. Please contact the administrator for assistance.</span></span>');
            } else if (!data.isSuccessful) {
                formProcessor.lightUpValidationSummary('CloneWebinarValSummary', data);

                _.each(_.keys(data.data), function (key) {
                    var id = key.slice(key.indexOf('-') + 1);

                    if (key === 'fromfluent-RegTypesGroupsXref') {
                        $('#regTypeGroupsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-Status') {
                        $('#SelectedStatus').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-idPresenter') {
                        $('#SelectedPresenter').after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } else if (key === 'fromfluent-WebinarTopicXrefs') {
                        $('#topicsPanel').append('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    }

                    try {
                        $('#' + id).after('<span class="webinar-form-error label label-important"><i class="icon icon-exclamation-sign"></i>&nbsp;invalid</span>');
                    } catch (e) {
                        console.log(key);
                    }
                });
            }


            $('#crunchingSpinnerOfSubmit').remove();
        });
    };

    ns.popAdditionalLocationsPrice = function(e) {

        e.preventDefault();

        var priceToAdd = ns.addAdditionalLocationsPriceInput.val();
        ns.addAdditionalLocationsPriceInput.val('');

        if (ns.nrOfAdditionalLocationsPrices < 1) {
            ns.additionalLocationsPrices.prepend('<button id="clearAddLocPrices" class="btn btn-default">Clear</button>');
            $('#clearAddLocPrices').on('click', function (e) {
                e.preventDefault();

                $(this).off('click');

                ns.additionalLocationsPrices.empty();
                ns.nrOfAdditionalLocationsPrices = 0;
            });
        }

        ns.additionalLocationsPrices.prepend('<div id="' + ns.nrOfAdditionalLocationsPrices + '_addLocPrice" style="display:inline-block;margin:0px 5px">' + priceToAdd + '</div>');

        ns.nrOfAdditionalLocationsPrices++;
    };

    ns.nonFormInputs = function () {

        var addedPayloadData = '';
        var addLocPrices = ns.additionalLocationsPrices.find('div');

        $.each(addLocPrices, function (idx, val) {
            addedPayloadData += '&AdditionalLocationsPrices[' + idx + ']=' + val.innerText;
        });

        return addedPayloadData;
    };

    ns.edit = 'edit';
    ns.create = 'create';
    ns.clone = 'clone';
    ns.nrOfAdditionalLocationsPrices = 0;

    ns.optionsForEditors = {
        width: 600, // width not including margins, borders or padding
        height: 250, // height not including margins, borders or padding
        bodyStyle: // style to assign to document body contained within the editor
                'margin: 4px; font: 11pt Helvetica Neue,Helvetica,Arial,sans-serif; cursor:text',
        colors: // colors in the color popup
                'FFF FCC FC9 FF9 FFC 9F9 9FF CFF CCF FCF ' +
                'CCC F66 F96 FF6 FF3 6F9 3FF 6FF 99F F9F ' +
                'BBB F00 F90 FC6 FF0 3F3 6CC 3CF 66C C6C ' +
                '999 C00 F60 FC3 FC0 3C0 0CC 36F 63F C3C ' +
                '666 900 C60 C93 990 090 399 33F 60C 939 ' +
                '333 600 930 963 660 060 366 009 339 636 ' +
                '000 300 630 633 330 030 033 006 309 303',
        controls: // controls to add to the toolbar
            'bold italic underline | font size style | highlight removeformat | bullets numbering | outdent indent | alignleft center alignright | ' +
            'rule image link unlink | source',
        docCSSFile: '', // CSS file used to style the document contained within the editor
        fonts: // font names in the font popup
            'Arial,Arial Black,Comic Sans MS,Courier New,Narrow,Garamond, Georgia,Impact,Sans Serif,Serif,Tahoma,Trebuchet MS,Verdana',
        styles: // styles in the style popup
            [['Paragraph', '<p>'], ['Header 3', '<h3>'], ['Header 4', '<h4>'], ['Header 5', '<h5>'], ['Header 6', '<h6>'], ['BlockQuote', '<blockquote>']],
        useCSS: false // use CSS to style HTML when possible (not supported in ie)
    };

    
})(MW);