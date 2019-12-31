
//general
function generateBudgetManagerView() {
    $(document).ready(function () {
        var initialState = configureDefaultState();
        var dashboardGenerationInfo = getManagerGenerationInfo(initialState);
        for (const value of Object.values(dashboardGenerationInfo)) {
            generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
        }

        activateDeleteSLCards();
        activateFinalizeSLCards();
        activateEditRBCards();
        activateDeleteRBCards();
        initializeCardsCancel();
        initializeFormCheckboxes('#shoppingDetailsDiv', '#groupMembersSFWrapper');
        initializeNewSLModal();
        initializeNewRBModal();
        initializeNewExpenseModal();
        initializeCoverCheckboxes();
        initializeCoverExpensesButton(dashboardGenerationInfo);
        setPastExpensesPeriodSelectionHandler();
    });
}
function configureDefaultState() {
    return {
        searchPeriodInDays: 3
    };
}
function getManagerGenerationInfo(state = configureDefaultState()) {
    var dashboardGenerationData = {
        shoppingInfo: {
            actionUrl: '/BudgetManager/ListShoppingInformation',
            actionData: {},
            onSuccess: {
                function: fillPlaceholderWithResult,
                params: {
                    placeholderSelector: '#plannedShoppingDiv'
                }
            }
        },
        RBInfo: {
            actionUrl: '/BudgetManager/ListRecurringBills',
            actionData: {},
            onSuccess: {
                function: fillPlaceholderWithResult,
                params: {
                    placeholderSelector: '#rbsInProgressDiv'
                }
            }
        },
        coveredSettlementInfo: {
            actionUrl: '/BudgetManager/ListCoveredSettlementInformation',
            actionData: { displayedDays: state.searchPeriodInDays },
            onSuccess: {
                function: fillPlaceholderAndSetInitialTabsState,
                params: {
                    buttonsSelectors: {
                        liabilityButton: '#executedLiabilitiesTab',
                        credibilityButton: '#executedCredibilitiesTab'
                    },
                    placeholderSelector: '#executedExpensesDiv'
                }
            }
        },
        coveredExpensesInfo: {
            actionUrl: '/BudgetManager/ListCoveredExpenses',
            actionData: { displayedDays: state.searchPeriodInDays },
            onSuccess: {
                function: assignSumsToLabelsAndCreateChart,
                params: {
                    labelsSelectors: {
                        liabilityLabel: '#coveredLiabilitiesLabel',
                        credibilityLabel: '#coveredCredibilitiesLabel'
                    }
                }
            }
        },
        currentSettlementInfo: {
            actionUrl: '/BudgetManager/ListCurrentSettlementInformation',
            actionData: {},
            onSuccess: {
                function: fillPlaceholderWithResult,
                params: {
                    placeholderSelector: '#currentExpensesDiv'
                }
            }
        },
        currentExpensesInfo: {
            actionUrl: '/BudgetManager/ListCurrentExpenses',
            actionData: {},
            onSuccess: {
                function: assignSettlementsSumsToLabels,
                params: {
                    labelsSelectors: {
                        liabilityLabel: '#currentLiabilitiesLabel',
                        credibilityLabel: '#currentCredibilitiesLabel'
                    }
                }
            }
        }
    };

    return dashboardGenerationData;
}
function generateManagerModuleInfo(actionUrl, actionData, successHandler) {
    $.ajax({
        type: "GET",
        url: actionUrl,
        data: actionData,
        success: function (result) {
            successHandler.params["result"] = result;
            successHandler.function(successHandler.params);
        }
    });
}

