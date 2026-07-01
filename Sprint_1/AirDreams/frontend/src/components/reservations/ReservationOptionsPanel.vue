<template>
  <div class="options-panel">
    <section>
      <h2>Opciones de viaje</h2>

      <button class="option-button" type="button" :disabled="disabled" @click="$emit('add-luggage')">
        <span class="option-left">
          <img
            class="option-icon"
            src="../../assets/icons/maleta_azul_sin_fondo_REAL.png"
            alt="Icono de equipaje"
          >
          Equipaje
        </span>

        <span>›</span>
      </button>
    </section>

    <section class="management-section">
      <h2>Manejo del viaje</h2>

      <button
        class="option-button"
        type="button"
        :disabled="loading || disabled"
        @click="$emit('cancel-reservation')"
      >
        <span class="option-left">
          <img
            class="option-icon"
            src="../../assets/icons/vuelo_cancelado.png"
            alt="Icono de cancelar itinerario"
          >
          {{ loading ? "Enviando..." : "Cancelar itinerario" }}
        </span>

        <span>›</span>
      </button>
    </section>
  </div>
</template>

<script>
export default {
  name: "ReservationOptionsPanel",

  props: {
    loading: {
      type: Boolean,
      default: false
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },

  emits: ["cancel-reservation", "add-luggage"],

  methods: {
  }
};
</script>

<style scoped>
.options-panel {
  min-height: 590px;
  background: white;
  border-radius: 10px;
  padding: 22px 16px;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.18);
}

.options-panel h2 {
  margin: 0 0 18px;
  color: #384467;
  font-size: 21px;
  font-weight: 800;
}

.option-button {
  width: 100%;
  border: none;
  border-radius: 999px;
  padding: 8px 14px;
  background: #fde7e7;
  color: #384467;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 600;
  cursor: pointer;
}

.option-button:hover {
  background: #fbd6d6;
}

.option-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.option-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.option-left i {
  color: #032056;
  font-size: 18px;
}

.management-section {
  margin-top: 235px;
}

.option-icon {
  width: 22px;
  height: 22px;
  object-fit: contain;
  display: block;
}

@media (max-width: 900px) {
  .options-panel {
    min-height: auto;
  }

  .management-section {
    margin-top: 40px;
  }
}
</style>