Type.registerNamespace('ise.Pages');


ise.Pages.ManageContacts = function () {
   
    var
        addContactModal = {},
        editContactModal = {},
        deleteContactModal = {},
        deleteCheckedContactModal = {},
        errorOnDeleteModal = {},
        errorOnMultiDeleteModal = {},
        listContacts = [],

        _manageContacts =
       {
           init: init,

           addContactModal: addContactModal,
           editContactModal: editContactModal,
           deleteContactModal: deleteContactModal,
           deleteCheckedContactModal: deleteCheckedContactModal,
           errorOnDeleteModal: errorOnDeleteModal,
           errorOnMultiDeleteModal: errorOnMultiDeleteModal,

           addContact: addContact,
           getContacts: getContacts,
       };

   
    function init() {
        setupAddContactModal();
        setupEditContactModal();
        setupDeleteContactModal();
        setupDeleteCheckedContactModal();
        setupErrorOnDeleteModal();
        setupErrorOnMultiDeleteModal();
        setEvents();

        //
        checkIfHasErrorOnDelete();
        checkIfHasErrorOnMultiDelete();
    }

    function getContacts() {
        return listContacts;
    }


    function addContact(contact) {
        if (contact) {
            listContacts.push(contact);
        }
    }



    function setupAddContactModal() {
        var htmlContent = $("#add-contact-modal")[0].outerHTML;
        $("#add-contact-modal").remove();
        addContactModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }

    function setupEditContactModal() {
        var htmlContent = $("#edit-contact-modal")[0].outerHTML;
        $("#edit-contact-modal").remove();
        editContactModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }

    function setupDeleteContactModal() {
        var htmlContent = $("#delete-contact-modal")[0].outerHTML;
        $("#delete-contact-modal").remove();
        deleteContactModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }

    function setupErrorOnDeleteModal() {
        var htmlContent = $("#error-ondelete-modal")[0].outerHTML;
        $("#error-ondelete-modal").remove();
        errorOnDeleteModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }

    function setupErrorOnMultiDeleteModal() {
        var htmlContent = $("#error-onmultidelete-modal")[0].outerHTML;
        $("#error-onmultidelete-modal").remove();
        errorOnMultiDeleteModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }

    function setupDeleteCheckedContactModal() {
        var htmlContent = $("#delete-checked-contact-modal")[0].outerHTML;
        $("#delete-checked-contact-modal").remove();
        deleteCheckedContactModal.modal = new DIYModal({
            content: htmlContent,
            minWidth: 400,
            maxWidth: 800
        });
    }


   
    function setEvents() {
        //button delete checked click event
        $("#btn-delete-checked-contacts").click(onButtonDeleteCheckedClick);

        //checkbox column change event
        $("#table-contacts > thead > tr > th > input#chkbox-allcontacts").change(onCheckAllCheckBoxesChange);

        //row edit button click event
        $("#table-contacts > tbody > tr > td > button.btn-edit-contact").click(onRowButtonEditClick);

        //row delete button click event
        $("#table-contacts > tbody > tr > td > button.btn-delete-contact").click(onRowButtonDeleteClick);

        $("#btn-search-contact").click(onButtonSearchContactClick);
        $("#txt-search-contact").keypress(onInputSearchContactKeypress);
    }

    function checkIfHasErrorOnDelete() {
        var error = getQueryString("errorondelete");
        var contact = getQueryString("contact");
        var name = getQueryString("name");

        if (error == null && contact == null && name == null) return;
        if ($.trim(error.value) == "" || $.trim(contact.value) == "" || $.trim(name.value) == "") return;


        errorOnDeleteModal.modal.open();
        var container = $("#error-ondelete-modal");
        container.find('#hidden-contactcode').val($.trim(contact.value));
        container.find('#hidden-fullname').val($.trim(name.value));
        container.find('#contact-to-delete').text($.trim(name.value));
        container.find('#error-ondelete-message > p').text($.trim(error.value));
    }

    function checkIfHasErrorOnMultiDelete() {

        var process = getQueryString("erroronmultidelete");
        if (process == null) return;
        var contacts = JSON.parse(process.value);

        errorOnMultiDeleteModal.modal.open();

        var $tbody = $("#error-onmultidelete-modal #table-multi-delete-result");
        $.each(contacts, function (index, contact) {
            var $tr = $("<tr/>");

            //$tr.data("contactcode", contact.contact);
            $tr.append($("<td/>").text(contact.name));

            if ($.trim(contact.error) != "") {
                var $error = $("<td class='error'/>") , $checkbox = $("<td/>").append(("<input type='checkbox' name='checkinactive_"+contact.contact + "' checked='checked'/>"));
                $error.text(contact.error);
             
                $tr.append($error);
                $tr.append($checkbox);
                
            } else if ($.trim(contact.success) != "") {
                $tr.append($("<td/>").text(contact.success));
               
            }
          

            $tbody.append($tr);

        });
     


       
        //var container = $("#error-ondelete-modal");
        //container.find('#hidden-contactcode').val($.trim(contact.value));
        //container.find('#hidden-fullname').val($.trim(name.value));
        //container.find('#contact-to-delete').text($.trim(name.value));
        //container.find('#error-ondelete-message > p').text($.trim(error.value));
    }

  
    function onCheckAllCheckBoxesChange(e) {
        $("#table-contacts > tbody > tr > td > input.chkbox-contact").prop('checked', $(this).is(":checked"));
    }

    function onRowButtonDeleteClick(e) {
        var contactCode = $(this).closest("tr").data("contact-code");

        $.each(listContacts, function (index, item) {
            if (item.ContactCode == contactCode) {
                deleteContactModal.modal.open();
                var containerId = '#delete-contact-modal';
                var container = $(containerId);
                var fullName = item.ContactFirstName + " " + item.ContactLastName;
                container.find('#hidden-contactcode').val(item.ContactCode);
                container.find('#hidden-fullname').val(fullName);
                container.find('#name-to-delete').text(fullName);
            }
        });
    }

    function onButtonDeleteCheckedClick(e) {
        var checkedContacts = [];
        $("#table-contacts > tbody > tr > td > input.chkbox-contact:checked").each(function () {
            checkedContacts.push($(this).closest("tr").data("contact-code"));
        });

        if (checkedContacts.length < 1) {
            return;
        }
        if (checkedContacts.length == 1) {
            var $btnDelete = $("#table-contacts > tbody > tr[data-contact-code='" + checkedContacts[0] + "'] > td > button.btn-delete-contact");
            $btnDelete.click();

        } else {
            deleteCheckedContactModal.modal.open();
            var container = $("#delete-checked-contact-modal");
            var $namesContainer = container.find('ul#names-to-delete');

            $namesContainer.empty();
            container.find("#hidden-contactcodes").val('');

            var contacts = [];

            $.each(checkedContacts, function (index, contactCode) {
                var contact = getContact(contactCode);
                if (contact) {
                    var fullName = contact.ContactFirstName + " " + contact.ContactLastName;
                    contacts.push({ contactCode: contactCode, fullName: fullName })
                    $namesContainer.append($("<li/>").text(fullName));
                }
            });
            container.find("#hidden-contacts").val(JSON.stringify(contacts));

        }

      
    }

    
    function onRowButtonEditClick(e) {
        var contactCode = $(this).closest("tr").data("contact-code");

        $.each(listContacts, function (index, item) {
            if (item.ContactCode == contactCode) {
                editContactModal.modal.open();
                var containerId = '#edit-contact-modal';
                var container = $(containerId);

                $("#select-salutation option").filter(function () {
                    return $(this).val() == item.ContactSalutationCode;
                }).prop('selected', true);

                $("#select-addressType option").filter(function () {
                    return $(this).val() == item.AddressType;
                }).prop('selected', true);

                $("#select-country option").filter(function () {
                    return $(this).val() == item.Country;
                }).prop('selected', true);

                container.find('#hidden-contactcode').val(item.ContactCode);
                container.find('#txt-firstName').val(item.ContactFirstName);
                container.find('#txt-lastName').val(item.ContactLastName);
                container.find('#txt-phone').val(item.BusinessPhone);
                container.find('#txt-phoneExt').val(item.BusinessPhoneExtension);
                container.find('#txt-mobile').val(item.Mobile);
                container.find('#txt-email').val(item.Email);
                container.find('#txtarea-address').val(item.Address);
                container.find('#txt-city').val(item.City);
                container.find('#txt-state').val(item.State);
                container.find('#txt-zipCode').val(item.PostalCode);


                return false;
            }
        });
    }

    function onButtonSearchContactClick(e) {
        search($("#txt-search-contact").val());
    }

    function onInputSearchContactKeypress(e) {
        if (e.which == 13) {
            search($("#txt-search-contact").val());
        }
    }

    function search(value) {
      //  if (value == "" || value == undefined || value == null) return;
        insertQueryStringParam("search", value);

    }

    function getQueryString(key) {
        var obj = null;
        var queryStrings = getQueryStrings();
        for (var i = 0; i < queryStrings.length; i++) {
            if (queryStrings[i].key == key) {
                obj = queryStrings[i];
            }
        }
        return obj;
    }

    function getQueryStrings() {
        var queryString = []
        var kvp = document.location.search.substr(1).split('&');
        var i = kvp.length;
        var x;
        while (i--) {
            x = kvp[i].split('=');
            var value = "";
            //0,1,2
            if (x.length > 1) {
                value = x[1];
                if (x.length > 2) {
                    for (var ctr = 2; ctr < x.length; ctr++) {
                        value += "=" + x[ctr];
                    }
                }
            }

            queryString.push({ key: x[0], value: decodeURI(value) });
        }
        return queryString;
    }

    function insertQueryStringParam(key, value) {
        key = encodeURI(key);
        value = encodeURI(value);

        var kvp = document.location.search.substr(1).split('&');

        var i = kvp.length; var x; while (i--) {
            x = kvp[i].split('=');

            if (x[0] == key) {
                x[1] = value;
                kvp[i] = x.join('=');
                break;
            }
        }

        if (i < 0) { kvp[kvp.length] = [key, value].join('='); }

        //this will reload the page, it's likely better to store this until finished
        var qStrings = [];
        $.each(kvp, function (index, item) {
            if (item != "") {
                qStrings.push(item);
            }
        });
        document.location.search = qStrings.join('&');
    }

    function getContact(contactCode) {
        var contact = null;
        $.each(listContacts, function (index, item) {
            if (item.ContactCode == contactCode) {
                contact = item;
                return false;
            }
        });
        return contact;
    }

    return _manageContacts;
};










