function editIsStatusProduct(checkbox, productId) {
    const isStatus = $(checkbox).is(":checked"); // گرفتن وضعیت true/false

    $.post("/Admin/Products/EditIsStatusProduct",
        {
            isStatus: isStatus,
            productId: productId
        })
        .done(function (res) {
            if (res.success) {
                toastr.success(res.message);
            } else {
                toastr.error(res.message);
            }
        })
        .fail(function () {
            toastr.error("خطا در ارتباط با سرور");
        });
}
