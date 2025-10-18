﻿
import { parseNumber } from '../utils.js';

$(document).ready(function () {

    $("#postButton").on("click", function () {
        const orderId = $("#orderId").val();
        const deliveryId = $("input[name=selection]:checked").val();
        const sumPrice = parseNumber($("#sumPrice").text());
        const token = $('input[name="__RequestVerificationToken"]').val();

        if (!orderId || !deliveryId)
            return toastr.warning("روش ارسال هنوز انتخاب نشده است.");
        $.ajax({
            url: '/Payment/ProcessPayment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ invoiceId: orderId, deliveryId, sumPrice, source: 'order' }),
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
