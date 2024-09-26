$(function () {

});

$('#btnAddCar').on('click', function () {
    $('#carActionModal').modal('show');
    const customerId = $(this).data('customer-id');
    const action = $(this).data('action');

})
