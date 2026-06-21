<template>
  <div class="reservation-details-page">
    <HeaderLogoNoAdmin />

    <main class="reservation-wrapper">
      <ReservationHero :reservation="reservation" />

      <section class="details-card">
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

          <ReservationOptionsPanel />
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
      reservation: {
        reservationCode: this.$route.query.reservationCode || "8473635",
        destinationCity: "Madrid",
        destinationCode: "MAD",
        daysLeft: 93,
        departureDate: "03/05/2025",
        passengerCount: 2,
        status: "Confirmada",
        isCancelled: false,

        purchase: {
          purchaseType: "layover",
          firstIsAirDreams: true,
          secondIsAirDreams: false,

          flightCode: "AD-956 / AD-824",
          firstFlightCode: "AD-956",
          secondFlightCode: "AD-824",

          origin: "SJO",
          layover: "YYZ",
          destination: "MAD",

          originName: "San José, CR",
          layoverName: "Toronto, CA",
          destinationName: "Madrid, ES",

          departureDate: "03/05/2025",
          departureShortDate: "Mar 03",

          arrivalDate: "04/05/2025",
          arrivalShortDate: "Mar 04",

          firstDepartureDate: "03/05/2025",
          secondDepartureDate: "03/05/2025",

          departureTime: "07:10",
          arrivalTime: "08:50",

          firstDepartureTime: "07:10",
          firstArrivalTime: "14:50",

          secondDepartureTime: "19:30",
          secondArrivalTime: "08:50",

          totalDuration: "17h 40m",
          layoverDuration: "4h 40m",

          originTerminal: "M",
          destinationTerminal: "1",

          firstAircraft: "Boeing 737 MAX 8",
          secondAircraft: "Airbus A330-300"
        },

        tickets: [
          {
            id: 1,
            passengerName: this.$route.query.passengerName || "Obando Vásquez",
            seatNumber: "12A",
            classType: "Turista"
          },
          {
            id: 2,
            passengerName: "Adrián Arrieta",
            seatNumber: "12B",
            classType: "Turista"
          }
        ]
      }
    };
  },
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

@media (max-width: 900px) {
  .details-layout {
    grid-template-columns: 1fr;
  }

  .tickets-content h1 {
    font-size: 34px;
  }
}
</style>