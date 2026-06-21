<template>
  <article class="flight-ticket-card">
    <div class="ticket-top">
      <div class="ticket-title">
        <h2>
          Salida ·
          <span>{{ purchase.departureShortDate || purchase.departureDate }}</span>
        </h2>
      </div>

      <div class="class-info">
        <strong>{{ ticket.classType }}</strong>
        <span>Basic</span>
      </div>
    </div>

    <p class="route-text">
      {{ purchase.originName }} ({{ purchase.origin }})
      →
      {{ purchase.destinationName }} ({{ purchase.destination }})
    </p>

    <p class="duration-text">
      <span v-if="isLayover">
        1 escala · {{ purchase.totalDuration || "No disponible" }}
      </span>

      <span v-else>
        Directo · {{ purchase.totalDuration || "No disponible" }}
      </span>
    </p>

    <div class="timeline">
      <div class="time-block left">
        <strong>{{ purchase.departureTime }}</strong>
        <span>{{ purchase.origin }}</span>
        <small>Terminal {{ purchase.originTerminal || "M" }}</small>
      </div>

      <div class="timeline-line">
        <span class="point start"></span>

        <span
          class="segment-marker segment-one"
          :class="{ 'default-marker': !purchase.firstIsAirDreams }"
        >
          <img
            :src="firstSegmentIcon"
            alt="Aerolínea tramo 1"
          >
        </span>

        <div
          v-if="isLayover"
          class="layover-center"
        >
          <span class="middle-point"></span>

          <div class="layover-info">
            <strong>{{ purchase.layover }}</strong>
            <small>{{ purchase.layoverDuration || "4h 40m" }}</small>
          </div>
        </div>

        <span
          v-if="isLayover"
          class="segment-marker segment-two"
          :class="{ 'default-marker': !purchase.secondIsAirDreams }"
        >
          <img
            :src="secondSegmentIcon"
            alt="Aerolínea tramo 2"
          >
        </span>

        <span class="point end"></span>
      </div>

      <div class="time-block right">
        <small class="arrival-date">
          {{ purchase.arrivalShortDate || purchase.arrivalDate }}
        </small>

        <strong>{{ purchase.arrivalTime }}</strong>
        <span>{{ purchase.destination }}</span>
        <small>Terminal {{ purchase.destinationTerminal || "1" }}</small>
      </div>
    </div>

    <div class="flight-codes">
      <span v-if="isLayover">
        {{ purchase.firstFlightCode }} · {{ purchase.firstAircraft || "Boeing 737 MAX 8" }}
        |
        {{ purchase.secondFlightCode }} · {{ purchase.secondAircraft || "Airbus A330-300" }}
      </span>

      <span v-else>
        {{ purchase.flightCode }} · {{ purchase.aircraft || "Boeing 737 MAX 8" }}
      </span>
    </div>

    <div class="passenger-info">
      <div>
        <span>Pasajero</span>
        <strong>{{ ticket.passengerName }}</strong>
      </div>

      <div>
        <span>Asiento</span>
        <strong>{{ ticket.seatNumber }}</strong>
      </div>
    </div>
  </article>
</template>

<script>
export default {
  name: "ReservationTicketCard",

  props: {
    ticket: {
      type: Object,
      required: true
    },

    purchase: {
      type: Object,
      required: true
    }
  },

  computed: {
    isLayover() {
      return this.purchase.purchaseType === "layover";
    },

    airDreamsLogo() {
      return "https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png";
    },

    defaultAirlineIcon() {
      return "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24'%3E%3Ccircle cx='12' cy='12' r='6' fill='%23032056'/%3E%3C/svg%3E";
    },

    firstSegmentIcon() {
      return this.purchase.firstIsAirDreams
        ? this.airDreamsLogo
        : this.defaultAirlineIcon;
    },

    secondSegmentIcon() {
      return this.purchase.secondIsAirDreams
        ? this.airDreamsLogo
        : this.defaultAirlineIcon;
    }
  }
};
</script>

<style scoped>
.flight-ticket-card {
  position: relative;
  background: white;
  border: 1px solid #e3e8ef;
  border-radius: 16px;
  padding: 24px 28px 22px;
  box-shadow: 0 4px 12px rgba(3, 32, 86, 0.08);
  color: #032056;
  overflow: hidden;
}

