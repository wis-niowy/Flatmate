
// Scheduler code

function generateCalendarView() {
    $(document).ready(function () {
        generateCalendarHeaderInner();
        generateCalendarCells();
        populateSchedulerWithEventData();
        initializeNewEventModal();
        initializeEventDetailsModal();
    });
}
function generateCalendarHeaderInner() {
    var cornerFieldClasses = ["border-light-dotted", "border-no-top-left"];
    var dayFieldsClasses = ["yellow", "lighten-5", "border-light-dotted", "border-no-top", "text-center", "calendar-cell-width"];
    var weekDays = getWeekDaysNames();

    var dateRow = document.getElementById("dateRow");

    var corner_th = document.createElement("th");
    for (i = 0; i < cornerFieldClasses.length; i++) {
        corner_th.classList.add(cornerFieldClasses[i]);
    }
    dateRow.appendChild(corner_th);

    var day_th;
    var current_date = new moment();
    var dayNumber = current_date.weekday();

    for (i = 0; i < weekDays.length; i++) {
        day_th = document.createElement("th");
        day_th.id = "day_th_" + i;
        day_th.innerHTML = generateCalendarHeaderSingleText(weekDays[i], i, dayNumber);
        for (j = 0; j < dayFieldsClasses.length; j++) {
            day_th.classList.add(dayFieldsClasses[j]);
        }
        dateRow.appendChild(day_th);
    }
}
function displayDateWithShift(moment, daysShift, weeksShift = 0) {
    return moment.add({ days: daysShift, weeks: weeksShift }).format('DD-MM-YYYY');
}
function displayDateWithWeekShift(moment, weeksShift = 0) {
    return moment.add({ weeks: weeksShift }).format('DD-MM-YYYY');
}
function getWeekDaysNames() {
    return ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
}
function shiftDates(setNextWeek) {
    var weekDays = getWeekDaysNames();
    
    weekDays.forEach((value, index, array) => {
        var elem = document.getElementById("day_th_" + index);
        var headerMoment = parseCalendarHeadDateToMoment(index);
        elem.innerHTML = regenerateCalendarHeaderSingleText(value, headerMoment, setNextWeek ? 1 : -1);
    });
    
    clearWeeklyEvents();
    populateSchedulerWithEventData();
}
function clearWeeklyEvents() {
    var weekDaysNumber = 7, hourPeriods = 6;
    for (i = 0; i < weekDaysNumber; i++) {
        for (j = 0; j < hourPeriods; j++) {
            var cellId = i.toString() + '-' + j.toString();
            var cell = document.getElementById(cellId);
            while (cell.firstChild) {
                cell.removeChild(cell.firstChild);
            }
        }
    }
}
function generateCalendarHeaderSingleText(weekDay, currentDayNumber, referenceDayNumber, headerMoment = moment(), weekShift = 0) {
    return weekDay + "<br /><small>" + displayDateWithShift(headerMoment, currentDayNumber - referenceDayNumber, weekShift) + "</small>";
}
function generateInnerCellId(weekDay, rowIndex) {
    return weekDay + '-' + rowIndex;
}
function regenerateCalendarHeaderSingleText(weekDay, headerMoment = moment(), weekShift = 0) {
    return weekDay + "<br /><small>" + displayDateWithWeekShift(headerMoment, weekShift) + "</small>";
}
function parseCalendarHeadDateToMoment(index) {
    var smallItemIndex = 1;
    var elem = document.getElementById("day_th_" + index).children[smallItemIndex].innerHTML;
    var parsedSetDateInfo = elem.split("-");
    parsedSetDateInfo.forEach((value, index, array) => {
        array[index] = parseInt(value);
    });

    return moment([parsedSetDateInfo[2], (parsedSetDateInfo[1] - 1) % 12, parsedSetDateInfo[0]]);
}
function generateCalendarCells() {
    var leftColumnClasses = ["green", "lighten-5", "text-center", "border-light-dotted", "border-no-top-left", "align-middle"];
    var innerCellsClasses = ["border-light-dotted", "border-no-top", "small-inner-padding", "overflow-hidden"];

    var timePeriodEndings = ["12:00 AM", "4:00 AM", "8:00 AM", "12:00 PM", "4:00 PM", "8:00 PM"];

    var tBody = document.getElementById("calendarBody");

    var innerRows = 6;
    for (i = 0; i < innerRows; i++) {
        var innerRow = document.createElement("tr");
        var leftColumnCell = document.createElement("td");
        leftColumnClasses.forEach((value, index, array) => {
            leftColumnCell.classList.add(value);
        });

        leftColumnCell.innerHTML = "<small>" + timePeriodEndings[i] + "<br />-<br />" + timePeriodEndings[(i + 1) % timePeriodEndings.length] + "</small>";
        innerRow.appendChild(leftColumnCell);

        var current_date = new moment();
        var dayNumber = current_date.weekday();
        var weekDays = 7;
        for (j = 0; j < weekDays; j++) {
            var innerCell = document.createElement("td");
            innerCellsClasses.forEach((value, index, array) => {
                innerCell.classList.add(value);
            });

            innerCell.id = generateInnerCellId(j, i);
            innerRow.appendChild(innerCell);
        }
        tBody.appendChild(innerRow);
    }
}
function populateSchedulerWithEventData() {
    var listEventInfoUrl = "/Scheduler/ListPeriodEventInfo";

    var indexOfstartingWeekHeader = 0,
        indexOfEndingWeekHeader = 6;    
    var listEventInfoData = {
        id: "1",
        weekStart: parseCalendarHeadDateToMoment(indexOfstartingWeekHeader).format("YYYY MM DD hh:mm:ss a"),
        weekEnd: parseCalendarHeadDateToMoment(indexOfEndingWeekHeader).add(1, 'days').format("YYYY MM DD hh:mm:ss a")
    };

    $.ajax({
        type: "GET",
        url: listEventInfoUrl,
        data: listEventInfoData,
        success: function (result) {
            result.sort((a, b) => {
                return new Date(a.startDate) - new Date(b.startDate);
            });
            result.forEach((value, index, array) => {
                var splitStartDate = value.startDate.split('T'),
                    splitEndDate = value.endDate.split('T');
                
                var cellId = generateCalendarCellIdFromDate(splitStartDate);

                var timePartNumber = 1;
                var startDateHour = splitStartDate[timePartNumber].split(":");
                var endDateHour = splitEndDate[timePartNumber].split(":");

                var cellToInjectEventElement = document.getElementById(cellId);
                var eventButton = initializeEventDetailsButton(value.title, startDateHour, endDateHour, index, value.id);
                cellToInjectEventElement.appendChild(eventButton);
            });
        }
    });
}

