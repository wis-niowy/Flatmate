function generateBudgetManagerView() {
    $(document).ready(function () {
        var dashboardGenerationInfo = getManagerGenerationInfo();
        dashboardGenerationInfo.forEach((value, index, array) => {
            generateManagerModuleInfo(value.actionUrl, value.actionData, value.placeholderElement);
        });
        initializeCardsCancel();
        initializeFormCheckboxes();
        initializeNewSLModal();
    });
}
function getManagerGenerationInfo() {

    var dashboardGenerationData = [
        {
            actionUrl: '/BudgetManager/ListShoppingInformation',
            actionData: {},
            placeholderElement: $('#plannedShoppingDiv')
        }
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
function activateFinalizeCard(shoppingOrderId) {
    var actionUrl = '/BudgetManager/ShoppingFinalization';
    var actionData = {
        orderId: shoppingOrderId
    };
    var placeholderElement = $('#shoppingDetailsDiv');

    cleanPlaceholder(placeholderElement);
    generateModuleInfo(actionUrl, actionData, placeholderElement);
} 
function activateDeleteCard(shoppingOrderId) {
    var actionUrl = '/BudgetManager/ShoppingRemoval';
    var actionData = {
        orderId: shoppingOrderId
    };
    var placeholderElement = $('#shoppingDetailsDiv');

    cleanPlaceholder(placeholderElement);
    generateModuleInfo(actionUrl, actionData, placeholderElement);

}
function cleanPlaceholder(placeholderElement) {
    placeholderElement.empty();
}
function initializeCardsCancel() {
    var placeholderElements = getCardsPlaceholders();

    placeholderElements.forEach((value, index, array) => {
        value.placeholderElement.on('click', '[data-dismiss="card"]', function (event) {
            event.preventDefault();
            value.placeholderElement.empty();
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
        var participantBillingInputs = $('#groupMembersSFWrapper input[type="text"]');
        var participantInputsLength = participantBillingInputs.length;
        var valueInput = $('#Value');

        var setChargeValues = function () {
            participantBillingInputs.each((index, element) => {
                element.setAttribute('disabled', 'disabled');
                
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
                element.removeAttribute('disabled');
            });
            valueInput.off('change', setChargeValues);
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
        {
            placeholderElement: $('#shoppingDetailsDiv')
        }];

    return placeholderElements;
}
function initializeSelectGroup() {
    var listGroupInfoUrl = "/Teams/ListGroupInfo";
    //TODO: change to current user id
    var currentUserId = "1";
    var selectData = { /*userId: currentUserId*/ };
    $.ajax({
        type: "GET",
        url: listGroupInfoUrl,
        data: selectData,
        success: function (result) {
            var groupSelect = document.getElementById("groupSCSelect");
            result.forEach((value, index, array) => {
                var selectOption = document.createElement("option");
                selectOption.text = value["name"];
                selectOption.value = value["id"];
                groupSelect.appendChild(selectOption);
            });

            $("#groupSCSelect").on('change', function () {
                var listGroupMembersUrl = "/Teams/ListGroupMembersInfo";
                var optionsData = { groupId: $('#groupSCSelect').find(':selected').val() };
                $.ajax({
                    type: "GET",
                    url: listGroupMembersUrl,
                    data: optionsData,
                    success: function (result) {
                        var outerDiv = document.getElementById("groupMembersSCWrapper");
                        while (outerDiv.firstChild) {
                            outerDiv.removeChild(outerDiv.firstChild);
                        }

                        var outerLabel = document.createElement("label");
                        outerLabel.classList.add("control-label");
                        outerLabel.setAttribute("for", "ParticipantIds");
                        outerLabel.innerHTML = "Attach your friends:";
                        outerDiv.appendChild(outerLabel);

                        result.forEach((value, index, array) => {
                            if (value.id.toString() === selectData.userId) {
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
            });
        }
    });
}
function initializeNewSLModal() {
    var placeholderElement = $('#new-shop-list-modal-placeholder');

    $('button[data-toggle="ajax-shopping-list-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            initializeSelectGroup();
            initializeAddSingleElementButton();
            placeholderElement.find('#modalNewShoppingListCreate').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();
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
                singleDataElement.value = value.value;
            }

            if (Object.entries(singleDataElement).length !== 0) {
                dataToSend.push(singleDataElement);
            }
        });

        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, dataToSend).done(function (data) {
            console.log(data);
            placeholderElement.find('#modalNewShoppingListCreate').modal('hide');            
        });
    });

    placeholderElement.on('change', '[name=SingleElementAmounts]', function (event) {
        convertInputValueToDoubleOnChange(event.target);
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

function convertInputValueToDoubleOnChange(eventTarget) {
    var insertedValue = eventTarget.value;
    if (isNaN(parseFloat(insertedValue))) {
        eventTarget.value = 0;
    }
    else {
        var parsedValue = insertedValue.replace(/\./g, ',');
        eventTarget.value = parsedValue;
    }
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