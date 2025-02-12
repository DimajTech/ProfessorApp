
$(document).ready(() => {

    $(document).on('submit', '#contact-form', function (event) {

        event.preventDefault();

        AuthenticateUser();
    });

    $(document).on('submit', '#register-form', function (event) {

        event.preventDefault();
        Add();

    });

    $(document).on('submit', '#add-news-form', function (event) {

        event.preventDefault();
        AddPieceOfNews();
    });

});


function setLoading(isloading) {

    if (isloading) {
        $('#loading-overlay').css('display', 'flex');

    } else {
        $('#loading-overlay').css('display', 'none');

    }
}
//------------------------------------------------
//---------LOGIN & REGISTER SECTION---------------
//------------------------------------------------
function AuthenticateUser() {

    setLoading(true);

    const email = $('#email').val();
    const password = $('#password').val();

    $.ajax({
        url: "/User/Login",
        type: "POST",
        data: { email, password },
        success: function (response) {
            if (response.success) {

                localStorage.setItem("email", email);
                localStorage.setItem("role", response.role);
                localStorage.setItem("userId", response.userId);


                localStorage.setItem("userPicture", response.picture);

                window.location.href = "/";


            } else {

                $('#validation').text(response.message).css('color', '#900C3F');
            }
            setLoading(false);

        },
        error: function () {

            $('#validation').text('Hubo un problema con el servidor. Intente de nuevo más tarde.').css('color', '#900C3F');
            setLoading(false);

        }
    });
}
function Add() {


    setLoading(true);

    var user = {

        name: $('#r-name').val(),
        email: $('#r-email').val(),
        password: $('#r-password').val(),
    };

    if (user.password != $('#confirm-password').val()) {

        $('#validation').text('Las contraseñas no coinciden');

    } else {
        $.ajax({
            url: "/User/Register",
            data: JSON.stringify(user), //convierte la variable estudiante en tipo json
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response) {


                if (response.success) {

                    $('#r-name').val('');
                    $('#r-email').val('');
                    $('#r-password').val('');
                    $('confirm-password').val('');
                    $('#validation').text(response.message);
                    $('#validation').css('color', 'green');

                } else {

                    $('#validation').text(response.message).css('color', '#900C3F');
                }

                setLoading(false);

            },
            error: function (errorMessage) {
                $('#validation').text(errorMessage.message).css('color', '#900C3F');
                setLoading(false);

            }
        });
    }
}

//------------------------------------------------
//--------------APPOINTMENTS SECTION--------------
//------------------------------------------------
function GetAppointments() {

    setLoading(true);

    const userEmail = localStorage.getItem("email");

    $.ajax({
        url: "/Appointment/GetAllAppointmentsByUser",
        type: "GET",
        data: { email: userEmail },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            setLoading(false);

            if (result.length === 0) {
                $('#myappointments-table').hide();
                $('#noAppointmentsMessage').show();
                return;
            }

            $('#myappointments-table').show();
            $('#noAppointmentsMessage').hide();

            var htmlTable = '';
            $.each(result, function (key, item) {
                const [fecha, hora] = item.date.split('T');

                htmlTable += '<tr>';
                htmlTable += '<td>' + fecha + '</td>';
                htmlTable += '<td>' + hora + '</td>';
                htmlTable += '<td>' + (item.mode == '1' ? 'Virtual' : 'Presencial') + '</td>';

                let statusText;
                switch (item.status) {
                    case 'pending': statusText = 'Pendiente'; break;
                    case 'approved': statusText = 'Aprobada'; break;
                    case 'rejected': statusText = 'Rechazada'; break;
                    default: statusText = item.status;
                }

                htmlTable += '<td>' + statusText + '</td>';
                htmlTable += '<td>' + (item.course ? item.course.name : 'Desconocido') + '</td>';
                htmlTable += '<td>' + (item.professorComment ? item.professorComment : 'Sin comentarios') + '</td>';
                htmlTable += '</tr>';
            });


            $('#myappointments-tbody').html(htmlTable);
        },
        error: function () {
            configureToastr();
            toastr.error("Error al obtener citas.");
            setLoading(false);

        }
    });
}

