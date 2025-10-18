import { formatCurrency,updateTotal } from '../utils.js';

updateTotal();
$(document).ready(function () {
    
    $("input:radio[name=selection]").change(function () {
        const deliveryId = $(this).val();

        // درخواست Ajax به سرور برای دریافت قیمت ارسال
        $.get('/Home/GetDeliveryPrice', { deliveryId }, function (res) {
            if (res.success) {
                $("#deliveryPrice").text(formatCurrency(res.price));

                // اگر تابع updateTotal در صفحه تعریف شده بود، اجراش کن
                if (typeof updateTotal === "function") {
                    updateTotal();
                }

            } else {
                toastr.error(res.message || 'خطا در دریافت هزینه ارسال');
            }
        }).fail(function () {
            toastr.error('ارتباط با سرور برقرار نشد');
        });
    });
});

