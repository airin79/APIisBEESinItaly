﻿document.addEventListener("DOMContentLoaded", function () {
    let currentPage = 1;
    const pageSize = 10;
    let totalPages = 1;

    function fetchPets(page) {
        fetch(`http://localhost:5155/api/pets?page=${page}&pageSize=${pageSize}`)
            .then(response => {
                totalPages = parseInt(response.headers.get("X-Total-Pages")) || 1;
                return response.json();
            })
            .then(data => {
                displayPetsInTable(data);
                updatePaginationInfo();
            })
            .catch(error => console.error("Error fetching pets:", error));
    }

    function displayPetsInTable(pets) {
        const tableBody = document.querySelector("#petsTable tbody");
        tableBody.innerHTML = "";

        pets.forEach(pet => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${pet.id}</td>
                <td>${pet.name}</td>
                <td>${pet.birthDate ? new Date(pet.birthDate).toLocaleDateString() : 'N/A'}</td>
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
        document.getElementById("firstPage").disabled = currentPage === 1;
        document.getElementById("lastPage").disabled = currentPage === totalPages;
    }

    /*
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

    document.getElementById("firstPage").addEventListener("click", function () {

        console.log("first page: ");
        console.log(currentPage);

        if (currentPage !== 1) {
            currentPage = 1;
            fetchPets(currentPage);
        }
    });

    document.getElementById("lastPage").addEventListener("click", function () {
        if (currentPage !== totalPages) {
            currentPage = totalPages;
            fetchPets(currentPage);
        }
    });*/

    // Attach event listener to "Generate All PDF" button
    const fullPdfButton = document.getElementById("downloadFullPdfButton");
    if (fullPdfButton) {
        fullPdfButton.addEventListener("click", generateFullPdf);    
    } else {
        console.error("Button #downloadFullPdfButton not found.");
    }


    function addPaginationEventListeners() {
        const firstPageBtn = document.getElementById("firstPage");
        const lastPageBtn = document.getElementById("lastPage");

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

        firstPageBtn.addEventListener("click", function () {
            console.log("Navegando a la primera página.");
            if (currentPage !== 1) {
                currentPage = 1;
                fetchPets(currentPage);
            }
        });

        lastPageBtn.addEventListener("click", function () {
            console.log("Navegando a la última página.");
            if (currentPage !== totalPages) {
                currentPage = totalPages;
                fetchPets(currentPage);
            }
        });
    }

    // Wait for DOM to be fully loaded
    addPaginationEventListeners();

    // Load first page
    fetchPets(currentPage);
});

// 🔹 Function to generate a full PDF (placed **before** it's used!)
function generateFullPdf() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    doc.text("COMPLETE PETS LIST (All Rows)", 105, 10, null, null, "center");

    const headers = ["ID", "Name", "Birth Date", "Age", "Breed"];
    let allPets = [];

    function fetchAllPages(page = 1) {
        return fetch(`http://localhost:5155/api/pets?page=${page}&pageSize=100`) // Fetch 100 per request
            .then(response => {
                const totalPages = parseInt(response.headers.get("X-Total-Pages")) || 1;
                return response.json().then(data => ({
                    pets: data,
                    totalPages
                }));
            });
    }

    // Fetch first page and determine total pages
    fetchAllPages(1)
        .then(result => {
            allPets = [...result.pets];
            const totalPages = result.totalPages;

            // Fetch remaining pages if needed
            const fetchPromises = [];
            for (let i = 2; i <= totalPages; i++) {
                fetchPromises.push(fetchAllPages(i).then(res => allPets.push(...res.pets)));
            }

            return Promise.all(fetchPromises);
        })
        .then(() => {
            const rows = allPets.map(pet => [pet.id, pet.name, pet.birthdate, pet.age, pet.breed]);

            doc.autoTable({
                head: [headers],
                body: rows,
                startY: 20,
                theme: 'striped'
            });

            doc.save("Pets_Report_Full.pdf");
            console.log(`PDF generated with ${allPets.length} records.`);
        })
        .catch(error => console.error("Error fetching full data:", error));
}