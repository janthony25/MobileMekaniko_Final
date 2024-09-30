$(function () {

    $('#carActionModal').on('hidden.bs.modal', function () {
        HideModal();
    });

    // Save the car to Db
    $('#addCustomer').on('click', function () {
        console.log('Trying to add car.');
        AddCar();
        
    });

    // Open add car modal and pass values
    $('#btnAddCar').on('click', function () {
        const carId = $(this).data('car-id');
        const action = $(this).data('action');
        const customerId = $(this).data('customer-id');

        console.log(customerId, carId, action);

        $('#CustomerId').val(customerId);

        CarModal(carId, action);
    });


    // Open update car modal and pass values
    $(document).on('click', '.btnUpdateCar', function () {
        const carId = $(this).data('car-id');
        const action = $(this).data('action');
        console.log(carId, action);
        CarModal(carId, action);
    });

    // Update the car details.
    $(document).on('click', '#UpdateCustomer', function () {
        console.log('Trying to edit car details.');
        UpdateCar();
    })

    // Open Delete car modal and pass values
    $(document).on('click', '.btnDeleteCar', function () {
        const carId = $(this).data('car-id');
        const action = $(this).data('action');
        console.log(carId, action);
        CarModal(carId, action);
    })

    // Delete Car Details
    $('#DeleteCustomer').on('click', function () {
        console.log('Trying to delete car.');
        DeleteCar();
    });
   
});



function CarModal(carId, action) {
    $.ajax({
        url: '/car/GetCarDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: {
            id: carId
        },
        success: function (response) {
            console.log(response);

            if (action === 'AddCar') {
                console.log(carId, action);
                // Clear the existing dropdown options within the car-choose-carMake-container div
                const makesDropdown = $('#car-choose-carMake-container #makeIdSelection');
                makesDropdown.empty();  // Empty previous dropdown content

                // Populate the makes dropdown inside car-choose-carMake-container
                if (response.makes && response.makes.length > 0) {
                    makesDropdown.append('<option value="" disabled selected>Select Car Make</option>');

                    // Populate the dropdown with car makes
                    response.makes.forEach(make => {
                        makesDropdown.append(`<option value="${make.makeId}">${make.makeName}</option>`);

                    });
                } else {
                    makesDropdown.append('<option value="" disabled>No makes available</option>');
                }

                $('#carActionModal').modal('show');
                $('#carModalTitle').text('Add Car');
                $('#car-dateAdded-container').hide();
                $('#car-dateEdited-container').hide();
                $('#UpdateCustomer').hide();
                $('#DeleteCustomer').hide();

            }
            else if (action === 'UpdateCar') {

                const makesDropdown = $('#makeIdSelection');
                makesDropdown.empty();  // Empty previous dropdown content

                // Populate the makes dropdown with the current car's make
                if (response.car && response.car.makeId && response.car.makeName) {
                    makesDropdown.append(`<option value="${response.car.makeId}" selected>${response.car.makeName}</option>`);
                } else {
                    makesDropdown.append('<option value="" disabled>No make available</option>');
                }

                makesDropdown.prop('disabled', true);

                $('#carActionModal').modal('show');
                $('#carModalTitle').text('Update Car');

                $('#btnUpdateCustomer').show();
                $('#addCustomer').hide();
                $('#btnDeleteCustomer').hide();

                $('#CarId').val(response.car.carId);
                $('#CarRego').val(response.car.carRego);
                $('#CarModel').val(response.car.carModel);
                $('#CarYear').val(response.car.carYear);

                // Date Added
                if (response.car.dateAdded && new Date(response.car.dateAdded).getTime() !== 0) {
                    let formattedDate = new Date(response.car.dateAdded).toLocaleString();
                    $('#DateAdded').val(formattedDate).prop('readonly', true);
                } else {
                    $('#DateAdded').val('').prop('readonly', true);
                }

                // Date Edited
                if (response.car.dateEdited && new Date(response.car.dateEdited).getTime() !== 0) {
                    let formattedDate = new Date(response.car.dateEdited).toLocaleString();
                    $('#DateEdited').val(formattedDate).prop('readonly', true);
                } else {
                    $('#DateEdited').val('').prop('readonly', true);
                }
            } else if (action === 'DeleteCar') {
                $('#carActionModal').modal('show');
                $('#carModalTitle').text('Delete Car');

                $('#CarId').val(response.car.carId);
                $('#CarRego').val(response.car.carRego).prop('readonly', true);
                $('#car-choose-carMake-container').hide();
                $('#car-carModel-container').hide();
                $('#car-carYear-container').hide();
                $('#car-dateEdited-container').hide();
                $('#car-dateAdded-container').hide();
                $('#addCustomer').hide();
                $('#UpdateCustomer').hide();
            }
        },
        error: function () {
            alert('An error occurred while fetching car details.');
        }
    })
}

