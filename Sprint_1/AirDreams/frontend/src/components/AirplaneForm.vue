<template>
  <form @submit.prevent="registrarAeronave" class="form-wrapper">
    <div class="form-card">

      <div class="form-column">
        <div class="mb-3">
          <label for="matricula" class="form-label">Matrícula (10 caracteres max)</label>
          <input v-model="aeronave.matricula" type="text" maxlength="10" class="form-control" id="matricula" placeholder="Ejm: TI-BFJ">
        </div>

        <div class="mb-3">
          <label for="pesoMax" class="form-label">Peso Max (kg)</label>
          <input v-model="aeronave.pesoMax" type="number" class="form-control" id="pesoMax" placeholder="Ejm: 1000">
        </div>

        <div class="mb-3">
          <label for="cantidadPasajeros" class="form-label">Cantidad de Pasajeros</label>
          <input v-model="aeronave.cantidadPasajeros" type="number" class="form-control" id="cantidadPasajeros" placeholder="Ejm: 400">
        </div>

        <div class="mb-3">
          <label for="modelo" class="form-label">Modelo</label>
          <input v-model="aeronave.modelo" type="text" class="form-control" id="modelo" placeholder="Ejm: Boeing-747">
        </div>
      </div>

      <div class="form-column">
        <div class="mb-3">
          <label for="asientosFilaFirstClass" class="form-label">Asientos por Fila (FirstClass)</label>
          <input v-model="aeronave.asientosFilaFirstClass" type="number" class="form-control" id="asientosFilaFirstClass" placeholder="Ejm: 5">
        </div>

        <div class="mb-3">
          <label for="filasFirstClass" class="form-label">Cantidad de Filas (FirstClass)</label>
          <input v-model="aeronave.filasFirstClass" type="number" class="form-control" id="filasFirstClass" placeholder="Ejm: 20">
        </div>

        <div class="mb-3">
          <label for="asientosFilaTurist" class="form-label">Asientos por Fila (Turist)</label>
          <input v-model="aeronave.asientosFilaTurist" type="number" class="form-control" id="asientosFilaTurist" placeholder="Ejm: 5">
        </div>

        <div class="mb-3">
          <label for="filasTurist" class="form-label">Cantidad de Filas (Turist)</label>
          <input v-model="aeronave.filasTurist" type="number" class="form-control" id="filasTurist" placeholder="Ejm: 20">
        </div>
      </div>

      <button type="submit" class="btn boton-submit">
        Registrar Aeronave
      </button>

    </div>
  </form>
</template>

<script>
export default {
  name: 'AirplaneForm',

  data() {
    return {
      aeronave: {
        matricula: '',
        pesoMax: null,
        cantidadPasajeros: null,
        modelo: '',
        asientosFilaFirstClass: null,
        filasFirstClass: null,
        asientosFilaTurist: null,
        filasTurist: null
      }
    }
  },

  methods: {
    registrarAeronave() {
      console.log(this.aeronave)
    }
  }
}
</script>

<style scoped>
.form-wrapper {
  justify-content: center;
  margin-top: 25px;
}

.form-card {
  width: 670px;
  padding: 28px 32px 38px;
  display: grid;
  grid-template-columns: 1fr 1fr;
  column-gap: 95px;
  background-color: white;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
}

.form-label {
  font-family: 'Inter', sans-serif;
  color: #222;
  font-weight: 500;
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