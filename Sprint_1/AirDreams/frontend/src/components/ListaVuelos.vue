<template>
  <div class="lista-container">
    <h2 class="titulo">{{ title }}</h2>
    <div v-if="flights.length === 0" class="no-flights">
      No hay vuelos disponibles
    </div>

    <div v-else class="vuelos-grid">
      <TarjetaVuelo 
        v-for="flight in flights" 
        :key="flight.id"
        :flight="flight"
        :departureDate="departureDate"
      />
    </div>
  </div>
</template>

<script>
import TarjetaVuelo from "./TarjetaVuelo.vue";

export default {
  props: {
  flights: {
    type: Array,
    default: () => []
  },
  title: {
    type: String,
    default: "Vuelos disponibles"
  }
  },
  components: {
    TarjetaVuelo
  }
};
</script>

<style scoped>
.lista-container {
  width: 80%;
  margin: 30px auto;
}

.titulo {
  font-size: 24px;
  font-weight: 700;
  color: #032056;
  margin-bottom: 20px;
}

.no-flights {
  text-align: center;
  padding: 40px;
  color: #666;
  font-size: 16px;
}

.vuelos-grid {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
</style>