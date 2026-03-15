// gestion de vehiculos

// Modal de agregar vehiculo
function openAddModal() {
    document.getElementById('addModal').classList.add('modal-open');
    document.body.style.overflow = 'hidden';
    
    document.getElementById('addPlaca').value = '';
    document.getElementById('addMarca').value = '';
    document.getElementById('addModelo').value = '';
    document.getElementById('addAno').value = '';
    document.getElementById('addCapacidad').value = '';
    document.getElementById('addEstado').value = 'Activo';
}

function closeAddModal() {
    document.getElementById('addModal').classList.remove('modal-open');
    document.body.style.overflow = 'auto';
}

// Modal de editar vehiculo
function openEditModal(id, placa, marca, modelo, año, capacidad, estado) {
    // Llenar los campos del formulario con los datos actuales
    document.getElementById('editId').value = id;
    document.getElementById('editPlaca').value = placa;
    document.getElementById('editMarca').value = marca;
    document.getElementById('editModelo').value = modelo;
    document.getElementById('editAno').value = año;
    document.getElementById('editCapacidad').value = capacidad;
    document.getElementById('editEstado').value = estado;
    document.getElementById('editModal').classList.add('modal-open');
    document.body.style.overflow = 'hidden';
}

function closeEditModal() {
    document.getElementById('editModal').classList.remove('modal-open');
    document.body.style.overflow = 'auto';
}


// Modal de eliminar
function openDeleteModal(id, placa) {
    document.getElementById('deleteId').value = id;   
    document.getElementById('deletePlaca').textContent = placa;   
    document.getElementById('deleteModal').classList.add('modal-open');
    document.body.style.overflow = 'hidden';
}

function closeDeleteModal() {
    document.getElementById('deleteModal').classList.remove('modal-open');
    document.body.style.overflow = 'auto';
}


// Se cierran los modales al hacer click afuera
function closeModalOnBackdrop(event, modalType) {
    if (event.target.classList.contains('modal')) {
        switch(modalType) {
            case 'add':
                closeAddModal();
                break;
            case 'edit':
                closeEditModal();
                break;
            case 'delete':
                closeDeleteModal();
                break;
        }
    }
}

document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        // Cerrar todos los modales abiertos
        const modals = document.querySelectorAll('.modal.modal-open');
        modals.forEach(modal => {
            modal.classList.remove('modal-open');
        });
        document.body.style.overflow = 'auto';
    }
});