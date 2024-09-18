$(function () {
    GetCustomers();

    $('#addCustomerModal').on('hidden.bs.modal', function () {
        HideModal();
    });

    $('#btnAddCustomer').on('click', function () {
        $('#addCustomerModal').modal('show');
    });

    // Add event listener with additional logs for debugging
    $('#tblBody').on('click', '.btnEditCustomer', function () {
        $('#updateDeleteModal').modal('show'); // Show the modal
        console.log('Attempting to open modal');  // Ensure this line is hit
    });
});


function GetCustomers() {
    $.ajax({
        url: '/customer/GetCustomers',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf=8',
        success: function (response) {
            let tableRows = '';

            if (response == null || response == undefined || response.length == 0) {
                tableRows = `
                <tr>
                    <td class="text-center" colspan="5">No customer data yet.</td>
                </tr>`;
            }
            else {
                $.each(response, function (index, item) {
                    tableRows += `
                        <tr>
                            <td><a href="#" class="btn btnEditCustomer" style="color: blue">${item.customerId}</a></td>
                            <td>${item.customerName}</td>
                            <td>${item.customerEmail ? item.customerEmail : ''}</td>
                            <td>${item.customerNumber ? item.customerNumber : ''}</td>
                            <td>
                            </td>
                        </tr>`;
                })
            }
            console.log(tableRows); // Log the generated HTML to inspect
            $('#tblBody').html(tableRows);
        },
        error: function () {
            alert('Unable to get customer data.');
        }
    });
}


// Add Customer button


function AddCustomer() {

    let result = Validate();

    if (result == false) {
        return false;
    }

    let token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        __RequestVerificationToken: token,
        customerName: $('#customerName').val(),
        customerAddress: $('#customerAddress').val(),
        customerEmail: $('#customerEmail').val(),
        customerNumber: $('#customerNumber').val()
    };

    $.ajax({
        url: '/customer/AddCustomer',
        data: formData,
        type: 'POST',
        success: function (response) {
            Validate();
            alert('Customer added successfully!');
            GetCustomers();
            HideModal();
        },
        error: function () {
            alert('Unable to add customer.');
        }
    })
}

function Validate() {
    let isValid = true;
    const customerName = $('#customerName').val().trim();
    const customerAddress = $('#customerAddress').val().trim();
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/; // Email regex pattern
    const customerEmail = $('#customerEmail').val().trim();
    const customerNumber = $('#customerNumber').val().trim();

    if (customerName == "") {
        $('#customerName').css('border-color', 'Red');
        $('#customerNameError').text('Customer name is required.');
        isValid = false;
    } else {
        $('#customerName').css('border-color', 'LightGrey');
        $('#customerNameError').text('');
    }

    if (customerEmail !== "" && !emailPattern.test(customerEmail)) {
        $('#customerEmail').css('border-color', 'Red');
        $('#customerEmailError').text('Please enter a valid email address.');
        isValid = false;
    } else {
        $('#customerEmail').css('border-color', 'LightGrey');
        $('#customerEmailError').text('');
    }

    return isValid;
}

function HideModal() {
    $('#addCustomerModal').modal('hide');
    ClearData();
}

function ClearData() {
    // Clear input values
    $('#customerName').val('');
    $('#customerAddress').val('');
    $('#customerEmail').val('');
    $('#customerNumber').val('');

    // Clear error message spans
    $('#customerNameError').text('');
    $('#customerEmailError').text('');

    // Reset the border color of inputs
    $('#customerName').css('border-color', 'LightGrey');
    $('#customerEmail').css('border-color', 'LightGrey');
}


