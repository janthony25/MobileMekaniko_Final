$(function () {
    GetCustomers();

    $('#customerModal').on('hidden.bs.modal', function () {
        HideModal();
    })
});

function GetCustomers() {
    $.ajax({
        url: '/customer/GetCustomers',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf8',
        success: function (response) {
            console.log(response);
            let tableRows = '';

            if (response == null || response == undefined || response.length == 0) {
                tableRows = `
                <tr>
                    <td colspan="5" class="text-center">No customers available.</td>
                </tr>`;
            }
            else {
                $.each(response, function (index, item) {
                    tableRows += `
                        <tr>
                            <td class="text-center">
                                <a href="#" onclick="CustomerModal(${item.customerId}, 'updateCustomer');">${item.customerId} </a>
                            </td>
                            <td class="text-center">${item.customerName}</td>
                            <td class="text-center">${item.customerEmail ? item.customerEmail : ''}</td>
                            <td class="text-center">${item.customerNumber ? item.customerNumber : ''}</td>
                            <td class="text-center">
                                <div>   
                                    <a class="btn btn-danger btn-sm"><i class="bi bi-trash3" onclick="CustomerModal(${item.customerId}, 'deleteCustomer')"></i></a>
                                </div>
                            </td>
                        </tr>`;
                });
            } 
            console.log(tableRows);
            $('#customerTblBody').html(tableRows);
        },
        error: function () {
            alert('An error occurred while fetching customer list.');
        }
    });
}



function CustomerModal(customerId, action) {
    console.log(customerId);
    $.ajax({
        url: '/customer/GetCustomerDetails?id=' + customerId,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            console.log(response);

            // Adding customer
            if (action == 'addCustomer') {
                console.log(customerId);
                $('#customerModal').modal('show');
                $('#customerModalTitle').text('Add Customer');
                $('#CustomerName').prop('readonly', false);
                $('#addCustomer').show();
                $('#btnDeleteCustomer').hide();
                $('#btnUpdateCustomer').css('display', 'none');
                $('#dateEdited-container').css('display', 'none');
                $('#dateAdded-container').css('display', 'none');
                $('#customerAddress-container').show();
                $('#customerEmail-container').show();
                $('#customerNumber-container').show();
                
            }
            // Updating customer
            else if (action == 'updateCustomer') {
                console.log(response);
                $('#customerModal').modal('show');
                $('#customerModalTitle').text('Update Customer');
                $('#addCustomer').hide();
                $('#CustomerName').prop('readonly', false);
                $('#btnDeleteCustomer').hide();
                $('#btnUpdateCustomer').show();
                $('#customerAddress-container').show();
                $('#customerEmail-container').show();
                $('#customerNumber-container').show();
                $('#dateEdited-container').show();
                $('#dateAdded-container').show();
                

                $('#CustomerId').val(response.customerId);
                $('#CustomerName').val(response.customerName);
                $('#CustomerAddress').val(response.customerAddress);
                $('#CustomerEmail').val(response.customerEmail);
                $('#CustomerNumber').val(response.customerNumber);
                // Showing Date Edited
                if (response.dateEdited && new Date(response.dateEdited).getTime() !== 0) {
                    var formattedDate = new Date(response.dateEdited).toLocaleString();
                    $('#DateEdited').val(formattedDate).prop('readonly', 'true');
                }
                else {
                    $('#DateEdited').val('');
                    $('#DateEdited').val(formattedDate).prop('readonly', 'true');
                }

                // Showing Date Added
                if (response.dateAdded && new Date(response.dateAdded).getTime() !== 0) {
                    var formattedDate = new Date(response.dateAdded).toLocaleDateString(); // Localized format
                    $('#DateAdded').val(formattedDate).prop('readonly', true); // Works well if #DateAdded is an <input type="text">
                } else {
                    $('#DateAdded').val(''); // Clear field if no date is provided
                }
            }
            // Deleting customer
            else if (action == 'deleteCustomer') {
                $('#customerModal').modal('show');
                $('#customerModalTitle').text('Delete Customer');
                $('#addCustomer').hide();
                $('#btnUpdateCustomer').hide();
                $('#btnDeleteCustomer').show();
                $('#customerAddress-container').hide();
                $('#customerEmail-container').hide();
                $('#customerNumber-container').hide();
                $('#dateEdited-container').hide();
                $('#dateAdded-container').hide();

                // Filling up customer details
                $('#CustomerId').val(response.customerId);
                $('#CustomerName').val(response.customerName).prop('readonly', 'true');

            }
        },
        error: function () {
            alert('An error occurred while fetching customer details.');
        }
    });
   
}

