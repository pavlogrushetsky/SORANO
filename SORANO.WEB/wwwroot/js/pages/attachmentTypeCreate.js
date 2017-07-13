$(document).ready(function () {
    $("input.input-validation-error").not(".recommendations").closest(".form-group").addClass("has-error");
    $("select.input-validation-error").closest(".form-group").addClass("has-error");
});