$(function () {


    // Clear modal fields when modal is closed
    $('#carInvoiceModal').on('hidden.bs.modal', function () {
        HideModal();
    });


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

    // Update Invoice to db
    $('#updateInvoice').on('click', function () {
        console.log('Updating invoice..');
        UpdateInvoice();
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

                // Parse the date without causing timezone offset
                let dateAdded = response.dateAdded ? response.dateAdded.split('T')[0] : '';
                let dueDate = response.dueDate ? response.dueDate.split('T')[0] : '';

                $('#DateAdded').val(dateAdded);
                $('#DueDate').val(dueDate);

                $('#InvoiceId').val(response.invoiceId);
                $('#IssueName').val(response.issueName);
                $('#PaymentTerm').val(response.paymentTerm);
                $('#Notes').val(response.notes);
                $('#LabourPrice').val(response.labourPrice);
                $('#Discount').val(response.discount);
                $('#ShippingFee').val(response.shippingFee);
                $('#SubTotal').val(response.subTotal);
                $('#TotalAmount').val(response.totalAmount);
                $('#AmountPaid').val(response.amountPaid);


                // Remove add invoice button 
                $('#addInvoice').hide();
                $('#updateInvoice').show();


                // clear the existing items
                $('#invoiceItems').empty();

                // Populate item list from response
                if (response.invoiceItemDto && response.invoiceItemDto.length > 0) {
                    // Add headers dynamically before adding items
                    let headers = `
                        <div class="row invoice-item-header d-flex justify-content-between align-items-center mb-2">
                            <div class="col-4"><strong>Item Name</strong></div>
                            <div class="col-2"><strong>Quantity</strong></div>
                            <div class="col-3"><strong>Price</strong></div>
                            <div class="col-3"><strong>Total Price</strong></div>
                        </div>
                    `;
                    $('#invoiceItems').append(headers);

                    // Append each item
                    $.each(response.invoiceItemDto, function (index, item) {
                        let newItem = $(`
                        <div class="row invoice-item d-flex justify-content-between align-items-center mb-2">
                            <div class="col-4">
                                <input type="text" class="form-control item-name" placeholder="Item Name" value="${item.itemName}" readonly>
                            </div>
                            <div class="col-2">
                                <input type="number" class="form-control item-quantity" placeholder="Quantity" value="${item.quantity}" readonly>
                            </div>
                            <div class="col-3">
                                <input type="number" class="form-control item-price" placeholder="Price" value="${item.itemPrice}" readonly>
                            </div>
                            <div class="col-3 d-flex">
                                <input type="number" class="form-control item-total me-2" placeholder="Total" value="${item.itemTotal}" readonly>
                                <button type="button" class="btn btn-danger remove-item"><i class="bi bi-trash3-fill"></i></button>
                            </div>
                        </div>
                        `);
                        $('#invoiceItems').append(newItem);
                    });
                }

                updateTotals();

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

            // Fields
            $('#IssueName').prop('readonly', false);
        },
        error: function () {

        }
    });
}