function initializeEventDetailsButton(eventTitle, startDateHour, endDateHour, buttonNumber, eventId) {
    var button = document.createElement("button");
    button.innerHTML = '<small>' + eventTitle + '<br/>' + startDateHour[0] + ':' + startDateHour[1] + ' - ' + endDateHour[0] + ':' + endDateHour[1] + '</small>';
    button.classList.add("btn-sm", "blue", "lighten-2", "no-inner-padding", "full-width-break-text", "animated", "bounceIn");
    button.setAttribute("data-toggle", "ajax-event-details-modal");
    button.setAttribute("data-target", "modalEventDetailsShow");
    var buttonId = 'eventButton-' + buttonNumber;
    button.id = buttonId;

    const actionToCofirmType = {
        DELETE: 'delete',
        UNSUBSCRIBE: 'unsub'
    };

    var placeholderElement = $('#event-details-modal-placeholder');
    button.addEventListener("click", function () {
        var eventDetailsUrl = "/Scheduler/EventDetails";
        var eventDetailsData = { eventId: eventId.toString() };
        $.get(eventDetailsUrl, eventDetailsData).done(function (data) {
            placeholderElement.html(data);

            var eventDeleteBtn = document.getElementById('eventDeleteBtn');
            eventDeleteBtn.addEventListener('click', function () {
                if ($.find('#confirmDelete').length === 0) {
                    removeConfirmSectionIfExists('#confirmUnsub');

                    var modalContentDiv = $('.modal-content');
                    modalContentDiv.append(generateConfirmActionSection(buttonId, actionToCofirmType.DELETE));
                }
            });

            var eventUnsubBtn = document.getElementById('eventUnsubBtn');
            eventUnsubBtn.addEventListener('click', function () {
                if ($.find('#confirmUnsub').length === 0) {
                    removeConfirmSectionIfExists('#confirmDelete');

                    var modalContentDiv = $('.modal-content');
                    modalContentDiv.append(generateConfirmActionSection(buttonId, actionToCofirmType.UNSUBSCRIBE));
                }
            });

            var startDateInput = document.getElementById('StartDate');
            var endDateInput = document.getElementById('EndDate');
            var parsedStartDate = parseToDatepickersDate(startDateInput.getAttribute('value'));
            var parsedEndDate = parseToDatepickersDate(endDateInput.getAttribute('value'));
            startDateInput.setAttribute('value', parsedStartDate);
            endDateInput.setAttribute('value', parsedEndDate);
            
            var editEventDetails = document.getElementById('editEventDetails');
            editEventDetails.setAttribute("data-clicked", "false");
            var modalNonEdit = placeholderElement.find('.modal');
            modalNonEdit.find('#EndDate').attr('value', parsedEndDate);
            modalNonEdit.find('#StartDate').attr('value', parsedStartDate);
            var modalBodyHtml = modalNonEdit.find('.modal-body')[0].outerHTML;
            var modalFooterHtml = modalNonEdit.find('.modal-footer')[0].outerHTML;
            var modalNonEditData = { modalBodyHtml: modalBodyHtml, modalFooterHtml: modalFooterHtml };
            
            modalNonEdit.modal('show');
            editEventDetails.addEventListener("click", function () { enterEventDetailsEditMode(modalNonEditData, editEventDetails, [parsedStartDate, parsedEndDate], buttonId); });
        });
    });

    return button;
}
function generateConfirmActionSection(eventButtonId, action) {

    const actionToCofirmType = {
        DELETE: 'delete',
        UNSUBSCRIBE: 'unsub'
    };

    var actionConfirmDiv = document.createElement('div');
    var divId = action === actionToCofirmType.DELETE ? 'confirmDelete' : 'confirmUnsub';
    actionConfirmDiv.setAttribute('id', divId);
    actionConfirmDiv.classList.add('modal-footer', 'd-block');

    var labelDiv = document.createElement('div');
    labelDiv.classList.add('w-100', 'd-flex', 'justify-content-center');

    var confirmLabel = document.createElement('label');
    var innerText = action === actionToCofirmType.DELETE ? 'Confirm delete?' : 'Confirm unsubscribe?';
    confirmLabel.innerText = innerText;
    labelDiv.appendChild(confirmLabel);
    actionConfirmDiv.appendChild(labelDiv);

    var confirmButtonsDiv = document.createElement('div');
    confirmButtonsDiv.classList.add('d-flex', 'justify-content-center');

    var cancelButton = document.createElement('button');
    cancelButton.classList.add('btn', 'btn-dark');
    cancelButton.innerHTML = 'No';
    cancelButton.addEventListener('click', function () {
        $('#' + divId).remove();
    });
    confirmButtonsDiv.appendChild(cancelButton);
    var confirmButton = document.createElement('button');
    confirmButton.classList.add('btn', 'btn-dark');
    confirmButton.innerHTML = 'Yes';
    confirmButton.addEventListener('click', function () {
        action === actionToCofirmType.DELETE ? confirmDelete(eventButtonId) : confirmUnsubcribe(eventButtonId);
    });
    confirmButtonsDiv.appendChild(confirmButton);
    actionConfirmDiv.appendChild(confirmButtonsDiv);
    return actionConfirmDiv;
}
function confirmDelete(deleteButtonId) {
    var deleteEventUrl = '/Scheduler/DeleteEvent';
    var eventId = document.getElementById('Id').value.toString();
    var deleteEventData = { eventId: eventId };
    $.ajax({
        type: "DELETE",
        url: deleteEventUrl,
        data: deleteEventData,
        success: function () {
            $('#' + deleteButtonId).remove();
            var placeholderElement = $('#event-details-modal-placeholder');
            placeholderElement.find('.modal').modal('hide');
        }
    });
}
function confirmUnsubcribe(unsubButtonId) {
    var unsubEventUrl = '/Scheduler/UnsubFromEvent';
    var eventId = document.getElementById('Id').value.toString();
    var unsubEventData = { eventId: eventId };
    $.ajax({
        type: "PUT",
        url: unsubEventUrl,
        data: unsubEventData,
        success: function () {
            $('#' + unsubButtonId).remove();
            var placeholderElement = $('#event-details-modal-placeholder');
            placeholderElement.find('.modal').modal('hide');
        }
    });
}