function setAvailableTimeAppointment() {
    const select = document.getElementById("time");

    for (let hour = 8; hour <= 20; hour++) {
        for (let minutes of [0, 30]) {
            let timeValue = `${hour.toString().padStart(2, "0")}:${minutes === 0 ? "00" : "30"}`;
            let option = new Option(timeValue, timeValue);
            select.appendChild(option);
        }
    }

}
function GetCourses() {
    const now = new Date();
    const formattedDate = now.toISOString().split('T')[0]; 
    $('#datetime').attr('min', formattedDate);

    setAvailableTimeAppointment();

    $.ajax({
        url: "/Course/GetAllCourses",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var htmlSelect = '';
            $.each(result, function (key, item) {
                htmlSelect += '<option value="' + item.id + '">' + item.name + '</option>';
            });
            $("#course").append(htmlSelect);

        },
        error: function (errorMessage) {
            configureToastr();
            toastr.error("Ha ocurrido un error al obtener los cursos.");
        }
    });

}

function AddAppointment() {

    configureToastr();

    const userId = localStorage.getItem("userId");

    var appointment = {
        date: $('#datetime').val() + "T" + $('#time').val(),
        mode: $('#mode').val(),
        courseid: $('#course').val(),
        userId,
    };
    var course = {
        id: $('#course').val(),
        name: $('#course').find('option:selected').text(),
    };
    var user = {
        id: userId,
    }
    appointment.course = course;
    appointment.user = user;
    if (!$('#datetime').val()) {
        toastr.error('Por favor, complete todos los campos correctamente.');
    } else {
        $.ajax({
            url: "/Appointment/CreateNewAppointment",
            data: JSON.stringify(appointment),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                $('#datetime').val('');
                $("#mode").val(1);
                $('#time').val('08:00')
                toastr.success('Registrado con éxito');
                GetAppointments();
            },
            error: function (errorMessage) {
                toastr.error("Ha ocurrido un error al agendar la cita, por favor inténtelo más tarde");
            }
        });
    }

};
//------------------------------------------------
//---------ADVISEMENT SECTION---------------
//------------------------------------------------

