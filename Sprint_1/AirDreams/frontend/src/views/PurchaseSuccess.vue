<template>
  <main class="purchase-success-page">
    <section class="success-hero">
      <div class="success-icon">✈️</div>

      <h1>¡Compra realizada con éxito!</h1>

      <p class="success-message">
        En AIRDREAMS creemos que cada viaje comienza con un sueño.
        Hoy estás un paso más cerca de vivir el tuyo.
      </p>

      <p class="purchase-id">
        Código de compra: <strong>#{{ idCompra }}</strong>
      </p>
    </section>

    <section class="summary-card">
      <h2>Resumen de compra</h2>

      <div class="summary-grid">
        <div>
          <span>Vuelo</span>
          <strong>{{ purchase.flightCode }}</strong>
        </div>

        <div>
          <span>Ruta</span>
          <strong v-if="purchase.purchaseType === 'layover'">
            {{ purchase.origin }} → {{ purchase.layover }} → {{ purchase.destination }}
          </strong>

          <strong v-else>
            {{ purchase.origin }} → {{ purchase.destination }}
          </strong>
        </div>

        <div>
          <span>Fecha</span>
          <strong>{{ purchase.departureDate }}</strong>
        </div>

        <div>
          <span>Total</span>
          <strong>${{ purchase.total }}</strong>
        </div>
      </div>
    </section>

    <section class="tickets-section">
      <h2>Tiquetes comprados</h2>

      <div class="tickets-list">
        <article
          v-for="ticket in tickets"
          :key="ticket.id"
          class="ticket-card"
        >
          <div class="ticket-header">
            <h3>{{ ticket.passengerName }}</h3>
            <span>{{ ticket.seatNumber }}</span>
          </div>

          <div
            v-if="purchase.purchaseType === 'layover'"
            class="ticket-route"
          >
            <strong>{{ purchase.origin }}</strong>
            <span>→</span>
            <strong>{{ purchase.layover }}</strong>
            <span>→</span>
            <strong>{{ purchase.destination }}</strong>
          </div>

          <div
            v-else
            class="ticket-route"
          >
            <strong>{{ purchase.origin }}</strong>
            <span>→</span>
            <strong>{{ purchase.destination }}</strong>
          </div>

          <div
            v-if="purchase.purchaseType === 'layover'"
            class="layover-label"
          >
            Vuelo con escala en {{ purchase.layover }}
          </div>

          <div
            v-else
            class="direct-label"
          >
            Vuelo directo
          </div>

          <div
            v-if="purchase.purchaseType === 'layover'"
            class="ticket-info"
          >
            <p><strong>Vuelo 1:</strong> {{ purchase.firstFlightCode }}</p>
            <p><strong>Salida:</strong> {{ purchase.firstDepartureTime }}</p>
            <p><strong>Vuelo 2:</strong> {{ purchase.secondFlightCode }}</p>
            <p><strong>Conexión:</strong> {{ purchase.secondDepartureTime }}</p>
            <p><strong>Fecha:</strong> {{ purchase.departureDate }}</p>
            <p><strong>Clase:</strong> {{ ticket.classType }}</p>
            <p><strong>Asiento:</strong> {{ ticket.seatNumber }}</p>
            <p><strong>Precio:</strong> ${{ ticket.price }}</p>
          </div>

          <div
            v-else
            class="ticket-info"
          >
            <p><strong>Vuelo:</strong> {{ purchase.flightCode }}</p>
            <p><strong>Fecha:</strong> {{ purchase.departureDate }}</p>
            <p><strong>Hora:</strong> {{ purchase.departureTime }}</p>
            <p><strong>Clase:</strong> {{ ticket.classType }}</p>
            <p><strong>Asiento:</strong> {{ ticket.seatNumber }}</p>
            <p><strong>Precio:</strong> ${{ ticket.price }}</p>
          </div>
        </article>
      </div>
    </section>

    <div class="actions">
      <button @click="goHome">Volver al inicio</button>
    </div>
  </main>
</template>

<script>
export default {
  name: "PurchaseSuccess",

  data() {
    return {
      idCompra: this.$route.params.idCompra,

      // Cambia purchaseType a "direct" o "layover"
      purchase: {
        purchaseType: "layover",

        flightCode: "AD-245 / AD-310",
        firstFlightCode: "AD-245",
        secondFlightCode: "AD-310",

        origin: "SJO",
        layover: "PTY",
        destination: "MIA",

        departureDate: "15 Julio 2026",
        departureTime: "08:30 AM",
        firstDepartureTime: "08:30 AM",
        secondDepartureTime: "01:15 PM",

        total: 700
      },

      tickets: [
        {
          id: 1,
          passengerName: "Adrian Arrieta",
          seatNumber: "12A",
          classType: "Turista",
          price: 350
        },
        {
          id: 2,
          passengerName: "Melissa Garita",
          seatNumber: "12B",
          classType: "Turista",
          price: 350
        }
      ]
    };
  },

  methods: {
    goHome() {
      this.$router.push("/");
    }
  }
};
</script>

<style scoped>
.purchase-success-page {
  min-height: 100vh;
  padding: 40px;
  background: #f4f8fb;
}

.success-hero,
.summary-card,
.tickets-section {
  max-width: 900px;
  margin: 0 auto 30px;
  padding: 30px;
  background: white;
  border-radius: 24px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
}

.success-hero {
  text-align: center;
  padding: 40px;
}

.success-icon {
  font-size: 56px;
  margin-bottom: 10px;
}

.success-hero h1 {
  color: #0b7fc3;
  margin-bottom: 15px;
}

.success-message {
  font-size: 18px;
  color: #444;
  max-width: 650px;
  margin: 0 auto 20px;
}

.purchase-id {
  color: #666;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.summary-grid div {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.summary-grid span {
  color: #777;
  font-size: 14px;
}

.tickets-list {
  display: grid;
  gap: 20px;
}

.ticket-card {
  border: 1px solid #dde6ee;
  border-radius: 18px;
  padding: 22px;
  background: #ffffff;
}

.ticket-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.ticket-header span {
  background: #0b7fc3;
  color: white;
  padding: 8px 14px;
  border-radius: 999px;
  font-weight: bold;
}

.ticket-route {
  display: flex;
  gap: 15px;
  font-size: 24px;
  margin: 20px 0 10px;
  color: #0b7fc3;
}

.layover-label,
.direct-label {
  display: inline-block;
  margin-bottom: 18px;
  padding: 8px 14px;
  border-radius: 999px;
  font-weight: 600;
}

.layover-label {
  background: #eaf6fc;
  color: #0b7fc3;
}

.direct-label {
  background: #eef9f1;
  color: #16833a;
}

.ticket-info {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;
}

.actions {
  text-align: center;
}

.actions button {
  padding: 14px 28px;
  border: none;
  border-radius: 12px;
  background: #0b7fc3;
  color: white;
  font-size: 16px;
  cursor: pointer;
}

.actions button:hover {
  background: #096fa9;
}

@media (max-width: 768px) {
  .summary-grid,
  .ticket-info {
    grid-template-columns: 1fr;
  }

  .ticket-route {
    font-size: 20px;
    flex-wrap: wrap;
  }

  .purchase-success-page {
    padding: 20px;
  }
}
</style>