function initializeDatePickersWithInitialDates(firstDTPSelector, secondDTPSelector, parsedStartDate, parsedEndDate) {
    var currentDate = new Date();
    $(firstDTPSelector).datetimepicker('maxDate', parsedEndDate);
    $(secondDTPSelector).datetimepicker('minDate',
        currentDate > new Date(parsedStartDate) ? currentDate : parsedStartDate
    );

    linkDatetimepickersBySelectors(firstDTPSelector, secondDTPSelector);
}
function initializeDatePickers(firstDTPSelector, secondDTPSelector) {

    //Initialize datetimepickers
    $(firstDTPSelector).datetimepicker({
        'minDate': new Date()
    });
    $(secondDTPSelector).datetimepicker({
        'useCurrent': false
    });

    linkDatetimepickersBySelectors(firstDTPSelector, secondDTPSelector);
}
function linkDatetimepickersBySelectors(firstDTPSelector, secondDTPSelector) {
    
    $(firstDTPSelector).on("change.datetimepicker", function (e) {
        var currentDate = new Date();
        $(secondDTPSelector).datetimepicker('minDate',
            currentDate > e.date ? moment(currentDate) : e.date
        );
    });
    $(secondDTPSelector).on("change.datetimepicker", function (e) {
        $(firstDTPSelector).datetimepicker('maxDate', e.date);
    });
}
function generateCalendarCellIdFromDate(splitDate) {

    var datePartNumber = 0, timePartNumber = 1, monthsNumber = 12;
    var hourDatePart = splitDate[timePartNumber].split(":");
    var startHoursMinutes = [hourDatePart[0], hourDatePart[1]];
    var datePart = splitDate[datePartNumber].split("-");
    var eventMoment = moment().year(datePart[0]).month((datePart[1] - 1) % monthsNumber).date(datePart[2]);
    var weekDay = eventMoment.weekday();

    var cellId = weekDay + '-' + Math.floor(startHoursMinutes[0] / 4);
    return cellId;
}

