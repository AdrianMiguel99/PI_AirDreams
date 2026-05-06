<template>
  <nav class="navbar">
    <div class="logo">Air Dreams</div>

    <div class="links">
      <a href="#">Buscar Vuelos</a>
      <a href="#">Consultar Vuelos</a>
      <a href="#">Contacto</a>

      <router-link v-if="!usuario" to="/login">
        <button class="login-btn">Iniciar Sesión</button>
      </router-link>

      <div v-else class="user-menu">
        <span>Hola, {{ usuario.nombreCompleto?.split(' ')[0] }}</span>
        <button @click="cerrarSesion" class="logout-btn">Cerrar Sesión</button>
      </div>
    </div>
  </nav>
</template>

<script>
export default {
  data() {
    return {
      usuario: null
    }
  },
  mounted() {
    this.checkUser();
    window.addEventListener('storage', this.checkUser);
  },
  beforeDestroy() {
    window.removeEventListener('storage', this.checkUser);
  },
  methods: {
    checkUser() {
      const user = localStorage.getItem('user');
      this.usuario = user ? JSON.parse(user) : null;
    },
    cerrarSesion() {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      this.usuario = null;
      this.$router.push('/login');
    }
  }
};
</script>

<style>
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 40px;
  background: white;
  box-shadow: 0px 2px 10px rgba(0,0,0,0.1);
}

.logo {
  font-weight: bold;
  font-size: 20px;
}

.links {
  display: flex;
  align-items: center;
  gap: 20px;
}

.links a {
  text-decoration: none;
  color: #333;
}

.login-btn, .logout-btn {
  padding: 8px 15px;
  border-radius: 20px;
  border: none;
  background: #2f3e5c;
  color: white;
  cursor: pointer;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 15px;
}

.user-menu span {
  color: #333;
}
</style>