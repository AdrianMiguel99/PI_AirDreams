<template>
  <div class="cancel-reservation-page">
    <HeaderLogoNoAdmin />

    <main class="cancel-container">
      <section class="cancel-card">
        <div class="cancel-title">
          <h1>Cancelar reserva</h1>
          <p>Ingrese el número de reserva para cancelar su itinerario.</p>
        </div>

        <form class="form-card" @submit.prevent="cancelReservation">
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

          <p v-if="errorMessage" class="error-message">
            {{ errorMessage }}
          </p>

          <p v-if="successMessage" class="success-message">
            {{ successMessage }}
          </p>

          <button class="cancel-button" type="submit" :disabled="isLoading">
            {{ isLoading ? "Cancelando..." : "Cancelar reserva" }}
          </button>
        </form>
      </section>
    </main>

    <div v-if="showConfirmModal" class="modal-overlay">
      <div class="confirm-modal">
        <div class="sad-icon">😢</div>

        <h2>¿Está seguro?</h2>

        <p>
          Nos entristece verlo cancelar su reserva.
          Esta acción marcará su itinerario como cancelado.
        </p>

        <div class="modal-actions">
          <button class="keep-button" type="button" @click="closeConfirmModal">
            No, mantener reserva
          </button>

          <button
            class="confirm-cancel-button"
            type="button"
            :disabled="isLoading"
            @click="confirmCancellation"
          >
            {{ isLoading ? "Cancelando..." : "Sí, cancelar reserva" }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import HeaderLogoNoAdmin from "../components/HeaderLogoNoAdmin.vue";

export default {
  name: "CancelReservationPage",

  components: {
    HeaderLogoNoAdmin
  },

  data() {
    return {
      reservationCode: "",
      errorMessage: "",
      successMessage: "",
      isLoading: false,
      showConfirmModal: false
    };
  },

  methods: {
    cancelReservation() {
      this.errorMessage = "";
      this.successMessage = "";

      const cleanReservationCode = this.reservationCode.trim();

      if (!cleanReservationCode) {
        this.errorMessage = "Debe ingresar el número de reserva.";
        return;
      }

      this.showConfirmModal = true;
    },

    closeConfirmModal() {
      this.showConfirmModal = false;
    },

    async confirmCancellation() {
      this.errorMessage = "";
      this.successMessage = "";

      const cleanReservationCode = this.reservationCode.trim();

      try {
        this.isLoading = true;

        const response = await fetch("https://localhost:7136/api/cancellation/cancel", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            transactionId: cleanReservationCode
          })
        });

        const text = await response.text();
        const data = text ? JSON.parse(text) : {};

        if (!response.ok) {
          this.errorMessage = data.message || "No se pudo cancelar la reserva.";
          this.showConfirmModal = false;
          return;
        }

        this.successMessage = data.message || "Reserva cancelada correctamente.";
        this.reservationCode = "";
        this.showConfirmModal = false;
      } catch (error) {
        console.error("Error al cancelar reserva:", error);
        this.errorMessage = "Ocurrió un error al conectar con el servidor.";
        this.showConfirmModal = false;
      } finally {
        this.isLoading = false;
      }
    }
  }
};
</script>

<style scoped>
.cancel-reservation-page {
  min-height: 100vh;
}

.cancel-container {
  min-height: calc(100vh - 70px);
  display: flex;
  justify-content: center;
  padding: 40px 20px;
}

.cancel-card {
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

.cancel-title {
  color: white;
  margin-bottom: 24px;
}

.cancel-title h1 {
  font-size: 34px;
  font-weight: 700;
  margin-bottom: 8px;
}

.cancel-title p {
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

.success-message {
  margin: 4px 0 18px;
  color: #1f7a3f;
  font-size: 14px;
  font-weight: 600;
}

.cancel-button {
  display: block;
  margin-left: auto;
  border: none;
  border-radius: 50px;
  padding: 12px 30px;
  background: #b00020;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.cancel-button:hover {
  background: #d11f3f;
}

.cancel-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(3, 32, 86, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  z-index: 1000;
}

.confirm-modal {
  width: 100%;
  max-width: 430px;
  background: white;
  border-radius: 22px;
  padding: 30px;
  text-align: center;
  box-shadow: 0 18px 45px rgba(0, 0, 0, 0.25);
}

.sad-icon {
  font-size: 48px;
  margin-bottom: 12px;
}

.confirm-modal h2 {
  color: #032056;
  font-size: 26px;
  margin-bottom: 10px;
}

.confirm-modal p {
  color: #384467;
  font-size: 15px;
  line-height: 1.5;
  margin-bottom: 24px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
  flex-wrap: wrap;
}

.keep-button {
  border: none;
  border-radius: 50px;
  padding: 11px 22px;
  background: #dde6ee;
  color: #032056;
  font-weight: 700;
  cursor: pointer;
}

.keep-button:hover {
  background: #cfd9e3;
}

.confirm-cancel-button {
  border: none;
  border-radius: 50px;
  padding: 11px 22px;
  background: #b00020;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.confirm-cancel-button:hover {
  background: #d11f3f;
}

.confirm-cancel-button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style>