<template>
  <div class="container">
    <AdminHeader />

    <div class="header-section">
      <h2>Usuarios</h2>
    </div>

    <div class="actions-bar">
      <div class="search-container">
        <input
          v-model="searchTerm"
          @input="handleSearch"
          type="text"
          placeholder="Buscar por nombre, apellido o correo"
          class="search-input"
        />
      </div>
      <button @click="navigateToRegister" class="add-button">Añadir+</button>
    </div>

    <div v-if="loading" class="state-message">Cargando usuarios...</div>
    <div v-else-if="errorMessage" class="state-message error">{{ errorMessage }}</div>
    <div v-else-if="users.length === 0" class="state-message">No hay usuarios disponibles.</div>

    <div v-else class="table-wrapper">
      <table class="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre Completo</th>
            <th>Email</th>
            <th>Rol</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.fullName }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.role }}</td>
            <td>
              <button @click="openEditModal(user)" class="btn-edit">Editar</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Editar Usuario</h3>
          <button class="close-btn" @click="closeModal">&times;</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="submitEdit">
            <div class="form-group">
              <label>Nombre</label>
              <input v-model="editForm.firstName" type="text" class="form-control" />
            </div>
            <div class="form-group">
              <label>Apellidos</label>
              <input v-model="editForm.lastName" type="text" class="form-control" />
            </div>

            <div v-if="currentUserRole === 'Admin'" class="admin-fields">
              <label class="role-label">Rol del usuario:</label>
              <div class="form-check">
                <input 
                  type="radio" 
                  id="roleAdmin" 
                  value="Admin"
                  v-model="selectedRole"
                  class="form-check-input" 
                />
                <label class="form-check-label">Administrador</label>
              </div>
              <div class="form-check">
                <input 
                  type="radio" 
                  id="roleOperator" 
                  value="Operator"
                  v-model="selectedRole"
                  class="form-check-input" 
                />
                <label class="form-check-label">Operador</label>
              </div>
            </div>

            <div class="modal-buttons">
              <button type="submit" class="btn-save">Guardar cambios</button>
              <button type="button" class="btn-cancel" @click="closeModal">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      @close="showPopup = false"
    />
  </div>
</template>

<script>
import axios from 'axios';
import { useRouter } from "vue-router";
import AdminHeader from "./AdminHeader.vue";
import PopupMessage from "./PopupMessage.vue";

export default {
  name: "UserList",
  components: { AdminHeader, PopupMessage },
  setup() {
    return { router: useRouter() };
  },
  data() {
    return {
      users: [],
      searchTerm: "",
      searchTimeout: null,
      loading: false,
      errorMessage: "",
      showModal: false,
      showPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: '',
      editForm: {
        userId: null,
        firstName: "",
        lastName: ""
      },
      selectedRole: "Operator",
      currentUserRole: localStorage.getItem("role") || "Operator",
    };
  },
  methods: {
    fetchUsers() {
      this.loading = true;

      const token = localStorage.getItem("token");

      axios.get("http://localhost:5276/api/User", {
        headers: {
          Authorization: `Bearer ${token}`
        }
      })
      .then(response => {
        this.users = response.data;
      })
      .catch(error => {
        this.errorMessage = "No se pudieron cargar los usuarios.";
      })
      .finally(() => {
        this.loading = false;
      });
    },
    handleSearch() {
      clearTimeout(this.searchTimeout);

      if (!this.searchTerm.trim()) {
        return this.fetchUsers();
      }

      this.searchTimeout = setTimeout(() => {

        this.loading = true;

        const token = localStorage.getItem("token");

        axios.get("http://localhost:5276/api/User/search", {
          params: {
            searchTerm: this.searchTerm
          },
          headers: {
            Authorization: `Bearer ${token}`
          }
        })
        .then(response => {
          this.users = response.data;
        })
        .catch(error => {
          this.errorMessage = "Error al buscar usuarios.";
        })
        .finally(() => {
          this.loading = false;
        });

      }, 300);
    },
    navigateToRegister() {
      this.router.push("/registro");
    },
    openEditModal(user) {
      const nameParts = user.fullName?.split(" ") || [];
      this.editForm = {
        userId: user.id,
        firstName: nameParts[0] || "",
        lastName: nameParts.slice(1).join(" ") || ""
      };
      this.selectedRole = user.role;
      this.showModal = true;
    },
    closeModal() {
      this.showModal = false;
    },
    submitEdit() {
      const token = localStorage.getItem("token");
      const updateData = {};
      
      if (this.editForm.firstName) updateData.firstName = this.editForm.firstName;
      if (this.editForm.lastName) updateData.lastName = this.editForm.lastName;
      
      if (this.currentUserRole === "Admin") {
        updateData.isAdmin = (this.selectedRole === "Admin");
        updateData.isOperator = (this.selectedRole === "Operator");
      }

      axios.put(`http://localhost:5276/api/User/${this.editForm.userId}`, updateData, {
        headers: { Authorization: `Bearer ${token}` }
      })
      .then(() => {
        this.showPopup = true;
        this.popupType = 'success';
        this.popupTitle = 'Éxito';
        this.popupMessage = 'Usuario actualizado correctamente';
        this.closeModal();
        this.fetchUsers();
      })
      .catch(error => {
        this.showPopup = true;
        this.popupType = 'error';
        this.popupTitle = 'Error';
        this.popupMessage = error.response?.data?.message || 'Error al actualizar el usuario';
      });
    }
  },
    created() {

      this.fetchUsers();

      if (!localStorage.getItem("token")) {
        this.router.push("/login");
      }
    }
};
</script>

<style scoped>
.container {
  padding: 20px;
}

.header-section {
  margin-bottom: 24px;
}

.header-section h2 {
  margin: 0;
  font-size: 28px;
  color: #333;
}

.actions-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 24px;
  gap: 15px;
}

.search-container {
  flex: 1;
}

.search-input {
  padding: 10px 15px;
  width: 100%;
  max-width: 350px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.add-button {
  padding: 10px 20px;
  background-color: #2f3e5c;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-edit {
  background-color: #3E4B78;
  color: white;
  border: none;
  border-radius: 999px;          
  padding: 6px 16px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 400;
  transition: 0.3s;
  cursor: pointer;
}

.state-message {
  text-align: center;
  color: #666;
  padding: 20px;
}

.state-message.error {
  color: #b42318;
}

.table-wrapper {
  overflow-x: auto;
  background: #fff;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th, td {
  text-align: left;
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
}

th {
  background: #f8fafc;
  color: #334155;
  font-weight: 600;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 20px;
  border-bottom: 1px solid #eee;
}

.modal-header h3 {
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
}

.form-control {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.admin-fields {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #eee;
}

.role-label {
  display: block;
  margin-bottom: 10px;
  font-weight: 500;
}

.form-check {
  margin-bottom: 10px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.modal-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.btn-save {
  padding: 8px 16px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-cancel {
  padding: 8px 16px;
  background-color: #ccc;
  color: #333;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
</style>