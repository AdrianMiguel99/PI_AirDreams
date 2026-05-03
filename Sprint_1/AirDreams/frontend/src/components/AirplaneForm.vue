<template>
  <form @submit.prevent="onSubmit(aeronave)" class="form-container">
    <div class="form-subcontainer">

      <div class="form-column">
        <div class="mb-3">
          <label class="form-label">Matrícula (10 caracteres max)</label>
          <input v-model="aeronave.plateNumber" type="text" maxlength="10" class="form-control" placeholder="Ejm: TI-BFJ">
        </div>

        <div class="mb-3">
          <label class="form-label">Peso Max (kg)</label>
          <input v-model="aeronave.maxWeight" type="number" class="form-control" placeholder="Ejm: 1000">
        </div>

        <div class="mb-3">
          <label class="form-label">Cantidad de Pasajeros</label>
          <input v-model="aeronave.cantPasajeros" type="number" class="form-control" placeholder="Ejm: 400">
        </div>

        <div class="mb-3">
          <label class="form-label">Modelo</label>
          <input v-model="aeronave.modelo" type="text" class="form-control" placeholder="Ejm: Boeing-747">
        </div>
      </div>

      <div class="form-column">
        <div class="mb-3">
          <label class="form-label">Asientos por Fila (FirstClass)</label>
          <input v-model="aeronave.cant_Asientos_Fila_Firstclass" type="number" class="form-control">
        </div>

        <div class="mb-3">
          <label class="form-label">Cantidad de Filas (FirstClass)</label>
          <input v-model="aeronave.cant_Filas_Firstclass" type="number" class="form-control">
        </div>

        <div class="mb-3">
          <label class="form-label">Asientos por Fila (Turista)</label>
          <input v-model="aeronave.cant_Asientos_Fila_Turista" type="number" class="form-control">
        </div>

        <div class="mb-3">
          <label class="form-label">Cantidad de Filas (Turista)</label>
          <input v-model="aeronave.cant_Filas_Turista" type="number" class="form-control">
        </div>
      </div>

      <button type="submit" class="btn boton-submit">
        Guardar Aeronave
      </button>

    </div>
  </form>
</template>

<script>
export default {
  name: 'AirplaneForm',

  props: {
    aeronave: Object,
    onSubmit: Function
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
  font-weight: 600;
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