function AddInvoice() {
    let result = Validate();
    if (result == false) {
        return false;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        __RequestVerificationToken: token,
        carId: $('#CarId').val(),
        dateAdded: $('#DateAdded').val(),
        dueDate: $('#DueDate').val(),
        issueName: $('#IssueName').val(),
        paymentTerm: $('#PaymentTerm').val(),
        notes: $('#Notes').val(),
        laborPrice: parseFloat($('#LabourPrice').val()) || 0,
        discount: parseFloat($('#Discount').val()) || 0,
        shippingFee: parseFloat($('#ShippingFee').val()) || 0,
        subTotal: parseFloat($('#SubTotal').val()) || 0,
        totalAmount: parseFloat($('#TotalAmount').val()) || 0,
        amountPaid: parseFloat($('#AmountPaid').val()) || 0,
        isPaid: $('#isPaid').val() === 'true',
        invoiceItems: []
    };

    $('.invoice-item').each(function () {
        let item = {
            itemName: $(this).find('.item-name').val(),
            quantity: parseFloat($(this).find('.item-quantity').val()) || 0,
            itemPrice: parseFloat($(this).find('.item-price').val()) || 0,
            itemTotal: parseFloat($(this).find('.item-total').val()) || 0
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
                HideModal();
                location.reload();
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

function UpdateInvoice() {
    let result = Validate();

    if (result == false) {
        return false;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        invoiceId: $('#InvoiceId').val(),
        dueDate: $('#DueDate').val(),
        issueName: $('#IssueName').val(),
        paymentTerm: $('#PaymentTerm').val(),
        notes: $('#Notes').val(),
        laborPrice: parseFloat($('#LabourPrice').val()) || 0,
        discount: parseFloat($('#Discount').val()) || 0,
        shippingFee: parseFloat($('#ShippingFee').val()) || 0,
        subTotal: parseFloat($('#SubTotal').val()) || 0,
        totalAmount: parseFloat($('#TotalAmount').val()) || 0,
        amountPaid: parseFloat($('#AmountPaid').val()) || 0,
        isPaid: $('#isPaid').val() === 'true'
    };

    console.log("formdata = ", formData);

    $.ajax({
        url: '/invoice/UpdateInvoice',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': token // Include the verification token in the headers
        },
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                console.log(formData);
                alert(response.message);
                HideModal();
                location.reload();
            } else {
                console.log(formData);
                alert(response.message);
            }
        },
        error: function () {
            alert('An error occurred while updating the invoice.');
        }
    });
}


function HideModal() {
    // Clear text fields
    $('#IssueName').val('');
    $('#PaymentTerm').val('');
    $('#Notes').val('');
    $('#LabourPrice').val('');
    $('#Discount').val('');
    $('#ShippingFee').val('');
    $('#SubTotal').val('');
    $('#TotalAmount').val('');
    $('#AmountPaid').val('');
    $('#isPaid').val('');
    $('#isPaidDisplay').val('');

    // Clear date fields
    $('#DateAdded').val('');
    $('#DueDate').val('');

    // Clear borders 
    $('#DueDate').css('border-color', 'Lightgrey');
    $('#IssueName').css('border-color', 'Lightgrey');

    // Clear fields error text
    $('#DueDateError').text('');
    $('#IssueNameError').text('');

    // Clear dynamically added item list
    $('#invoiceItems').empty();

    $('#carInvoiceModal').modal('hide');
}

function Validate() {
    let isValid = true;

    if ($('#IssueName').val().trim() === "") {
        $('#IssueName').css('border-color', 'Red');
        $('#IssueNameError').text('Issue name is required.');
        isValid = false;
    } else {
        $('#IssueName').css('border-color', 'Lightgrey');
        $('#IssueNameError').text('');
    }


    if ($('#DateAdded').val().trim() === "") {
        let currentDate = new Date();
        let formattedDate = currentDate.toISOString().split('T')[0];
        $('#DateAdded').val(formattedDate);
    }

    if ($('#DueDate').val().trim() === "") {
        $('#DueDate').css('border-color', 'Red');
        $('#DueDateError').text('Due date is required.');
        isValid = false;
    } else {
        $('#DueDate').css('border-color', 'Lightgrey');
        $('#DueDateError').text('');
    }

    if ($('.item-name').length > 0) {
        $('.item-name').each(function () {
            let itemNameInput = $(this);
            let itemNameError = $(this).find('.item-name-error');

            // Check if item name is empty
            if (itemNameInput.val().trim() === "") {
                itemNameInput.css('border-color', 'Red');

                isValid = false;
            } else {
                itemNameInput.css('border-color', 'Lightgrey');
                itemNameError.remove();  // Remove the error message if the field is valid
            }
        });
    }

    return isValid;
}