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

    <div v-else-if="users.length === 0" class="state-message">
      No hay usuarios disponibles.
    </div>

    <div v-else class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre Completo</th>
            <th>Email</th>
            <th>Rol</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.fullName }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.role }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useRouter } from "vue-router";
import AdminHeader from "./AdminHeader.vue";

export default {
  name: "UserList",
  components: {
    AdminHeader
  },
  setup() {
    return {
      router: useRouter()
    };
  },
  data() {
    return {
      users: [],
      searchTerm: "",
      searchTimeout: null,
      loading: false,
      errorMessage: ""
    };
  },
  methods: {
    fetchUsers() {
      this.loading = true;
      this.errorMessage = "";

      axios
        .get("http://localhost:5276/api/User")
        .then((response) => {
          this.users = response.data;
        })
        .catch((error) => {
          console.error("Error fetching users:", error);
          this.errorMessage = "No se pudieron cargar los usuarios.";
        })
        .finally(() => {
          this.loading = false;
        });
    },
    handleSearch() {
      clearTimeout(this.searchTimeout);
      if (this.searchTerm.trim() === "") {
        this.fetchUsers();
        return;
      }

      this.searchTimeout = setTimeout(() => {
        this.loading = true;
        this.errorMessage = "";

        axios
          .get("http://localhost:5276/api/User/search", {
            params: { searchTerm: this.searchTerm }
          })
          .then((response) => {
            this.users = response.data;
          })
          .catch((error) => {
            console.error("Error searching users:", error);
            this.errorMessage = "Error al buscar usuarios.";
          })
          .finally(() => {
            this.loading = false;
          });
      }, 300);
    },
    navigateToRegister() {
      this.router.push("/registro");
    }
  },
  created: function () {
    this.fetchUsers();
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
  align-items: center;
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
  font-size: 14px;
  border: 1px solid #ddd;
  border-radius: 4px;
  outline: none;
  transition: border-color 0.3s;
}

.search-input:focus {
  border-color: #2f3e5c;
}

.add-button {
  padding: 10px 20px;
  background-color: #2f3e5c;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.add-button:hover {
  background-color: #1f2c47;
}

.state-message {
  text-align: center;
  color: #666;
  padding: 20px;
  font-size: 16px;
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

th,
td {
  text-align: left;
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
}

th {
  background: #f8fafc;
  color: #334155;
  font-weight: 600;
}
</style>
