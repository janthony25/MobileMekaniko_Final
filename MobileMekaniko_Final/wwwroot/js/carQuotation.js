$(function () {

    $('#carQuotationModal').on('hidden.bs.modal', function () {
        HideModal();
    });

    $(document).on('click', '.btnAddQuotation', function () {
        const carId = $(this).data('car-id');
        AddQuotationModal(carId);
    });

    // Add new quotation
    $('#addQuotation').on('click', function () {
        console.log('Adding new quotation');
        AddQuotation();
    });

    $('#addItemButton').on('click', function () {
        var newItem = $(`
        <div class="row quotation-item d-flex justify-content-between align-items-center mb-2">
            <div class="col-4"><input type="text" class="form-control item-name" placeholder="Item Name"></div>
            <div class="col-2"><input type="number" class="form-control item-quantity" placeholder="Quantity"></div>
            <div class="col-3"><input type="number" class="form-control item-price" placeholder="Price"></div>
            <div class="col-3 d-flex">
                <input type="number" class="form-control item-total me-2" placeholder="Total" readonly>
                <button type="button" class="btn btn-danger remove-item"><i class="bi bi-trash3-fill"></i></button>
            </div>
        </div>
    `);
        $('#quotationItems').append(newItem);
    });

    // Remove Quotation Item functionality
    $(document).on('click', '.remove-item', function () {
        $(this).closest('.quotation-item').remove();
        updateTotals();
    });

    // Update item total and quotation totals when quantity or price changes
    $(document).on('input', '.item-quantity, .item-price', function () {
        var item = $(this).closest('.quotation-item');
        var quantity = parseFloat(item.find('.item-quantity').val()) || 0;
        var price = parseFloat(item.find('.item-price').val()) || 0;

        // Calculate the item total
        var total = quantity * price;
        item.find('.item-total').val(total.toFixed(2));

        // Update the overall quotation totals
        updateTotals();
    });

    // Input event to update totals when labor price, discount, shipping fee, or amount paid changes
    $('#LaborPrice, #Discount, #ShippingFee, #AmountPaid').on('input', function () {
        updateTotals();
    });


});


// Function to update quotation totals
function updateTotals() {
    var subTotal = 0;

    // Calculate subtotal by summing up all item totals
    $('.item-total').each(function () {
        subTotal += parseFloat($(this).val()) || 0;
    });

    // Retrieve additional values
    var laborPrice = parseFloat($('#LaborPrice').val()) || 0;
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

function AddQuotationModal(carId) {
    $.ajax({
        url: '/invoice/GetCustomerCarDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: {
            id: carId
        },
        success: function (response) {
            $('#carQuotationModal').modal('show');
            $('#carQuotationModalTitle').text('Add Quotation');

            // show add quotation button
            $('#addQuotation').show();

            // Hide update delete button
            $('#updateQuotation').hide();
            $('#deleteQuotation').hide();

            $('#CustomerName').val(response.customerName);
            $('#CarRego').val(response.carRego);
            $('#DateAdded').val('');
            $('#CarId').val(response.carId);
        },
        error: function () {
            alert('An error occurred while fetching customer car details.');
        }
    });
}



function AddQuotation() {

    let result = Validate();

    if (result == false) {
        return false;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        IssueName: $('#IssueName').val(),
        Notes: $('#Notes').val(),
        laborPrice: parseFloat($('#LabourPrice').val()) || 0,
        discount: parseFloat($('#Discount').val()) || 0,
        shippingFee: parseFloat($('#ShippingFee').val()) || 0,
        subTotal: parseFloat($('#SubTotal').val()) || 0,
        totalAmount: parseFloat($('#TotalAmount').val()) || 0,
        quotationItems: [],
        CarId: $('#CarId').val()
    };

    $('.quotation-item').each(function () {
        let item = {
            ItemName: $(this).find('.item-name').val() || "",
            Quantity: parseFloat($(this).find('.item-quantity').val()) || 0,
            ItemPrice: parseFloat($(this).find('.item-price').val()) || 0,
            ItemTotal: parseFloat($(this).find('.item-total').val()) || 0
        };
        formData.quotationItems.push(item);
    });

    console.log(formData);

    $.ajax({
        url: '/quotation/AddQuotation',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        headers: {
            'RequestVerificationToken': token
        },
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert(response.message);
                HideModal();
                location.reload();
            }
        },
        error: function () {
            alert('An error occured while adding new quotation');
        }
    });
}

function HideModal() {
    $('#DateAdded').val('');
    $('#IssueName').val('');
    $('#Notes').val('');
    $('#SubTotal').val('');
    $('#LaborPrice').val('');
    $('#Discount').val('');
    $('#ShippingFee').val('');
    $('#TotalAmount').val('');

    $('#IssueNameError').text('');
    $('#IssueName').css('border-color', 'Lightgrey');

    $('#quotationItems').empty();
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