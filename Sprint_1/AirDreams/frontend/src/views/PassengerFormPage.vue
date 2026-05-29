<template>
  <StepperLayout :currentStep="1">
    <div class="page-header">
      <h2 class="section-title">Datos de pasajeros</h2>
      <p class="section-subtitle">
        Completa la informacion de cada pasajero para continuar con el equipaje.
      </p>
    </div>

    <div v-if="selectedPurchase" class="flight-summary">
      <div>
        <span class="summary-label">Vuelo seleccionado</span>
        <strong>#{{ selectedPurchase.itinerary?.itineraryId }}</strong>
      </div>

      <div>
        <span class="summary-label">Clase</span>
        <strong>{{ seatClassLabel }}</strong>
      </div>

      <div>
        <span class="summary-label">Precio por pasajero</span>
        <strong>{{ formatCurrency(selectedPurchase.price) }}</strong>
      </div>
    </div>

    <form @submit.prevent="continueToLuggage">
      <div
        v-for="(passenger, index) in passengers"
        :key="passenger.index"
        class="passenger-form"
      >
        <div class="passenger-title">
          <span>Pasajero {{ index + 1 }}</span>
        </div>

        <div class="form-grid">
          <label class="form-field">
            Identificacion
            <input
              v-model.trim="passenger.idPassenger"
              type="number"
              min="1"
              required
            />
          </label>

          <label class="form-field">
            Pasaporte
            <input
              v-model.trim="passenger.passport"
              type="text"
              maxlength="8"
              required
            />
          </label>

          <label class="form-field">
            Nombre
            <input
              v-model.trim="passenger.namePassenger"
              type="text"
              maxlength="30"
              required
            />
          </label>

          <label class="form-field">
            Apellidos
            <input
              v-model.trim="passenger.lastnamesPassenger"
              type="text"
              maxlength="30"
              required
            />
          </label>

          <label class="form-field">
            Correo
            <input
              v-model.trim="passenger.emailPassenger"
              type="email"
              maxlength="50"
              required
            />
          </label>

          <label class="form-field">
            Codigo pais
            <input
              v-model.trim="passenger.countryCode"
              type="number"
              min="1"
              max="255"
            />
          </label>

          <label class="form-field">
            Telefono
            <input
              v-model.trim="passenger.telephone"
              type="number"
              min="1"
            />
          </label>
        </div>
      </div>

      <div class="footer-actions">
        <button type="button" class="btn-back" @click="$router.push({ name: 'home' })">
          ← Volver
        </button>

        <button type="submit" class="btn-continue">
          Continuar a equipaje
        </button>
      </div>
    </form>

    <PopupMessage
      :show="showPopup"
      :title="popupTitle"
      :message="popupMessage"
      :type="popupType"
      @close="showPopup = false"
    />
  </StepperLayout>
</template>

<script>
import StepperLayout from '../components/StepperLayout.vue';
import PopupMessage from '../components/PopupMessage.vue';

