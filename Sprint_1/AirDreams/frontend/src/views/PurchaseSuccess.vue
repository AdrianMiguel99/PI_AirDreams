<template>
  <div class="purchase-success-page">
    <div class="top-bar">
      <div class="brand">
    <img
      src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png"
      alt="AirDreams"
      class="logo-image"
    >
    <span>AirDreams</span>
  </div>
      <button
        class="btn btn-primary btn"
        @click="goHome"
      >
        Volver al inicio
      </button> 
</div>

    <div class="success-hero">
      <div class="success-logo">
        <img src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png" alt="AirDreams" class="logo-image" />
  <span>AirDreams</span>
</div>

      <h1>¡Compra realizada con éxito!</h1>

      <p class="success-message">
        En Air Dreams creemos que cada viaje comienza con un sueño.
        Hoy estás un paso más cerca de vivir el tuyo.
      </p>

      <p class="purchase-id">
        Código de compra: <strong>#{{ idCompra }}</strong>
      </p>
    </div>

    <div class="summary-card">
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
    </div>

    <div class="tickets-section mb-5">
      <h2>Tiquetes comprados</h2>

      <div class="d-flex flex-column gap-3">
        <article
          v-for="ticket in tickets"
          :key="ticket.id"
          class="ticket-card"
        >
          <div class="d-flex justify-content-between align-items-center">
            <h3>{{ ticket.passengerName }}</h3>
            <span class="seat-badge"> {{ ticket.seatNumber }} </span>
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
            <p><strong>Fecha vuelo 1:</strong> {{ purchase.firstDepartureDate }}</p>
            <p><strong>Salida vuelo 1:</strong> {{ purchase.firstDepartureTime }}</p>
            <p><strong>Llegada vuelo 1:</strong> {{ purchase.firstArrivalTime }}</p>

            <p><strong>Vuelo 2:</strong> {{ purchase.secondFlightCode }}</p>
            <p><strong>Fecha vuelo 2:</strong> {{ purchase.secondDepartureDate }}</p>
            <p><strong>Salida vuelo 2:</strong> {{ purchase.secondDepartureTime }}</p>
            <p><strong>Llegada vuelo 2:</strong> {{ purchase.secondArrivalTime }}</p>

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
            <p><strong>Salida:</strong> {{ purchase.departureTime }}</p>
            <p><strong>Llegada:</strong> {{ purchase.arrivalTime }}</p>
            <p><strong>Clase:</strong> {{ ticket.classType }}</p>
            <p><strong>Asiento:</strong> {{ ticket.seatNumber }}</p>
            <p><strong>Precio:</strong> ${{ ticket.price }}</p>
          </div>
        </article>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "PurchaseSuccess",

  data() {
    return {
      idCompra: "",
      purchase: {
        purchaseType: "direct",
        flightCode: "",
        firstFlightCode: "",
        secondFlightCode: "",
        origin: "",
        layover: "",
        destination: "",
        departureDate: "",
        departureTime: "",
        firstDepartureTime: "",
        secondDepartureTime: "",
        total: 0
      },
      tickets: []
    };
  },

  created() {
    this.loadPurchaseSuccessData();
  },

  methods: {

    formatDate(value) {
      const date = new Date(value);
      const day = String(date.getDate()).padStart(2, '0');
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const year = date.getFullYear();

      return `${day}/${month}/${year}`;
    },

    loadPurchaseSuccessData() {
      const successData = JSON.parse(
        sessionStorage.getItem("purchaseSuccessData") || "{}"
      );

      const purchase = successData.purchase || {};
      const passengers = successData.passengers || [];
      const segments = purchase.itinerary?.segments || [];

      const isLayover = segments.length > 1;
      const firstSegment = segments[0] || {};
      const secondSegment = segments[1] || {};

      this.idCompra =
        successData.transactionId ||
        this.$route.params.idCompra ||
        "SIN-CODIGO";

      this.purchase = {
        purchaseType: isLayover ? "layover" : "direct",

        flightCode: isLayover
          ? `${firstSegment.flightNumber || "N/A"} / ${secondSegment.flightNumber || "N/A"}`
          : firstSegment.flightNumber || purchase.flightNumber || "N/A",

        firstFlightCode: firstSegment.flightNumber || "N/A",
        secondFlightCode: secondSegment.flightNumber || "N/A",

        origin:
          firstSegment.departureAirport?.code || "Origen",

        layover:
          isLayover ? secondSegment.departureAirport?.code || "" : "",

        destination:
          isLayover
            ? secondSegment.arrivalAirport?.code || "Destino"
            : firstSegment.arrivalAirport?.code || "Destino",

        departureDate: this.formatDate(
          purchase.departureDate ||
          firstSegment.departureDate ||
          secondSegment.departureDate ||
          "Fecha no disponible"
        ),

        departureTime:
          firstSegment.departureTime ||
          purchase.departureTime ||
          "Hora no disponible",

        firstDepartureDate: this.formatDate(
          firstSegment.departureDate ||
          "Fecha no disponible"
        ), 

        firstArrivalDate: this.formatDate(
          firstSegment.arrivalDate ||
          "Fecha no disponible"
        ),

        firstArrivalTime:
          firstSegment.arrivalTime ||
          "Hora no disponible",


        firstDepartureTime:
          firstSegment.departureTime ||
          "Hora no disponible",

        secondDepartureDate: this.formatDate(
          secondSegment.departureDate ||
          "Fecha no disponible"
        ),

        secondArrivalDate: this.formatDate(
          secondSegment.arrivalDate ||
          "Fecha no disponible"
        ),

        secondArrivalTime:
          secondSegment.arrivalTime ||
          "Hora no disponible",
          
        secondDepartureTime:
          secondSegment.departureTime ||
          "Hora no disponible",

          arrivalDate: this.formatDate( firstSegment.arrivalDate || "Fecha no disponible" ),
          arrivalTime: firstSegment.arrivalTime || "Hora no disponible",
        total: Number(successData.grandTotal || 0)
      };

      this.tickets = passengers.map((passenger, index) => {
        return {
          id: index + 1,
          passengerName: `${passenger.namePassenger || ""} ${passenger.lastnamesPassenger || ""}`.trim(),
          seatNumber: this.generateRandomSeat(index),
          classType: successData.seatClassLabel || "Turista",
          price: Number(successData.pricePerPassenger || 0)
        };
      });
    },

    generateRandomSeat(index) {
      const letters = ["A", "B", "C", "D", "E", "F"];
      const row = (index % 30) + 1;
      const letter = letters[index % letters.length];

      return `${row}${letter}`;
    },

    goHome() {
      this.$router.push({ name: "home" });
    }
  }
};
</script>
<style scoped>
.purchase-success-page {
  min-height: 100vh;
  padding: 40px;
  background: #f7f8fc;
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
  color: #032056;
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
  color: #032056;
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

.ticket-card {
  border: 1px solid #dde6ee;
  border-radius: 18px;
  padding: 22px;
  background: #ffffff;
}

.ticket-route {
  display: flex;
  gap: 15px;
  font-size: 24px;
  margin: 20px 0 10px;
  color: #032056;
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
  background: #e8edf7;
  color: #032056;
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

.btn-primary {
  background-color: #032056;
  border-color: #032056;
}

.btn-primary:hover {
  background-color: #384467;
  border-color: #384467;
}

.seat-badge {
  background: #032056;
  color: white;
  padding: 8px 14px;
  border-radius: 999px;
  font-weight: bold;
  min-width: 55px;
  text-align: center;
}

.tickets-section h2 {
  margin-bottom: 40px;
}

.top-bar {
  position: absolute;
  top: 20px;
  left: 20px;
  right: 20px;

  display: flex;
  justify-content: space-between;
  align-items: center;

  z-index: 100;
}

.brand,
.success-logo {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #032056;
  font-weight: 700;
  font-size: 20px;
}

.logo-image {
  width: 50px;
  height: 50px;
  object-fit: contain;
}

.success-logo {
  justify-content: center;
  margin-bottom: 15px;
}
</style>