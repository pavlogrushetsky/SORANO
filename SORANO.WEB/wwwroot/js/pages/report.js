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

    initGenericSelect({
        selectElementClass: '#select-location',
        valueElementId: '#location-id',
        displayElementId: '#location-name',
        noResultsText: 'Магазины не найдены',
        searchingText: 'Поиск магазинов...',
        errorLoadingText: 'Невозможно загрузить результаты поиска',
        placeholderText: 'Магазин',
        url: '/Location/GetLocations'
    });

    $(document).on('change', '#report-type', function (e) {
        e.preventDefault();
        var selectedReportType = $('#report-type').val();
        if (selectedReportType === 'Обороты') {
            $('#location').hide(100, function () {
                $('#export-button').hide();
                $('#date-range').show(100);
            });
        } else {
            $('#date-range').hide(100, function () {
                $('#export-button').show();
                $('#location').show(100);
            });
        }
    });

    $(document).on('click', '#export-button', function (e) {
        //e.preventDefault();
        var hrefAttr = $(this).attr("href");
        $(this).attr("href", "/Report/Export" + "?locationId=" + $('#location-id').val() + "&locationName=" + $('#location-name').val());
    });

    $(document).on('submit', '#report-form', function(e) {
        e.preventDefault();

        var data = {
            reportType: $('#report-type').val(),
            from: $('#report-from-date').val(),
            to: $('#report-to-date').val(),
            locationId: $('#location-id').val(),
            locationName: $('#location-name').val()
        };

        clearTimeout(wto);

        wto = setTimeout(function () {
            $('#progress-bar').animate({ opacity: 1.0 }, 100, function () {
                var submitButton = $('#submit-button');
                var submitButtonText = $('#submit-button > span');
                submitButtonText.text('Формирование...');
                submitButton.prop('disabled', true);                    

                $.ajax({
                    url: '/Report/Report/',
                    type: 'POST',
                    data: data,
                    success: function (report) {
                        submitButtonText.text('Сформировать');
                        submitButton.prop('disabled', false);
                        $('#report').html(report);
                        $('#progress-bar').animate({ opacity: 0.0 }, 100, function () {
                            if ($(".inventory-datatable").length) {
                                initInventoryDataTable();
                            }
                        });
                    }
                });
            });
        }, 1000);    
      });
});

function initInventoryDataTable() {
    var inventoryDataTable = $(".inventory-datatable").DataTable({
        responsive: true,
        "autoWidth": false,
        "scrollX": false,
        "columnDefs": [
            { type: "num", "targets": 1 }
        ],
        "pagingType": "numbers",
        "language": {
            "lengthMenu": "Отобразить _MENU_ позиций на странице",
            "zeroRecords": "Товары отсутствуют",
            "info": "Отображение страницы _PAGE_ из _PAGES_",
            "infoEmpty": "Записи отсутствуют",
            "infoFiltered": "(Отфильтровано из _MAX_ записей)",
            "search": "Поиск"
        },
        "drawCallback": function () {
            $('.pagination').addClass('pagination-sm');
        }
    });

    inventoryDataTable.columns().eq(0).each(function (colIdx) {
        $('input', $('.inventory-datatable th')[colIdx]).on('keyup change', function () {
            inventoryDataTable
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });   
}