function generateBudgetManagerView() {
    $(document).ready(function () {
        var dashboardGenerationInfo = getManagerGenerationInfo();
        dashboardGenerationInfo.forEach((value, index, array) => {
            generateManagerModuleInfo(value.actionUrl, value.actionData, value.placeholderElement);
        });

        initializeCardsCancel();
        initializeFormCheckboxes();
        initializeNewSLModal();
        initializeNewRBModal();
    });
}
function getManagerGenerationInfo() {

    var dashboardGenerationData = [
        {
            actionUrl: '/BudgetManager/ListShoppingInformation',
            actionData: {},
            placeholderElement: $('#plannedShoppingDiv')
        },
        {
            actionUrl: '/BudgetManager/ListRecurringBills',
            actionData: {},
            placeholderElement: $('#rbsInProgressDiv')
        },
    ];

    return dashboardGenerationData;
}
function generateManagerModuleInfo(actionUrl, actionData, placeholderElement) {
    $.ajax({
        type: "GET",
        url: actionUrl,
        data: actionData,
        success: function (result) {
            placeholderElement.html(result);
        }
    });
}
function activateFinalizeSLCard(shoppingOrderId) {
    var actionUrl = '/BudgetManager/ShoppingFinalization';
    var actionData = {
        orderId: shoppingOrderId
    };
    var placeholderElement = $('#shoppingDetailsDiv');

    cleanPlaceholder(placeholderElement);

    $.ajax({
        type: "GET",
        url: actionUrl,
        data: actionData,
        success: function (result) {
            placeholderElement.html(result);

            var checkboxes = $('#groupMembersSFWrapper input[type="checkbox"]');
            console.log(checkboxes);
            checkboxes.each((index, element) => {
                element.addEventListener('change', function () {
                    const currentValue = $('#hidden-' + index).val();
                    if (currentValue === "false") {
                        $('#hidden-' + index).val("true");
                    }
                    else {
                        $('#hidden-' + index).val("false");
                    }
                });
            });
        }
    });
    
    placeholderElement.on('click', '[data-save="card"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.card').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();

        var dataToSend = [];
        primaryData.forEach((value, index, array) => {
            var singleDataElement = {};
            if (value.name.match(/Value|ParticipantsCharge/)) {
                var formattedString = value.value.replace(/\./g, ",");
                singleDataElement.name = value.name;
                singleDataElement.value = formattedString;
            }
            else {
                singleDataElement.name = value.name;
                singleDataElement.value = value.value;
            }

            if (Object.entries(singleDataElement).length !== 0 && singleDataElement.value !== 'on') {
                dataToSend.push(singleDataElement);
            }
        });
        
        $.post(actionUrl, dataToSend).done(function (data) {
            cleanPlaceholder(placeholderElement);
            cleanPlaceholder($('#plannedShoppingDiv'));

            var actionUrl = '/BudgetManager/ListShoppingInformation';
            var actionData = {};
            var shoppingListPlaceholder = $('#plannedShoppingDiv');
            generateManagerModuleInfo(actionUrl, actionData, shoppingListPlaceholder);
        });
    });
}
function activateDeleteSLCard(shoppingOrderId) {
    var actionUrl = '/BudgetManager/ShoppingRemoval';
    var actionData = {
        orderId: shoppingOrderId
    };
    var placeholderElement = $('#shoppingDetailsDiv');

    cleanPlaceholder(placeholderElement);
    generateManagerModuleInfo(actionUrl, actionData, placeholderElement);

    placeholderElement.on('click', '[data-delete="card"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.card').find('form');
        var actionUrl = form.attr('action');
        var actionData = form.serializeArray();

        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, actionData).done(function (data) {
            var shoppingListPlaceholder = $('#plannedShoppingDiv');

            cleanPlaceholder(placeholderElement);
            cleanPlaceholder(shoppingListPlaceholder);

            var actionUrl = '/BudgetManager/ListShoppingInformation';
            var actionData = {};
            generateManagerModuleInfo(actionUrl, actionData, shoppingListPlaceholder);
        });
    });
}
function activateEditRBCard(recurringBillId) {
    var actionUrl = '/BudgetManager/BillEdit';
    var actionData = {
        RBId: recurringBillId
    };
    var placeholderElement = $('#rbDetailsDiv');

    cleanPlaceholder(placeholderElement);

    $.ajax({
        type: "GET",
        url: actionUrl,
        data: actionData,
        success: function (result) {
            placeholderElement.html(result);

            var groupId = $('#GroupId').val();
            var startDateDTPSelector = '#startDateEditRB';
            var endDateDTPSelector = '#endDateEditRB';

            var startDateInput = document.getElementById('StartDate');
            var endDateInput = document.getElementById('ExpirationDate');
            var parsedStartDate = parseToDatepickersDate(startDateInput.value);
            var parsedEndDate = parseToDatepickersDate(endDateInput.value);
            startDateInput.value = parsedStartDate;
            endDateInput.value = parsedEndDate;
            initializeDatePickersWithInitialDates(startDateDTPSelector, endDateDTPSelector, parsedStartDate, parsedEndDate);

            generateChooseUsersList(groupId);            
        }
    });

    placeholderElement.on('click', '[data-edit="card"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.card').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();

        var dataToSend = [];
        primaryData.forEach((value, index, array) => {
            var singleDataElement = {};
            if (value.name.match(/Value/)) {
                var formattedString = value.value.replace(/\./g, ",");
                singleDataElement.name = value.name;
                singleDataElement.value = formattedString;
            }
            else {
                if (value.name.match(/ParticipantIds/)) {
                    var idString = value.name.split('-');
                    singleDataElement.name = "ParticipantIds";
                    singleDataElement.value = parseInt(idString[1]);
                }
                else {
                    singleDataElement.name = value.name;
                    switch (value.name) {
                        case 'StartDate':
                        case 'Expirationdate':
                            singleDataElement.value = parseDatepickersDate(value.value);
                            break;
                        default:
                            singleDataElement.value = value.value;
                            break;
                    }
                }
            }

            if (Object.entries(singleDataElement).length !== 0) {
                dataToSend.push(singleDataElement);
            }
        });

        $.post(actionUrl, dataToSend).done(function (data) {
            var billsListPlaceholder = $('#rbsInProgressDiv');

            cleanPlaceholder(placeholderElement);
            cleanPlaceholder(billsListPlaceholder);

            var actionUrl = '/BudgetManager/ListRecurringBills';
            var actionData = {};
            generateManagerModuleInfo(actionUrl, actionData, billsListPlaceholder);
        });
    });
}
function activateDeleteRBCard(recurringBillId) {
    var actionUrl = '/BudgetManager/BillRemoval';
    var actionData = {
        RBId: recurringBillId
    };
    var placeholderElement = $('#rbDetailsDiv');

    cleanPlaceholder(placeholderElement);
    generateManagerModuleInfo(actionUrl, actionData, placeholderElement);

    placeholderElement.on('click', '[data-delete="card"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.card').find('form');
        var deleteBillUrl = form.attr('action');
        var deleteBillData = { billId: document.getElementById('Id').value };

        console.log(deleteBillUrl);
        //TODO: check if working for bad values - modal shouldn't hide
        $.ajax({
            type: "DELETE",
            url: deleteBillUrl,
            data: deleteBillData,
            success: function () {
                var billsListPlaceholder = $('#rbsInProgressDiv');

                cleanPlaceholder(placeholderElement);
                cleanPlaceholder(billsListPlaceholder);

                var actionUrl = '/BudgetManager/ListRecurringBills';
                var actionData = {};
                generateManagerModuleInfo(actionUrl, actionData, billsListPlaceholder);
            }
        });
    });
}
function initializeNewRBModal() {
    var placeholderElement = $('#new-bill-modal-placeholder');

    $('button[data-toggle="ajax-recurring-bill-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            initializeDatePickers('#startDateNewRB', '#endDateNewRB');
            initializeNewItemSelectGroup('groupNRBSelect', 'groupMembersNRBWrapper');
            placeholderElement.find('#modalNewRBCreate').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();
        console.log(form);
        console.log(primaryData);

        var dataToSend = [];
        primaryData.forEach((value, index, array) => {
            var singleDataElement = {};
            if (value.name.match(/ParticipantIds/)) {
                var idString = value.name.split('-');
                singleDataElement.name = "ParticipantIds";
                singleDataElement.value = parseInt(idString[1]);
            }
            else {
                singleDataElement.name = value.name;
                switch (value.name) {
                    case 'StartDate':
                    case 'ExpirationDate':
                        singleDataElement.value = parseDatepickersDate(value.value);
                        break;
                    default:
                        singleDataElement.value = value.value;
                        break;
                }
            }

            if (Object.entries(singleDataElement).length !== 0) {
                dataToSend.push(singleDataElement);
            }
        });

        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, dataToSend).done(function (data) {
            var billingListPlaceholder = $('#rbsInProgressDiv');
            placeholderElement.find('#modalNewRBCreate').modal('hide');
            cleanPlaceholder(billingListPlaceholder);

            var actionUrl = '/BudgetManager/ListRecurringBills';
            var actionData = {};
            generateManagerModuleInfo(actionUrl, actionData, billingListPlaceholder);
        });
    });

    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
}
function cleanPlaceholder(placeholderElement) {
    placeholderElement.empty();
}
function initializeCardsCancel() {
    var placeholderElements = getCardsPlaceholders();

    placeholderElements.forEach((value, index, array) => {
        value.on('click', '[data-dismiss="card"]', function (event) {
            event.preventDefault();
            value.empty();
        });
    });
}
function initializeFormCheckboxes() {
    initializeDivideBillEqually();
    initializeSettleWholeBillCheckboxes();
}
function initializeDivideBillEqually() {
    var shoppingDetailsDiv = $('#shoppingDetailsDiv');

    shoppingDetailsDiv.on('change', '#divideBillEquallyCheckbox', function (event) {
        event.preventDefault();
        
        var isDivideEquallyChecked = $('#divideBillEquallyCheckbox').prop('checked');
        var participantBillingInputs = $('#groupMembersSFWrapper input').not('[type="checkbox"], [type="hidden"]');
        var participantInputsLength = participantBillingInputs.length;
        var valueInput = $('#Value');

        var setChargeValues = function () {
            participantBillingInputs.each((index, element) => {
                element.setAttribute('readonly', true);
                
                var initialValue = (valueInput.val() / participantInputsLength).toFixed(2);
                var dividedValue = isNaN(initialValue) ? 0.00 : parseFloat(initialValue);
                if (participantInputsLength - 1 === index) {
                    var billingSum = (participantInputsLength - 1) * dividedValue;
                    var lastBillingPart = isNaN(initialValue) ? 0.00 : valueInput.val() - billingSum;
                    element.value = lastBillingPart.toFixed(2);
                }
                else {
                    element.value = dividedValue.toFixed(2);
                }
            });
        };

        if (isDivideEquallyChecked === true) {
            setChargeValues();
            valueInput.on('change', setChargeValues);
        }
        else {
            participantBillingInputs.each((index, element) => {
                element.removeAttribute('readonly');
            });
            valueInput.off('change', setChargeValues);
        }
    });
}
function changeChargeInput(hiddenElementId) {
    const currentValue = $('#' + hiddenElementId).val();
    console.log(hiddenElementId + ' ' + currentValue);
    if (currentValue === "false") {
        $('#' + hiddenElementId).val("true");
    }
    else {
        $('#' + hiddenElementId).val("false");
    }
}
function generateChooseUsersList(groupId) {
    var listGroupMembersUrl = "/Teams/ListGroupMembersInfo";
    var optionsData = { groupId: groupId };
    $.ajax({
        type: "GET",
        url: listGroupMembersUrl,
        data: optionsData,
        success: function (result) {
            var outerDiv = document.getElementById("groupMembersRBEWrapper");

            var outerLabel = document.createElement("label");
            outerLabel.classList.add("control-label");
            outerLabel.setAttribute("for", "ParticipantIds");
            outerLabel.innerHTML = "Attach your friends:";
            outerDiv.appendChild(outerLabel);

            result.forEach((value, index, array) => {
                var currentUserId = "1";
                if (value.id.toString() === currentUserId) {
                    return;
                }
                var groupMembersDiv = document.createElement("div");
                groupMembersDiv.classList.add("form-check");

                var divLabel = document.createElement("label");
                divLabel.classList.add("form-check-label");

                var inputElem = document.createElement("input");
                inputElem.setAttribute('type', 'radio');
                inputElem.setAttribute('name', 'ParticipantIds' + '-' + value["id"]);
                inputElem.classList.add("form-check-input");

                divLabel.appendChild(inputElem);
                divLabel.append(value["fullName"]);
                groupMembersDiv.appendChild(divLabel);

                outerDiv.appendChild(groupMembersDiv);
            });
        }
    });
}
function initializeSettleWholeBillCheckboxes() {
    var shoppingDetailsDiv = $('#shoppingDetailsDiv');

    shoppingDetailsDiv.on('change', '#IsCovered', function (event) {
        event.preventDefault();

        var participantSettlementCheckboxes = $('#groupMembersSFWrapper input[type="checkbox"]');
        var settleWholeBillCheckbox = $('#IsCovered');
        
        var wholeSettlementValue = settleWholeBillCheckbox.prop('checked');
        if (wholeSettlementValue === true) {
            participantSettlementCheckboxes.each((index, element) => {
                element.setAttribute('disabled', 'disabled');
                element.setAttribute('checked', true);
            });
        }
        else {
            participantSettlementCheckboxes.each((index, element) => {
                element.removeAttribute('disabled');
                element.removeAttribute('checked');
            });
        }
    });
}
function getCardsPlaceholders() {
    var placeholderElements = [
        $('#shoppingDetailsDiv'),
        $('#rbDetailsDiv')
    ];

    return placeholderElements;
}
function initializeNewSLModal() {
    var placeholderElement = $('#new-shop-list-modal-placeholder');

    $('button[data-toggle="ajax-shopping-list-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            initializeNewItemSelectGroup('groupSCSelect', 'groupMembersSCWrapper');
            initializeAddSingleElementButton();
            placeholderElement.find('#modalNewShoppingListCreate').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();
        console.log(form);
        console.log(primaryData);

        var dataToSend = [];
        primaryData.forEach((value, index, array) => {
            var singleDataElement = {};
            if (value.name.match(/ParticipantIds/)) {
                var idString = value.name.split('-');
                singleDataElement.name = "ParticipantIds";
                singleDataElement.value = parseInt(idString[1]);
            }
            else {
                if (value.name.match(/SingleElementAmounts/)) {
                    var formattedString = value.value.replace(/\./g, ",");
                    singleDataElement.name = value.name;
                    singleDataElement.value = formattedString;
                }
                else {
                    singleDataElement.name = value.name;
                    singleDataElement.value = value.value;
                }
            }

            if (Object.entries(singleDataElement).length !== 0) {
                dataToSend.push(singleDataElement);
            }
        });

        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, dataToSend).done(function (data) {
            placeholderElement.find('#modalNewShoppingListCreate').modal('hide');        

            cleanPlaceholder($('#plannedShoppingDiv'));

            var actionUrl = '/BudgetManager/ListShoppingInformation';
            var actionData = {};
            var shoppingListPlaceholder = $('#plannedShoppingDiv');
            generateManagerModuleInfo(actionUrl, actionData, shoppingListPlaceholder);
        });
    });
    
    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
}
function initializeAddSingleElementButton() {
    var unitOptions = listSelectUnitOptions();

    var addElementHandle = $('#addNewSingleElement');
    var singleElementWrapper = $('#singleItemsDiv');

    var singleElementIndex = 1;

    addElementHandle.on('click', function (event) {
        event.preventDefault();
        var mainElementdiv = document.createElement('div');
        mainElementdiv.classList.add('mb-1');
        mainElementdiv.id = 'singleItem-' + singleElementIndex.toString();

        var titleInput = document.createElement('input');
        titleInput.classList.add('form-control', 'form-control-sm', 'col-4', 'd-inline');
        titleInput.setAttribute('name', 'SingleElementTitles');
        titleInput.setAttribute('placeholder', 'Title...');
        mainElementdiv.appendChild(titleInput);

        var amountInput = document.createElement('input');
        amountInput.classList.add('form-control', 'form-control-sm', 'col-3', 'd-inline');
        amountInput.setAttribute('name', 'SingleElementAmounts');
        amountInput.setAttribute('placeholder', 'Amount...');
        amountInput.style.marginLeft = '4px';
        mainElementdiv.appendChild(amountInput);

        var selectDiv = document.createElement('div');
        selectDiv.classList.add('dropdown', 'd-inline');

        var selectUnit = document.createElement('select');
        selectUnit.setAttribute('name', 'SingleElementUnits');
        selectUnit.classList.add('form-control', 'form-control-sm', 'col-3', 'd-inline');
        selectUnit.style.marginLeft = '4px';

        var defaultOptionTitle = document.createElement('option');
        defaultOptionTitle.setAttribute('selected', true);
        defaultOptionTitle.setAttribute('disabled', true);
        defaultOptionTitle.setAttribute('hidden', true);
        defaultOptionTitle.setAttribute('value', "");
        defaultOptionTitle.text = "Unit";
        selectUnit.appendChild(defaultOptionTitle);

        unitOptions.forEach((value, index, array) => {
            var selectOption = document.createElement("option");
            selectOption.text = value.text;
            selectOption.value = value.value;
            selectUnit.appendChild(selectOption);
        });

        selectDiv.appendChild(selectUnit);
        mainElementdiv.appendChild(selectDiv);  

        var deleteElementButton = document.createElement('button');
        var currentIndex = singleElementIndex;
        deleteElementButton.classList.add('btn', 'btn-dark', 'btn-sm', 'fas', 'fa-times', 'd-inline');
        deleteElementButton.style.padding = '.45rem';
        deleteElementButton.addEventListener('click', function (event) {
            event.preventDefault();
            $('#singleItem-' + currentIndex).remove();
        });

        singleElementIndex++;

        mainElementdiv.appendChild(deleteElementButton);
        singleElementWrapper.append(mainElementdiv);
    });
}
function listSelectUnitOptions() {
    var listGroupInfoUrl = "/BudgetManager/ListUnitValues";
    var optionList = [];
    $.ajax({
        type: "GET",
        url: listGroupInfoUrl,
        success: function (result) {
            result.forEach((value, index, array) => {
                optionList.push({
                    text: value,
                    value: index
                });
            });
        }
    });

    return optionList;
}