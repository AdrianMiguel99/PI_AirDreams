<template>
  <!-- Icono -->
  <div class="d-flex align-items-center">
    <img 
      src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png"
      class="img-fluid"
      alt="imagen de aerolinea"
      style="width:50px"
    >
    <h2 class="letra_semibold" style="font-size: 1.3rem; padding-top: 10px;">
      Air Dreams
    </h2>
  </div>

  <!-- Título -->
  <div class="container mt-5">

    <div class="d-flex justify-content-end align-items-center">
      <a href="/listPlanes"class="btn-volver-lista me-3">
        Listar Aeronaves
      </a>

      <a href="/admin"class="btn-volver-lista">
        Regresar
      </a>

    </div>

    <h1 class="letra_bold text-center" style="font-size: 2.5rem; margin-top: 20px; margin-bottom: 50px;">
      Registro de Aeronave
    </h1>
  </div>

 <!-- Formulario -->
  <AirplaneForm
    :aeronave="aeronave"
    :onSubmit="saveAirplane"
  />

</template>

<script>
import axios from 'axios'
import AirplaneForm from '../components/AirplaneForm.vue'

export default {
  name: 'AddAirplaneView',

  components: {
    AirplaneForm
  },

  data() {
    return {
      aeronave: {
        plateNumber: '',
        maxWeight: null,
        cantPasajeros: null,
        cant_Asientos_Fila_Firstclass: null,
        cant_Filas_Firstclass: null,
        cant_Asientos_Fila_Turista: null,
        cant_Filas_Turista: null,
        modelo: '',
      }
    }
  },

  methods: {
    saveAirplane(aeronave) {

      const token = localStorage.getItem("token");
      axios.post('http://localhost:5276/api/Airplane',aeronave,
      {
        headers: {Authorization: `Bearer ${token}`}
      })
      .then(() => {
        alert('Aeronave registrada correctamente');
      })
      .catch((error) => {
        console.error(error);

        if (error.response?.status === 401) {
          alert('Debes iniciar sesión');
        return;
        }

        if (error.response?.status === 403) {
          alert('No tienes permisos');
          return;
        }

        alert('Error al registrar aeronave');
      });
    }
  }
}
</script>

<style scoped>
.letra_bold {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
}

.letra_semibold {
  font-family: 'Inter', sans-serif;
  font-weight: 600;
  color: #384467;
}

.boton_listar {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: 600;
  border: 2px solid #384467;
}

.boton_listar:hover {
  background-color: #384467;
  color: white;
}

.btn-volver-lista {

  display: inline-block;

  background-color: #384467;
  color: white;

  padding: 10px 20px;

  text-decoration: none;

  border-radius: 8px;

  font-family: 'Inter', sans-serif;
  font-weight: 500;

  transition: 0.3s;
}
</style>