.flight-ticket-card::after {
  content: "";
  position: absolute;
  top: 28px;
  right: 0;
  width: 8px;
  height: 48px;
  background: #032056;
  border-radius: 8px 0 0 8px;
}

.ticket-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 18px;
}

.ticket-title h2 {
  margin: 0;
  color: #032056;
  font-size: 24px;
  font-weight: 800;
}

.ticket-title h2 span {
  color: #032056;
  font-weight: 800;
}

.class-info {
  text-align: right;
  padding-right: 18px;
}

.class-info strong {
  display: block;
  color: #032056;
  font-size: 15px;
  font-weight: 800;
  text-transform: uppercase;
}

.class-info span {
  color: #384467;
  font-size: 13px;
  font-weight: 700;
}

.route-text {
  margin: 12px 0 34px;
  color: #032056;
  font-size: 15px;
  font-weight: 500;
}

.duration-text {
  margin: 0 0 14px;
  color: #032056;
  font-size: 16px;
  font-weight: 800;
}

.timeline {
  display: grid;
  grid-template-columns: 86px 1fr 96px;
  align-items: center;
  gap: 12px;
}

.time-block {
  display: flex;
  flex-direction: column;
  color: #032056;
}

.time-block strong {
  color: #032056;
  font-size: 28px;
  line-height: 1;
  font-weight: 900;
}

.time-block span {
  margin-top: 6px;
  color: #032056;
  font-size: 14px;
  font-weight: 700;
}

.time-block small {
  margin-top: 2px;
  color: #032056;
  font-size: 13px;
}

.time-block.right {
  text-align: right;
}

.arrival-date {
  margin-bottom: 4px;
  color: #032056;
  font-size: 14px !important;
  font-weight: 600;
}

.timeline-line {
  position: relative;
  height: 2px;
  background: #032056;
}

.point {
  position: absolute;
  top: -3px;
  width: 8px;
  height: 8px;
  background: #032056;
  border-radius: 50%;
}

.point.start {
  left: 0;
}

.point.end {
  right: 0;
}

.segment-marker {
  position: absolute;
  top: -15px;
  transform: translateX(-50%);
  width: 30px;
  height: 30px;
  background: white;
  border: 2px solid #032056;
  border-radius: 50%;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  z-index: 2;
}

.segment-marker img {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

.segment-marker.default-marker {
  border: none;
  background: transparent;
}

.segment-marker.default-marker img {
  width: 18px;
  height: 18px;
}

.segment-one {
  left: 25%;
}

.segment-two {
  left: 75%;
}

.layover-center {
  position: absolute;
  left: 50%;
  top: -3px;
  transform: translateX(-50%);
  text-align: center;
}

.middle-point {
  display: block;
  width: 8px;
  height: 8px;
  background: #032056;
  border-radius: 50%;
  margin: 0 auto;
}

.layover-info {
  margin-top: 14px;
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #384467;
}

.layover-info strong {
  color: #384467;
  font-size: 15px;
  font-weight: 700;
}

.layover-info small {
  color: #384467;
  font-size: 13px;
}

.flight-codes {
  margin-top: 46px;
  color: #032056;
  font-size: 14px;
  font-weight: 700;
}

.passenger-info {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
  margin-top: 18px;
  padding-top: 16px;
  border-top: 1px solid #e3e8ef;
}

.passenger-info div {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.passenger-info span {
  color: #6b7280;
  font-size: 13px;
}

.passenger-info strong {
  color: #032056;
  font-size: 15px;
}

@media (max-width: 700px) {
  .ticket-top {
    flex-direction: column;
  }

  .timeline {
    grid-template-columns: 1fr;
    gap: 24px;
  }

  .timeline-line {
    height: 120px;
    width: 2px;
    margin: 0 auto;
  }

  .point.start {
    top: 0;
    left: -3px;
  }

  .point.end {
    top: auto;
    bottom: 0;
    right: auto;
    left: -3px;
  }

  .segment-one {
    left: 50%;
    top: 25%;
  }

  .segment-two {
    left: 50%;
    top: 75%;
  }

  .layover-center {
    top: 50%;
    left: 50%;
  }

  .time-block.right {
    text-align: left;
  }

  .passenger-info {
    grid-template-columns: 1fr;
  }
}
</style>