//// GET THE ID OF THE SELECTED MAKE
//$('#makeIdSelection').on('change', function () {
//    const selectedMakeId = $(this).val();
//    console.log("Selected MakeId:", selectedMakeId);
//});

function AddCar() {

    let result = Validate();

    if (result == false) {
        return false;
    }
    const token = $('input[name="__RequestVerificationToken"]').val();

    let formData = {
        __RequestVerificationToken: token,
        carRego: $('#CarRego').val(),
        carModel: $('#CarModel').val(),
        carYear: $('#CarYear').val(),
        customerId: $('#CustomerId').val(),
        makeId: $('#makeIdSelection').val()
    };


    console.log(formData);

    $.ajax({
        url: '/car/AddCar',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                HideModal();
                location.reload();
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('An error occurred while adding new car to customer.');
        }
    });
}

function UpdateCar() {
    let result = Validate();
    if (result == false) {
        return false;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();
    const carId = $('#CarId').val();

    let formData = {
        __RequestVerificationToken: token,
        carId: $('#CarId').val(),
        carRego: $('#CarRego').val(),
        carModel: $('#CarModel').val(),
        carYear: $('#CarYear').val(),
        dateEdited: $('#DateEdited').val(),
        makeId: $('#makeIdSelection').val()
    };

    $.ajax({
        url: '/car/UpdateCar',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                HideModal();
                location.reload();
            }
            else {
                alert(response.message);
            }
        },
        error: function () {
            alert("An error occurred while updating car.");
        }
    });
}

function DeleteCar() {
    const token = $('input[name="__RequestVerificationToken"]').val();
    const carId = $('#CarId').val();

    $.ajax({
        url: '/car/DeleteCar',
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            id: carId
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                HideModal();
                location.reload();
            }
        },
        error: function () {
            alert('An error occurred while deleting car.');
        }
    });
}

function HideModal() {
    $('#CarRego').val('');  // Clear Car Rego
    $('#CarModel').val('');  // Fixed typo (changed .va() to .val())
    $('#CarYear').val('');   // Clear Car Year

    // Reset border colors to default and clear any error messages
    $('#CarRego').css('border-color', 'Lightgrey');
    $('#makeIdSelection').css('border-color', 'Lightgrey');
    $('#CarRegoError').text('');   // Clear error message for Car Rego
    $('#MakeIdError').text('');    // Clear error message for Car Make

    $('#carActionModal').modal('hide');
}


function Validate() {
    let isValid = true;

    if ($('#CarRego').val().trim() === "") {
        $('#CarRego').css('border-color', 'Red');
        $('#CarRegoError').text('Car Rego is required');
        isValid = false;
    } else {
        $('#CarRego').css('border-color', 'Lightgrey');
        $('#CarRegoError').text('');
    }

    if ($('#makeIdSelection').val() === null || $('#makeIdSelection').val() === "") {
        $('#makeIdSelection').css('border-color', 'Red');
        $('#MakeIdError').text('Car Make is required.');
        isValid = false;
    } else {
        $('#makeIdSelection').css('border-color', 'Lightgrey');
        $('#MakeIdError').text('');
    }
    return isValid;
}