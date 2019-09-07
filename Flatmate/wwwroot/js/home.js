
//Dashboard code

function generateDashboardView() {
    $(document).ready(function () {
        var dashboardGenerationInfo = getDashboardGenerationInfo();
        dashboardGenerationInfo.forEach((value, index, array) => {
            generateHomeModuleInfo(value.actionUrl, value.actionData, value.placeholderElement);
        });
        addNewEventPeriodSelectionHandler();
    });
}

function getDashboardGenerationInfo() {

    var dashboardGenerationInfo = [
        {
            actionUrl: '/Home/ListShoppingInformation',
            actionData: {},
            placeholderElement: $('#plannedShoppingDiv')
        },
        {
            actionUrl: '/Home/ListRecurringBills',
            actionData: {},
            placeholderElement: $('#plannedExpensesDiv')
        },
        {            
            actionUrl: '/Home/ListUpcomingEvents',
            actionData: {},
            placeholderElement: $('#upcomingEventsDiv')
        },
        {
            actionUrl: '/Home/ListSettlementInformation',
            actionData: {},
            placeholderElement: $('#expensesBalanceDiv')
        }
    ];

    return dashboardGenerationInfo;
}

function generateHomeModuleInfo(actionUrl, actionData, placeholderElement) {
    $.ajax({
        type: "GET",
        url: actionUrl,
        data: actionData,
        success: function (result) {
            placeholderElement.html(result);
        }
    });
}

function addNewEventPeriodSelectionHandler() {
    var eventPeriodSelect = document.getElementById('eventPeriodSelect');
    eventPeriodSelect.addEventListener('change', function () {
        var selectedValue = this.value;
        var displayedDays;
        switch (selectedValue) {
            case "3days":
                displayedDays = 3;
                break;
            case "1week":
                displayedDays = 7;
                break;
            case "2weeks":
                displayedDays = 14;
                break;
            case "3weeks":
                displayedDays = 21;
                break;
        }

        var actionUrl = '/Home/ListUpcomingEvents';
        var actionData = { displayedDays: displayedDays };
        var placeholderElement = $('#upcomingEventsDiv');
        $.ajax({
            type: "GET",
            url: actionUrl,
            data: actionData,
            success: function (result) {
                placeholderElement.html(result);
            }
        });
    });
}
