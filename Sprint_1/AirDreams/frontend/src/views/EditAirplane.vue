<template>
  <div class="container mt-5">
    <h1 class="mb-4 text-center" style="color: #384467; font-family: 'Inter', sans-serif; font-weight: bold;">Editar Aeronave</h1>

    <AirplaneForm
      :aeronave="airplane"
      :onSubmit="updateAirplane"
    />

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="goBack"
    />

    <a href="/pruebas" class="mt-3" style="background-color: #384467; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; 
                          font-family: 'Inter', sans-serif; font-weight: 400;"> 
      Regresar
    </a>
  </div>
</template>

<script>
import axios from 'axios';
import AirplaneForm from '../components/Airplane/AirplaneForm.vue';
import PopupMessage from '../components/PopupMessage.vue'

export default {
  components: { AirplaneForm, PopupMessage },

  data() {
    return {
      airplane: null,
      showPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: '',
      popupActionText: 'Volver a la lista'
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
        this.showPopup = true;
        this.popupType = 'success';
        this.popupTitle = 'Éxito';
        this.popupMessage = 'Aeronave actualizada correctamente';
        this.getAirplane();
      })
      .catch(error => {
        console.error(error.response?.data);
        this.showPopup = true;
        this.popupType = 'error';
        this.popupTitle = 'Error';
        this.popupMessage = 'Error al actualizar la aeronave';
      });
    }
  },

  created() {
    this.getAirplane();
  }
};
</script>