<template>
  <div class=" mt-5 add-airplane-view">

    <div class="d-flexj ustify-content-between mb-4">
      <h1 class="container title">
        Aeronaves Registradas
      </h1>
    </div>

    <AirplaneForm
      :aeronave="aeronave"
      :onSubmit="saveAirplane"
    />

  </div>
</template>

<script>
import axios from 'axios'
import AirplaneForm from '../components/Airplane/AirplaneForm.vue'

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
        modelo: ''
      }
    }
  },

  methods: {
    saveAirplane(aeronave) {
      const token = localStorage.getItem('token')

      axios.post('http://localhost:5276/api/Airplane', aeronave, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      })
        .then(() => {
          alert('Aeronave registrada correctamente')

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
        })
        .catch((error) => {
          console.error(error)

          if (error.response?.status === 401) {
            alert('Debes iniciar sesión')
            return
          }

          if (error.response?.status === 403) {
            alert('No tienes permisos')
            return
          }

          alert('Error al registrar aeronave')
        })
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

.letra_bold {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 2.5rem;
  margin-bottom: 50px;
}

.container {
  padding-right: 100px;
  width: 50%;
}
</style>