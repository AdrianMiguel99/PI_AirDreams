<template>
  <div>

    <BarraNavegacion />
    <SeccionHero />

    <FormularioBusqueda @search="handleSearch" />

    <div 
      v-if="searchPerformed && flights.length === 0" 
      class="no-results"
    >
      No hay vuelos disponibles para los filtros seleccionados
    </div>

    <ListaVuelos 
      v-if="searchPerformed && flights.length > 0"
      :flights="paginatedFlights"
      :departureDate="departureDate"
    />

    <Paginacion 
      v-if="searchPerformed && flights.length > 0"
      :total="flights.length" 
      :perPage="10"
      @changePage="changePage"
    />
  </div>
</template>

<script>
import BarraNavegacion from "../components/BarraNavegacion.vue";
import SeccionHero from "../components/SeccionHero.vue";
import FormularioBusqueda from "../components/FormularioBusqueda.vue";
import ListaVuelos from "../components/ListaVuelos.vue";
import Paginacion from "../components/Paginacion.vue";

export default {
  components: {
    BarraNavegacion,
    SeccionHero,
    FormularioBusqueda,
    ListaVuelos,
    Paginacion
  },

  data() {
    return {
      currentPage: 1,
      flights: [],
      searchPerformed: false,
      departureDate: ''
    };
  },

  computed: {
    paginatedFlights() {
      const start = (this.currentPage - 1) * 10;
      return this.flights.slice(start, start + 10);
    }
    
  },
  
      


  methods: {
    handleSearch(data) {
      this.flights = data.flights || [];
      this.departureDate = data.departureDate;
      this.searchPerformed = true;
      this.currentPage = 1;
    },

    changePage(page) {
      this.currentPage = page;
    }
  }
};
</script>

<style scoped>
.no-results {
  width: 80%;
  margin: 20px auto;
  padding: 20px;
  text-align: center;
  background: #f4f6fb;
  border-radius: 10px;
  color: #032056;
  font-weight: 600;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}
</style>