function AddNewResponse(advisementId) {


    var text = ($('#response-text-area').val() || "").trim();

    if (text.length > 0) {
        $("#response-text-area").css("border-color", "black");

        const userID = localStorage.getItem("userId");

        var response = {

            advisementId,
            user: {
                id: userID
            },
            text: text
        }

        setLoading(true);
        $.ajax({
            url: "/Advisement/AddNewResponse",
            data: JSON.stringify(response), //convierte la variable estudiante en tipo json
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {

                setLoading(false);

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Respuesta publicada con éxito');

                LoadAdvisementResponses(advisementId)



            },
            error: function (errorMessage) {
                console.log(errorMessage.responseText);

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('Ha ocurrido un error al agregar la respuesta, intente de nuevo más tarde.');

                setLoading(false);

            }
        });
    } else {

        toastr.options.positionClass = 'toast-bottom-right';
        toastr.error('Por favor rellene todos los campos');

        $("#response-text-area").css("border-color", "red");

    }


}
function LoadAdvisementResponses(advisementId) {


    $.ajax({
        url: "/Advisement/GetAdvisementResponsesById/" + advisementId,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (advisementResponses) {


            //Caja para comentar
            var htmlContent = ``;
            $("#commentsContainer>").html(`<div class="loader"></div>`);

            htmlContent += `
                <div class="comment-add-container" id="responses-advise-form" style="">

                    <textarea id="response-text-area" class="comment-text-area" maxlength="200" rows="4" placeholder="Escribe tu respuesta..." required></textarea><br>
                    <button id="addCommentBtn" class="comment-button" onclick="AddNewResponse('${advisementId}')" >
                        Agregar Respuesta
                    </button>
                </div>
                <div style="text-align:right; padding-right:10px;">
                    <br> <h6 id= "totalResponses"></h6><br>
                </div>
            `;

            let index = 0;
            totalResponses = advisementResponses.length;

            //Comentarios
            advisementResponses.forEach(response => {


                index++;
                htmlContent += `
                    <div class="comment-card">
                        <div class="comment-header">
                            <div>
                                <span class="comment-user">${response.user.name}</span>
                                <span class="comment-role">(${response.user.role})</span>
                            </div>
                           
                            <span class="comment-date">${new Date(response.dateTime).toLocaleString()}</span>
                        </div>
                        <div class="comment-body">
                            <p>${response.text}</p>
                        </div>
                        </div>
                `;
            });

            $('#responses-container').html(htmlContent);
            $('#totalResponses').html(totalResponses + " Respuesta(s)");


        },
        error: function (errorMessage) {
            console.log(errorMessage.responseText);

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error al cargar las respuestas, intente de nuevo más tarde.');

            setLoading(false);
        }
    });
}
function GetAdvisementsByUser(email) {

    setLoading(true);

    $.ajax({
        url: "/Advisement/GetAdvisementsByUser",
        type: "GET",
        data: { email: email },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            setLoading(false);

            var userHtmlTable = '';
            $.each(result, function (key, item) {
                userHtmlTable += '<tr>';
                userHtmlTable += '<td>' + item.course.code + '</td>';
                userHtmlTable += '<td>' + item.user.name + '</td>';
                userHtmlTable += '<td>' + new Date(item.createdAt).toLocaleDateString() + '</td>';
                userHtmlTable += `<td><button class="btn btn-info" style="color:white; background-color:#66c5e3;" onclick="loadSection('view/advisementdetails/${item.id}')">Ver más</button></td>`;
                userHtmlTable += '</tr>';
            });

            $('#user-advisements').html(userHtmlTable);

        },
        error: function (errorMessage) {
            configureToastr();
            toastr.error(errorMessage.responseText);
            setLoading(false);

        }
    });
}

function GetPublicAdvisements(email) {

    setLoading(true);

    $.ajax({
        url: "/Advisement/GetPublicAdvisements",
        type: "GET",
        data: { email: email },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
           
            setLoading(false);

            var publicHtmlTable = '';
            $.each(result, function (key, item) {
                publicHtmlTable += '<tr>';
                publicHtmlTable += '<td>' + item.course.code + '</td>';
                publicHtmlTable += '<td>' + item.user.name + '</td>';
                publicHtmlTable += '<td>' + new Date(item.createdAt).toLocaleDateString() + '</td>';
                publicHtmlTable += `<td><button class="btn btn-info" style="color:white; background-color:#66c5e3;" onclick="loadSection('view/advisementdetails/${item.id}')">Ver más</button></td>`;
                publicHtmlTable += '</tr>';
            });

            $('#public-advisements').html(publicHtmlTable);

        },
        error: function (errorMessage) {
            configureToastr();
            toastr.error(errorMessage.responseText);
            setLoading(false);

        }
    });
}

function GetAdvisementDetails(id) {

    setLoading(true);
    $.ajax({
        url: "/Advisement/GetAdvisementById",
        type: "GET",
        data: { id: id },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            setLoading(false);

            if (result && Object.keys(result).length > 0) {

                $("#course").val(result.course.name);
                $("#author").val(result.user.name);
                $("#content").val(result.content);

          
                $(".section-advisements, #create-advisement").hide(); 
                $("#advisement-details").show(); 

                LoadAdvisementResponses(id);
            } else {

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('La consulta no existe o ha sido eliminada.');

                loadSection('view/advisement');
            }

           
        },
        error: function (errorMessage) {
            configureToastr();
            toastr.error(errorMessage.responseText);
            setLoading(false);
        }
    });
}

function GetCoursesForAdvisement() {
    $.ajax({
        url: "/Course/GetAllCourses",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var htmlSelect = '';
            $.each(result, function (key, item) {
                htmlSelect += '<option value="' + item.id + '">' + item.name + '</option>';
            });
            $("#advisement-course-select").append(htmlSelect);

        },
        error: function (errorMessage) {
            configureToastr();
            toastr.error(errorMessage.responseText);
        }
    });
    
}

