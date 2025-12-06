$(document).ready(function () {
    var errorMessage = $('#toastrError').val();
    if (errorMessage) {
        toastr.error(errorMessage);
    }

    var successMessage = $('#toastrSuccess').val();
    if (successMessage) {
        toastr.success(successMessage);
    }
});
