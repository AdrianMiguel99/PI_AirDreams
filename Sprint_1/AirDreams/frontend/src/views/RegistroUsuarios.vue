<template>
  <div class="registro-container">
    <div class="registro-card">
      <h2>Registro de Usuarios</h2>

      <input
        v-model="nombreCompleto"
        type="text"
        placeholder="Nombre completo"
      />

      <select v-model="tipoUsuario">
        <option disabled value="">Seleccione tipo de usuario</option>
        <option>Administrador</option>
        <option>Operario</option>
      </select>

      <input
        v-model="correo"
        type="email"
        placeholder="Correo electrónico"
      />

      <input
        v-model="cedula"
        type="text"
        placeholder="Cédula (0-0000-0000)"
      />

      <button @click="registrarUsuario">
        Registrar
      </button>

      <p v-if="mensaje" class="mensaje">
        {{ mensaje }}
      </p>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      nombreCompleto: "",
      tipoUsuario: "",
      correo: "",
      cedula: "",
      mensaje: ""
    };
  },

  methods: {
    async registrarUsuario() {
      this.mensaje = "";

      if (
        !this.nombreCompleto ||
        !this.tipoUsuario ||
        !this.correo ||
        !this.cedula
      ) {
        this.mensaje = "Todos los campos son obligatorios";
        return;
      }

      try {
        const response = await fetch("http://localhost:5276/api/Auth/register", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            nombreCompleto: this.nombreCompleto,
            tipoUsuario: this.tipoUsuario,
            correo: this.correo,
            cedula: this.cedula
          })
        });

        const data = await response.text();
        this.mensaje = data;

        if (response.ok) {
          this.nombreCompleto = "";
          this.tipoUsuario = "";
          this.correo = "";
          this.cedula = "";
        }

      } catch (error) {
        console.error(error);
        this.mensaje = "Error de conexión con el servidor";
      }
    }
  }
};
</script>

<style>
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
  margin-bottom: 20px;
}

.registro-card input,
.registro-card select {
  width: 100%;
  margin: 10px 0;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid #ccc;
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
}

.mensaje {
  margin-top: 15px;
  color: #2f3e5c;
  font-weight: bold;
}
</style>