function updateTotal() {
    let subtotal = 0;

    // جمع قیمت‌ها از ستون‌های جدول
    $(".item-price").each(function () {
        let priceText = $(this).text();
        let price = Number(priceText.replace(/[^\d]/g, ''));
        subtotal += price;
    });

    // هزینه ارسال
    let deliveryText = $("#deliveryPrice").text();
    let delivery = Number(deliveryText.replace(/[^\d]/g, ''));

    // تخفیف
    let disTotalText = $("#disTotal").text();
    let disTotal = Number(disTotalText.replace(/[^\d]/g, ''));

    // محاسبه جمع کل
    let total = subtotal - disTotal + delivery;
    if (total < 0) total = 0;

    // نمایش در صفحه
    $("#SubtotalPrice").text(subtotal.toLocaleString() + " ریال");
    $("#sumOrder").text(total.toLocaleString() + " ریال");
    $("#sumOrderBtn").text(total.toLocaleString() + " ریال");
}

// حذف آیتم‌ها
function removeItems(orderId) {
    var selectedIds = [];
    $(".delete-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning('هیچ موردی انتخاب نشده است.');
        return;
    }

    $.ajax({
        url: '/Orders/RemoveItemsByOrder',
        type: 'POST',
        data: { orderId: selectedIds },
        traditional: true,
        success: function (response) {
            if (response.success) {
                toastr.success("آیتم‌ها با موفقیت حذف شدند.");

                // حذف ردیف‌ها از جدول
                $(".delete-checkbox:checked").closest("tr").remove();

                // بروزرسانی جمع
                updateTotal();

                // ریست کردن selectAll
                $("#selectAll").prop("checked", false);

            } else {
                toastr.error("خطا در حذف آیتم‌ها.");
            }
        },
        error: function () {
            toastr.error("مشکلی در برقراری ارتباط با سرور رخ داد.");
        }
    });
}

let selectedDeliveryId = null;

$(document).ready(function () {
    // بارگذاری اولیه جمع
    updateTotal();

    // گرفتن orderId و توکن CSRF
    var orderId = $(".orderId").first().val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    // وقتی چک‌باکس بالای جدول تغییر کرد
    $("#selectAll").on("change", function () {
        let isChecked = $(this).is(":checked");
        $(".delete-checkbox").prop("checked", isChecked);
    });

    // اگر یکی از چک‌باکس‌های تک‌تک تغییر کرد، وضعیت selectAll را بروز کن
    $(document).on("change", ".delete-checkbox", function () {
        let allChecked = $(".delete-checkbox").length === $(".delete-checkbox:checked").length;
        $("#selectAll").prop("checked", allChecked);
    });

    // تغییر روش ارسال
    $("input:radio[name=selection]").on("change", function () {
        selectedDeliveryId = $(this).val();
        $("#postButton").prop("disabled", false);

        $.ajax({
            type: "GET",
            url: "/Home/GetDeliveryPrice",
            data: { deliveryId: selectedDeliveryId },
            success: function (result) {
                if (result.success) {
                    $("#deliveryPrice").text(result.price.toLocaleString());
                    updateTotal();
                }
            }
        });
    });

    // اعمال تخفیف (دسکتاپ و موبایل)
    $("#applyDiscountBtn, #applyDiscountBtnMobile").click(function (e) {
        e.preventDefault();
        var discountCode = $(this).siblings(".discountCode").val();

        $.ajax({
            url: "/Orders/UseDiscount",
            type: 'POST',
            data: {
                orderId: parseInt(orderId),
                discountCode: discountCode,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    $("#disTotal").text(response.discountAmount.toLocaleString());
                    updateTotal();
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                console.error(xhr.responseText);
                alert("خطا در ارتباط با سرور");
            }
        });
    });

    // پرداخت آنلاین
    $("#postButton").click(function () {
        var sumOrder = Number($("#sumOrder").text().replace(/[^\d]/g, ''));

        const dataToSend = {
            sumOrder: sumOrder,
            invoiceId: parseInt(orderId),
            deliveryId: selectedDeliveryId,
            source: 'order'
        };

        $.ajax({
            url: '/Payment/ProcessPayment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataToSend),
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
                alert("خطا در ارسال درخواست پرداخت");
            }
        });
    });
});
