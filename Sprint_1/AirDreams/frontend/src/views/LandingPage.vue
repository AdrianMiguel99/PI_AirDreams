<template>
  <div>
    <BarraNavegacion />
    <SeccionHero />

    <FormularioBusqueda @search="handleSearch" />

    <div
      v-if="searchPerformed && outboundFlights.length === 0 && returnFlights.length === 0"
      class="no-results"
    >
      No hay vuelos disponibles para los filtros seleccionados
    </div>

    <ListaVuelos
      v-if="searchPerformed && outboundFlights.length > 0"
      title="Vuelos de ida"
      :flights="paginatedOutboundFlights"
      :departureDate="departureDate"
      @buy="handleBuyFlight($event, 'outbound')"
    />

    <Paginacion
      v-if="searchPerformed && outboundFlights.length > outboundPerPage"
      :total="outboundFlights.length"
      :perPage="outboundPerPage"
      @changePage="changeOutboundPage"
    />

    <ListaVuelos
      v-if="searchPerformed && tripType === 'roundTrip' && returnFlights.length > 0"
      title="Vuelos de regreso"
      :flights="paginatedReturnFlights"
      @buy="handleBuyFlight($event, 'return')"
    />

    <Paginacion
      v-if="searchPerformed && tripType === 'roundTrip' && returnFlights.length > returnPerPage"
      :total="returnFlights.length"
      :perPage="returnPerPage"
      @changePage="changeReturnPage"
    />

    <div
      v-if="searchPerformed && outboundFlights.length === 0 && returnFlights.length > 0"
      class="no-results"
    >
      No hay vuelos de ida disponibles para los filtros seleccionados
    </div>

    <div
      v-if="searchPerformed && tripType === 'roundTrip' && outboundFlights.length > 0 && returnFlights.length === 0"
      class="no-results"
    >
      No hay vuelos de regreso disponibles para los filtros seleccionados
    </div>
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
      outboundFlights: [],
      returnFlights: [],
      tripType: '',
      outboundPage: 1,
      returnPage: 1,
      outboundPerPage: 10,
      returnPerPage: 10,
      searchPerformed: false,
      departureDate: '',
      passengersCount: 1,
      selectedPurchase: null
    };
  },

  computed: {
    paginatedOutboundFlights() {
      const start = (this.outboundPage - 1) * this.outboundPerPage;
      return this.outboundFlights.slice(start, start + this.outboundPerPage);
    },

    paginatedReturnFlights() {
      const start = (this.returnPage - 1) * this.returnPerPage;
      return this.returnFlights.slice(start, start + this.returnPerPage);
    }
    },
  
    methods: {
      handleSearch(data) {
        this.tripType = data.type;
        this.outboundFlights = data.outboundFlights || [];
        this.returnFlights = data.returnFlights || [];
        this.departureDate = data.departureDate;
        this.passengersCount = Number(data.passengers || 1);
        this.searchPerformed = true;
        this.currentPage = 1;
      },

      changePage(page) {
        this.currentPage = page;
      },

      handleBuyFlight(selection, direction) {
        const purchaseSelection = {
          tripType: this.tripType,
          direction,
          seatClass: selection.seatClass,
          price: selection.price,
          passengerCount: this.passengersCount,
          itinerary: selection.flight,
          selectedAt: new Date().toISOString()
        };

        this.selectedPurchase = purchaseSelection;
        sessionStorage.setItem(
          'selectedFlightPurchase',
          JSON.stringify(purchaseSelection)
        );

        this.$router.push({ name: 'passengers' });
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
