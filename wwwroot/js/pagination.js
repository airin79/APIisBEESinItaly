document.addEventListener("DOMContentLoaded", function () {

let currentPage = 1;
const pageSize = 10;
let totalPages = 1;

function fetchPets(page = 1) {
    fetch(`http://localhost:5155/api/pets?page=${page}&pageSize=${pageSize}`)
        .then(response => {
            totalPages = parseInt(response.headers.get("X-Total-Pages")) || 1;
            return response.json();
        })
        .then(data => {
            displayPetsInTable(data);
            updatePaginationInfo();
        })
        .catch(error => console.log(error));
}

function displayPetsInTable(pets) {
    const tableBody = document.querySelector("#petsTable tbody");
    tableBody.innerHTML = "";

    pets.forEach(pet => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${pet.id}</td>
            <td>${pet.name}</td>
            <td>${pet.age}</td>
            <td>${pet.breed}</td>
        `;
        tableBody.appendChild(row);
    });
}

function updatePaginationInfo() {
    document.getElementById("pageInfo").textContent = `Página ${currentPage} de ${totalPages}`;

    document.getElementById("prevPage").disabled = currentPage === 1;
    document.getElementById("nextPage").disabled = currentPage === totalPages;
}

document.getElementById("prevPage").addEventListener("click", function () {
    if (currentPage > 1) {
        currentPage--;
        fetchPets(currentPage);
    }
});

document.getElementById("nextPage").addEventListener("click", function () {
    if (currentPage < totalPages) {
        currentPage++;
        fetchPets(currentPage);
    }
});

// Cargar la primera página al iniciar
    fetchPets(currentPage);
});
