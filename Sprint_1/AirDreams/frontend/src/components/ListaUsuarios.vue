<template>
  <div>
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
import CardUser from "./CardUser.vue";

export default {
  name: "UserList",
  components: {
    CardUser
  },
  data() {
    return {
      users: []
    };
  },
  methods: {
    fetchUsers() {
      fetch('http://localhost:5276/api/User')
        .then(response => response.json())
        .then(data => {
          this.users = data;
        });
    }
  },
  created: function () {
    this.fetchUsers();
  }
};
</script>
