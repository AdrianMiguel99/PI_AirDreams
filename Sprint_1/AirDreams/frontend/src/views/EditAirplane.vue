<template>
  <div class="container mt-5">

    <ButtomNavigationAirplanes
      mode="edit"
      class="mb-5"
    />

    <div class="form-title-container">
      <h1 class="form-title">
        Editar Aeronave
      </h1>
    </div>

    <AirplaneForm
      v-if="airplane"
      :aeronave="airplane"
      :onSubmit="updateAirplane"
      :isEditMode="true"
      @show-popup="openPopup"
    />

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="handlePopupAction"
    />

  </div>
</template>

<script>
import axios from 'axios';
import AirplaneForm from '../components/Airplane/AirplaneForm.vue';
import PopupMessage from '../components/PopupMessage.vue';
import ButtomNavigationAirplanes from '../components/Airplane/ButtomNavigationAirplanes.vue';

export default {

  components: {
    AirplaneForm,
    PopupMessage,
    ButtomNavigationAirplanes
  },

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

      const modelo =
        this.$route.params.modelo;

      axios
        .get(`http://localhost:5276/api/Airplane/${modelo}`).then(response => {

          this.airplane = response.data;
          delete this.airplane.cantPasajeros;
        }).catch(error => {
          console.error(
            error.response?.data
          );

        });
    },

    updateAirplane(updatedAirplane) {

      axios
        .put(
          `http://localhost:5276/api/Airplane/${updatedAirplane.modelo}`,updatedAirplane).then(() => {
          this.showPopup = true;
          this.popupType = 'success';
          this.popupTitle = 'Éxito';
          this.popupMessage = 'Aeronave actualizada correctamente';
          this.popupActionText = 'Volver a la lista';
          this.getAirplane();

        })

        .catch(error => {

          console.error(
            error.response?.data
          );
          this.showPopup = true;
          this.popupType = 'error';
          this.popupTitle = 'Error';
          this.popupMessage = 'Error al actualizar la aeronave';
          this.popupActionText = '';
        });
    },

    goBack() {
      this.$router.push('/pruebas');
    },

    handlePopupAction() {
      this.showPopup = false;
    if (
      this.popupActionText ===
      'Volver a la lista'
    ) {
      this.goBack();
    }

    },

    openPopup(popupData) {
      this.showPopup = true;
      this.popupType = popupData.type;
      this.popupTitle = popupData.title;
      this.popupMessage = popupData.message;
      this.popupActionText = popupData.actionText || 'Aceptar';
    },

  },

  created() {
    this.getAirplane();
  }

};
</script>

<style scoped>

.form-title-container {
  width: 50%;
  margin: 0 auto 20px;
}

.form-title {
  color: #384467;
  font-family: 'Inter', sans-serif;
  font-weight: bold;
  text-align: left;
  margin: 0;
}

</style>