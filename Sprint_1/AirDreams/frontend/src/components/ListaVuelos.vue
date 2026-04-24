<template>
  <div>
    <h2>Vuelos disponibles</h2>

    <div v-if="vuelos.length === 0">
      No hay vuelos disponibles
    </div>

    <div v-else>
      <TarjetaVuelo 
        v-for="vuelo in vuelos" 
        :key="vuelo.id"
        :vuelo="vuelo"
      />
    </div>
  </div>
</template>

<script>
import TarjetaVuelo from "./TarjetaVuelo.vue";

export default {
  props: ["vuelos"],
  components: {
    TarjetaVuelo
  }
};
</script>