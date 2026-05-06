<template>
  <div class="container mt-5">
    <h1 class="mb-4 text-center" style="color: #384467; font-family: 'Inter', sans-serif; font-weight: bold;">Editar Aeronave</h1>

    <AirplaneForm
      :aeronave="airplane"
      :onSubmit="updateAirplane"
    />
    <a href="/listPlanes" class="mt-3" style="background-color: #384467; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; 
                          font-family: 'Inter', sans-serif; font-weight: 400;"> 
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

      axios.get(`http://localhost:5276/api/Airplane/${plateNumber}`).then(response => {
          this.airplane = response.data;
        })
        .catch(error => {
          console.error(error.response?.data);
        });
    },

    updateAirplane(updatedAirplane) {
      axios.put(`http://localhost:5276/api/Airplane/${updatedAirplane.plateNumber}`, updatedAirplane).then(() => {
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