$(function () {
    const getToken = () => $("input[name='__RequestVerificationToken']").val();

    // حذف تکی
    $(document).on("click", ".remove-item-btn", function () {
        const $btn = $(this);
        let id = $btn.data("factorid") || $btn.closest("[data-factorid]").data("factorid");

        console.log("click remove single id:", id);
        if (id === undefined || id === null) {
            toastr.warning('هیچ آیتمی انتخاب نشده است.');
            return;
        }

        removeItemsByIds([id]);
    });

    // حذف چندتایی
    $(document).on("click", "#deleteSelectedBtn", function () {
        const selectedIds = $(".delete-checkbox:checked")
            .map(function () {
                const v = $(this).val();
                return (v === undefined || v === null) ? null : String(v).trim();
            })
            .get()
            .filter(v => v !== "")
            .map(v => {
                const n = parseInt(v, 10);
                return isNaN(n) ? v : n;
            });

        console.log("✅ selectedIds:", selectedIds);

        if (selectedIds.length === 0) {
            toastr.warning('هیچ موردی انتخاب نشده است.');
            return;
        }

        removeItemsByIds(selectedIds);
    });

    // انتخاب همه
    $(document).on("change", "#selectAll", function () {
        const isChecked = $(this).is(":checked");
        $(".delete-checkbox").prop("checked", isChecked).trigger("change");
        $("#deleteSelectedBtn").prop("disabled", !isChecked);
    });

    // بررسی وضعیت دکمه حذف و چک‌باکس اصلی
    $(document).on("change", ".delete-checkbox", function () {
        const totalCheckboxes = $(".delete-checkbox").length;
        const checkedCount = $(".delete-checkbox:checked").length;

        $("#deleteSelectedBtn").prop("disabled", checkedCount === 0);
        $("#selectAll").prop("checked", checkedCount === totalCheckboxes);
    });

    // تابع حذف
    const removeItemsByIds = ids => {
        if (!ids || ids.length === 0) {
            toastr.warning('هیچ موردی انتخاب نشده است.');
            return;
        }

        console.log("Sending remove request for ids:", ids);

        const $deleteBtn = $("#deleteSelectedBtn");
        $deleteBtn.prop("disabled", true);

        $.ajax({
            url: "/Factors/RemoveItemsByFactor",
            type: "POST",
            data: {
                factorId: ids,
                __RequestVerificationToken: getToken()
            },
            traditional: true
        })
            .done(res => {
                if (!res || !res.success) {
                    toastr.error((res && res.message) || 'خطا در حذف آیتم‌ها');
                    return;
                }

                const successMessage = ids.length > 1 ? 'آیتم‌ها با موفقیت حذف شدند.' : 'آیتم با موفقیت حذف شد.';
                toastr.success(successMessage);

                // ریست چک‌باکس‌ها
                $(".delete-checkbox, #selectAll").prop("checked", false);
                $("#deleteSelectedBtn").prop("disabled", true);

                // چند لحظه صبر کن تا پیام دیده بشه، بعد رفرش کن
                setTimeout(() => {
                    location.reload();
                }, 1200); // 1.2 ثانیه تأخیر
            })
            .fail(() => {
                toastr.error('خطا در ارتباط با سرور');
            })
            .always(() => {
                $deleteBtn.prop("disabled", false);
            });
    };

    console.log("initial checkbox count:", $(".delete-checkbox").length);
});