function enterEventDetailsEditMode(nonEditModalData, editButtonHandle, parsedDates, buttonId) {
    var isEditModeEntered = editButtonHandle.getAttribute("data-clicked");
    
    var participantsLabel = document.createElement("label");
    var startDateDatetimepicker = document.getElementById('startDateEventDetails');
    var endDateDatetimepicker = document.getElementById('endDateEventDetails');
    var groupIdInput = document.getElementById("GroupId");
    switch (isEditModeEntered) {
        case 'false':
            editButtonHandle.setAttribute('data-clicked', 'true');

            if ($.find('#confirmDelete').length !== 0) {
                $('#confirmDelete').remove();
            }

            var formInputs = $('#EventDetailsForm input,textarea');
            formInputs.each((index, element) => {
                element.removeAttribute('disabled');
            });

            var startDateDatetimepickerSelector = '#' + startDateDatetimepicker.id;
            var endDateDatetimepickerSelector = '#' + endDateDatetimepicker.id;
            var startDateInnerDiv = generateDatepickerInputGroup(startDateDatetimepickerSelector);
            var endDateInnerDiv = generateDatepickerInputGroup(endDateDatetimepickerSelector);
            startDateDatetimepicker.appendChild(startDateInnerDiv);
            endDateDatetimepicker.appendChild(endDateInnerDiv);

            initializeDatePickersWithInitialDates(startDateDatetimepickerSelector, endDateDatetimepickerSelector, parsedDates[0], parsedDates[1]);

            var eventParticipantsWrapper = document.getElementById("eventParticipantsWrapper");
            while (eventParticipantsWrapper.firstChild) {
                eventParticipantsWrapper.removeChild(eventParticipantsWrapper.firstChild);
            }

            participantsLabel.innerHTML = "Choose participants: ";
            participantsLabel.setAttribute("for", "ParticipantIds");
            eventParticipantsWrapper.appendChild(participantsLabel);

            var groupId = groupIdInput.getAttribute("value");
            var listGroupMembersInfoUrl = "/Teams/ListGroupMembersInfo";
            var listGroupMembersInfoData = { groupId: groupId };

            //TODO: change id to currentUser with cookies maybe
            var currentUserId = "1";
            $.ajax({
                type: "GET",
                url: listGroupMembersInfoUrl,
                data: listGroupMembersInfoData,
                success: function (result) {
                    result.forEach((value, index, array) => {
                        if (value.id.toString() === currentUserId) {
                            return;
                        }
                        var groupMembersDiv = document.createElement("div");
                        groupMembersDiv.classList.add("form-check");

                        var divLabel = document.createElement("label");
                        divLabel.classList.add("form-check-label");

                        var inputElem = document.createElement("input");
                        inputElem.setAttribute('type', 'checkbox');
                        inputElem.setAttribute('name', 'ParticipantIds' + '-' + value.id);
                        inputElem.classList.add("form-check-input");

                        divLabel.appendChild(inputElem);
                        divLabel.append(value.fullName);
                        groupMembersDiv.appendChild(divLabel);
                        eventParticipantsWrapper.appendChild(groupMembersDiv);
                    });
                }
            });

            var modalFooter = $('.modal-footer');
            modalFooter.find('#eventDeleteBtn').remove();
            modalFooter.find('#eventUnsubBtn').remove();
            var confirmEditButton = $("<button data-save='modal'></button>");
            confirmEditButton.addClass("btn btn-dark");
            confirmEditButton.attr({
                id: "btnSubmit",
                type: "submit"
            });
            confirmEditButton.html("Edit!");
            modalFooter.append(confirmEditButton);         
            break;
        case 'true':
            editButtonHandle.setAttribute('data-clicked', 'false');

            $('.modal-body').replaceWith(nonEditModalData.modalBodyHtml);
            $('.modal-footer').replaceWith(nonEditModalData.modalFooterHtml);

            const actionToCofirmType = {
                DELETE: 'delete',
                UNSUBSCRIBE: 'unsub'
            };

            removeConfirmSectionIfExists('#confirmDelete');
            removeConfirmSectionIfExists('#confirmUnsub');

            var eventDeleteBtn = document.getElementById('eventDeleteBtn');
            eventDeleteBtn.addEventListener('click', function () {
                if ($.find('#confirmDelete').length === 0) {
                    removeConfirmSectionIfExists('#confirmUnsub');

                    var modalContentDiv = $('.modal-content');
                    modalContentDiv.append(generateConfirmActionSection(buttonId, actionToCofirmType.DELETE));
                }
            });

            var eventUnsubBtn = document.getElementById('eventUnsubBtn');
            eventUnsubBtn.addEventListener('click', function () {
                if ($.find('#confirmUnsub').length === 0) {
                    removeConfirmSectionIfExists('#confirmDelete');

                    var modalContentDiv = $('.modal-content');
                    modalContentDiv.append(generateConfirmActionSection(buttonId, actionToCofirmType.UNSUBSCRIBE));
                }
            });
            break;
    }
}

