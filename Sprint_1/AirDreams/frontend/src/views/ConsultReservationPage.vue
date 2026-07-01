<template>
  <div class="consult-reservation-page">
    <HeaderLogoNoAdmin />

    <main class="consult-container">
      <section class="consult-card">
        <div class="consult-title">
          <h1>Consultar reserva</h1>
          <p>Ingrese los datos de su reservación para ver los detalles del vuelo.</p>
        </div>

        <form class="form-card" @submit.prevent="goToReservationDetails">
          <h2>Datos de la reserva</h2>

          <div class="form-group">
            <label>Número de reserva</label>
            <input
              v-model="reservationCode"
              type="text"
              placeholder="Ej: TXN-d30ae4ba"
              autocomplete="off"
            >
          </div>

          <div class="form-group">
            <label>Apellidos</label>
            <input
              v-model="passengerName"
              type="text"
              placeholder="Ej: Obando Vásquez"
              autocomplete="off"
            >
          </div>

          <p v-if="errorMessage" class="error-message">
            {{ errorMessage }}
          </p>

          <div class="form-actions">
            <button class="home-button" type="button" @click="goHome">
              Inicio
            </button>

            <button class="search-button" type="submit">
              Buscar reserva
            </button>
          </div>
        </form>
      </section>
    </main>
  </div>
</template>

<script>
import HeaderLogoNoAdmin from "../components/HeaderLogoNoAdmin.vue";

export default {
  name: "ConsultReservationPage",

  components: {
    HeaderLogoNoAdmin
  },

  data() {
    return {
      reservationCode: "",
      passengerName: "",
      errorMessage: ""
    };
  },

  methods: {
    goHome() {
      this.$router.push("/");
    },

    goToReservationDetails() {
      this.errorMessage = "";

      const cleanReservationCode = this.reservationCode.trim();
      const cleanPassengerName = this.passengerName.trim();

      if (!cleanReservationCode || !cleanPassengerName) {
        this.errorMessage = "Debe ingresar el número de reserva y el nombre del pasajero.";
        return;
      }

      this.$router.push({
        name: "ReservationDetails",
        query: {
          reservationCode: cleanReservationCode,
          passengerName: cleanPassengerName
        }
      });
    }
  }
};
</script>

<style scoped>

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.home-button,
.search-button {
  border: none;
  border-radius: 50px;
  padding: 12px 30px;
  background: #384467;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.home-button:hover,
.search-button:hover {
  background: #4c5d8f;
}

.consult-reservation-page {
  min-height: 100vh;
}

.consult-container {
  min-height: calc(100vh - 70px);
  display: flex;
  justify-content: center;
  padding: 40px 20px;
}

.consult-card {
  width: 100%;
  max-width: 950px;
  background:
    linear-gradient(rgba(56, 68, 103, 0.85), rgba(56, 68, 103, 0.85)),
    url("https://images.unsplash.com/photo-1533105079780-92b9be482077?auto=format&fit=crop&w=1400&q=80");
  background-size: cover;
  background-position: center;
  border-radius: 22px;
  padding: 36px;
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.18);
}

.consult-title {
  color: white;
  margin-bottom: 24px;
}

.consult-title h1 {
  font-size: 34px;
  font-weight: 700;
  margin-bottom: 8px;
}

.consult-title p {
  margin: 0;
  color: #e8edf7;
}

.form-card {
  background: white;
  border-radius: 18px;
  padding: 28px;
}

.form-card h2 {
  color: #032056;
  font-size: 22px;
  margin-bottom: 22px;
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 18px;
}

.form-group label {
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 6px;
  color: #384467;
}

.form-group input {
  border: 1px solid #dde6ee;
  border-radius: 12px;
  padding: 12px 14px;
  font-size: 15px;
  color: #032056;
  outline: none;
  background: #f7f8fc;
}

.form-group input:focus {
  border-color: #032056;
}

.error-message {
  margin: 4px 0 18px;
  color: #b00020;
  font-size: 14px;
  font-weight: 600;
}

.search-button {
  display: block;
  margin-left: auto;
  border: none;
  border-radius: 50px;
  padding: 12px 30px;
  background: #384467;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.search-button:hover {
  background: #4c5d8f;
}
</style>