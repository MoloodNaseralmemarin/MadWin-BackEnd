// اعتبارسنجی فرم
window.validateData = () => {
    if (!$("#userName").val()) {
        toastr.error("نام کاربری را وارد کنید");
        return false;
    }
    if (!$("#password").val()) {
        toastr.error("لطفاً کلمه عبور را وارد کنید");
        return false;
    }
    return true;
};

