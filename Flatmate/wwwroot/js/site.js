// Write your JavaScript code.




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
//});