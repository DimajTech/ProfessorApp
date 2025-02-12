//Intercepta clics en los enlaces del header
document.addEventListener("DOMContentLoaded", () => {

    setLoading(false);

    document.querySelectorAll("a[data-section]").forEach(link => {

        link.addEventListener("click", (event) => {

            event.preventDefault(); 
            const section = link.getAttribute("data-section");
            history.pushState(null, "", `view/${section}`); //Cambia la URL sin recargar
            loadSection(`${section}`); 

        });
    });

    //Maneja navegación directa a una URL
    window.addEventListener("popstate", () => {

        const section = location.pathname.substring(1); //obtiene el segmento de la uri
        if (section) {
            loadSection(section); //carga la sección en el contenedor
        }
    });

    //Carga la sección inicial basada en la URL
    const initialSection = location.pathname.substring(1);
    if (initialSection) {
        loadSection(initialSection);
    }
});

function loadSection(section) {
    const mainContent = document.getElementById("main-content");

    mainContent.innerHTML = "";
    toggleHeader(section);

    section.toLowerCase();
    //separar la sección del ID si existe
    let [baseSection, id] = section.startsWith("view/newsdetails/")
        ? ["view/newsdetails", section.split("/").slice(2).join("/")]
        : section.startsWith("view/advisementdetails/")
            ? ["view/advisementdetails", section.split("/").slice(2).join("/")]
            : [section, null];


    toggleHeader();

    fetch(`/${baseSection}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Error al cargar la sección: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = html;

            const sectionContent = tempDiv.querySelector("#main-content").innerHTML;
            mainContent.innerHTML = sectionContent;

            if (baseSection === "view/news") {
                LoadNewsItems();
            }
            if (baseSection === "view/appointment") {
                GetAppointments();
                GetCourses();
            }
            if (baseSection === "view/advisement") {
                var userEmail = localStorage.getItem("email");
                GetAdvisementsByUser(userEmail);
                GetPublicAdvisements(userEmail); // paso email para filtrar y no traer mis consultas de nuevo
            }

            if (baseSection === "view/profile") {
                GetUserData();
            }
            if (baseSection === "view/newsdetails" && id) {
                LoadNewsDetail(id);
            }

            if (baseSection === "view/advisementdetails" && id) {
                GetAdvisementDetails(id);

            }


            if (baseSection === "view/advisementdetails" && id) {
                GetAdvisementDetails(id);

            }
            if (baseSection === "user/login" && id) {

                setLoading(false);
            }


            history.pushState(null, "", `/${section}`); // Cambia la URL sin recargar
        })
        .catch(error => {
            console.error(error);
            mainContent.innerHTML = `<p>Error: No se pudo cargar la sección "${section}".</p>`;
        });
}

function getCookie(name) {
    const cookies = document.cookie.split("; ");
    for (let cookie of cookies) {
        const [cookieName, cookieValue] = cookie.split("=");
        if (cookieName === name) {
            return cookieValue;
        }
    }
    return null;
}

function toggleHeader() {

    const isAuthenticated = getCookie("AuthCookie") !== null;

    $("#page-container").removeClass();
    const picture = localStorage.getItem("userPicture");

    if (isAuthenticated) {
        $("#page-container").addClass("container");
        $("#header").html(`
            <li class="scroll-to-section"><a href="/view/news" data-section="view/news">Noticias</a></li>
            <li class="scroll-to-section"><a href="/view/appointment" data-section="view/appointment">Horas consulta</a></li>
            <li class="scroll-to-section"><a href="/view/advisement" data-section="view/advisement">Consulta de Cursos</a></li>
            <li class="scroll-to-section"><a href="/view/profile" data-section="view/profile"><img src="${picture}" id="p-picture-header"></a> </li>
            <li class="scroll-to-section">
                <a href="javascript:void(0);" onclick="logoutUser()">
                    <img src="/images/door-check-out-icon.png" id="p-picture-header" style="border-radius:0; height:30px; width:30px;">
                </a>
            </li>
            `);
    } else {
        $("#header").html(`
            <li><a href="/user/login">Iniciar sesión</a></li>
            <li><a href="/user/register">Registrarse</a></li>
        `);
    }

    // Reasigna eventos a los enlaces del header
    document.querySelectorAll("a[data-section]").forEach(link => {
        link.addEventListener("click", (event) => {
            event.preventDefault();
            const section = link.getAttribute("data-section");
            history.pushState(null, "", `/${section}`);
            loadSection(section);
        });
    });
}

function logoutUser() {
    Swal.fire({
        text: "Estás a punto de salir de cuenta.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#218838',
        cancelButtonColor: '#dc3545',
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) { // Cambiado de result.success a result.isConfirmed
            document.cookie = "AuthCookie=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            window.location.href = "/user/login";
        }
    });
}
