$(document).ready(function () {
  var wto;

  $('#report-from').datetimepicker({
    locale: 'ru',
    maxDate: moment(),
    format: 'MM.YYYY',
    showClear: true,
    showClose: true,
    icons: {
      clear: 'fa fa-trash',
      close: 'fa fa-times',
      today: 'fa fa-calendar-check-o'
    },
    tooltips: {
      today: 'Текущая дата',
      clear: 'Очистить выбор',
      close: 'Закрыть окно',
      nextMonth: 'Следующий месяц',
      prevMonth: 'Предыдущий месяц',
      selectMonth: 'Выбрать месяц',
      selectYear: 'Выбрать год',
      selectDecade: 'Выбрать десятилетие',
      nextYear: 'Следующий год',
      nextDecade: 'Следующее десятилетие',
      prevYear: 'Предыдущий год',
      prevDecade: 'Предыдущее десятилетие'
    }
  });

  $('#report-to').datetimepicker({
    locale: 'ru',
    maxDate: moment(),
    format: 'MM.YYYY',
    showClear: true,
    showClose: true,
    icons: {
      clear: 'fa fa-trash',
      close: 'fa fa-times',
      today: 'fa fa-calendar-check-o'
    },
    tooltips: {
      today: 'Текущая дата',
      clear: 'Очистить выбор',
      close: 'Закрыть окно',
      nextMonth: 'Следующий месяц',
      prevMonth: 'Предыдущий месяц',
      selectMonth: 'Выбрать месяц',
      selectYear: 'Выбрать год',
      selectDecade: 'Выбрать десятилетие',
      nextYear: 'Следующий год',
      nextDecade: 'Следующее десятилетие',
      prevYear: 'Предыдущий год',
      prevDecade: 'Предыдущее десятилетие'
    }
  });

  $(document).on('submit', '#report-form', function(e) {
    e.preventDefault();
    clearTimeout(wto);

    wto = setTimeout(function() {
      $('#progress-bar').animate({ opacity: 1.0 }, 100, function() {
        var submitButton = $('#submit-button');
        submitButton.text('Формирование...');
        submitButton.prop('disabled', true);
        var data = {
          reportType: $('#report-type').val(),
          from: $('#report-from-date').val(),
          to: $('#report-to-date').val()
        };

        $.ajax({
          url: '/Report/Report/',
          type: 'POST',
          data: data,
          success: function (report) {
            submitButton.text('Сформировать');
            submitButton.prop('disabled', false);
            $('#report').html(report);
            $('#progress-bar').animate({ opacity: 0.0 }, 100);
          }
        });
      });      
    }, 1000);   
  });
});