<template>
  <nav class="navbar">
    <router-link to="/" class="logo-section">
      <img
        src="../assets/logo.png"
        alt="Air Dreams Logo"
        class="logo-img"
      />
      <span class="logo-text">Air Dreams</span>
    </router-link>

    <button
      v-if="isLoggedIn"
      type="button"
      class="btn adminPanel-button" 
      style = "font-size: 15px;"
      @click="$router.push({ name: 'admin' })"
      >
        Ir al panel administrador
    </button>
    

    <div class="links">
      <a href="/">Buscar Vuelos</a>
      <a href="/">Consultar Vuelos</a>
      <a href="/">Contacto</a>

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
      usuario: null,
      isLoggedIn: false
    }
  },
  mounted() {
    this.checkUser();
    window.addEventListener('storage', this.checkUser);
    this.isLoggedIn = !!localStorage.getItem("token");
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

.logo-section {
  display: flex;
  align-items: center;
  gap: 10px;
}

.logo-img {
  height: 40px;
  width: auto;
}

.logo-text {
  font-weight: bold;
  font-size: 20px;
  color: #2f3e5c;
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

.adminPanel-button {
    border: 1px solid #032056;
    background: white;
    color: #032056;
    border-radius: 6px;
    padding: 10px 16px;
    cursor: pointer;
    font-weight: 400;
}

.adminPanel-button:hover {
    background: #e0ecff;
}

.logo-section {
  display: flex;
  align-items: center;
  gap: 10px;
  text-decoration: none; /* quita la línea */
  color: inherit;        /* mantiene el color actual */
  cursor: pointer;
}
</style>