function ShowCreateAdvisementForm() {

    $(".section-advisements, #advisement-details").hide();

    $("#create-advisement").show();

    GetCoursesForAdvisement();
}

function AddAdvisement() {

    setLoading(true);

    //configureToastr(); 
    var selectedCourseId = $("#advisement-course-select option:selected").val();
   
    var advisementContent = $('#advisement-content').val();
    var isPublic = $('#publicCheck').is(':checked');


    if (selectedCourseId === "0") {

        toastr.options.positionClass = 'toast-bottom-right';
        toastr.error('Por favor selecciona un curso.');
        return;
    }

    if (advisementContent.trim() === "") {
     
        toastr.error("Por favor, ingrese un mensaje.");
        return;
    }

    //Se debe obtener el usuario autenticado correctamente
    var user = {
        id: localStorage.getItem("userId"), 
        name: " " // 
    };


    // Convertir ID del curso a string GUID en minúsculas
    var formattedCourseId = selectedCourseId ? selectedCourseId.toLowerCase() : null;


    var advisement = {
        course: { id: formattedCourseId }, //porque el constructor de curso espera GUUID
        content: advisementContent,
        status: "Pending",
        isPublic: isPublic,
        user: user
    };

    $.ajax({
        url: "/Advisement/CreateNewAdvisement",
        type: "POST",
        data: JSON.stringify(advisement),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response) {

            configureToastr(); 
            toastr.success("Consulta creada con éxito."); 

            loadSection("view/advisement")

            setLoading(false);

        },
        error: function (errorMessage) {
            toastr.error(errorMessage.responseText);
            setLoading(false);

        }
    });
}

function CancelCreateAdvisement() {
    $("#create-advisement").hide();
    $('#advisements').show();
    $("#advisement-course-select").html('<option value="0" selected>Seleccione un curso</option>');
}

//------------------------------------------------
//--------------PROFILE SECTION-------------------
//------------------------------------------------
function GetUserData() {

    setLoading(true);

    const userEmail = localStorage.getItem("email");

    $.ajax({
        url: "/User/GetByEmail",
        type: "GET",
        data: { email: userEmail },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            $('#p-id').val(result.id);
            $('#p-name').text(result.name);
            $('#p-name2').val(result.name);
            $('#p-email').val(result.email);
            $('#p-description').val(result.description);
            $('#p-linkedin').val(result.linkedIn);
            $('#p-password').val(result.password);

            if (result.picture) {

                $('#p-picture').attr('src', result.picture);
            }

            setLoading(false);

        },
        error: function (errorMessage) {
            console.error(errorMessage);
            setLoading(false);

        }
    });
}


function HandleEditing() {
    if ($('#p-button').text() === 'Editar') {
        AllowFieldEditing();
    } else if ($('#p-button').text() === 'Confirmar cambios') {
        if ($('#p-name2').val() == '' || $('#p-email').val() == '' || $('#p-password').val() == '') {
            configureToastr();
            toastr.error('Por favor rellene todos los campos');
        } else if (!ValidatePassword()) {
            configureToastr();
            toastr.error('La contraseña debe tener una longitud mínima de 8 caracteres y debe contener al menos un número');
        }
        else {
            Swal.fire({
                text: "¿Deseas guardar los cambios realizados?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, guardar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    EditUser();
                }
            });
        }
    }
}

function AllowFieldEditing() {
    var originalValues = {
        id: $('#p-id').val(),
        name: $('#p-name2').val(),
        email: $('#p-email').val(),
        password: $('#p-password').val(),
        description: $('#p-description').val(),
        linkedIn: $('#p-linkedin').val(),
        picture: $('#p-picture').val()
    };

    $('#p-name2').prop("readonly", false);
    $('#email-warning').prop("hidden", false);
    $('#p-password').prop("readonly", false);
    $('#p-description').prop("readonly", false);
    $('#p-linkedin').prop("readonly", false);

    $('#p-button').text("Confirmar cambios");
    $('#p-cancel-button').prop("hidden", false);
    $('#p-upload-img-label').prop("hidden", false);
    $('#p-delete-button').prop("hidden", true);

    $('#p-email').css("margin-bottom", "0px");
}