function removeConfirmSectionIfExists(sectionSelector) {
    var confirmSection = $.find(sectionSelector);
    if (confirmSection.length === 1) {
        $(sectionSelector).remove();
    }
}

function generateDatepickerInputGroup(datepickerSelector) {
    var outerDatePickerDiv = document.createElement("div");
    outerDatePickerDiv.classList.add('input-group-append');
    outerDatePickerDiv.setAttribute('data-target', datepickerSelector);
    outerDatePickerDiv.setAttribute('data-toggle', 'datetimepicker');

    var innerDatePickerDiv = document.createElement("div");
    innerDatePickerDiv.classList.add('input-group-text');
    innerDatePickerDiv.innerHTML = '<i class="fa fa-calendar"></i>';
    outerDatePickerDiv.appendChild(innerDatePickerDiv);
    return outerDatePickerDiv;
}
function initializeEventDetailsModal() {
    var placeholderElement = $('#event-details-modal-placeholder');
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
                        singleDataElement.value = parseDatepickersDate(value.value);
                        startTime = singleDataElement.value.split(' ');
                        break;
                    case 'EndDate':
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
        dataToSend.pop();

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            placeholderElement.find('.modal-body').replaceWith(newBody);

            var isValid = newBody.find('[name="IsValid"]').val() === 'True';
            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
            }
            clearWeeklyEvents();
            populateSchedulerWithEventData();
        });
    });

    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
}
function initializeNewEventModal() {
    var placeholderElement = $('#new-event-modal-placeholder');

    $('button[data-toggle="ajax-new-event-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            initializeDatePickers('#startDateNewEvent', '#endDateNewEvent');
            initializeNewItemSelectGroup('groupNESelect', 'groupMembersNEWrapper');
            placeholderElement.find('#modalNewEventCreate').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var primaryData = form.serializeArray();

        var startDate;
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
                        singleDataElement.value = parseDatepickersDate(value.value);
                        startDate = moment(singleDataElement.value);
                        break;
                    case 'EndDate':
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
            var startWeekDayIndex = 0, endWeekDayIndex = 6;
            var weekStartDate = parseCalendarHeadDateToMoment(startWeekDayIndex),
                weekEndDate = setMomentEndDayHour(parseCalendarHeadDateToMoment(endWeekDayIndex));

            placeholderElement.find('#modalNewEventCreate').modal('hide');
            
            if (weekStartDate <= startDate && startDate < weekEndDate) {
                clearWeeklyEvents();
                populateSchedulerWithEventData();
            }
        });
    });

    placeholderElement.on('hidden.bs.modal', '.modal', function (event) {
        placeholderElement.find('.modal').remove();
    });
}

