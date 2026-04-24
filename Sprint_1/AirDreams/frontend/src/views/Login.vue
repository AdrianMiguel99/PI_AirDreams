<template>
  <div class="login-container">

    <div class="login-card">
      <h2>Iniciar Sesión</h2>

      <input 
        v-model="correo"
        type="email" 
        placeholder="Correo electrónico" 
      />

      <input 
        v-model="password"
        type="password" 
        placeholder="Contraseña" 
      />

      <button @click="login">Ingresar</button>

      <p v-if="error" class="error">
        Correo o contraseña incorrectos.
      </p>
    </div>

  </div>
</template>

<script>
export default {
  data() {
    return {
      correo: "",
      password: "",
      error: false
    };
  },

  methods: {
    login() {
      // SIMULACIÓN (luego backend)
      if (this.correo === "admin@air.com" && this.password === "1234") {
        alert("Login correcto");
      } else {
        this.error = true;
      }
    }
  }
};
</script>

<style>
.login-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f5f5f5;
}

.login-card {
  background: white;
  padding: 30px;
  border-radius: 15px;
  width: 300px;
  box-shadow: 0px 5px 20px rgba(0,0,0,0.1);
  text-align: center;
}

.login-card input {
  width: 100%;
  margin: 10px 0;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #ccc;
}

.login-card button {
  width: 100%;
  padding: 10px;
  border-radius: 20px;
  border: none;
  background: #2f3e5c;
  color: white;
}

.error {
  color: red;
  margin-top: 10px;
}
</style>