function EditUser() {
    configureToastr();
    setLoading(true);

    var newValues = {
        id: $('#p-id').val(),
        name: $('#p-name2').val(),
        password: $('#p-password').val(),
        description: $('#p-description').val(),
        linkedIn: $('#p-linkedin').val(),
        picture: $('#p-picture').attr("src")
    }

    $.ajax({
        url: "/User/UpdateUser",
        type: "PUT",
        data: JSON.stringify(newValues),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            GetUserData();

            $('#p-name2').prop("readonly", true);
            $('#email-warning').prop("hidden", true);
            $('#p-password').prop("readonly", true);
            $('#p-description').prop("readonly", true);
            $('#p-linkedin').prop("readonly", true);

            $('#p-button').text("Editar");
            $('#p-cancel-button').prop("hidden", true);
            $('#p-upload-img-label').prop("hidden", true);

            $('#p-email').css("margin-bottom", "30px");

            configureToastr();
            toastr.success('Los datos fueron actualizados correctamente');


            localStorage.setItem("userPicture", newValues.picture);
            $('#p-picture-header').attr("src", newValues.picture);

            setLoading(false);

        },
        error: function (errorMessage) {
            toastr.error('Algo salió mal');
            setLoading(false);

        }
    });
    
}

function CancelEditing() {
    GetUserData();

    $('#p-name2').prop("readonly", true);
    $('#email-warning').prop("hidden", true);
    $('#p-password').prop("readonly", true);
    $('#p-description').prop("readonly", true);
    $('#p-linkedin').prop("readonly", true);

    $('#p-button').text("Editar");
    $('#p-cancel-button').prop("hidden", true);
    $('#p-upload-img-label').prop("hidden", true);
    $('#p-delete-button').prop("hidden", false);

    $('#p-email').css("margin-bottom", "30px");
}

function ValidatePassword(){
    var password = $('#p-password').val();

    if (password.length < 8) {
        return false;
    }

    var hasNumber = /\d/.test(password); //verifies that it contains at least a number
    return hasNumber;
}


function DeleteAccount() {
    const userId = localStorage.getItem("userId");
    setLoading(true);

    Swal.fire({
        text: "¿Seguro de que deseas eliminar la cuenta?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/User/DeleteUser?id=${userId}`,
                type: "DELETE",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Cuenta eliminada',
                            text: 'Tu cuenta ha sido eliminada exitosamente'
                        }).then(() => {
                            //TODO: Redirect to login
                            logoutUser();
                        });
                    }
                    setLoading(false);

                },
                error: function (xhr, status, error) {
                    console.error("Error:", error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'No se pudo eliminar la cuenta'
                    });
                    setLoading(false);

                }
            });
        }
    });
}

function ShowImage(input) {

    if (input.files && input.files[0]) {
        const file = input.files[0];

        if (file.type.startsWith("image/")) {
            const reader = new FileReader();

            reader.onload = function (e) {
                $("#p-picture").attr("src", e.target.result);
            };

            reader.readAsDataURL(file);
        } else {
            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Por favor seleccione una imagen.');

            $("#previewImage").attr("src", "/images/istockphoto-1128826884-612x612.jpg");

            $(input).val(""); // Resetear input si no es imagen
            $("#fileName").text("Sin archivos seleccionados");
        }
    }

}


//------------------------------------------------
//--------------NEWS SECTION----------------------
//------------------------------------------------

//Add News