function Validate() {

    let isValid = true;

    // Validate Customer Name
    if ($('#CustomerName').val().trim() == "") {
        $('#CustomerName').css('border-color', 'Red');
        $('#CustomerNameError').text('Customer name is required.');
        isValid = false;
    } else {
        $('#CustomerName').css('border-color', 'Lightgrey');
        $('#CustomerNameError').text('');
    } 

    // Validate Customer Email
    let email = $('#CustomerEmail').val().trim();
    let emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/; // Email regex pattern

    if (email !== "" && !emailPattern.test(email)) {
        $('#CustomerEmail').css('border-color', 'Red');
        $('#CustomerEmailError').text('Please enter a valid email address.');
        isValid = false;
    } else {
        $('#CustomerEmail').css('border-color', 'Lightgrey');
        $('#CustomerEmailError').text('');
    }
    return isValid;
}

function HideModal() {
    ClearData();
    $('#customerModal').modal('hide');
}

function ClearData() {
    // Clearing Customer Modal input fields
    $('#CustomerId').val('');
    $('#CustomerName').val('');
    $('#CustomerAddress').val('');
    $('#CustomerEmail').val('');
    $('#CustomerNumber').val('');

    // Clear span input errors
    $('#CustomerName').css('border-color', 'Lightgrey');
    $('#CustomerNameError').text('');
    $('#CustomerEmail').css('border-color', 'Lightgrey');
    $('#CustomerEmailError').text('');

}

function AddCustomer() {

    let result = Validate();

    if (result == false) {
        return false;
    }
    
    let token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        __RequestVerificationToken: token,
        customerName: $('#CustomerName').val(),
        customerEmail: $('#CustomerEmail').val(),
        customerNumber: $('#CustomerNumber').val()
    };

    $.ajax({
        url: '/customer/AddCustomer',
        data: formData,
        type: 'POST',
        success: function (response) {
                Validate();
                GetCustomers();
                alert('Customer added successfully.');
                HideModal(); 
        },
        error: function () {
            alert('An error occurred while adding customer.');
        }
    });
}

function UpdateCustomer() {
    let result = Validate();
    if (result == false) {
        return false;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();
    const customerId = $('#CustomerId').val();

    let formData = {
        __RequestVerificationToken: token,
        customerId: customerId,
        customerName: $('#CustomerName').val(),
        customerAddress: $('#CustomerAddress').val(),
        customerEmail: $('#CustomerEmail').val(),
        customerNumber: $('#CustomerNumber').val()
    };

    $.ajax({
        url: '/customer/UpdateCustomer',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                GetCustomers();
                HideModal();
            }
        },
        error: function () {
            alert('An error occurred while updating customer.');
        }
    });
}

function DeleteCustomer() {
    const token = $('input[name="__RequestVerificationToken"]').val();
    const customerId = $('#CustomerId').val();

    $.ajax({
        url: '/customer/DeleteCustomer',
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            id: customerId // Send the ID in the body
        },
        success: function (response) {
            alert(response.message);
            GetCustomers();
            HideModal();
        },
        error: function () {
            alert('An error occurred while deleting customer.');
        }
    });
}


function SearchCustomer() {
    const searchCustomer = $('#searchCustomer').val();

    $.ajax({
        url: 'customer/SearchCustomers',
        type: 'GET',
        data: {
            customerName: searchCustomer
        },
        success: function (response) {
            if (response.success) {
                // populate customer table or handle the search results
                console.log(response.customer);
                UpdateCustomerTable(response.customers);
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('An errror occurred while searching for customer.');
        }
    });

}

// Update customer table in the UI
function UpdateCustomerTable(customers) {
    let tableRows = '';

    if (customers.length === 0) {
        tableRows = `
            <tr>
                <td colspan="5" class="text-center">No customers found.</td>
            </tr>`;
    } else {
        $.each(customers, function (index, customer) {
            tableRows += `
                <tr>
                    <td class="text-center">${customer.customerId}</td>
                    <td class="text-center">${customer.customerName}</td>
                    <td class="text-center">${customer.customerEmail}</td>
                    <td class="text-center">${customer.customerNumber}</td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-danger" onclick="DeleteCustomer(${customer.customerId})">Delete</button>
                    </td>
                </tr>`;
        });
    }

    $('#customerTblBody').html(tableRows);  // Assuming you have a table with this ID
}