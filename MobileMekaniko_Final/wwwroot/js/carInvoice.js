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

    $(document).on('click', '.delete-invoice-btn', function () {
        const invoiceId = $(this).data('invoiceId');
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

    $('#deleteInvoice').on('click', function () {
        console.log('Deleting invoice...');
        DeleteInvoice();
    });


    // Invoice to PDF
    $(document).on('click', '.view-pdf', function () {
        const invoiceId = $(this).data('invoice-id');
        viewPdf(invoiceId);
    });

    // Download Invoice PDF
    $(document).on('click', '.download-pdf', function (e) {
        e.preventDefault();
        const invoiceId = $(this).data('invoice-id');
        downloadPdf(invoiceId);
        console.log('Trying to download pdf..');
    });

    // Send PDF to Email
    $(document).on('click', '.send-invoice-email', function () {
        const invoiceId = $(this).data('invoice-id');
        SendInvoiceEmail(invoiceId);
        console.log('sending invoice to email...', invoiceId);
    });

    $(document).on('click', '.mark-as-paid', function () {
        const invoiceId = $(this).data('invoice-id');
        console.log('marking invoice id as paid: ', invoiceId);

        MarkInvoiceAsPaid(invoiceId);
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

    // Calculate the taxable amount
    var taxableAmount = (subTotal + laborPrice + shippingFee) - discount;

    // Calculate 15% GST (TaxAmount) based on the taxable amount
    var gst = taxableAmount * 0.15;

    // Calculate the total amount by adding GST to the taxable amount
    var totalAmount = taxableAmount + gst;

    // Ensure that the total amount is not negative
    if (totalAmount < 0) {
        totalAmount = 0;
    }

    // Update subtotal, tax amount, and total amount fields
    $('#SubTotal').val(subTotal.toFixed(2));
    $('#TaxAmount').val(gst.toFixed(2)); // Update the GST field
    $('#TotalAmount').val(totalAmount.toFixed(2)); // Update the total amount field

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
                let dueDate = response.dueDate ? response.dueDate.split('T')[0] : '';

                $('#DueDate').val(dueDate).prop('readonly', false);

                $('#InvoiceId').val(response.invoiceId);

                $('#IssueName').val(response.issueName).prop('readonly', false);
                $('#PaymentTerm').val(response.paymentTerm).prop('readonly', false);
                $('#Notes').val(response.notes).prop('readonly', false);
                $('#LabourPrice').val(response.labourPrice).prop('readonly', false);
                $('#Discount').val(response.discount).prop('readonly', false);
                $('#ShippingFee').val(response.shippingFee).prop('readonly', false);
                $('#SubTotal').val(response.subTotal).prop('readonly', true);
                $('#TaxAmount').val(response.taxAmount).prop('readonly', true);
                $('#TotalAmount').val(response.totalAmount).prop('readonly', true);
                $('#AmountPaid').val(response.amountPaid).prop('readonly', false);

                // Hide Date Added
                $('#DateAdded').closest('.date-added-container').hide();


                // Remove add invoice button 
                $('#addInvoice').hide();
                $('#updateInvoice').show();
                $('#deleteInvoice').hide();


                // Remove Add invoie item btn
                $('#addItemButton').hide();


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
                            </div>
                        </div>
                        `);
                        $('#invoiceItems').append(newItem);
                    });
                }
                updateTotals();
            }
            else if (action === 'deleteInvoice') {
                $('#carInvoiceModal').modal('show');
                $('#carInvoiceModalTitle').text('Delete Invoice');

                $('#CustomerName').val(response.customerName).prop('disabled', true);
                $('#CarRego').val(response.carRego).prop('disabled', true);

                $('#InvoiceId').val(response.invoiceId);

                // Parse the date without causing timezone offset
                let dateAdded = response.dateAdded ? response.dateAdded.split('T')[0] : '';
                let dueDate = response.dueDate ? response.dueDate.split('T')[0] : '';

                $('#DateAdded').val(dateAdded).prop('readonly', true);
                $('#DueDate').val(dueDate).prop('readonly', true);

                $('#IssueName').val(response.issueName).prop('readonly', true);
                $('#PaymentTerm').val(response.paymentTerm).prop('readonly', true);
                $('#Notes').val(response.notes).prop('readonly', true);
                $('#LabourPrice').val(response.labourPrice).prop('readonly', true);
                $('#Discount').val(response.discount).prop('readonly', true);
                $('#ShippingFee').val(response.shippingFee).prop('readonly', true);
                $('#SubTotal').val(response.subTotal).prop('readonly', true);
                $('#TaxAmount').val(response.taxAmount).prop('readonly', true);
                $('#TotalAmount').val(response.totalAmount).prop('readonly', true);
                $('#AmountPaid').val(response.amountPaid).prop('readonly', true);


                // Remove add invoice button 
                $('#addInvoice').hide();
                $('#updateInvoice').hide();
                $('#deleteInvoice').show();

                // Remove Add invoie item btn
                $('#addItemButton').hide();


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

            $('#DateAdded').prop('disabled', false);
            $('#DueDate').prop('disabled', false);

            $('#IssueName').val(response.issueName).prop('disabled', false);
            $('#PaymentTerm').val(response.paymentTerm).prop('disabled', false);
            $('#Notes').val(response.notes).prop('disabled', false);
            $('#LabourPrice').val(response.labourPrice).prop('disabled', false);
            $('#Discount').val(response.discount).prop('disabled', false);
            $('#ShippingFee').val(response.shippingFee).prop('disabled', false);
            $('#SubTotal').val(response.subTotal).prop('disabled', false);
            $('#TotalAmount').val(response.totalAmount).prop('disabled', false);
            $('#AmountPaid').val(response.amountPaid).prop('disabled', false);


            // show Date Added
            $('#DateAdded').closest('.date-added-container').show();

            // Fields
            $('#IssueName').prop('readonly', false);

            //  Add invoie item btn
            $('#addItemButton').show();

            // Update Invoice button
            $('#updateInvoice').hide();
            $('#addInvoice').show(); // shows the invoice btn
            $('#deleteInvoice').hide();
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
        taxAmount: parseFloat($('#TaxAmount').val()) || 0,
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
        formData.invoiceItems.push(item);
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
        taxAmount: parseFloat($('#TaxAmount').val()) || 0,
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

function DeleteInvoice() {
    const token = $('input[name="__RequestVerificationToken"]').val();
    const invoiceId = $('#InvoiceId').val();

    console.log('id = ', invoiceId);

    $.ajax({
        url: '/invoice/DeleteInvoice',
        type: 'POST',
        dataType: 'json',
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            id: invoiceId
        },
        success: function (response) {
            console.log(invoiceId);
            if (response.success) {
                alert(response.message);
                HideModal();
                location.reload();
            } else {
                console.log(invoiceId);
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


// View Invoice PDF 
function viewPdf(invoiceId) {
    // Create a modal to display the PDF
    const modal = $('<div class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">')
        .append($('<div class="modal-dialog modal-xl" style="max-width: 90%; width: 90%; height: 90vh;">')
            .append($('<div class="modal-content h-100">')
                .append($('<div class="modal-header">')
                    .append($('<h5 class="modal-title">').text('View Invoice PDF'))
                    .append($('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">')))
                .append($('<div class="modal-body p-0" style="height: calc(100% - 56px);">')
                    .append($('<iframe>')
                        .attr('src', `/Invoice/ViewPdf/${invoiceId}`)
                        .attr('width', '100%')
                        .attr('height', '100%')
                        .attr('frameborder', '0')
                        .attr('style', 'overflow: hidden;')))));

    // Add the modal to the body and show it
    $('body').append(modal);
    modal.modal('show');

    // Remove the modal from the DOM when it's hidden
    modal.on('hidden.bs.modal', function () {
        modal.remove();
    });
}

// Download Invoice PDF
function downloadPdf(invoiceId) {
    // Log to console for debugging
    console.log('Downloading PDF for invoice:', invoiceId);

    // Create a temporary anchor element
    var link = document.createElement('a');
    link.href = `/Invoice/DownloadPdf/${invoiceId}`;

    // Set download attribute to force download
    link.setAttribute('download', '');

    // Append to body, click programmatically, and remove
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Send Invoice PDF to Email
function SendInvoiceEmail(invoiceId) {
    const token = $('input[name="__RequestVerificationToken"]').val();

    console.log('sending invoice to email...', invoiceId); // Debugging log

    $.ajax({
        url: '/invoice/SendInvoiceEmail',
        type: 'POST',
        dataType: 'json',
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            invoiceId: invoiceId  // Use the same data format as DeleteInvoice
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                location.reload();
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function () {
            alert('An error occurred while sending the invoice email.');
        }
    });
}

// Mark invoice as paid
function MarkInvoiceAsPaid(invoiceId) {
    const token = $('input[name="__RequestVerificationToken"]').val();

    // Use the correct confirmation prompt
    const isConfirmed = confirm('Are you sure you want to mark this invoice as paid?');

    // Check if the user confirmed the action
    if (isConfirmed) {
        console.log('Marking invoice as paid InvoiceId:', invoiceId);

        $.ajax({
            url: '/invoice/MarkInvoiceAsPaid',
            type: 'POST',
            dataType: 'json',
            headers: {
                'RequestVerificationToken': token
            },
            data: {
                id: invoiceId
            },
            success: function (response) {
                alert(response.message);  // Show the success message after confirmation
                location.reload();        // Reload the page to reflect the changes
            },
            error: function () {
                alert('An error occurred while marking the invoice as paid.');
            }
        });
    } else {
        console.log('User cancelled marking the invoice as paid.');  // Log if the action is canceled
        // No action is taken if the user cancels
    }
}