export default {
  name: 'PassengerFormPage',

  components: {
    StepperLayout,
    PopupMessage
  },

  data() {
    return {
      selectedPurchase: null,
      passengers: [],
      showPopup: false,
      popupType: 'error',
      popupTitle: 'No se pudo continuar',
      popupMessage: ''
    };
  },

  computed: {
    seatClassLabel() {
      if (!this.selectedPurchase) return '';
      return this.selectedPurchase.seatClass === 'FirstClass'
        ? 'Primera clase'
        : 'Turista';
    }
  },

  created() {
    this.loadPurchaseSelection();
    this.loadPassengers();
  },

  methods: {
    loadPurchaseSelection() {
      const savedPurchase = sessionStorage.getItem('selectedFlightPurchase');

      if (!savedPurchase) {
        this.popupMessage = 'Primero debes seleccionar un vuelo para comprar.';
        this.showPopup = true;
        return;
      }

      this.selectedPurchase = JSON.parse(savedPurchase);
    },

    loadPassengers() {
      const savedPassengers = sessionStorage.getItem('purchasePassengers');

      if (savedPassengers) {
        this.passengers = JSON.parse(savedPassengers);
        return;
      }

      const passengerCount = Number(this.selectedPurchase?.passengerCount || 1);

      this.passengers = Array.from({ length: passengerCount }, (_, index) =>
        this.createEmptyPassenger(index + 1)
      );
    },

    createEmptyPassenger(index) {
      return {
        index,
        idPassenger: '',
        passport: '',
        namePassenger: '',
        lastnamesPassenger: '',
        emailPassenger: '',
        countryCode: '',
        telephone: ''
      };
    },

    normalizePassengers() {
      return this.passengers.map((passenger) => ({
        ...passenger,
        idPassenger: Number(passenger.idPassenger),
        passport: passenger.passport.trim().toUpperCase(),
        namePassenger: passenger.namePassenger.trim(),
        lastnamesPassenger: passenger.lastnamesPassenger.trim(),
        emailPassenger: passenger.emailPassenger.trim().toLowerCase(),
        countryCode: passenger.countryCode ? Number(passenger.countryCode) : null,
        telephone: passenger.telephone ? Number(passenger.telephone) : null,
        fullName: `${passenger.namePassenger.trim()} ${passenger.lastnamesPassenger.trim()}`
      }));
    },

    continueToLuggage() {
      if (!this.selectedPurchase) {
        this.popupMessage = 'Primero debes seleccionar un vuelo para comprar.';
        this.showPopup = true;
        return;
      }

      const normalizedPassengers = this.normalizePassengers();

      sessionStorage.setItem(
        'purchasePassengers',
        JSON.stringify(normalizedPassengers)
      );

      this.$router.push({ name: 'luggage' });
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(value || 0);
    }
  }
};
</script>

<style scoped>
.page-header {
  margin-bottom: 24px;
}

.section-title {
  font-size: 20px;
  font-weight: 700;
  color: #032056;
  margin: 0 0 6px;
}

.section-subtitle {
  color: #667085;
  font-size: 14px;
  margin: 0;
}

.flight-summary {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
  background: #ffffff;
  border: 1px solid #e8ecf4;
  border-radius: 12px;
  padding: 16px 20px;
  margin-bottom: 20px;
}

.flight-summary div {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.summary-label {
  color: #8a94a8;
  font-size: 12px;
  font-weight: 600;
}

.flight-summary strong {
  color: #032056;
  font-size: 15px;
}

.passenger-form {
  background: #ffffff;
  border: 1px solid #e8ecf4;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 16px;
  box-shadow: 0 1px 4px rgba(3, 32, 86, 0.06);
}

.passenger-title {
  color: #032056;
  font-size: 15px;
  font-weight: 700;
  margin-bottom: 16px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.form-field {
  color: #032056;
  display: flex;
  flex-direction: column;
  font-size: 13px;
  font-weight: 600;
  gap: 6px;
}

.form-field input {
  border: 1px solid #c8d0de;
  border-radius: 8px;
  color: #032056;
  font-size: 14px;
  padding: 11px 12px;
  outline: none;
  transition: border 0.2s ease, box-shadow 0.2s ease;
}

.form-field input:focus {
  border-color: #032056;
  box-shadow: 0 0 0 3px rgba(3, 32, 86, 0.1);
}

.footer-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 32px;
}

.btn-back {
  background: transparent;
  border: none;
  color: #032056;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  padding: 0;
  transition: opacity 0.15s ease;
}

.btn-back:hover {
  opacity: 0.7;
}

.btn-continue {
  background: #032056;
  color: #ffffff;
  border: none;
  border-radius: 10px;
  padding: 14px 36px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, transform 0.1s ease;
}

.btn-continue:hover {
  background: #0a3a7a;
  transform: translateY(-1px);
}

.btn-continue:active {
  transform: translateY(0);
}

@media (max-width: 720px) {
  .flight-summary,
  .form-grid {
    grid-template-columns: 1fr;
  }

  .footer-actions {
    align-items: stretch;
    flex-direction: column;
    gap: 18px;
  }
}
</style>
