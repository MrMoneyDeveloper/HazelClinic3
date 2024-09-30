$(function () {
    var startDatePicker = $("#start-date");
    var endDatePicker = $("#end-date");

    startDatePicker.datepicker({
        changeMonth: true,
        minDate: new Date(),
        onSelect: function (selectedDate) {
            endDatePicker.datepicker("option", "minDate", selectedDate);
        }
    });

    endDatePicker.datepicker({
        changeMonth: true,
        beforeShowDay: function (date) {
            var startDate = startDatePicker.datepicker("getDate");
            if (startDate) {
                var startDateMs = startDate.getTime();
                var dateMs = date.getTime();

                if (dateMs < startDateMs) {
                    return [false];
                }
            }
            return [true];
        }
    });
});


  
