// MANAGEWEBINARWEBINAR namespace
if (MANAGEWEBINAR === null || typeof MANAGEWEBINAR === 'undefined')
    var MANAGEWEBINAR = {};

var MW = MANAGEWEBINAR; // create shortcus to namespace

// jQuery doc.ready function
$(function () {

    MW.primeDomVariables();
    MW.wireUpHandlers();

});

// self-invoking function.
(function (ns) {

    ns.primeDomVariables = function (operation) {

        if (operation === ns.edit || operation === ns.create) {
            ns.statusSelectList = $('#Status');
            ns.ceuTextArea = $('#ceu');
            ns.webinarDescriptionTextArea = $('#Description');
            ns.webinarLongDescriptionTextArea = $('#DescriptionLong');
            ns.editWebinarForm = $('#editWebinarForm');

        } else {
            ns.webinarSearchButton = $('#webinarSearchButton');
            ns.webinarSearchInput = $('#webinarSearchInput');
            ns.webinarCreateButton = $('#webinarCreateButton');
            ns.webinarContent = $('#webinarContent');
        }

        if (operation === ns.create) {
            ns.submitCreatedDetailsButton = $('#submitCreatedDetailsButton');
        } else if (operation === ns.edit) {
            ns.submitEditedDetailsButton = $('#submitEditedDetailsButton');
        }

    };

    ns.wireUpHandlers = function(operation) {

        if (operation === ns.edit) {
            ns.submitEditedDetailsButton.on('click', ns.submitWebinarDetails);
        } else if (operation === ns.create) {
            ns.submitCreatedDetailsButton.on('click', ns.submitNewWebinarDetails);
        } else {
            ns.webinarSearchButton.on('click', ns.searchWebinar);
            ns.webinarCreateButton.on('click', ns.createWebinar);
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

        $(this).append('<span id="crunchingSpinnerOfSubmit">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        var payload = ns.editWebinarForm.serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/webinar/edit',
            dataType: constants.JsonDataType,
            data: payload,
            beforeSend: function() {
                //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
            }
        }).done(function(data) {
            $('#crunchingSpinnerOfSubmit').remove();
        });
    };

    ns.submitNewWebinarDetails = function(e) {
        
        e.preventDefault();

        $(this).append('<span id="crunchingSpinnerOfSubmit">&nbsp;<i class="icon-spinner icon-spin"></i></span>');

        var payload = ns.editWebinarForm.serialize();

        $.ajax({
            type: 'POST',
            contentType: constants.FormPostContentType,
            cache: false,
            url: '/webinar/create',
            dataType: constants.JsonDataType,
            data: payload,
            beforeSend: function() {
                //$('#labelEmail').html('<span class="label label-warning">&nbsp;<i class="icon-spinner icon-spin"></i>&nbsp;Working...</span>');
            }
        }).done(function(data) {
            $('#crunchingSpinnerOfSubmit').remove();
        });
    };

    ns.edit = 'edit';
    ns.create = 'create';

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