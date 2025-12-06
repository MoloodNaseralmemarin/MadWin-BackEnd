function removeCart(productId) {
    $.post('/Cart/Remove', { productId: productId }, function (response) {
        updateCartUI(response);
        debugger
        toastr.success('محصول انتخابی با موفقیت از سبد خرید حذف شد.');
    }).fail(function () {
        toastr.error('خطا در حذف محصول از سبد خرید. لطفاً دوباره تلاش کنید.');
    });
}