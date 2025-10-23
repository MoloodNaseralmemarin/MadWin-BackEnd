


import { formatCurrency, updateTotal } from '../utils.js';

// discountOrder.js

    // 🔹 اعمال تخفیف دسکتاپ
    $('#applyDiscountBtn').click(function (e) {
        e.preventDefault();
        applyDiscount("#discountCode");
    });

    // 🔹 اعمال تخفیف موبایل
    $('#applyDiscountBtnMobile').click(function (e) {
        e.preventDefault();
        applyDiscount("#discountCodeMobile");
    });

    // 🔹 تابع مشترک اعمال تخفیف
    function applyDiscount(inputSelector) {
        const discountCode = $(inputSelector).val().trim();
        const orderId = $("#orderId").val();
        const token = $("input[name='__RequestVerificationToken']").val();

        if (!discountCode) {
            toastr.warning("لطفا کد تخفیف را وارد کنید.");
            return;
        }

        $.post("/Orders/UseDiscountForOrder", {
            orderId: orderId,
            discountCode: discountCode,
            __RequestVerificationToken: token
        })
            .done(function (res) {
                if (res.success) {
                    $("#distotalPrice").text(formatCurrency(res.discountAmount));
                    updateTotal();
                    toastr.success(res.message);
                } else {
                    toastr.error(res.message);
                }
            })
            .fail(function () {
                toastr.error("خطا در ارتباط با سرور");
            });
    }
