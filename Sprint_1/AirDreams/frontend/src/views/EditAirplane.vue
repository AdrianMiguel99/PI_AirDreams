<template>
  <div class="container mt-5">
    <h1 class="mb-4 text-center">Editar Aeronave</h1>

    <AirplaneForm
      :aeronave="airplane"
      :onSubmit="updateAirplane"
    />
    <a href="/listPlanes" class="btn btn-secondary mt-3">
  Volver a lista
</a>
  </div>
</template>

<script>
import axios from 'axios';
import AirplaneForm from '../components/AirplaneForm.vue';

export default {
  components: { AirplaneForm },

  data() {
    return {
      airplane: null
    };
  },

  methods: {
    getAirplane() {
      const plateNumber = this.$route.params.plateNumber;

      axios.get(`https://localhost:7136/api/Airplane/${plateNumber}`).then(response => {
          this.airplane = response.data;
        })
        .catch(error => {
          console.error(error.response?.data);
        });
    },

    updateAirplane(updatedAirplane) {
      axios.put(`https://localhost:7136/api/Airplane/${updatedAirplane.plateNumber}`, updatedAirplane).then(() => {
        alert("Aeronave actualizada correctamente");
        this.getAirplane();
      })
      .catch(error => {
        console.error(error.response?.data);
        alert("Error al actualizar");
      });
    }
  },

  created() {
    this.getAirplane();
  }
};
</script>