function fillPlaceholderWithResult({ placeholderSelector, result }) {
    $(placeholderSelector).html(result);
}
function fillPlaceholderAndSetInitialTabsState({ buttonsSelectors, placeholderSelector, result }) {
    $(buttonsSelectors["liabilityButton"]).addClass('active');
    $(buttonsSelectors["credibilityButton"]).removeClass('active');
    fillPlaceholderWithResult({ placeholderSelector: placeholderSelector, result: result });
}
function fillPlaceholderWithResultAndAddClassColumn({ placeholderSelector, columnSelector, className, result }) {
    $(placeholderSelector).html(result);
    $(columnSelector).addClass(className);
}
function fillPlaceholderWithResultAndRemoveClassColumn({ placeholderSelector, columnSelector, className, result }) {
    $(placeholderSelector).html(result);
    $(columnSelector).removeClass(className);
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
function getCardsPlaceholders() {
    var placeholderElements = [
        $('#shoppingDetailsDiv'),
        $('#rbDetailsDiv'),
        $('settlementDetailsDiv')
    ];

    return placeholderElements;
}

function assignSettlementsSumsToLabels({ labelsSelectors, result }) {
    var credibilitiesSum = 0, liabilitiesSum = 0;
    result["perDateCredibilities"].forEach((dateValue, index, array) => {
        dateValue.forEach((value, index, array) => {
            credibilitiesSum += value["value"];
        });
    });

    result["perDateLiabilities"].forEach((dateValue, index, array) => {
        dateValue.forEach((value, index, array) => {
            liabilitiesSum += value["value"];
        });
    });

    $(labelsSelectors["credibilityLabel"]).text("SUM: " + credibilitiesSum.toFixed(2) + " PLN");
    $(labelsSelectors["liabilityLabel"]).text("SUM: " + liabilitiesSum.toFixed(2) + " PLN");    
}

function initializeFormCheckboxes(outerSelector, wrapperSelector) {
    initializeDivideBillEqually(outerSelector, wrapperSelector);
    initializeSettleWholeBillCheckboxes(outerSelector, wrapperSelector, false);
}
function initializeSettleWholeBillCheckboxes(outerSelector, wrapperSelector, excludeFirstElement) {
    var outerSelectorDiv = $(outerSelector);

    outerSelectorDiv.on('change', '#IsCovered', function (event) {
        event.preventDefault();

        var participantSettlementCheckboxes = $(wrapperSelector + ' input[type="checkbox"]').not('[class="form-check-input"]');
        participantSettlementCheckboxes = excludeFirstElement ? $(wrapperSelector + ' input[type="checkbox"]').not('[class="form-check-input"],[id="currentUserCheckbox"]') : $(wrapperSelector + ' input[type="checkbox"]').not('[class="form-check-input"]');
        var settleWholeBillCheckbox = $('#IsCovered');
        console.log(participantSettlementCheckboxes);

        var wholeSettlementValue = settleWholeBillCheckbox.prop('checked');
        if (wholeSettlementValue === true) {
            participantSettlementCheckboxes.prop('disabled', true);
            participantSettlementCheckboxes.prop('checked', true);
            participantSettlementCheckboxes.each((index, element) => {
                $('#hidden-' + index).val("true");
            });
        }
        else {
            participantSettlementCheckboxes.prop('disabled', false);
            participantSettlementCheckboxes.prop('checked', false);
            participantSettlementCheckboxes.each((index, element) => {
                $('#hidden-' + index).val("false");
            });
        }
    });
}

function initializeDivideBillEqually(outerSelector, wrapperSelector) {
    var shoppingDetailsDiv = $(outerSelector);

    shoppingDetailsDiv.on('change', '#divideBillEquallyCheckbox', function (event) {
        event.preventDefault();

        var isDivideEquallyChecked = $('#divideBillEquallyCheckbox').prop('checked');
        var participantBillingInputs = $(wrapperSelector + ' input').not('[type="checkbox"], [type="hidden"]');
        var valueInput = $('#Value');

        var internalSetCharges = function () { setChargeValues(wrapperSelector); };
        if (isDivideEquallyChecked === true) {
            setChargeValues(wrapperSelector);
            valueInput.on('change', internalSetCharges);
        }
        else {
            participantBillingInputs.each((index, element) => {
                element.removeAttribute('readonly');
            });
            valueInput.off('change');
        }
    });
}
function setChargeValues(wrapperSelector) {
    var participantBillingInputs = $(wrapperSelector + ' input').not('[type="checkbox"], [type="hidden"]');
    var participantInputsLength = participantBillingInputs.length;
    var valueInput = $('#Value');

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
}
//end

//debug
function displayActionResult({ result }) {
    console.log(result);
}
//end

//inProgress
function showPlannedExpenseDetails(totalExpenseId, userId, teamId) {
    var actionUrl = '/BudgetManager/ExpenseDetails';
    var actionData = {
        totalExpenseId: totalExpenseId,
        userId: userId,
        groupId: teamId
    };

    var placeholderElementSelector = '#settlementDetailsDiv';
    var placeholderElement = $(placeholderElementSelector);
    var successHandler = {
        function: fillPlaceholderWithResult,
        params: {
            placeholderSelector: placeholderElementSelector
        }
    };

    cleanPlaceholder(placeholderElement);
    generateManagerModuleInfo(actionUrl, actionData, successHandler);
}
function changeChargeInput(hiddenElementId) {
    const currentValue = $('#' + hiddenElementId).val();
    if (currentValue === "false") {
        $('#' + hiddenElementId).val("true");
    }
    else {
        $('#' + hiddenElementId).val("false");
    }
}
//end

//SHmodule
function activateSingleFinalizeSLCard(shoppingOrderId) {
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

            $('#shoppingDetailsDiv').addClass("accordion-content-tall");
        }
    });
}
function activateFinalizeSLCards() {
    var placeholderElement = $('#shoppingDetailsDiv');
    $('#shoppingDetailsDiv').on('click', '[data-save="card"]', function (event) {
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

            var managerGenerationInfo = getManagerGenerationInfo();
            var deconstructModulesInfo = ({ shoppingInfo }) => ({ shoppingInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });
}
function activateSingleDeleteSLCard(shoppingOrderId) {
    var actionUrl = '/BudgetManager/ShoppingRemoval';
    var actionData = {
        orderId: shoppingOrderId
    };

    var placeholderElementSelector = '#shoppingDetailsDiv';
    var placeholderElement = $(placeholderElementSelector);
    var successHandler = {
        function: fillPlaceholderWithResult,
        params: {
            placeholderSelector: placeholderElementSelector
        }
    };

    cleanPlaceholder(placeholderElement);
    generateManagerModuleInfo(actionUrl, actionData, successHandler);
}
function activateDeleteSLCards() {
    var placeholderElement = $('#shoppingDetailsDiv');
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

            var managerGenerationInfo = getManagerGenerationInfo();
            var deconstructModulesInfo = ({ shoppingInfo }) => ({ shoppingInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });
}
function initializeNewExpenseModal() {
    var placeholderElement = $('#new-settlement-modal-placeholder');

    $('button[data-toggle="ajax-new-expense-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            initializeNewItemSelectGroupWithSelectionHandler('groupExpenseCreateSelect', 'groupMembersExpenseCreateWrapper');
            initializeFormCheckboxesBlockCurrentUser('#modalExpenseCreate', '#groupMembersExpenseCreateWrapper');
            placeholderElement.find('#modalExpenseCreate').modal('show');
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
            if (value.name.match(/Value|ParticipantsCharge/)) {
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
                    singleDataElement.value = value.value;
                }
            }

            if (Object.entries(singleDataElement).length !== 0 && singleDataElement.value !== 'on') {
                dataToSend.push(singleDataElement);
            }
        });

        console.log(dataToSend);

        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, dataToSend).done(function (data) {
            var currentExpensesListPlaceholder = $('#currentExpensesDiv');
            var coveredExpensesListPlaceholder = $('#executedExpensesDiv');

            placeholderElement.find('#modalExpenseCreate').modal('hide');

            cleanPlaceholder(currentExpensesListPlaceholder);
            cleanPlaceholder(coveredExpensesListPlaceholder);
            resetSettlementTabsLabelsText();

            var currentState = { searchPeriodInDays: 3 };
            var managerGenerationInfo = getManagerGenerationInfo(currentState);
            var deconstructModulesInfo = ({ coveredSettlementInfo, coveredExpensesInfo, currentSettlementInfo, currentExpensesInfo }) => ({ coveredSettlementInfo, coveredExpensesInfo, currentSettlementInfo, currentExpensesInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });

    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
}
function initializeFormCheckboxesBlockCurrentUser(outerSelector, wrapperSelector) {
    initializeDivideBillEqually(outerSelector, wrapperSelector);
    initializeSettleWholeBillCheckboxes(outerSelector, wrapperSelector, true);
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

        console.log(dataToSend);
        //TODO: check if working for bad values - modal shouldn't hide
        $.post(actionUrl, dataToSend).done(function (data) {
            placeholderElement.find('#modalNewShoppingListCreate').modal('hide');

            cleanPlaceholder($('#plannedShoppingDiv'));

            var managerGenerationInfo = getManagerGenerationInfo();
            var deconstructModulesInfo = ({ shoppingInfo }) => ({ shoppingInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
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
//end

//RBmodule
function activateSingleEditRBCard(recurringBillId) {
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
}
function activateEditRBCards() {
    var placeholderElement = $('#rbDetailsDiv');
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
                        case 'ExpirationDate':
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

        console.log(dataToSend);
        $.post(actionUrl, dataToSend).done(function (data) {
            var billsListPlaceholder = $('#rbsInProgressDiv');

            cleanPlaceholder(placeholderElement);
            cleanPlaceholder(billsListPlaceholder);

            var managerGenerationInfo = getManagerGenerationInfo();
            var deconstructModulesInfo = ({ RBInfo }) => ({ RBInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });
}
function activateSingleDeleteRBCard(recurringBillId) {
    var actionUrl = '/BudgetManager/BillRemoval';
    var actionData = {
        RBId: recurringBillId
    };

    var placeholderElementSelector = '#rbDetailsDiv';
    var placeholderElement = $(placeholderElementSelector);
    var successHandler = {
        function: fillPlaceholderWithResult,
        params: {
            placeholderSelector: placeholderElementSelector
        }
    };

    cleanPlaceholder(placeholderElement);
    generateManagerModuleInfo(actionUrl, actionData, successHandler);
}
function activateDeleteRBCards() {
    var placeholderElement = $('#rbDetailsDiv');
    placeholderElement.on('click', '[data-delete="card"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.card').find('form');
        var deleteBillUrl = form.attr('action');
        var deleteBillData = { billId: document.getElementById('Id').value };

        //TODO: check if working for bad values - modal shouldn't hide
        $.ajax({
            type: "DELETE",
            url: deleteBillUrl,
            data: deleteBillData,
            success: function () {
                var billsListPlaceholder = $('#rbsInProgressDiv');

                cleanPlaceholder(placeholderElement);
                cleanPlaceholder(billsListPlaceholder);

                var managerGenerationInfo = getManagerGenerationInfo();
                var deconstructModulesInfo = ({ RBInfo }) => ({ RBInfo });
                var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
                for (const value of Object.values(modulesGenerationInfo)) {
                    generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
                }
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

            var managerGenerationInfo = getManagerGenerationInfo();
            var deconstructModulesInfo = ({ RBInfo }) => ({ RBInfo });
            var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });

    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
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
                inputElem.setAttribute('type', 'checkbox');
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
//end

//EXPmodule
function setPastExpensesPeriodSelectionHandler() {
    var expensePeriodSelect = document.getElementById('expenseHistoryDurationSelect');
    expensePeriodSelect.addEventListener('change', function () {
        var selectedValue = this.value;
        var displayedDays;
        switch (selectedValue) {
            case "3days":
                displayedDays = 3;
                break;
            case "1week":
                displayedDays = 7;
                break;
            case "1month":
                displayedDays = 30;
                break;
        }

        var currentState = { searchPeriodInDays: displayedDays };
        var managerGenerationInfo = getManagerGenerationInfo(currentState);
        var deconstructModulesInfo = ({ coveredSettlementInfo, coveredExpensesInfo }) => ({ coveredSettlementInfo, coveredExpensesInfo });
        var modulesGenerationInfo = deconstructModulesInfo(managerGenerationInfo);
        for (const value of Object.values(modulesGenerationInfo)) {
            generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
        }
    });
}
function assignSumsToLabelsAndCreateChart({ labelsSelectors, result }) {
    assignSettlementsSumsToLabels({ labelsSelectors, result });
    createExpenseChart(result);
}
function createExpenseChart(result) {
    var expensePeriodSelect = document.getElementById('expenseHistoryDurationSelect');
    switch (expensePeriodSelect.value) {
        case "3days":
            createChart({ data: 3, moneyFlowData: result, useShortDateFormat: false });
            break;
        case "1week":
            createChart({ data: 7, moneyFlowData: result, useShortDateFormat: false });
            break;
        case "1month":
            createChart({ data: 30, moneyFlowData: result, useShortDateFormat: true });
            break;
    }    
}

function createChart({ days, moneyFlowData, useShortDateFormat }) {
    var currentDate = new moment();
    var labels = [];
    for (var i = 0; i < days; i++) {
        if (useShortDateFormat) {
            labels.push(currentDate.add(-i, 'd').format("MMM"));
        }
        else {
            labels.push(currentDate.add(-i, 'd').format("MMM Do GGGG"));
        }
    }
    
    var credibilityData = getSummarizedMoneyFlowData(moneyFlowData, "perDateCredibilities");
    var liabilityData = getSummarizedMoneyFlowData(moneyFlowData, "perDateLiabilities");

    var ctx = document.getElementById('myChart').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                createChartDataset('Incomes', credibilityData, "green", "green", 'incomes-axis'),
                createChartDataset('Expenses', liabilityData, "red", "red", 'expenses-axis')
            ]
        }
    });
}

function createChartDataset(label, data, backgroundColor, borderColor, yAxisId) {
    return {
        label: label,
        data: data,
        backgroundColor: backgroundColor,
        borderColor: borderColor,
        yAxisID: yAxisId
    };
}

function getSummarizedMoneyFlowData(result, key) {
    var data = [], dataPerDay = 0;

    result[key].forEach((dateValue, index, array) => {

        dateValue.forEach((value, index, array) => {
            dataPerDay += value["value"];
        });
        
        data.push(dataPerDay);
    });

    return data;
}
//end

//SETmodule
function initializeCoverExpensesButton(dashboardGenerationInfo) {
    $('#coverExpensesButton').click(function () {
        var checkedCheckboxes = $('#currentExpensesDiv input[type="checkbox"]:checked');
        var checkboxIds = checkedCheckboxes.map((key, value) => value.id);
        var expenseIds = [], userIds = [], groupIds = [];
        checkboxIds.each((index, element) => {
            var [prefix, expenseId, userId, groupId] = element.split('-');
            expenseIds.push(parseInt(expenseId));
            userIds.push(parseInt(userId));
            groupIds.push(parseInt(groupId));
        });

        var actionUrl = '/BudgetManager/MarkExpensesAsCovered';
        var actiondata = { expenseIds: expenseIds, userIds: userIds, groupIds: groupIds };
        var currentExpensesPlaceholder = $('#currentExpensesDiv');
        var coveredExpensePlaceholder = $('#executedExpensesDiv');

        $.post(actionUrl, actiondata).done(function (data) {
            cleanPlaceholder(currentExpensesPlaceholder);
            cleanPlaceholder(coveredExpensePlaceholder);
            var deconstructModulesInfo = ({ currentSettlementInfo, coveredSettlementInfo, currentExpensesInfo, coveredExpensesInfo }) => ({ currentSettlementInfo, coveredSettlementInfo, currentExpensesInfo, coveredExpensesInfo });
            var modulesGenerationInfo = deconstructModulesInfo(dashboardGenerationInfo);
            for (const value of Object.values(modulesGenerationInfo)) {
                generateManagerModuleInfo(value.actionUrl, value.actionData, value.onSuccess);
            }
        });
    });
}
function initializeCoverCheckboxes() {
    var currentExpensesDiv = $('#currentExpensesDiv');
    var currentSettlementsDiv = $('#currentSettlementsDiv');

    var singleCheckboxChecked = 1, noCheckboxesChecked = 0;

    currentExpensesDiv.on('click', '[data-check="expense-cover"]', function (event) {

        var checkboxes = $('#currentExpensesDiv input[type="checkbox"]');

        var checkboxesChecked = checkboxes.filter(':checked').length;
        if (checkboxesChecked === singleCheckboxChecked) {
            showCoverExpensesButton();
        }
        else {
            if (checkboxesChecked === noCheckboxesChecked) {
                hideCoverExpensesButton();
            }
        }
    });

    currentSettlementsDiv.on('click', '[data-toggle="tab"]', function (event) {
        var activeTabId = getActiveExpenseButtonTab();
        uncheckExpenseCheckboxes(activeTabId);
        hideCoverExpensesButton();
    });
}
function getActiveExpenseButtonTab() {
    var nonactiveButton = $('#currentExpensesTab button.active');
    return nonactiveButton.attr("href");
}
function uncheckExpenseCheckboxes(settlementDivSelector) {
    $(settlementDivSelector + ' input[type="checkbox"]').prop('checked', false);
}
function hideCoverExpensesButton() {
    $('#coverExpensesButton').css('visibility', 'hidden');
}
function showCoverExpensesButton() {
    $('#coverExpensesButton').css('visibility', 'visible');
}
function resetSettlementTabsLabelsText() {
    $('#currentLiabilitiesLabel').html("SUM : Loading..");
    $('#currentCredibilitiesLabel').html("SUM : Loading..");
}
//end
