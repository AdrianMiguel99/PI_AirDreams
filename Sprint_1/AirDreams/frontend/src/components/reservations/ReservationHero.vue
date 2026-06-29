<template>
  <section class="reservation-hero">
    <!-- TODO: Mejorar calidad de imagen-->
    <img
      class="hero-background"
      src="https://i.ibb.co/sd8Gx1wK/banner-consultar.png"
      alt="Banner de consulta de reserva"
    >

    <div class="hero-info" :class="{ cancelled: reservation.isCancelled }">
      <span class="days-badge":class="{ cancelled: reservation.isCancelled }">
        {{ reservation.isCancelled ? "RESERVA CANCELADA" : `${reservation.daysLeft} días para el vuelo` }}
      </span>

      <h1>
        Trip to {{ reservation.destinationCity }}
        <span>{{ reservation.destinationCode }}</span>
      </h1>

      <div class="reservation-data">
        <span>Número de reserva:</span>
        <strong>{{ reservation.reservationCode }}</strong>

        <span>Fecha de salida:</span>
        <strong>{{ reservation.departureDate }}</strong>

        <span>Cantidad pasajeros:</span>
        <strong>{{ reservation.passengerCount }}</strong>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: "ReservationHero",

  props: {
    reservation: {
      type: Object,
      required: true
    }
  }
};
</script>

<style scoped>

.reservation-hero {
  position: relative;
  height: 240px;
  overflow: hidden;
}

.hero-background {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  display: block;
  border-radius: 10px 10px 0 0;
}

.hero-info {
  position: absolute;
  top: 42px;
  left: 28px;
  width: 330px;
  padding: 22px 28px;
  background: rgba(3, 32, 86, 0.78);
  border-radius: 12px;
  color: white;
}

.days-badge {
  display: inline-block;
  margin-bottom: 14px;
  padding: 8px 18px;
  background: white;
  color: #384467;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 700;
}

.days-badge.cancelled {
  color: #961414;
}

.hero-info h1 {
  margin: 0 0 16px;
  font-size: 23px;
  font-weight: 800;
}

.hero-info.cancelled {
  background: rgba(150, 20, 20, 0.85);
}

.hero-info h1 span {
  font-weight: 600;
}

.reservation-data {
  display: grid;
  grid-template-columns: 1fr 1fr;
  row-gap: 8px;
  column-gap: 12px;
  font-size: 13px;
}

.reservation-data span {
  color: #f0f4ff;
}

.reservation-data strong {
  color: white;
}

@media (max-width: 700px) {
  .reservation-hero {
    height: 300px;
  }

  .hero-info {
    left: 18px;
    right: 18px;
    width: auto;
  }
}
</style>
