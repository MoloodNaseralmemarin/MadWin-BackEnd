function addToCart(productId) {
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            const totalCount = response.totalCount || 0;
            const totalPrice = response.totalPrice || 0;

            const badgeContainer = $("#CartBadgeContainer");
            const cartTotal = $("#Cart-total");
            const priceTotal = $("#Price-total");
            const cartBadge = $("#cartBadge");

            // آپدیت عدد و قیمت
            cartTotal.text(totalCount);
            if (priceTotal.length) {
                priceTotal.text(totalPrice.toLocaleString());
            }

            // نمایش یا پنهان کردن badge بر اساس تعداد
            if (totalCount > 0) {
                badgeContainer.show();
                cartBadge.css({ 'opacity': '1', 'pointer-events': 'auto' });
            } else {
                badgeContainer.hide();
                cartBadge.css({ 'opacity': '0.5', 'pointer-events': 'none' });
            }

            // پیام موفقیت
            toastr.success('با موفقیت به سبد خرید اضافه شد.', 'پیام سیستم');
        },
        error: function () {
            toastr.error('خطا در افزودن محصول به سبد خرید.', 'پیام سیستم');
        }
    });
}