function setMomentEndDayHour(moment) {
    return moment.hour(23).minute(59).second(59);
}

function parseDatepickersDate(dateString) {
    var splitDate = dateString.split(/[.\s]+/);
    splitDate = splitDate[2] + '-' + splitDate[1] + '-' + splitDate[0] + ' ' + splitDate[3] + ':00';
    return splitDate;
}
function parseToDatepickersDate(dateString) {
    var splitDate = dateString.split(/[-\s]+/);
    splitDate = splitDate[2] + '.' + splitDate[1] + '.' + splitDate[0] + ' ' + splitDate[3].substring(0,5);
    return splitDate;
}

function initializeNewItemSelectGroup(selectId, groupMembersWrapperId) {
    //TODO: change to teams or UsersController
    var listGroupInfoUrl = "/Scheduler/ListGroupInfo";
    //TODO: change to currentUserId
    var selectData = { userId: "1" };
    $.ajax({
        type: "GET",
        url: listGroupInfoUrl,
        data: selectData,
        success: function (result) {
            var groupSelect = document.getElementById(selectId);
            result.forEach((value, index, array) => {
                var selectOption = document.createElement("option");
                selectOption.text = value["name"];
                selectOption.value = value["id"];
                groupSelect.appendChild(selectOption);
            });

            $("#" + selectId).on('change', function () {
                var listGroupMembersUrl = "/Teams/ListGroupMembersInfo";
                var optionsData = { groupId: $('#' + selectId).find(':selected').val() };
                $.ajax({
                    type: "GET",
                    url: listGroupMembersUrl,
                    data: optionsData,
                    success: function (result) {
                        var outerDiv = document.getElementById(groupMembersWrapperId);
                        while (outerDiv.firstChild) {
                            outerDiv.removeChild(outerDiv.firstChild);
                        }

                        var outerLabel = document.createElement("label");
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
            });
        }
    });
}

function initializeNewItemSelectGroupWithSelectionHandler(selectId, groupMembersWrapperId) {
    var listGroupInfoUrl = "/Scheduler/ListGroupInfo";
    //TODO: change to currentUserId
    var selectData = { userId: "1" };
    $.ajax({
        type: "GET",
        url: listGroupInfoUrl,
        data: selectData,
        success: function (result) {
            var groupSelect = document.getElementById(selectId);
            result.forEach((value, index, array) => {
                var selectOption = document.createElement("option");
                selectOption.text = value["name"];
                selectOption.value = value["id"];
                groupSelect.appendChild(selectOption);
            });

            $("#" + selectId).on('change', function () {
                var listGroupMembersUrl = "/Teams/ListGroupMembersInfo";
                var optionsData = { groupId: $('#' + selectId).find(':selected').val() };
                $.ajax({
                    type: "GET",
                    url: listGroupMembersUrl,
                    data: optionsData,
                    success: function (result) {
                        var outerDiv = document.getElementById(groupMembersWrapperId);
                        while (outerDiv.firstChild) {
                            outerDiv.removeChild(outerDiv.firstChild);
                        }

                        var outerLabel = document.createElement("label");
                        outerLabel.setAttribute("for", "ParticipantIds");
                        outerLabel.innerHTML = "Attach your friends:";
                        outerDiv.appendChild(outerLabel);

                        var groupMemberDivId, divLabel, inputElem,
                            detailsChargingDiv, chargeLabel, innerChargeLabel, chargeInput,
                            isPaidLabel, isPaidInput, isPaidHiddenInput, smallIsPaidLabel;
                        
                        var participantChargeInputName = "ParticipantsCharge",
                            isChargePaidName = "DidParticipantsPay";

                        result.forEach((value, index, array) => {
                            if (value.id.toString() === selectData.userId) {
                                var groupMembersDiv = document.createElement("div");
                                groupMemberDivId = "memberDiv-" + index;
                                groupMembersDiv.id = groupMemberDivId;
                                groupMembersDiv.classList.add("form-check");

                                divLabel = document.createElement("label");
                                divLabel.classList.add("form-check-label");

                                inputElem = document.createElement("input");
                                inputElem.setAttribute('type', 'hidden');
                                inputElem.setAttribute('name', 'ParticipantIds' + '-' + value["id"]);
                                inputElem.value = true;

                                divLabel.appendChild(inputElem);
                                divLabel.append(value["fullName"]);
                                groupMembersDiv.appendChild(divLabel);

                                outerDiv.appendChild(groupMembersDiv);

                                detailsChargingDiv = document.createElement('div');
                                detailsChargingDiv.id = 'chargingDiv-' + index;

                                chargeLabel = document.createElement('label');
                                chargeLabel.classList.add("px-1", "col-3");

                                innerChargeLabel = document.createElement('small');
                                innerChargeLabel.innerHTML = "Charge value:";
                                chargeLabel.appendChild(innerChargeLabel);
                                detailsChargingDiv.appendChild(chargeLabel);

                                chargeInput = document.createElement('input');
                                chargeInput.classList.add("col-4", "mr-3", "d-inline", "form-control", "form-control-sm");
                                chargeInput.name = participantChargeInputName;
                                detailsChargingDiv.appendChild(chargeInput);

                                isPaidLabel = document.createElement('label');
                                isPaidLabel.classList.add("px-1", "col-4", "pl-3", "big-screen-left-border", "form-check-inline");

                                isPaidInput = document.createElement('input');
                                isPaidInput.name = isChargePaidName;
                                isPaidInput.type = 'checkbox';
                                isPaidInput.id = 'currentUserCheckbox';
                                isPaidInput.setAttribute('checked', true);
                                isPaidInput.setAttribute('disabled', true);
                                isPaidInput.classList.add("my-3", "form-check-input");
                                isPaidLabel.appendChild(isPaidInput);

                                isPaidHiddenInput = document.createElement('input');
                                isPaidHiddenInput.id = 'hidden-' + index;
                                isPaidHiddenInput.name = isChargePaidName;
                                isPaidHiddenInput.type = 'hidden';
                                isPaidHiddenInput.value = 'true';
                                isPaidLabel.appendChild(isPaidHiddenInput);

                                smallIsPaidLabel = document.createElement('small');
                                smallIsPaidLabel.innerHTML = "Bill was settled";
                                isPaidLabel.appendChild(smallIsPaidLabel);
                                detailsChargingDiv.appendChild(isPaidLabel);
                                groupMembersDiv.appendChild(detailsChargingDiv);
                            }
                            else {
                                groupMembersDiv = document.createElement("div");
                                groupMemberDivId = "memberDiv-" + index;
                                groupMembersDiv.id = groupMemberDivId;
                                groupMembersDiv.classList.add("form-check");

                                divLabel = document.createElement("label");
                                divLabel.classList.add("form-check-label");

                                inputElem = document.createElement("input");
                                inputElem.setAttribute('type', 'checkbox');
                                inputElem.setAttribute('name', 'ParticipantIds' + '-' + value["id"]);
                                inputElem.classList.add("form-check-input");
                                inputElem.addEventListener('click', function () {
                                    if (document.getElementById('chargingDiv-' + index) === null) {

                                        detailsChargingDiv = document.createElement('div');
                                        detailsChargingDiv.id = 'chargingDiv-' + index;

                                        chargeLabel = document.createElement('label');
                                        chargeLabel.classList.add("px-1", "col-3");

                                        innerChargeLabel = document.createElement('small');
                                        innerChargeLabel.innerHTML = "Charge value:";
                                        chargeLabel.appendChild(innerChargeLabel);
                                        detailsChargingDiv.appendChild(chargeLabel);

                                        chargeInput = document.createElement('input');
                                        chargeInput.classList.add("col-4", "mr-3", "d-inline", "form-control", "form-control-sm");
                                        chargeInput.name = participantChargeInputName;
                                        detailsChargingDiv.appendChild(chargeInput);

                                        isPaidLabel = document.createElement('label');
                                        isPaidLabel.classList.add("px-1", "col-4", "pl-3", "big-screen-left-border", "form-check-inline");

                                        isPaidInput = document.createElement('input');
                                        isPaidInput.name = isChargePaidName;
                                        isPaidInput.type = 'checkbox';
                                        isPaidInput.classList.add("my-3", "form-check-input");
                                        isPaidInput.addEventListener('change', function () {
                                            const currentValue = $('#hidden-' + index).val();
                                            console.log(currentValue);
                                            if (currentValue === "false") {
                                                $('#hidden-' + index).val("true");
                                            }
                                            else {
                                                $('#hidden-' + index).val("false");
                                            }
                                        });
                                        isPaidLabel.appendChild(isPaidInput);

                                        isPaidHiddenInput = document.createElement('input');
                                        isPaidHiddenInput.id = 'hidden-' + index;
                                        isPaidHiddenInput.name = isChargePaidName;
                                        isPaidHiddenInput.type = 'hidden';
                                        isPaidHiddenInput.value = 'false';
                                        isPaidLabel.appendChild(isPaidHiddenInput);

                                        smallIsPaidLabel = document.createElement('small');
                                        smallIsPaidLabel.innerHTML = "Bill was settled";
                                        isPaidLabel.appendChild(smallIsPaidLabel);
                                        detailsChargingDiv.appendChild(isPaidLabel);
                                        groupMembersDiv.appendChild(detailsChargingDiv);
                                    }
                                    else {
                                        $('#chargingDiv-' + index).remove();
                                    }

                                    var isDivideEquallyChecked = $('#divideBillEquallyCheckbox').prop('checked');

                                    if (isDivideEquallyChecked) {
                                        var groupMembersWrapperSelector = '#' + groupMembersWrapperId;
                                        setChargeValues(groupMembersWrapperSelector);
                                    }
                                });

                                divLabel.appendChild(inputElem);
                                divLabel.append(value["fullName"]);
                                groupMembersDiv.appendChild(divLabel);

                                outerDiv.appendChild(groupMembersDiv);
                            }
                        });      

                        var isDivideEquallyChecked = $('#divideBillEquallyCheckbox').prop('checked');
                        if (isDivideEquallyChecked === true) {
                            setChargeValues('#groupMembersExpenseCreateWrapper');
                        }
                    }
                });
            });
        }
    });
}

// Scheduler code - end