$(document).ready(function () {
    $.fn.select2.defaults.set('theme', 'bootstrap');
    $('.article-select').select2({
        "language": {
            "noResults": function() {
                return "Артикулы не найдены";
            }
        }
    });

    $('.delivery_item_quantity').bind('keyup mouseup',
        function() {
            var id = $(this).attr('id');
            var quantity = $(this).val();
            if (isNumeric(quantity)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var unitPrice = $('#delivery_item_unitprice_' + number).val();
                    var discount = $('#delivery_item_discount_' + number).val();
                    if (isNumeric(unitPrice) && isNumeric(discount)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__GrossPrice').val(formatDecimal(grossPrice));
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalGrossPrice();
                        updateTotalDiscountPrice();
                    }
                }
            }            
        });

    $('.delivery_item_unitprice').bind('keyup mouseup',
        function () {
            var id = $(this).attr('id');
            var unitPrice = $(this).val();
            if (isNumeric(unitPrice)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var quantity = $('#delivery_item_quantity_' + number).val();
                    var discount = $('#delivery_item_discount_' + number).val();
                    if (isNumeric(quantity) && isNumeric(discount)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__GrossPrice').val(formatDecimal(grossPrice));
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalGrossPrice();
                        updateTotalDiscountPrice();
                    }
                }
            }
        });

    $('.delivery_item_discount').bind('keyup mouseup',
        function () {
            var id = $(this).attr('id');
            var discount = $(this).val();
            if (isNumeric(discount)) {
                var matches = id.match(/\d+$/);
                if (matches) {
                    var number = matches[0];
                    var quantity = $('#delivery_item_quantity_' + number).val();
                    var unitPrice = $('#delivery_item_unitprice_' + number).val();
                    if (isNumeric(quantity) && isNumeric(unitPrice)) {
                        var grossPrice = quantity * unitPrice;
                        var discountPrice = grossPrice - discount;
                        $('#DeliveryItems_' + number + '__DiscountPrice').val(formatDecimal(discountPrice));
                        updateTotalDiscount();
                        updateTotalDiscountPrice();
                    }
                }
            }
        });

    $('.article-select').on('change',
        function() {
            var id = $(this).attr('id');
            var articleId = $(this).val();
            var matches = id.match(/\d+$/);
            if (matches) {
                var number = matches[0];
                $.post('GetArticleName?id=' + articleId, function (name) {
                    $('#delivery_item_article_name_' + number).val(name);
                });
            }
        });

    updateTotalGrossPrice();
    updateTotalDiscount();
    updateTotalDiscountPrice();
});

function updateTotalGrossPrice() {
    var totalGrossPrice = 0;
    $('.delivery_item_grossprice').each(function() {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalGrossPrice += parseFloat(value);
        }
    });
    $('#TotalGrossPrice').val(formatDecimal(totalGrossPrice));
    $('#delivery_totalgrossprice').text(formatDecimal(totalGrossPrice));
}

function updateTotalDiscount() {
    var totalDiscount = 0;
    $('.delivery_item_discount').each(function() {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalDiscount += parseFloat(value);
        }
    });
    $('#TotalDiscount').val(formatDecimal(totalDiscount));
    $('#delivery_totaldiscount').text(formatDecimal(totalDiscount));
}

function updateTotalDiscountPrice() {
    var totalDiscountPrice = 0;
    $('.delivery_item_discountprice').each(function () {
        var value = $(this).val();
        if (isNumeric(value)) {
            totalDiscountPrice += parseFloat(value);
        }
    });
    $('#TotalDiscountPrice').val(formatDecimal(totalDiscountPrice));
    $('#delivery_totaldiscountprice').text(formatDecimal(totalDiscountPrice));
}

function getMimeType(num) {
    getMimeType(num, "Delivery/GetMimeType?id=");
}