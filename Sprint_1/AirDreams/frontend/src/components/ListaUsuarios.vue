<template>
  <div class="container">
    <AdminHeader />
    
    <div class="header-section">
      <h2>Usuarios</h2>
      <div class="actions-container">
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
    </div>

    <div v-if="users.length === 0" class="no-users">
      No hay usuarios disponibles.
    </div>

    <div v-else>
      <CardUser 
        v-for="user in users" 
        :key="user.id"
        :user="user"
      />
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useRouter } from "vue-router";
import CardUser from "./CardUser.vue";
import AdminHeader from "./AdminHeader.vue";

export default {
  name: "UserList",
  components: {
    CardUser,
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
      searchTimeout: null
    };
  },
  methods: {
    fetchUsers() {
      axios.get("http://localhost:5276/api/User").then((response) => {
        this.users = response.data;
      });
    },
    handleSearch() {
      clearTimeout(this.searchTimeout);
      if (this.searchTerm.trim() === "") {
        this.fetchUsers();
        return;
      }

      this.searchTimeout = setTimeout(() => {
        axios
          .get("http://localhost:5276/api/User/search", {
            params: { searchTerm: this.searchTerm }
          })
          .then((response) => {
            this.users = response.data;
          })
          .catch((error) => {
            console.error("Error searching users:", error);
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
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.header-section h2 {
  margin: 0;
  font-size: 28px;
  color: #333;
}

.actions-container {
  display: flex;
  gap: 15px;
  align-items: center;
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
}

.add-button:hover {
  background-color: #1f2c47;
}

.search-container {
  display: flex;
  justify-content: flex-end;
}

.search-input {
  padding: 10px 15px;
  width: 350px;
  font-size: 14px;
  border: 1px solid #ddd;
  border-radius: 4px;
  outline: none;
  transition: border-color 0.3s;
}

.search-input:focus {
  border-color: #2f3e5c;
}

.no-users {
  text-align: center;
  color: #666;
  padding: 20px;
  font-size: 16px;
}
</style>
