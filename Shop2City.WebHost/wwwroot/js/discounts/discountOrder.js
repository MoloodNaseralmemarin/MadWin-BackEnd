$(document).ready(function () {
    $(document).on("click", "#applyDiscountBtnMobile, .applyDiscountBtn", function (e) {
        e.preventDefault();

        const discountCode = $(this).closest(".card-body").find(".discountCode").val() || $(".discountCode").first().val();

        if (!discountCode) {
            toastr.warning("لطفا کد تخفیف را وارد کنید.");
            return;
        }

        $.post("/Orders/UseDiscount", { orderId: orderId, discountCode: discountCode, __RequestVerificationToken: token })
            .done(function (res) {
                if (res.success) {
                    $("#disTotal").text(formatCurrency(res.discountAmount));
                    updateTotal();
                    toastr.success(res.message);
                } else {
                    toastr.error(res.message);
                }
            })
            .fail(function () {
                toastr.error("خطا در ارتباط با سرور");
            });
    });
});
