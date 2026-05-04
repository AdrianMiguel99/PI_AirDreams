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
  
  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de Aeronaves</h1>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
      <thead>
        <tr>
          <th>Matrícula</th>
          <th>Modelo</th>
          <th>Acciones</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="airplane in airplanes" :key="airplane.plateNumber">
          <td>{{ airplane.plateNumber }}</td>
          <td>{{ airplane.modelo }}</td>
          <td>
            <a :href="`/editPlane/${airplane.plateNumber}`" class="btn btn-primary">
              Editar
            </a>

            <button @click="deleteAirplane(airplane.plateNumber)" class="btn btn-danger">
              Eliminar
            </button>

          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'ListAirplane',

  data() {
    return {
      airplanes: []
    };
  },

  methods: {
    getAirplanes() {
      axios.get('https://localhost:7136/api/Airplane')
        .then(response => {
          this.airplanes = response.data;
        })
        .catch(error => {
          console.error("ERROR:", error);
        });
    },

    deleteAirplane(plateNumber) {
      axios.delete(`https://localhost:7136/api/Airplane/${plateNumber}`).then(() => {
          alert('Aeronave eliminada con éxito');
          //recargar pagina para actualizar la lista de aeronaves
          this.getAirplanes();
        })
        .catch(error => {
          console.error("ERROR:", error.response?.data);
          alert('Error al eliminar la aeronave');
        });
    }
  },

  created() {
    this.getAirplanes();
  }
};
</script>

<style scoped>
</style>