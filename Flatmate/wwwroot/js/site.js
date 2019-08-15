// Write your JavaScript code.

// Scheduler code

function generateCalendarHeaderInner() {
    var cornerFieldClasses = ["border-light-dotted", "border-no-top-left"];
    var dayFieldsClasses = ["yellow", "lighten-5", "border-light-dotted", "border-no-top", "text-center"];
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

    var currentDateInfo = moment();
    var dayNumber = currentDateInfo.weekday();
    
    
    weekDays.forEach((value, index, array) => {
        var elem = document.getElementById("day_th_" + index);
        var headerMoment = parseCalendarHeadDateToMoment(index);
        elem.innerHTML = regenerateCalendarHeaderSingleText(value, headerMoment, setNextWeek ? 1 : -1);
    });    
}
function generateCalendarHeaderSingleText(weekDay, currentDayNumber, referenceDayNumber, headerMoment = moment(), weekShift = 0) {
    return weekDay + "<br /><small>" + displayDateWithShift(headerMoment, currentDayNumber - (referenceDayNumber - 1), weekShift) + "</small>";
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

    return moment([parsedSetDateInfo[2], (parsedSetDateInfo[1]-1)%12, parsedSetDateInfo[0]]);
}
function generateCalendarCells() {
    var leftColumnClasses = ["green", "lighten-5", "text-center", "border-light-dotted", "border-no-top-left"];
    var innerCellsClasses = ["border-light-dotted", "border-no-top"];

    var timePeriodEndings = ["12:00 PM", "4:00 AM", "8:00 AM", "12:00 AM", "4:00 PM", "8:00 PM"];

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

        var weekDays = 7;
        for (j = 0; j < weekDays; j++) {
            var innerCell = document.createElement("td");
            innerCellsClasses.forEach((value, index, array) => {
                innerCell.classList.add(value);
            });
            innerRow.appendChild(innerCell);
        }
        tBody.appendChild(innerRow);
    }
}
function populateSchedulerWithEventData() {
    var url = "/Scheduler/ListEventInfo";
    var data = { id: "1" };
    $.ajax({

            type: "GET",
            url: url,
            data: data,
        success: function (result) {

            }
    });
}
// Scheduler code - end

    
// TODO: jak niepotrzebne będzie to usunąć kod poniżej
// Poniżej kod do manualnego obrabiania wnętrza request'u

//$(document).ready(function () {
//    $("#btnSubmit").click(function () {

//        var myformdata = $("#myForm").serializeArray();
//        console.log(JSON.stringify(myformdata));
//        var url = '/Home/Create';

//        //var formObject = [];
//        //var invitationAddress = [];
//        //var antiForgeryToken;

//        //myformdata.forEach(function (value, index) {
//        //    console.log(JSON.stringify(value) + ' ' + index);
//        //    if (value.name !== "InvitationEmails[4]") {
//        //        if (value.name === "__RequestVerificationToken") {
//        //            antiForgeryToken = value;
//        //        }
//        //        else {
//        //            formObject.push(value);
//        //        }
//        //    }
//        //    else {
//        //        invitationAddress.push(value.value);
//        //    }
//        //});
//        //formObject.push({ "name": "InvitationEmails", "value": invitationAddress });
//        //formObject.push(antiForgeryToken);

//        $.ajax({

//            type: "POST",
//            url: url,
//            data: myformdata,
//            success: function () {
//                $("#myModal").modal("hide");
//            }
//        });
//    });
// });