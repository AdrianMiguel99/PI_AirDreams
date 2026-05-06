<template>
  <div class="login-container">
    <div class="login-card">
      <h2>Iniciar Sesión</h2>

      <input 
        v-model="correo"
        type="email" 
        placeholder="Correo electrónico" 
        :class="{ errorInput: errorCorreo }"
      />

      <input 
        v-model="password"
        type="password" 
        placeholder="Contraseña" 
        :class="{ errorInput: errorPassword }"
      />

      <button @click="login" :disabled="cargando">
        {{ cargando ? "Ingresando..." : "Ingresar" }}
      </button>

      <p v-if="mensaje" class="error">
        {{ mensaje }}
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
      mensaje: "",
      cargando: false,
      errorCorreo: false,
      errorPassword: false
    };
  },

  methods: {
    async login() {
      this.mensaje = "";
      this.errorCorreo = false;
      this.errorPassword = false;

      if (!this.correo || !this.password) {
        this.mensaje = "Correo o contraseña incorrecta";
        this.errorCorreo = true;
        this.errorPassword = true;
        return;
      }

      this.cargando = true;

      try {
        const response = await fetch("http://localhost:5276/api/auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            correo: this.correo,
            password: this.password
          })
        });

        const data = await response.json();

        if (response.ok) {
          localStorage.setItem("user", JSON.stringify(data.user));
          this.$router.push("/");
        } else {
          this.mensaje = data.message || "Correo o contraseña incorrecta";
          this.errorCorreo = true;
          this.errorPassword = true;
        }
      } catch (error) {
        console.error(error);
        this.mensaje = "Error de conexión con el servidor";
        this.errorCorreo = true;
        this.errorPassword = true;
      } finally {
        this.cargando = false;
      }
    }
  }
};
</script>

<style scoped>
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

.login-card h2 {
  margin-bottom: 20px;
}

.login-card input {
  width: 100%;
  margin: 10px 0;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #ccc;
}

.login-card input.errorInput {
  border-color: red;
}

.login-card button {
  width: 100%;
  padding: 10px;
  border-radius: 20px;
  border: none;
  background: #2f3e5c;
  color: white;
  cursor: pointer;
}

.login-card button:disabled {
  background: #999;
}

.error {
  color: red;
  margin-top: 10px;
}

.registro-link {
  display: block;
  margin-top: 15px;
  text-decoration: none;
  color: #2f3e5c;
  font-weight: bold;
}
</style>