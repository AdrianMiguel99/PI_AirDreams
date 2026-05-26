<template>
  <form @submit.prevent="handleSubmit" class="form-container">
    <div class="form-subcontainer">

      <div class="form-column">
        <div class="mb-3">
          <label class="form-label">Modelo</label>
          <input
            v-model="aeronave.modelo"
            type="text"
            class="form-control"
            placeholder="Ejm: Boeing-747"
            :disabled="isEditMode"
          >
        </div>

        <div class="mb-3">
          <label class="form-label">Asientos por Fila (FirstClass)</label>
          <input
            v-model="aeronave.cant_Asientos_Fila_Firstclass"
            type="number"
            class="form-control"
            placeholder="Ejm: 5"
          >
        </div>

        <div class="mb-3">
          <label class="form-label">Asientos por Fila (Turista)</label>
          <input
            v-model="aeronave.cant_Asientos_Fila_Turista"
            type="number"
            class="form-control"
            placeholder="Ejm: 5"
          >
        </div>
      </div>

      <div class="form-column">
        <div class="mb-3">
          <label class="form-label">Peso Max (kg)</label>
          <input
            v-model="aeronave.maxWeight"
            type="number"
            class="form-control"
            placeholder="Ejm: 1000"
          >
        </div>

        <div class="mb-3">
          <label class="form-label">Cantidad de Filas (FirstClass)</label>
          <input
            v-model="aeronave.cant_Filas_Firstclass"
            type="number"
            class="form-control"
            placeholder="Ejm: 20"
          >
        </div>

        <div class="mb-3">
          <label class="form-label">Cantidad de Filas (Turista)</label>
          <input
            v-model="aeronave.cant_Filas_Turista"
            type="number"
            class="form-control"
            placeholder="Ejm: 100"
          >
        </div>
      </div>

      <div class="tamano-container">

        <label class="form-label">
          Tamaño de Aeronave
        </label>

        <select
          v-model="aeronave.aircraftSize"
          class="form-control select-tamano"
        >
          <option value="No definido">
            No definido
          </option>

          <option value="Pequena">
            Pequeña
          </option>

          <option value="Mediana">
            Mediana
          </option>

          <option value="Grande">
            Grande
          </option>

        </select>

      </div>

      <button
        type="submit" class="btn boton-submit"
      >
        Guardar Aeronave
      </button>

    </div>
  </form>
</template>

<script>
export default {
  name: 'AirplaneForm',

  props: {
    aeronave: {
      type: Object,
      required: true
    },

    onSubmit: {
      type: Function,
      required: true
    },

    isEditMode: {
      type: Boolean,
      default: false
    }
  },

  methods: {

    showValidationError(title, message) {
    this.$emit("show-popup", {
      type: "error",
      title,
      message
    });
    },

    validateForm() {

      if (!this.aeronave.modelo?.trim()) {
        this.showValidationError("Modelo requerido", "El modelo de la aeronave es obligatorio.");
        return false;
      }

      if (Number(this.aeronave.maxWeight) <= 0) {
        this.showValidationError("Peso inválido", "El peso de la aeronave debe ser un valor positivo.");
        return false;
      }

      if (
        Number(this.aeronave.cant_Asientos_Fila_Firstclass) < 0 ||
        Number(this.aeronave.cant_Filas_Firstclass) < 0
      ) {
        this.showValidationError("Valores inválidos", "Los valores para FirstClass no pueden ser negativos.");
        return false;
      }

      if (
        Number(this.aeronave.cant_Asientos_Fila_Firstclass) < 0 ||
        Number(this.aeronave.cant_Filas_Firstclass) < 0
      ) {
        this.showValidationError("Valores inválidos", "Los valores para FirstClass no pueden ser negativos.");
        return false;
      }


      if (
        Number(this.aeronave.cant_Asientos_Fila_Turista) < 0 ||
        Number(this.aeronave.cant_Filas_Turista) < 0
      ) {
        this.showValidationError("Valores inválidos", "Los valores para Turista no pueden ser negativos.");
        return false;
      }

      if (this.aeronave.aircraftSize === "No definido") {
        this.showValidationError("Tamaño requerido", "Seleccione un tamaño para la aeronave.");
        return false;
      }

      return true;
    },

    handleSubmit() {

      if (!this.validateForm()) {
        return;
      }

      this.onSubmit(this.aeronave);

    }

  }
}
</script>

<style scoped>
.form-container {
  display: flex;
  justify-content: center;
  width: 100%;
}

.form-subcontainer {
  width: 50%;

  display: grid;
  grid-template-columns: 1fr 1fr;

  column-gap: 60px;

  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.2);
}

.form-column {
  display: flex;
  flex-direction: column;
}

.form-label {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: 600;
  text-align: left;
}

.form-control,
.form-control option {
  font-family: 'Inter', sans-serif;
  color: #384467;
}

.tamano-container {
  grid-column: 1 / 3;
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.select-tamano {
  width: 45%;
}

.boton-submit {
  grid-column: 1 / 3;

  width: 70%;

  margin: 25px auto 0;

  background-color: #4c587d;
  color: white;

  font-family: 'Inter', sans-serif;
  font-weight: 600;

  border-radius: 8px;
}

.boton-submit:hover {
  background-color: #384467;
  color: white;
}
</style>