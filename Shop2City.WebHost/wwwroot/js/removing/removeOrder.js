
const token = $("input[name='__RequestVerificationToken']").val();

// 🔹 حذف تکی
$(document).on("click", ".remove-item-btn", function () {
    const $btn = $(this);
    const id = $btn.data("orderid") || $btn.closest("[data-orderid]").data("orderid");

    if (!id) {
        toastr.warning('هیچ آیتمی انتخاب نشده است.');
        return;
    }

    removeItemsByIds([id]);
});

// 🔹 حذف چندتایی با چک‌باکس
$("#deleteSelectedBtn").on("click", function () {
    const selectedIds = $(".delete-checkbox:checked").map(function () {
        return $(this).val();
    }).get();

    removeItemsByIds(selectedIds);
});

// 🔹 انتخاب/عدم انتخاب همه چک‌باکس‌ها
$("#selectAll").on("change", function () {
    const isChecked = $(this).is(":checked");
    $(".delete-checkbox").prop("checked", isChecked);
    $("#deleteSelectedBtn").prop("disabled", !isChecked);
});

// 🔹 فعال/غیرفعال کردن دکمه حذف چندتایی
$(document).on("change", ".delete-checkbox", function () {
    const anyChecked = $(".delete-checkbox:checked").length > 0;
    $("#deleteSelectedBtn").prop("disabled", !anyChecked);
    $("#selectAll").prop(
        "checked",
        $(".delete-checkbox:checked").length === $(".delete-checkbox").length
    );
});

// 🔹 تابع حذف با انیمیشن
const removeItemsByIds = ids => {
    if (!ids || ids.length === 0) {
        toastr.warning('هیچ موردی انتخاب نشده است.');
        return;
    }

    $.ajax({
        url: "/Orders/RemoveItemsByOrder",
        type: "POST",
        data: { orderId: ids, __RequestVerificationToken: token },
        traditional: true
    })
        .done(res => {
            if (!res.success) {
                toastr.error(res.message || 'خطا در حذف آیتم‌ها');
                return;
            }

            toastr.success('آیتم‌ها با موفقیت حذف شدند.');

            ids.forEach(id => {
                $(`[data-orderid='${id}']`).animate({ opacity: 0, height: 0, padding: 0 }, 400, function () {
                    $(this).remove();

                    // اگر همه آیتم‌ها پاک شد
                    if ($("#ordersContainer [data-orderid]").length === 0) {
                        $("#ordersContainer").hide();
                        $("#emptyMessage").show();
                    }

                });
            });

            // ریست چک‌باکس‌ها و دکمه‌ها
            $(".delete-checkbox, #selectAll").prop("checked", false);
            $("#deleteSelectedBtn").prop("disabled", true);
            // ✅ چند لحظه تأخیر برای نمایش پیام، سپس رفرش
            setTimeout(() => {
                location.reload();
            }, 1000); // ۱ ثانیه تأخیر برای نمایش بهتر پیام
        })
        .fail(() => toastr.error('خطا در ارتباط با سرور'));
};
