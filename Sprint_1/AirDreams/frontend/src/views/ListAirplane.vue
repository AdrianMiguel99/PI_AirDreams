<template>
  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de Aeronaves</h1>
    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
      <thead>
        <tr>
          <th>Matricula</th>
          <th>Modelo</th>
          <th>Acciones</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="(airplane, index) of airplanes" :key="index">
          <td>{{ airplane.plateNumber }}</td>
          <td>{{ airplane.modelo }}</td>
          <td>
            <button @click="editAirplane(index)" class="btn btn-primary">Editar</button>
            <button @click="deleteAirplane(index)" class="btn btn-danger">Eliminar</button>
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
    deleteAirplane(index) {
      this.airplanes.splice(index, 1);
    },

    editAirplane(index) {
      console.log('Editar:', this.airplanes[index]);
    },

    getAirplanes() {
  axios.get('https://localhost:7136/api/Airplane')
    .then(response => {
      console.log("DATA:", response.data);
      this.airplanes = response.data;
    })
    .catch(error => {
      console.error("ERROR:", error);
    });
}
  },

  created() {
    this.getAirplanes();
  }
};
</script>

<style lang="scss" scoped>

</style>