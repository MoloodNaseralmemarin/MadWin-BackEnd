
import { parseNumber } from '../utils.js';

$(document).ready(function () {

    $("#postButton").on("click", function () {
        const factorId = $("#factorId").val();
        const deliveryId = $("input[name=selection]:checked").val();
        const sumPrice = parseNumber($("#sumPrice").text());
        const description = $("#description").val();
        const token = $('input[name="__RequestVerificationToken"]').val();
        debugger
        console.log(description);

        if (!factorId || !deliveryId)
            return toastr.warning("روش ارسال هنوز انتخاب نشده است.");
        $.ajax({
            url: '/Payment/ProcessPayment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ invoiceId: factorId, deliveryId, sumPrice, description: description, source: 'factor' }),
            headers: { 'RequestVerificationToken': token },
            success: function (res) {
                if (res.success) window.location.href = res.redirectUrl;
                else toastr.error(res.message);
            },
            error: function () {
                toastr.error("خطا در برقراری ارتباط با سرور.");
            }
        });
    });

});
