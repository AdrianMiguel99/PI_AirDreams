<template>
  <div>
    <AdminHeader />
    <h2>Usuarios</h2>

    <div v-if="users.length === 0">
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
import CardUser from "./CardUser.vue";
import AdminHeader from "./AdminHeader.vue";

export default {
  name: "UserList",
  components: {
    CardUser,
    AdminHeader
  },
  data() {
    return {
      users: []
    };
  },
  methods: {
    fetchUsers() {
      axios.get("http://localhost:5276/api/User").then((response) => {
        this.users = response.data;
      });
    }
  },
  created: function () {
    this.fetchUsers();
  }
};
</script>