//// register namespace
//Type.registerNamespace('ise.Pages');


//ise.Pages.ManageContacts = function () {
//    //public
//    var me =
//       {
//           fields: {
//               salutation: null,
//               firstName: null,
//               lastName: null,
//               phone: null,
//               phoneExt: null,
//               mobile: null,
//               email: null,
//               address: null,
//               addressType: null,
//               city: null,
//               state: null,
//               zipCode: null,
//               country: null,
//           },

//           fn: {
//               bindField: bindField,
//               validate: validate,
//               createNewContact: createNewContact,
//           },
//           events: {
//               onValidationComplete : { listeners: [], notify: function (value) { notifyEvent(ise.Pages.ManageContacts.events.onValidationComplete, value) } }
//           }
//       };

//    //private
//    function bindField(element, getValue, setValue) {
//        return { element: element, getValue: getValue, setValue: setValue };
//    }

//    function validate() {
//        //  alert(me.fields.firstName.getValue());


//        //  $(createValidationField("First Name is required.")).insertAfter($(me.fields.firstName.element));

//        // me.events.onValidationComplete.notify('success');
//    }

//    function createValidationField(message) {
//        var e = $("<div/>");
//        e.addClass("validation-error");
//        e.text(message);
//        return e;
//    }

//    function createNewContact() {
//        // validate();
//        var info =
//          {
//              Salutation: me.fields.salutation.getValue(),
//              FirstName: me.fields.firstName.getValue(),
//              LastName: me.fields.lastName.getValue(),
//              Phone: me.fields.phone.getValue(),
//              PhoneExt: me.fields.phoneExt.getValue(),
//              Mobile: me.fields.mobile.getValue(),
//              Email: me.fields.email.getValue(),
//              Address: me.fields.address.getValue(),
//              AddressType: me.fields.addressType.getValue(),
//              City: me.fields.city.getValue(),
//              State: me.fields.state.getValue(),
//              ZipCode: me.fields.zipCode.getValue(),
//              Country: me.fields.country.getValue(),
//          };
//        AjaxCallCommon("ActionService.asmx/AddCustomerContact", { contactInfo: info }, createNewContactSuccess, createNewContactError);
//    }

//    function createNewContactSuccess(response) {
//        var data = response;
//    }
//    function createNewContactError(err) {
//        var data = err;
//    }


//    function notifyEvent(event, value) {
//        for (var i = 0; i < event.listeners.length; i++) {
//            event.listeners[i](value);
//        }
//    }

//    //return
//    return me;
//};



