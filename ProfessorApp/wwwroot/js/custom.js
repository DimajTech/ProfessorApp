
function setupPagination(tableBodyId) {
    const rowsPerPage = 10;
    let currentPage = 1;
    const tableBody = document.getElementById(tableBodyId);
    const rows = Array.from(tableBody.getElementsByTagName("tr"));
    const totalPages = Math.ceil(rows.length / rowsPerPage);

    // **Agregar botones dinámicamente si aún no existen**
    let paginationContainer = document.getElementById(`pagination-${tableBodyId}`);
    if (!paginationContainer) {
        paginationContainer = document.createElement("div");
        paginationContainer.id = `pagination-${tableBodyId}`;
        paginationContainer.innerHTML = `
            <button id="prevPage-${tableBodyId}"> < </button>
            <span id="page-info-${tableBodyId}"></span>
            <button id="nextPage-${tableBodyId}"> > </button>
        `;

        paginationContainer.style.marginTop = "10px";
        paginationContainer.style.marginBottom = "10px";

        tableBody.parentNode.appendChild(paginationContainer);
    }

    const prevButton = document.getElementById(`prevPage-${tableBodyId}`);
    const nextButton = document.getElementById(`nextPage-${tableBodyId}`);
    const pageInfo = document.getElementById(`page-info-${tableBodyId}`);

    prevButton.style.background = "transparent";
    prevButton.style.border = "none";
    prevButton.style.cursor = "pointer";

    nextButton.style.background = "transparent";
    nextButton.style.border = "none";
    nextButton.style.cursor = "pointer";

    function showPage(page) {
        tableBody.innerHTML = "";
        const start = (page - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        const paginatedRows = rows.slice(start, end);
        paginatedRows.forEach(row => tableBody.appendChild(row));

        pageInfo.innerText = `${currentPage} de ${totalPages}`;
        prevButton.disabled = currentPage === 1;
        nextButton.disabled = currentPage === totalPages;
    }

    // **Eliminar event listeners previos para evitar duplicados**
    prevButton.replaceWith(prevButton.cloneNode(true));
    nextButton.replaceWith(nextButton.cloneNode(true));

    document.getElementById(`prevPage-${tableBodyId}`).addEventListener("click", function () {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
        }
    });

    document.getElementById(`nextPage-${tableBodyId}`).addEventListener("click", function () {
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
        }
    });

    if (rows.length > 0) showPage(currentPage);
}


