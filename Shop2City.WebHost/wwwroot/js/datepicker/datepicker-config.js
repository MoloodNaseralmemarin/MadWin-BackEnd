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


$(document).ready(function () {
    $("#FromDate").persianDatepicker({
        format: 'YYYY/MM/DD',
        initialValueType: 'gregorian',
        altField: '#FromDateHidden',
        altFormat: 'YYYY/MM/DD',
        autoClose: true,
        calendarType: 'persian'
    });

    $("#ToDate").persianDatepicker({
        format: 'YYYY/MM/DD',
        initialValueType: 'gregorian',
        altField: '#ToDateHidden',
        altFormat: 'YYYY/MM/DD',
        autoClose: true,
        calendarType: 'persian'
    });
});
