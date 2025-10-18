// اعتبارسنجی فرم
window.validateData = () => {
    if (!$("#firstName").val()) {
        toastr.error("نام را وارد کنید");
        return false;
    }
    if (!$("#lastName").val()) {
        toastr.error("نام خانوادگی را وارد کنید");
        return false;
    }
    if (!$("#telPhone").val()) {
        toastr.error("شماره ثابت را وارد کنید");
        return false;
    }
    if (!$("#cellPhone").val()) {
        toastr.error("شماره همراه را وارد کنید");
        return false;
    }
    if (!$("#address").val()) {
        toastr.error("آدرس پستی را وارد کنید");
        return false;
    }
    if (!$("#password").val()) {
        toastr.error("لطفاً کلمه عبور را وارد کنید");
        return false;
    }
    if (!$("#confirmPassword").val()) {
        toastr.error("لطفاً ‌تکرار کلمه عبور را وارد کنید");
        return false;
    }
    return true;
};