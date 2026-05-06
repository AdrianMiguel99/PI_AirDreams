<template>
  <div class="completar-container">
    <div class="completar-card">
      <h2>Completar Registro</h2>
      
      <div v-if="tokenInvalido" class="error">
        Token inválido o expirado. Solicita una nueva invitación.
      </div>

      <div v-else-if="cargandoToken">
        <p>Validando invitación...</p>
      </div>

      <div v-else>
        <p class="info">Email: {{ email }}</p>
        <p class="info">Tipo: {{ tipoUsuario }}</p>

        <input
          v-model="nombreCompleto"
          type="text"
          placeholder="Nombre completo"
        />

        <input
          v-model="cedula"
          type="text"
          placeholder="Cédula (0-0000-0000)"
        />

        <input
          v-model="password"
          type="password"
          placeholder="Contraseña (Mayúscula, número, carácter especial)"
        />

        <input
          v-model="confirmPassword"
          type="password"
          placeholder="Confirmar contraseña"
        />

        <button @click="completarRegistro" :disabled="cargando">
          {{ cargando ? "Registrando..." : "Completar Registro" }}
        </button>

        <p v-if="mensaje" :class="mensajeError ? 'error' : 'exito'">
          {{ mensaje }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CompleteRegistration',
  data() {
    return {
      token: null,
      email: "",
      tipoUsuario: "",
      tokenInvalido: false,
      cargandoToken: true,
      nombreCompleto: "",
      cedula: "",
      password: "",
      confirmPassword: "",
      mensaje: "",
      mensajeError: false,
      cargando: false
    };
  },

  mounted() {
    this.token = this.$route.query.token;
    console.log("Token recibido:", this.token);
    
    if (this.token) {
      this.validarToken();
    } else {
      this.tokenInvalido = true;
      this.cargandoToken = false;
      this.mensaje = "No se proporcionó token de invitación";
      this.mensajeError = true;
    }
  },

  methods: {
    async validarToken() {
      try {
        const response = await fetch(
          `http://localhost:5276/api/auth/validate-invitation?token=${this.token}`
        );

        const data = await response.json();
        console.log("Respuesta validación:", data);

        if (response.ok && data.isValid) {
          this.email = data.email;
          this.tipoUsuario = data.role;
          this.tokenInvalido = false;
        } else {
          this.tokenInvalido = true;
          this.mensaje = data.message || "Token inválido o expirado";
          this.mensajeError = true;
        }
      } catch (error) {
        console.error("Error:", error);
        this.tokenInvalido = true;
        this.mensaje = "Error al validar el token";
        this.mensajeError = true;
      } finally {
        this.cargandoToken = false;
      }
    },

    async completarRegistro() {
      this.mensaje = "";
      this.mensajeError = false;

      if (!this.nombreCompleto || !this.cedula || !this.password || !this.confirmPassword) {
        this.mensaje = "Todos los campos son obligatorios";
        this.mensajeError = true;
        return;
      }

      if (this.password !== this.confirmPassword) {
        this.mensaje = "Las contraseñas no coinciden";
        this.mensajeError = true;
        return;
      }

      this.cargando = true;

      try {
        const response = await fetch("http://localhost:5276/api/auth/complete-registration", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            token: this.token,
            fullName: this.nombreCompleto,
            documentId: this.cedula,
            password: this.password
          })
        });

        const data = await response.json();
        console.log("Respuesta completar registro:", data);

        if (response.ok) {
          this.mensaje = data.message;
          this.mensajeError = false;
          
          setTimeout(() => {
            this.$router.push("/login");
          }, 2000);
        } else {
          this.mensaje = data.message || "Error al completar el registro";
          this.mensajeError = true;
        }
      } catch (error) {
        console.error("Error:", error);
        this.mensaje = "Error de conexión con el servidor";
        this.mensajeError = true;
      } finally {
        this.cargando = false;
      }
    }
  }
};
</script>

<style scoped>
.completar-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f5f5f5;
}

.completar-card {
  background: white;
  padding: 35px;
  border-radius: 15px;
  width: 450px;
  box-shadow: 0px 5px 20px rgba(0,0,0,0.1);
  text-align: center;
}

.completar-card h2 {
  margin-bottom: 20px;
}

.info {
  text-align: left;
  margin: 10px 0;
  padding: 8px;
  background: #f0f0f0;
  border-radius: 5px;
}

.completar-card input {
  width: 100%;
  margin: 10px 0;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid #ccc;
  font-size: 14px;
}

.completar-card button {
  width: 100%;
  padding: 12px;
  margin-top: 15px;
  border: none;
  border-radius: 20px;
  background: #2f3e5c;
  color: white;
  cursor: pointer;
  font-size: 16px;
}

.completar-card button:disabled {
  background: #999;
  cursor: not-allowed;
}

.exito {
  margin-top: 15px;
  color: green;
  font-weight: bold;
}

.error {
  margin-top: 15px;
  color: red;
  font-weight: bold;
}
</style>