$(function () {

});

$('#btnAddCar').on('click', function () {
    const carId = $(this).data('car-id');
    const action = $(this).data('action');
    console.log(carId, action);

    CarModal(carId, action);

})

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
                $('#btnUpdateCustomer').hide();
                $('#btnDeleteCustomer').hide();
              
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