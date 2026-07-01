<template>
  <div class="reservation-details-page">
    <HeaderLogoNoAdmin />

    <main class="reservation-wrapper">
      <ReservationHero :reservation="reservation" />

      <section class="details-card">
        <p v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </p>

        <p v-if="cancellationMessage" class="success-message">
          {{ cancellationMessage }}
        </p>

        <div class="details-layout">
          <section class="tickets-content">
            <h1>Detalles del vuelo</h1>

            <div class="tickets-list">
              <ReservationTicketCard
                v-for="ticket in reservation.tickets"
                :key="ticket.id"
                :ticket="ticket"
                :purchase="reservation.purchase"
              />
            </div>
          </section>

          <ReservationOptionsPanel
            :loading="cancellationLoading"
            :disabled="reservation.isCancelled"
            @cancel-reservation="requestCancellation"
            @add-luggage="goToExtraLuggage"
          />
        </div>
      </section>
    </main>
  </div>
</template>

<script>
import HeaderLogoNoAdmin from "../components/HeaderLogoNoAdmin.vue";
import ReservationHero from "../components/reservations/ReservationHero.vue";
import ReservationTicketCard from "../components/reservations/ReservationTicketCard.vue";
import ReservationOptionsPanel from "../components/reservations/ReservationOptionsPanel.vue";

const API_URL = import.meta.env.VITE_API_URL;

export default {
  name: "ReservationDetailsPage",

  components: {
    HeaderLogoNoAdmin,
    ReservationHero,
    ReservationTicketCard,
    ReservationOptionsPanel
  },

  data() {
    return {
      loading: false,
      errorMessage: "",
      cancellationLoading: false,
      cancellationMessage: "",

      reservation: {
        reservationCode: "",
        destinationCity: "",
        destinationCode: "",
        daysLeft: 0,
        departureDate: "",
        passengerCount: 0,
        status: "Confirmada",
        isCancelled: false,

        purchase: {
          purchaseType: "direct",
          firstIsAirDreams: true,
          secondIsAirDreams: false,

          flightCode: "",
          firstFlightCode: "",
          secondFlightCode: "",

          origin: "",
          layover: "",
          destination: "",

          originName: "",
          layoverName: "",
          destinationName: "",

          departureDate: "",
          departureShortDate: "",

          arrivalDate: "",
          arrivalShortDate: "",

          firstDepartureDate: "",
          secondDepartureDate: "",

          departureTime: "",
          arrivalTime: "",

          firstDepartureTime: "",
          firstArrivalTime: "",

          secondDepartureTime: "",
          secondArrivalTime: "",

          totalDuration: "",
          layoverDuration: "",

          originTerminal: "",
          destinationTerminal: "",

          firstAircraft: "",
          secondAircraft: "",

          firstAirlineName: "",
          secondAirlineName: ""
        },

        tickets: []
      }
    };
  },

  async mounted() {
    await this.loadReservationDetails();
  },

  methods: {
    async loadReservationDetails() {
      try {
        this.loading = true;
        this.errorMessage = "";

        const reservationCode = this.$route.query.reservationCode;

        if (!reservationCode) {
          this.errorMessage = "No se recibió el código de reserva.";
          return;
        }

        const response = await fetch(
          `${API_URL}/Reservation/${encodeURIComponent(reservationCode)}`
        );

        if (!response.ok) {
          throw new Error("No se pudo obtener la reserva.");
        }

        const data = await response.json();

        this.reservation = this.mapReservationDetails(reservationCode, data);
      } catch (error) {
        console.error(error);
        this.errorMessage = "No se pudo cargar la información de la reserva.";
      } finally {
        this.loading = false;
      }
    },

    async requestCancellation() {
      this.errorMessage = "";
      this.cancellationMessage = "";

      const transactionId =
        this.reservation.reservationCode || this.$route.query.reservationCode;

      if (!transactionId) {
        this.errorMessage = "No se encontró el número de reserva para cancelar.";
        return;
      }

      this.cancellationLoading = true;

      try {
        const response = await fetch(`${API_URL}/cancellations/request`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            transactionId: transactionId
          })
        });

        const responseText = await response.text();

        if (!response.ok) {
          throw new Error(responseText || "No se pudo solicitar la cancelación.");
        }

        this.cancellationMessage =
          "Solicitud de cancelación enviada correctamente. Revise su correo para confirmar la cancelación.";
      } catch (error) {
        console.error(error);
        this.errorMessage =
          error.message || "Ocurrió un error al solicitar la cancelación.";
      } finally {
        this.cancellationLoading = false;
      }
    },

    goToExtraLuggage() {
      const reservationCode =
        this.reservation.reservationCode || this.$route.query.reservationCode;

      if (!reservationCode) {
        this.errorMessage = "No se encontró el código de reserva.";
        return;
      }

      this.$router.push({
        name: "ExtraLuggage",
        query: {
          reservationCode
        }
      });
    },

    mapReservationDetails(reservationCode, data) {
      const passengers = data.passengers || [];
      const flights = data.flights || [];

      const firstFlight = flights[0];
      const lastFlight = flights[flights.length - 1];

      const hasLayover = flights.length > 1;
      const itineraryStatus = firstFlight?.itineraryStatus || "Confirmada";
      const normalizedStatus = itineraryStatus.trim().toLowerCase();

      return {
        reservationCode: reservationCode,

        destinationCity: lastFlight?.destinationCity || "",
        destinationCode: lastFlight?.destinationCode || "",

        daysLeft: this.calculateDaysLeft(firstFlight?.departureDate),
        departureDate: this.formatDate(firstFlight?.departureDate),
        passengerCount: passengers.length,

        status: itineraryStatus,
        isCancelled: normalizedStatus === "cancelled",

        purchase: {
          purchaseType: hasLayover ? "layover" : "direct",

          firstIsAirDreams: flights[0]?.isAirDreams !== false,
          secondIsAirDreams: flights[1]?.isAirDreams !== false,

          flightCode: flights.map(flight => flight.flightNumber).join(" / "),
          firstFlightCode: flights[0]?.flightNumber || "",
          secondFlightCode: flights[1]?.flightNumber || "",

          origin: firstFlight?.originCode || "",
          layover: hasLayover ? flights[0]?.destinationCode : "",
          destination: lastFlight?.destinationCode || "",

          originName: firstFlight
            ? `${firstFlight.originCity}, ${firstFlight.originCountry}`
            : "",

          layoverName: hasLayover
            ? `${flights[0].destinationCity}, ${flights[0].destinationCountry}`
            : "",

          destinationName: lastFlight
            ? `${lastFlight.destinationCity}, ${lastFlight.destinationCountry}`
            : "",

          departureDate: this.formatDate(firstFlight?.departureDate),
          departureShortDate: this.formatShortDate(firstFlight?.departureDate),

          arrivalDate: "",
          arrivalShortDate: "",

          firstDepartureDate: this.formatDate(flights[0]?.departureDate),
          secondDepartureDate: this.formatDate(flights[1]?.departureDate),

          departureTime: "",
          arrivalTime: "",

          firstDepartureTime: "",
          firstArrivalTime: "",

          secondDepartureTime: "",
          secondArrivalTime: "",

          totalDuration: flights
            .map(flight => this.formatDuration(flight.duration))
            .join(" + "),
          layoverDuration: "",

          originTerminal: "",
          destinationTerminal: "",

          firstAircraft: flights[0]?.aircraftModel || "",
          secondAircraft: flights[1]?.aircraftModel || "",

          firstAirlineName: flights[0]?.airlineName || "",
          secondAirlineName: flights[1]?.airlineName || ""
        },

        tickets: [
          {
            id: reservationCode,
            passengers: passengers.map(passenger => ({
              id: passenger.idPassenger,
              name: passenger.passengerName,
              seatNumber: passenger.seatNumber || ""
            })),
            classType: firstFlight?.seatClass || "Turista"
          }
        ]
      };
    },

    formatDate(dateValue) {
      if (!dateValue) return "";

      const date = new Date(dateValue);

      return date.toLocaleDateString("es-CR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
      });
    },

    formatShortDate(dateValue) {
      if (!dateValue) return "";

      const date = new Date(dateValue);

      return date.toLocaleDateString("es-CR", {
        month: "short",
        day: "2-digit"
      });
    },
    formatDuration(value) {
      if (!value) return "No disponible";

      const parts = String(value).split(":");
      const hours = Number(parts[0] || 0);
      const minutes = Number(parts[1] || 0);

      if (hours && minutes) return `${hours}h ${minutes}m`;
      if (hours) return `${hours}h`;
      if (minutes) return `${minutes}m`;

    return "No disponible";
    },

    calculateDaysLeft(departureDate) {
      if (!departureDate) return 0;

      const today = new Date();
      const departure = new Date(departureDate);

      const timeDiff = departure.getTime() - today.getTime();
      const daysLeft = Math.ceil(timeDiff / (1000 * 3600 * 24));

      return daysLeft >= 0 ? daysLeft : 0;
    }
  }
};
</script>

