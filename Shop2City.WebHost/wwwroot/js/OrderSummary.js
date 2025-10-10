// Helper functions
function parseNumber(text) {
    return Number((text || "0").toString().replace(/[^\d]/g, '')) || 0;
}

function formatCurrency(num) {
    return num.toLocaleString();
}

// Update total calculation
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

// Remove items function
function removeItems(orderId = null) {
    let selectedIds = [];

    if (orderId) {
        selectedIds.push(orderId);
    } else {
        $(".delete-checkbox:checked").each(function () {
            selectedIds.push($(this).val());
        });

        if (selectedIds.length === 0) {
            toastr.warning('هیچ موردی انتخاب نشده است.');
            return;
        }
    }

    // Get CSRF token
    const token = $('#csrfToken').val();

    $.ajax({
        url: '/Orders/RemoveItemsByOrder',
        type: 'POST',
        data: { orderId: selectedIds, __RequestVerificationToken: token },
        traditional: true
    })
        .done(function (response) {
            if (response.success) {
                toastr.success('حذف با موفقیت انجام شد.');

                selectedIds.forEach(id => {
                    let mainRow = $("input.delete-checkbox[value='" + id + "']").closest("tr");
                    let detailRow = $("tr.detail-row[data-orderid='" + id + "']");
                    mainRow.add(detailRow).remove();

                    $(".card").has("a[onclick='removeItems(" + id + ")']").remove();
                });

                updateTotal();
                $("#selectAll").prop("checked", false);

                // Optional: Refresh subtotal from server
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

$(document).ready(function () {
    updateTotal();

    const orderId = $(".orderId").first().val();
    const token = $('#csrfToken').val();
    let selectedDeliveryId = null;

    // Select all checkboxes
    $("#selectAll").on("change", function () {
        $(".delete-checkbox").prop("checked", $(this).is(":checked"));
    });

    // Sync selectAll checkbox
    $(document).on("change", ".delete-checkbox", function () {
        $("#selectAll").prop(
            "checked",
            $(".delete-checkbox").length === $(".delete-checkbox:checked").length
        );
    });

    // Change delivery method
    $(document).on("change", "input:radio[name=selection]", function () {
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

    // Apply discount code (both desktop and mobile)
    $(document).on("click", ".applyDiscountBtn", function (e) {
        e.preventDefault();
        let discountCode = $(this).siblings(".discountCode").val();

        if (!discountCode) {
            toastr.warning("لطفا کد تخفیف را وارد کنید.");
            return;
        }

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

    // Online payment
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
            }),
            headers: { "RequestVerificationToken": token }
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