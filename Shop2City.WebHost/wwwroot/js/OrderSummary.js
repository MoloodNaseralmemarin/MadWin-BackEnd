// --- Helpers ---
function parseNumber(text) {
    return Number((text || "0").toString().replace(/[^\d]/g, '')) || 0;
}

function formatCurrency(num) {
    return num.toLocaleString();
}

// --- محاسبه جمع ---
function updateTotal() {
    let subtotal = 0;

    $(".item-price").each(function () {
        subtotal += parseNumber($(this).text());
    });

    let delivery = parseNumber($("#deliveryPrice").text());
    let disTotal = parseNumber($("#disTotal").text());

    let total = subtotal - disTotal + delivery;
    if (total < 0) total = 0;

    $("#SubtotalPrice").text(formatCurrency(subtotal));
    $("#sumOrder").text(formatCurrency(total));
    $("#sumOrderBtn").text(formatCurrency(total));
}

// --- حذف آیتم‌ها ---
function removeItems(orderId = null) {
    let selectedIds = [];

    if (orderId) {
        // حالت موبایل → فقط یک آیتم خاص
        selectedIds.push(orderId);
    } else {
        // حالت دسکتاپ → همه چک‌باکس‌های انتخاب‌شده
        $(".delete-checkbox:checked").each(function () {
            selectedIds.push($(this).val());
        });

        if (selectedIds.length === 0) {
            toastr.warning('هیچ موردی انتخاب نشده است.');
            return;
        }
    }

    $.ajax({
        url: '/Orders/RemoveItemsByOrder',
        type: 'POST',
        data: { orderId: selectedIds },
        traditional: true
    })
        .done(function (response) {
            if (response.success) {
                toastr.success('حذف با موفقیت انجام شد.');

                selectedIds.forEach(id => {
                    // حذف ردیف اصلی و جزئیات
                    let mainRow = $("input.delete-checkbox[value='" + id + "']").closest("tr");
                    let detailRow = $("tr.detail-row[data-orderid='" + id + "']");
                    mainRow.add(detailRow).remove();

                    // موبایل
                    $(".card").has("a[onclick='removeItems(" + id + ")']").remove();
                });

                updateTotal();
                $("#selectAll").prop("checked", false);

                // بروزرسانی جمع کل از سرور
                $.getJSON("/Orders/GetSumPriceWithFeeByOrder", { orderId: selectedIds[0] }, function (data) {
                    if (data && data.price !== undefined) {
                        $("#SubtotalPrice").text(formatCurrency(data.price));
                        updateTotal();
                    }
                });

            } else {
                toastr.error('خطا در حذف.');
            }
        })
        .fail(function () {
            toastr.error('مشکلی در ارتباط با سرور رخ داد.');
        });
}

// --- رویدادها ---
$(document).ready(function () {
    updateTotal();

    const orderId = $(".orderId").first().val();
    const token = $('input[name="__RequestVerificationToken"]').val();
    let selectedDeliveryId = null;

    // انتخاب همه
    $("#selectAll").on("change", function () {
        $(".delete-checkbox").prop("checked", $(this).is(":checked"));
    });

    // هماهنگ‌سازی selectAll
    $(document).on("change", ".delete-checkbox", function () {
        $("#selectAll").prop(
            "checked",
            $(".delete-checkbox").length === $(".delete-checkbox:checked").length
        );
    });

    // تغییر روش ارسال
    $("input:radio[name=selection]").on("change", function () {
        selectedDeliveryId = $(this).val();
        $("#postButton").prop("disabled", false);

        $.get("/Home/GetDeliveryPrice", { deliveryId: selectedDeliveryId })
            .done(function (result) {
                if (result.success) {
                    $("#deliveryPrice").text(formatCurrency(result.price));
                    updateTotal();
                }
            });
    });

    // اعمال کد تخفیف
    $("#applyDiscountBtn, #applyDiscountBtnMobile").click(function (e) {
        e.preventDefault();
        let discountCode = $(this).siblings(".discountCode").val();

        $.post("/Orders/UseDiscount", {
            orderId: parseInt(orderId),
            discountCode: discountCode,
            __RequestVerificationToken: token
        })
            .done(function (response) {
                if (response.success) {
                    $("#disTotal").text(formatCurrency(response.discountAmount));
                    updateTotal();
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            })
            .fail(function (xhr) {
                console.error(xhr.responseText);
                toastr.error("خطا در ارتباط با سرور");
            });
    });

    // پرداخت آنلاین
    $("#postButton").click(function () {
        let sumOrder = parseNumber($("#sumOrder").text());

        $.ajax({
            url: '/Payment/ProcessPayment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                sumOrder: sumOrder,
                invoiceId: parseInt(orderId),
                deliveryId: selectedDeliveryId,
                source: 'order'
            })
        })
            .done(function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    toastr.error(response.message);
                }
            })
            .fail(function (xhr, status, error) {
                console.error(error);
                toastr.error("خطا در ارسال درخواست پرداخت");
            });
    });
});
