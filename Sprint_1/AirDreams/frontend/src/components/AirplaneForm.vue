<template>
  <form @submit.prevent="registrarAeronave" class="form-container">
    <div class="form-subcontainer">
      <div class="form-column">

        <div class="mb-3">
          <label for="matricula" class="form-label">Matrícula (10 caracteres max)</label>
          <input v-model="aeronave.plateNumber" type="text" maxlength="10" class="form-control" id="matricula" placeholder="Ejm: TI-BFJ">
        </div>

        <div class="mb-3">
          <label for="pesoMax" class="form-label">Peso Max (kg)</label>
          <input v-model="aeronave.maxWeight" type="number" class="form-control" id="pesoMax" placeholder="Ejm: 1000">
        </div>

        <div class="mb-3">
          <label for="cantidadPasajeros" class="form-label">Cantidad de Pasajeros</label>
          <input v-model="aeronave.cantPasajeros" type="number" class="form-control" id="cantidadPasajeros" placeholder="Ejm: 400">
        </div>

        <div class="mb-3">
          <label for="modelo" class="form-label">Modelo</label>
          <input v-model="aeronave.modelo" type="text" class="form-control" id="modelo" placeholder="Ejm: Boeing-747">
        </div>
      </div>

      <div class="form-column">
        <div class="mb-3">
          <label for="asientosFilaFirstClass" class="form-label">Asientos por Fila (FirstClass)</label>
          <input v-model="aeronave.cant_Asientos_Fila_Firstclass" type="number" class="form-control" id="asientosFilaFirstClass" placeholder="Ejm: 5">
        </div>

        <div class="mb-3">
          <label for="filasFirstClass" class="form-label">Cantidad de Filas (FirstClass)</label>
          <input v-model="aeronave.cant_Filas_Firstclass" type="number" class="form-control" id="filasFirstClass" placeholder="Ejm: 20">
        </div>

        <div class="mb-3">
          <label for="asientosFilaTurist" class="form-label">Asientos por Fila (Turist)</label>
          <input v-model="aeronave.cant_Asientos_Fila_Turista" type="number" class="form-control" id="asientosFilaTurist" placeholder="Ejm: 5">
        </div>

        <div class="mb-3">
          <label for="filasTurist" class="form-label">Cantidad de Filas (Turist)</label>
          <input v-model="aeronave.cant_Filas_Turista" type="number" class="form-control" id="filasTurist" placeholder="Ejm: 20">
        </div>
      </div>

      <button type="submit" class="btn boton-submit">
        Registrar Aeronave
      </button>

    </div>
  </form>
</template>

<script>
import axios from 'axios';
export default {
  name: 'AirplaneForm',

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
    registrarAeronave() {
      console.log(this.aeronave)
      axios.post('https://localhost:7136/api/Airplane', this.aeronave).then(response => {
        alert('Aeronave guardada con éxito');
        this.$emit('airplane-registered');
      }).catch(error => {
  console.error("STATUS:", error.response?.status);
  console.error("DATA:", error.response?.data);
  console.error("FULL ERROR:", error);
  alert('Error al guardar la aeronave');
});
    }
  }
}
</script>

<style scoped>
.form-container {
  display: flex;
  justify-content: center;
  max-width: 50%;
}

.form-subcontainer {
  width: 90%;
  display: grid;
  column-gap: 60px;
}

.form-label {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: semi-bold;
   text-align: left;
}

.form-control {
  color: #384467;
}

.boton-submit {
  grid-column: 1 / 3;
  width: 70%;
  margin: 25px auto 0;
  background-color: #4c587d;
  color: white;
  font-family: 'Inter', sans-serif;
  font-weight: 600;
  border-radius: 0;
}

.boton-submit:hover {
  background-color: #384467;
  color: white;
}
</style>