// Edit Modal
$(document).ready(function () {
    $(".editbtn").click(function () {
        var userId = $(this).data("id");
        EditUser(userId);
    });
    GetTotalResident();
});

function EditUser(id) {
    $.ajax({
        url: '/Auth/EditUser?uid='+ id,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (res) {
            $("#editUserModal").modal('show');

            $("#UserId").val(res.id);
            $("#Name").val(res.name);
            $("#Email").val(res.email);
            $("#PhoneNumber").val(res.phoneNumber);
            $("#FlatNumber").val(res.flatNumber);
            $("#Role").val(res.role);
        },
        error: function () {
            alert("something is wrong")
        }

    });
}


$("#editUserForm").submit(function (event) {
    event.preventDefault();

    var obj = {
        UserId: $("#UserId").val(),
        Name: $("#Name").val(),
        Email: $("#Email").val(),
        PhoneNumber: $("#PhoneNumber").val(),
        FlatNumber: $("#FlatNumber").val(),
        Role: $("#Role").val()
    };

    $.ajax({
        url: '/Auth/UpdateUser',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        data: JSON.stringify(obj),
        success: function (response) {
            alert(response.message);
            if (response.success) {
                $("#editUserModal").modal('hide');
                 fetchUserData();
            }
        },
        error: function () {
            alert("Something went wrong.");
        }
    });
});

// Admin Dashboard
function GetTotalResident(){
    $.ajax({
        url: '/Home/GetTotalResident',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        success: function (res) {
            console.log(res);
            $('#residentCount').text(res);
        },
        error: function (err) {
            console.error(err);
        }
    });
}
