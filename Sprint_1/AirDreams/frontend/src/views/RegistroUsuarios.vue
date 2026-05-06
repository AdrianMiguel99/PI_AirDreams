<template>
  <div class="registro-container">
    <div class="registro-card">
      <h2>Invitar Usuario</h2>
      <p class="subtitulo">Solo administradores pueden invitar nuevos usuarios</p>

      <input
        v-model="correo"
        type="email"
        placeholder="Correo electrónico"
        :class="{ errorInput: errorCorreo }"
        @input="errorCorreo = false"
      />

      <select v-model="tipoUsuario" :class="{ errorInput: errorTipo }">
        <option disabled value="">Seleccione tipo de usuario</option>
        <option value="Admin">Admin</option>
        <option value="Operator">Operator</option>
      </select>

      <button @click="enviarInvitacion" :disabled="cargando">
        {{ cargando ? "Enviando..." : "Enviar Invitación" }}
      </button>

      <p v-if="mensaje" :class="mensajeError ? 'error' : 'exito'">
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
      tipoUsuario: "",
      mensaje: "",
      mensajeError: false,
      cargando: false,
      errorCorreo: false,
      errorTipo: false
    };
  },

  methods: {
    async enviarInvitacion() {
      this.mensaje = "";
      this.mensajeError = false;
      
      if (!this.correo) {
        this.errorCorreo = true;
        this.mensaje = "El correo es obligatorio";
        this.mensajeError = true;
        return;
      }
      
      if (!this.tipoUsuario) {
        this.errorTipo = true;
        this.mensaje = "Debe seleccionar un tipo de usuario";
        this.mensajeError = true;
        return;
      }

      this.cargando = true;

      try {
        const token = localStorage.getItem("token");
        const response = await fetch("http://localhost:5276/api/auth/invite", {
          method: "POST",
          headers: {
            'Authorization': `Bearer ${token}`,
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            email: this.correo,
            role: this.tipoUsuario
          })
        });

        const data = await response.json();

        if (response.ok) {
          this.mensaje = data.message || "Invitación enviada exitosamente";
          this.mensajeError = false;
          this.correo = "";
          this.tipoUsuario = "";
        } else {
          this.mensaje = data.message || "Error al enviar la invitación";
          this.mensajeError = true;
        }

      } catch (error) {
        console.error(error);
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
.registro-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f5f5f5;
}

.registro-card {
  background: white;
  padding: 35px;
  border-radius: 15px;
  width: 400px;
  box-shadow: 0px 5px 20px rgba(0,0,0,0.1);
  text-align: center;
}

.registro-card h2 {
  margin-bottom: 10px;
}

.subtitulo {
  color: #666;
  font-size: 14px;
  margin-bottom: 20px;
}

.registro-card input,
.registro-card select {
  width: 100%;
  margin: 10px 0;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid #ccc;
  font-size: 16px;
}

.registro-card input.errorInput,
.registro-card select.errorInput {
  border-color: red;
}

.registro-card button {
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

.registro-card button:disabled {
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