function AddPieceOfNews() {

    setLoading(true);

    const fileInput = document.getElementById("add-news-image-upload");
    const file = fileInput.files[0];

    if (file) {
        const reader = new FileReader();
        reader.readAsDataURL(file);

        reader.onload = function () {

            var title = ($('#add-news-title').val() || "").trim();
            var description = ($('#add-news-description').val() || "").trim();

            if (title.length > 0 && description.length > 0) {

                var news = {
                    title: $('#add-news-title').val(),
                    description: $('#add-news-description').val(),
                    picture: reader.result,
                    user: {
                        id: localStorage.getItem("userId"),
                        role: localStorage.getItem("role")
                    }
                };
                $.ajax({
                    url: "/PieceOfNews/CreateNews",
                    data: JSON.stringify(news), //convierte la variable estudiante en tipo json
                    type: "POST",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        console.log(response)

                        if (response === -1) {
                            $('#add-news-title').val('');
                            $('#add-news-description').val('');

                            $("#previewImage").attr("src", "/images/photo-editor-icon.png");
                            $(fileInput).val(""); // Resetear input si no es imagen
                            $("#fileName").text("Sin archivos seleccionados");


                            toastr.options.positionClass = 'toast-bottom-right';
                            toastr.success('La noticia se ha publicado con éxito');

                        } else {

                            toastr.options.positionClass = 'toast-bottom-right';
                            toastr.error('No cuentas con los permisos para realizar esta acción.');
                        }

                        setLoading(false);



                    },
                    error: function (errorMessage) {

                        toastr.options.positionClass = 'toast-bottom-right';
                        toastr.error('Ha ocurrido un error al guardar la noticia. Intente de nuevo más tarde.');

                        console.error(errorMessage.message);
                        setLoading(false);

                    }
                });
            } else {
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('No se permiten espacios vacíos...');
                setLoading(false);
            }
        };

        reader.onerror = function () {

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error al cargar la imagen. Intente de nuevo.');

            setLoading(false);
        };
    } else {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.error('Debes seleccionar una imagen de portada.');
        setLoading(false);
    }

}
function previewSelectedImage(input) {

    if (input.files && input.files[0]) {
        const file = input.files[0];

        if (file.type.startsWith("image/")) {
            const reader = new FileReader();

            reader.onload = function (e) {
                $("#previewImage").attr("src", e.target.result);
                $("#fileName").text(file.name);
            };

            reader.readAsDataURL(file);
        } else {
            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Por favor seleccione una imagen.');

            $("#previewImage").attr("src", "/images/istockphoto-1128826884-612x612.jpg");

            $(input).val(""); // Resetear input si no es imagen
            $("#fileName").text("Sin archivos seleccionados");
        }
    }
}


//News Items
function LoadNewsItems() {

    setLoading(true);

    const role = localStorage.getItem("role");
    const addBtn = $(`#add-news-btn`);

    if (role === "President") {
        addBtn.css("display", "block");
    }else {
        addBtn.css("display", "none");
    }

    $.ajax({
        url: "/PieceOfNews/GetNews",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            htmlContent = ` `;


            $.each(result, (key, item) => {

                htmlContent += `
                    <div class="col-lg-4 col-md-6 align-self-center mb-90 event_outer col-md-6 wordpress design">
                        <div class="events_item">
                            <div class="thumb">
                                <a href="#" onclick="loadSection('view/newsdetails/${item.id}'); return false;">
                                    <img src="${item.picture}" alt="">
                                </a>
                                <span class="category">${item.date}</span>
                            </div>
                            <div class="down-content">
                                <span class="author">${item.user.name}</span>
                                <h5>${item.title}</h5>
                                <br/>
                                <h7>${item.description.length > 80 ? item.description.substring(0, item.description.lastIndexOf(' ', 80)) + "..." : item.description}</h7>
                            </div>
                        </div>
                    </div>
                    `;

            });


            $("#news-container").html(htmlContent);
            setLoading(false);

            $("#news-container").css('height', 'auto');

        },
        error: function (errorMessage) {
            console.log(errorMessage.responseText);

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error al cargar las noticias, intente de nuevo más tarde.');

            setLoading(false);
        }
    });
}

