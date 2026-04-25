<template>
  <nav class="navbar">
    <div class="logo">Air Dreams</div>

    <div class="links">
      <a href="#">Buscar Vuelos</a> <!-- Placeholder -->
      <a href="#">Consultar Vuelos</a>
      <a href="#">Contacto</a>

      <router-link to="/login">
      <button class="login-btn">Iniciar Sesión</button>
      </router-link>
      
    </div>
  </nav>
</template>

<style>
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 40px;
  background: white;
  box-shadow: 0px 2px 10px rgba(0,0,00.1);
}

.logo {
  font-weight: bold;
  font-size: 20px;
}

.links a {
  margin: 0 15px;
  text-decoration: none;
  color: #333;
}

.login-btn {
  padding: 8px 15px;
  border-radius: 20px;
  border: none;
  background: #2f3e5c;
  color: white;
}
</style>