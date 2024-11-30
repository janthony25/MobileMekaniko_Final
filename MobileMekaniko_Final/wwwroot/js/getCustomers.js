$(function () {
    //GetCustomers();

    $('#customerModal').on('hidden.bs.modal', function () {
        HideModal();
    });

    $('#btnSearchCustomer').on('click', function () {
        let searchCustomerVal = $('#searchCustomerValue').val();
        console.log(searchCustomerVal);

        SearchCustomer(searchCustomerVal);
    });

    //$(document).on('click', '.btn-add-customer', function () {
    //    customerId = $(this).data('customer-id');
    //    action = $(this).data('action');
    //    console.log(customerId, action);
    //    CustomerModal(customerId, action);
    //});

    //// Add customer to db
    //$('#AddCustomer').on('click', function () {
    //    console.log('Adding customer');
    //  /*  AddCustomer();*/
    //});


});

//function GetCustomers() {
//    $.ajax({
//        url: '/customer/GetCustomers',
//        type: 'GET',
//        dataType: 'json',
//        contentType: 'application/json;charset=utf8',
//        success: function (response) {
//            console.log(response);
//            let tableRows = '';

//            if (response == null || response == undefined || response.length == 0) {
//                tableRows = `
//                <tr>
//                    <td colspan="5" class="text-center">No customers available.</td>
//                </tr>`;
//            }
//            else {
//                $.each(response, function (index, item) {
//                    tableRows += `
//                        <tr>
//                            <td class="text-center">
//                                <a href="#" onclick="CustomerModal(${item.customerId}, 'updateCustomer');">${item.customerId} </a>
//                            </td>
//                            <td class="text-center">${item.customerName}</td>
//                            <td class="text-center">${item.customerEmail ? item.customerEmail : ''}</td>
//                            <td class="text-center">${item.customerNumber ? item.customerNumber : ''}</td>
//                            <td class="text-center">
//                                <div>
//                                    <a href="/customer/GetCustomerCars/${item.customerId}" class="btn btn-primary btn-sm"><i class="bi bi-car-front" id="viewCar"></i> View</a>
//                                    <a class="btn btn-secondary btn-sm" onclick="CustomerModal(${item.customerId}, 'deleteCustomer')"><i class="bi bi-trash3"></i></a>
//                                </div>
//                            </td>
//                        </tr>`;
//                });
//            } 
//            console.log(tableRows);
//            $('#customerTblBody').html(tableRows);
//        },
//        error: function () {
//            alert('An error occurred while fetching customer list.');
//        }
//    });
//}



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

                $('#CustomerAddress').val('');
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
        customerAddress: $('#CustomerAddress').val(),
        customerName: $('#CustomerName').val(),
        customerEmail: $('#CustomerEmail').val(),
        customerNumber: $('#CustomerNumber').val()
    };

    console.log('adding customer details: ', formData);

    $.ajax({
        url: '/customer/AddCustomer',
        data: formData,
        type: 'POST',
        success: function (response) {
            console.log('Response received:', response);
            Validate();
            console.log('After validate');
            alert('Customer added successfully.');
            console.log('After alert');
            HideModal();
            console.log('After hide modal');
            window.location.reload(true);
            console.log('After reload command'); // This might not show if reload works
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
                //GetCustomers();
                HideModal();
                window.location.reload(true);
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
            //GetCustomers();
            HideModal();
            window.location.reload(true);
        },
        error: function () {
            alert('An error occurred while deleting customer.');
        }
    });
}


function SearchCustomer(searchCustomerVal) {
    //const searchCustomer = $('#searchCustomer').val();
    //console.log(searchCustomer);

    $.ajax({
        url: 'SearchCustomers',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: {
            customerName: searchCustomerVal
        },
        success: function (response) {
            console.log(response);
            UpdateCustomerTable(response.customers);
            $('#searchCustomerValue').val('');
        },
        error: function () {
            alert('An error occurred while searching customer.');
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
                    <td class="text-center">${customer.customerEmail ? customer.customerEmail : ''}</td>
                    <td class="text-center">${customer.customerNumber ? customer.customerNumber : ''}</td>
                    <td class="text-center">
                        <a href="/customer/GetCustomerCars/${customer.customerId}" class="btn btn-primary btn-sm"><i class="bi bi-car-front" id="viewCar"></i> View</a>
                        <button class="btn btn-sm btn-secondary" onclick="DeleteCustomer(${customer.customerId})"><i class="bi bi-trash3"></i></a></button>
                    </td>
                </tr>`;
        });
    }

    $('#customerTblBody2').html(tableRows);  // Assuming you have a table with this ID
}