function LoadNewsDetail(pieceOfNewsID) {

    setLoading(true);

    $.ajax({
        url: "/PieceOfNews/GetById/" + pieceOfNewsID,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (newsItem) {

            setLoading(false);

            if (newsItem && Object.keys(newsItem).length > 0) {

                $('#img-news-detail').attr('src', newsItem.picture);
                $('#title-news-detail').text(newsItem.title);
                $('#autor-news-detail').text("Autor: " + newsItem.user.name + " (" + newsItem.user.role + ")");
                $('#date-news-detail').text(newsItem.date);
                $('#description-news-detail').text(newsItem.description);

                //$("#news-container").html(detailHtml);
                LoadNewsComments(pieceOfNewsID);
            } else {

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('La noticia no existe o ha sido eliminada.');

                loadSection('view/news');
            }

        },
        error: function (errorMessage) {
            console.log(errorMessage.responseText);

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error al cargar la noticia, intente de nuevo más tarde.');

            setLoading(false);
        }
    });
}

let totalResponses = 0;
function LoadNewsComments(pieceOfNewsID) {


    $.ajax({
        url: "/CommentNews/GetCommentNewsByPieceOfNewsId/" + pieceOfNewsID,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (newsComments) {



            totalResponses = newsComments.length;

            //Caja para comentar
            var htmlContent = `<br><h3>Comentarios</h3>`;
            $("#commentsContainer>").html(`<div class="loader"></div>`);

            htmlContent += `
                <div class="comment-add-container" id="comment-news-form">

                    <textarea id="comment-text-area" class="comment-text-area" maxlength="200" rows="4" placeholder="Escribe tu comentario..." required></textarea><br>
                    <button id="addCommentBtn" class="comment-button" onclick="AddNewsComment('${pieceOfNewsID}')">
                        Agregar Comentario
                    </button>
                </div>
                <div style="text-align:right; padding-right:10px;">
                    <br> <h6 id= "totalComments"></h6><br>
                </div>
            `;

            let index = 0;
            //Comentarios
            newsComments.forEach(comment => {

                totalResponses += comment.totalResponses

                index++;
                htmlContent += `
                    <div class="comment-card">
                        <div class="comment-header">
                            <div>
                                <span class="comment-user">${comment.user.name}</span>
                                <span class="comment-role">(${comment.user.role})</span>
                            </div>
                           
                            <span class="comment-date">${new Date(comment.dateTime).toLocaleString()}</span>
                        </div>
                        <div class="comment-body">
                            <p>${comment.text}</p>
                        </div>

                        <div class="comment-footer">

                        <div style="display: flex;flex-flow: row;margin-top:10px;">
                            <p style: "align-content_center;" id=${index + "-response-counter"}></p>
                            <img src="/images/comment-box-icon.png" alt="" style="width: 20px;height: 20px;margin-left: 10px;margin-top:5px;">
                        </div>

                            <button onclick="toggleAddResponse('${index + "-response-news-form"}','${index + "-response-news-form-btn"}' )" class="add-response-btn" id= ${index + "-response-news-form-btn"} >Responder</button>

                        </div>

                          <div class="comment-add-container" id= ${index + "-response-news-form"} style="display:none;">
                             <textarea id= ${index + "-response-text-area"} class="comment-text-area" maxlength="200" rows="4" placeholder="Escribe tu respuesta para ${comment.user.name}..." required></textarea><br>
                             <button id="addCommentBtn" class="comment-button" onclick="AddNewsCommentResponse('${comment.id}', '${index + "-response-news-container"}', '${index + "-response-text-area"}', '${index + "-response-counter"}')">
                                 Agregar Respuesta
                             </button>

                          </div>
                          
                        </div>

                        <div id=${index + "-response-news-container"}>
                        
                        </div>
                `;

                LoadNewsCommentsResponse(comment.id, index + "-response-news-container", index + "-response-counter");

            });

            $('#commentsContainer').html(htmlContent);
            $('#totalComments').html(totalResponses + " Comentario(s)");
            

        },
        error: function (errorMessage) {
            console.log(errorMessage.responseText);

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error al cargar los comentarios, intente de nuevo más tarde.');

            setLoading(false);
        }
    });
}

