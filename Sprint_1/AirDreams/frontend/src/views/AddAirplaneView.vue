<template>
  <!-- Icono -->
  <div class="align-items-center">
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
    <h1 class="letra_bold text-center" style="font-size: 2.5rem;">
      Registro de Aeronave
    </h1>

    <div class="row justify-content-end letra_bold">
      <div class="col-2">
        <a href="/listPlanes">
            <button type="button" class="btn btn-outline-secondary boton_listar">
              Listar Aeronaves
            </button>
        </a>
      </div>
    </div>
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
</style>