<style scoped>
.reservation-details-page {
  min-height: 100vh;
  color: #032056;
}

.top-header {
  padding: 8px 24px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #032056;
  font-weight: 700;
  font-size: 22px;
}

.home-button {
  border: none;
  border-radius: 999px;
  padding: 10px 22px;
  background: #032056;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.home-button:hover {
  background: #384467;
}

.reservation-wrapper {
  max-width: 1168px;
  margin: 18px auto 50px;
  padding: 0 16px;
}

.details-card {
  background: white;
  border-radius: 0 0 10px 10px;
  padding: 38px 28px 48px;
  box-shadow: 0 14px 38px rgba(0, 0, 0, 0.16);
}

.details-layout {
  display: grid;
  grid-template-columns: 1fr 275px;
  gap: 26px;
}

.tickets-content h1 {
  font-size: 42px;
  font-weight: 800;
  color: #384467;
  margin: 0 0 34px;
}

.tickets-list {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.error-message {
  margin: 0 0 18px;
  padding: 12px 16px;
  border-radius: 10px;
  background: #fde8e8;
  color: #b00020;
  font-weight: 700;
}

.success-message {
  margin: 0 0 18px;
  padding: 12px 16px;
  border-radius: 10px;
  background: #e8f7ee;
  color: #1f7a3f;
  font-weight: 700;
}

@media (max-width: 900px) {
  .details-layout {
    grid-template-columns: 1fr;
  }

  .tickets-content h1 {
    font-size: 34px;
  }
}
</style>