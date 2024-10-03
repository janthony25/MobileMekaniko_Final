$(function () {
    $(document).on('click', '.btnAddInvoice', function () {
        const carId = $(this).data('car-id');
        console.log(carId);

        AddInvoiceModal(carId);
    });

    $(document).on('click', '.btnUpdateInvoice', function () {
        const invoiceId = $(this).data('invoice-id');
        const action = $(this).data('action');
        console.log(invoiceId, action);

        UpdateDeleteInvoiceModal(invoiceId, action);
    });

    $('#addItemButton').on('click', function () {
        var newItem = $(`
        <div class="row invoice-item d-flex justify-content-between align-items-center mb-2">
            <div class="col-4"><input type="text" class="form-control item-name" placeholder="Item Name"></div>
            <div class="col-2"><input type="number" class="form-control item-quantity" placeholder="Quantity"></div>
            <div class="col-3"><input type="number" class="form-control item-price" placeholder="Price"></div>
            <div class="col-3 d-flex">
                <input type="number" class="form-control item-total me-2" placeholder="Total" readonly>
                <button type="button" class="btn btn-danger remove-item"><i class="bi bi-trash3-fill"></i></button>
            </div>
        </div>
    `);
        $('#invoiceItems').append(newItem);
    });

    // Remove Invoice Item functionality
    $(document).on('click', '.remove-item', function () {
        $(this).closest('.invoice-item').remove();
        updateTotals();
    });

    // Update item total and invoice totals when quantity or price changes
    $(document).on('input', '.item-quantity, .item-price', function () {
        var item = $(this).closest('.invoice-item');
        var quantity = parseFloat(item.find('.item-quantity').val()) || 0;
        var price = parseFloat(item.find('.item-price').val()) || 0;

        // Calculate the item total
        var total = quantity * price;
        item.find('.item-total').val(total.toFixed(2));

        // Update the overall invoice totals
        updateTotals();
    });

    // Input event to update totals when labor price, discount, shipping fee, or amount paid changes
    $('#LabourPrice, #Discount, #ShippingFee, #AmountPaid').on('input', function () {
        updateTotals();
    });

    // Adding invoice to db
    $('#addInvoice').on('click', function () {
        console.log('add invoice clicked.');
        console.log('IssueName:', $('#IssueName').val());
        AddInvoice();
    });


});

// Function to update invoice totals
function updateTotals() {
    var subTotal = 0;

    // Calculate subtotal by summing up all item totals
    $('.item-total').each(function () {
        subTotal += parseFloat($(this).val()) || 0;
    });

    // Retrieve additional values
    var laborPrice = parseFloat($('#LabourPrice').val()) || 0;
    var discount = parseFloat($('#Discount').val()) || 0;
    var shippingFee = parseFloat($('#ShippingFee').val()) || 0;

    // Calculate the total amount
    var totalAmount = subTotal + laborPrice - discount + shippingFee;

    // Ensure that the total amount is not negative
    if (totalAmount < 0) {
        totalAmount = 0;
    }

    // Update subtotal and total amount fields
    $('#SubTotal').val(subTotal.toFixed(2));
    $('#TotalAmount').val(totalAmount.toFixed(2));

    // Determine and update payment status
    var amountPaid = parseFloat($('#AmountPaid').val()) || 0;
    var isPaid = (amountPaid >= totalAmount && totalAmount > 0);
    $('#isPaid').val(isPaid);  // Set it to the boolean value directly

    // Set the display value for the user
    var paymentStatusDisplay = isPaid ? 'Paid' : 'Not Paid';
    $('#isPaidDisplay').val(paymentStatusDisplay);
}
function UpdateDeleteInvoiceModal(invoiceId, action) {
    $.ajax({
        url: '/invoice/GetInvoiceDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: {
            id: invoiceId
        },
        success: function (response) {
            console.log('response', response);

            if (action === 'UpdateInvoice') {
                $('#carInvoiceModal').modal('show');
                $('#carInvoiceModalTitle').text('Update Invoice');

                $('#CustomerName').val(response.customerName).prop('disabled', true);
                $('#CarRego').val(response.carRego).prop('disabled', true);
                $('#DateAdded').val(response.dateAdded);
            }
        },
        error: function () {
            alert('An error occurred while fetching invoice details.');
        }
    });
}

function AddInvoiceModal(carId) {
    $.ajax({
        url: '/invoice/GetCustomerCarDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: {
            id: carId
        },
        success: function (response) {
            console.log('response', response);
            $('#carInvoiceModal').modal('show');
            $('#carInvoiceModalTitle').text('Add Invoice');


            $('#CarId').val(response.carId);
            $('#CustomerName').val(response.customerName).prop('disabled', true);
            $('#CarRego').val(response.carRego).prop('disabled', true);
        },
        error: function () {

        }
    });
}

function AddInvoice() {
    const token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        __RequestVerificationToken: token,
        carId: $('#CarId').val(),
        dateAdded: $('#DateAdded').val(),
        dueDate: $('#DueDate').val(),
        issueName: $('#IssueName').val(),
        paymentTerm: $('#PaymentTerm').val(),
        notes: $('#Notes').val(),
        LaborPrice: parseFloat($('#LabourPrice').val()) || 0,
        Discount: parseFloat($('#Discount').val()) || 0,
        ShippingFee: parseFloat($('#ShippingFee').val()) || 0,
        SubTotal: parseFloat($('#SubTotal').val()) || 0,
        TotalAmount: parseFloat($('#TotalAmount').val()) || 0,
        AmountPaid: parseFloat($('#AmountPaid').val()) || 0,
        IsPaid: $('#isPaid').val() === 'true',
        InvoiceItems: []
    };

    $('.invoice-item').each(function () {
        let item = {
            ItemName: $(this).find('.item-name').val(),
            Quantity: parseFloat($(this).find('.item-quantity').val()) || 0,
            ItemPrice: parseFloat($(this).find('.item-price').val()) || 0,
            ItemTotal: parseFloat($(this).find('.item-total').val()) || 0
        };
        formData.InvoiceItems.push(item);
    });

    $.ajax({
        url: '/invoice/AddInvoice',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        headers: {
            'RequestVerificationToken': token
        },
        data: JSON.stringify(formData),
        success: function (response) {
            console.log("trying to add invoice:", response);
            if (response.success) {
                console.log(formData);
                alert(response.message);
            } else {
                console.log(formData);
                alert(response.message);
            }
        },
        error: function () {
            alert('An error occurred while adding new invoice.');
        }
    });
}