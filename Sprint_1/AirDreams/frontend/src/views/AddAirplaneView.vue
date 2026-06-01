<template>
  <div class="mt-5 add-airplane-view">

    <div class="d-flex justify-content-between mb-4">
      <h1 class="container title">
        Registrar Aeronave
      </h1>
    </div>

    <AirplaneForm
      :aeronave="aeronave"
      :onSubmit="saveAirplane"
      @show-popup="showErrorFromForm"
    />

    <!-- popup -->
    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="goToList"
    />

  </div>
</template>

<script>
import axios from 'axios'

import AirplaneForm from '../components/Airplane/AirplaneForm.vue'
import PopupMessage from '../components/PopupMessage.vue'

export default {
  name: 'AddAirplaneView',

  components: {
    AirplaneForm,
    PopupMessage
  },

  data() {
    return {
      aeronave: {
        maxWeight: null,
        cant_Asientos_Fila_Firstclass: null,
        cant_Filas_Firstclass: null,
        cant_Asientos_Fila_Turista: null,
        cant_Filas_Turista: null,
        modelo: ''
      },

      // POPUP
      showPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: '',
      popupActionText: ''
    }
  },

  methods: {

    resetForm() {
      this.aeronave = {
        plateNumber: '',
        maxWeight: null,
        cantPasajeros: null,
        cant_Asientos_Fila_Firstclass: null,
        cant_Filas_Firstclass: null,
        cant_Asientos_Fila_Turista: null,
        cant_Filas_Turista: null,
        modelo: ''
      }
    },

    showSuccessPopup() {
      this.popupType = 'success'
      this.popupTitle = 'Aeronave registrada exitosamente'
      this.popupMessage = 'La aeronave fue creada correctamente.'
      this.popupActionText = 'Ver lista'
      this.showPopup = true
    },

    showErrorPopup(message) {
      this.popupType = 'error'
      this.popupTitle = 'Error al registrar aeronave'
      this.popupMessage = message
      this.popupActionText = ''
      this.showPopup = true
    },

    goToList() {
      this.showPopup = false
      this.$emit('change-view', 'list')
    },

    saveAirplane(aeronave) {
      const token = localStorage.getItem('token')

      axios.post(
        'http://localhost:5276/api/Airplane',
        aeronave,
        {
          headers: {
            Authorization: `Bearer ${token}`
          }
        }
      )
      .then(() => {
        this.showSuccessPopup()
        this.resetForm()
      })
      .catch((error) => {
        if (error.response?.status === 401) {
          this.showErrorPopup('Debes iniciar sesión')
          return
        }

        if (error.response?.status === 403) {
          this.showErrorPopup('No tienes permisos')
          return
        }

        this.showErrorPopup(
          error.response?.data?.message ||
          error.response?.data ||
          'Ocurrió un error inesperado'
        )
      })
    },

    showErrorFromForm(popupData) {
      this.popupType = popupData.type
      this.popupTitle = popupData.title
      this.popupMessage = popupData.message
      this.popupActionText = ''
      this.showPopup = true
    }
  }
}
</script>

<style scoped>
.add-airplane-view {
  width: 100%;
  margin-top: 40px;
}

.title {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 2.5rem;
}

.container {
  padding-right: 100px;
  width: 50%;
}
</style>