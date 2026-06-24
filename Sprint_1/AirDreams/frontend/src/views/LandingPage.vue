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
      title="Vuelos disponibles"
      :flights="paginatedFlights"
      :departureDate="departureDate"
      @buy="handleBuyFlight"
    />

    <Paginacion
      v-if="searchPerformed && flights.length > flightsPerPage"
      :total="flights.length"
      :perPage="flightsPerPage"
      @changePage="changePage"
    />
    </div>

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="popupAction"
    />
</template>

<script>
import BarraNavegacion from "../components/BarraNavegacion.vue";
import SeccionHero from "../components/SeccionHero.vue";
import FormularioBusqueda from "../components/FormularioBusqueda.vue";
import ListaVuelos from "../components/ListaVuelos.vue";
import Paginacion from "../components/Paginacion.vue";
import PopupMessage from "../components/PopupMessage.vue";

export default {
  components: {
    BarraNavegacion,
    SeccionHero,
    FormularioBusqueda,
    ListaVuelos,
    Paginacion,
    PopupMessage
  },

  data() {
    return {
      currentPage: 1,
      flights: [],
      flightsPerPage: 10,
      searchPerformed: false,
      departureDate: '',
      passengersCount: 1,
      selectedPurchase: null,
      showPopup: false,
      popupType: '',
      popupTitle: '',
      popupMessage: '',
      popupActionText: '',
      popupAction: null
    };
  },

  computed: {
    paginatedFlights() {
      const start = (this.currentPage - 1) * this.flightsPerPage;
      return this.flights.slice(start, start + this.flightsPerPage);
    }
  },
  
  methods: {
    handleSearch(data) {
      this.flights = data.flights || [];
      this.departureDate = data.departureDate;
      this.passengersCount = Number(data.passengers || 1);
      this.searchPerformed = true;
      this.currentPage = 1;
    },

    changePage(page) {
      this.currentPage = page;
    },

    async handleBuyFlight(selection) {
      const seatClass = 
        selection.seatClass === "FirstClass" || selection.seatClass === "firstClass" 
          ? "FirstClass" 
          : "Turist";

      for (const segment of selection.flight.segments) {
        
        const response = await fetch("http://localhost:5276/api/payment/check-availability", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            numberFlight: segment.flightNumber,
            seatClass: seatClass,
            requestedSeats: this.passengersCount
          })
        });

        const result = await response.json();

        if (!result.isAvailable) {
          this.popupType = 'error'
          this.popupTitle = 'Sin disponibilidad'
          this.popupMessage = `No hay campos disponibles para el vuelo ${segment.flightNumber}.`
          this.popupActionText = ''
          this.showPopup = true
          return
        }
      }

      const purchaseSelection = {
        seatClass: seatClass,
        price: selection.price,
        passengerCount: this.passengersCount,
        itinerary: {
          ...selection.flight,
          segments: selection.flight.segments.map(segment => ({
            ...segment,
            departureDate: this.departureDate
          }))
        },
        selectedAt: new Date().toISOString()
      };

      this.selectedPurchase = purchaseSelection;
      sessionStorage.setItem(
        'selectedFlightPurchase',
        JSON.stringify(purchaseSelection)
      );
      sessionStorage.removeItem('purchasePassengers');
      sessionStorage.removeItem('purchaseLuggage');

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