function LoadNewsCommentsResponse(commentID, container, counter) {

    let totalResponses = 0;

    $.ajax({
        url: "/CommentNews/GetCommentNewsResponsesByCommentId/" + commentID,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (responses) {


            var htmlContent = '';
            //Itera sobre las respuestas de los comentarios 
            responses.forEach(response => {

                totalResponses++;

                htmlContent += `
                    <div class="response-card">
                        <div class="comment-header">
                            <div>
                                <span class="comment-user">${response.user.name}</span>
                                <span class="comment-role">(${response.user.role})</span>
                            </div>
                           
                            <span class="comment-date">${new Date(response.dateTime).toLocaleString()}</span>
                        </div>
                        <div class="comment-body">
                            <p>${response.text}</p>
                        </div>
                    </div>
                `;

            });

            
            $('#' + container).html(htmlContent);
            $('#' + counter).text(totalResponses);

        },
        error: function (errorMessage) {
            console.log(errorMessage.responseText);

            toastr.options.positionClass = 'toast-bottom-right';
            toastr.error('Ha ocurrido un error  cargando los comentarios, intente de nuevo más tarde.');

            setLoading(false);        }
    });
}
const toggleAddResponse = (id, btnId) => {
    const element = $(`#${id}`);
    const btn = $(`#${btnId}`);


    if (element.is(":visible")) {
        element.css("display", "none");
        btn.text("Responder");
    } else {
        element.css("display", "block");

        btn.text("Cancelar");
    }
};

function AddNewsComment(pieceOfNewsID) {


    var text = ($('#comment-text-area').val() || "").trim(); 

    if (text.length > 0) {
        $("#comment-text-area").css("border-color", "black");

        const userID = localStorage.getItem("userId");

        var comment = {

            pieceOfNews: {
                id: pieceOfNewsID
            },
            user: {
                id: userID
            },
            text: text
        }

        setLoading(true);
        $.ajax({
            url: "/CommentNews/AddNewsComment",
            data: JSON.stringify(comment), //convierte la variable estudiante en tipo json
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Comentario publicado con éxito');

                LoadNewsComments(pieceOfNewsID);
                setLoading(false);



            },
            error: function (errorMessage) {
                console.log(errorMessage.responseText);

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('Ha ocurrido un error al agregar el comentario, intente de nuevo más tarde.');

                setLoading(false);

            }
        });
    } else {

        toastr.options.positionClass = 'toast-bottom-right';
        toastr.error('Por favor rellene todos los campos');

        $("#comment-text-area").css("border-color", "red");

    }
  

}

function AddNewsCommentResponse(commentID, container, textarea, counter) {


    var text = ($('#' + textarea).val() || "").trim();

    if (text.length > 0) {
        $('#' + textarea).css("border-color", "black");

        const userID = localStorage.getItem("userId");

        var response = {

            CommentNews: {
                id: commentID
            },
            user: {
                id: userID
            },
            text: text
        }

        setLoading(true);
        $.ajax({
            url: "/CommentNews/AddNewsCommentResponse",
            data: JSON.stringify(response), //convierte la variable estudiante en tipo json
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Respuesta publicada con éxito');

                LoadNewsCommentsResponse(commentID, container, counter);
                setLoading(false);
                $('#' + textarea).val('');

                totalResponses++;
                $('#totalComments').html(totalResponses + " Comentario(s)");


            },
            error: function (errorMessage) {
                console.log(errorMessage.responseText);

                toastr.options.positionClass = 'toast-bottom-right';
                toastr.error('Ha ocurrido un error al agregar la respuesta, intente de nuevo más tarde.');

                setLoading(false);

            }
        });
    } else {

        toastr.options.positionClass = 'toast-bottom-right';
        toastr.error('Por favor rellene todos los campos');

        $('#' + textarea).css("border-color", "red");

    }


}

//------------------------------------------------
//-------------------UTILITY----------------------
//------------------------------------------------
function configureToastr() {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        // ... otras opciones
    };
}
