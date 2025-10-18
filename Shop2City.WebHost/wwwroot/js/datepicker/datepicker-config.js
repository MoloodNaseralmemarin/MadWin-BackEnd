document.addEventListener('DOMContentLoaded', function () {

    $('#startDate').persianDatepicker(
        {
            observer: true,
            format: 'YYYY/MM/DD',
            Datepicker: '#startDate',
            autoClose: true,
            minDate: new persianDate().unix()

        });
    $('#endDate').persianDatepicker(
        {
            observer: true,
            format: 'YYYY/MM/DD',
            Datepicker: '#endDate',
            autoClose: true
        });
});