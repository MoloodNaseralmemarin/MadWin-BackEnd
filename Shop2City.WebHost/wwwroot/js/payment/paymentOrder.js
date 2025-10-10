
$(document).ready(function () {
    $("#postButton").click(function () {
        const sumOrder = parseNumber($("#sumOrder").text());
        $.ajax({
            url: '/Payment/ProcessPayment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ sumOrder: sumOrder, invoiceId: orderId, deliveryId: selectedDeliveryId, source: 'order' }),
            headers: { "RequestVerificationToken": token }
        })
            .done(function (res) {
                if (res.success) window.location.href = res.redirectUrl;
                else toastr.error(res.message);
            })
            .fail(function () {
                toastr.error("خطا در ارسال درخواست پرداخت");
            });
    });
});
