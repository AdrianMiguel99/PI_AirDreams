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
          <span>
            Pasajero {{ index + 1 }}
            <small v-if="isMainPassenger(index)">Principal</small>
          </span>
        </div>

        <div class="form-grid">
          <label class="form-field">
            Genero
            <select v-model="passenger.gender" required>
              <option value="" disabled>Selecciona una opcion</option>
              <option value="Female">Femenino</option>
              <option value="Male">Masculino</option>
              <option value="Other">Otro</option>
            </select>
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
            Fecha de nacimiento
            <input
              v-model="passenger.birthDate"
              type="date"
              required
            />
          </label>

          <label class="form-field">
            Pais del pasaporte
            <select
              v-model="passenger.country"
              required
            >
              <option value="" disabled>Selecciona un pais</option>
              <option
                v-for="country in countries"
                :key="country"
                :value="country"
              >
                {{ country }}
              </option>
            </select>
          </label>

          <template v-if="isMainPassenger(index)">
            <label class="form-field">
              Telefono
              <input
                v-model.trim="passenger.telephone"
                type="tel"
                maxlength="20"
                required
              />
            </label>

            <label class="form-field">
              Email
              <input
                v-model.trim="passenger.emailPassenger"
                type="email"
                maxlength="50"
                required
              />
            </label>
          </template>
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
import axios from 'axios'; 
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
      countries: [],
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
    this.loadCountries();
    this.loadPurchaseSelection();
    this.loadPassengers();
  },

  methods: {
    async loadCountries() {
      try {
        const response = await axios.get('http://localhost:5276/api/locations/countries');
        this.countries = response.data;
      } catch (error) {
        console.error('Error al cargar paises:', error);
        this.popupTitle = 'No se pudieron cargar los paises';
        this.popupMessage = 'Intenta de nuevo antes de continuar.';
        this.showPopup = true;
      }
    },

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
        this.passengers = JSON.parse(savedPassengers).map((passenger, index) => ({
          ...this.createEmptyPassenger(index + 1),
          ...passenger,
          country: passenger.country || ''
        }));
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
        isMainPassenger: index === 1,
        gender: '',
        namePassenger: '',
        lastnamesPassenger: '',
        birthDate: '',
        country: '',
        emailPassenger: '',
        telephone: ''
      };
    },

    normalizePassengers() {
      return this.passengers.map((passenger, index) => ({
        ...passenger,
        isMainPassenger: this.isMainPassenger(index),
        gender: passenger.gender,
        namePassenger: passenger.namePassenger.trim(),
        lastnamesPassenger: passenger.lastnamesPassenger.trim(),
        birthDate: passenger.birthDate,
        passportCountry: passenger.country.trim(),
        emailPassenger: this.isMainPassenger(index)
          ? passenger.emailPassenger.trim().toLowerCase()
          : null,
        telephone: this.isMainPassenger(index)
          ? passenger.telephone.trim()
          : null,
        fullName: `${passenger.namePassenger.trim()} ${passenger.lastnamesPassenger.trim()}`
      }));
    },

    isMainPassenger(index) {
      return index === 0;
    },

    formatCurrency(value) {  
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(value || 0);
    },

    async continueToLuggage() {
      if (!this.selectedPurchase) {
        this.popupMessage = 'Primero debes seleccionar un vuelo para comprar.';
        this.showPopup = true;
        return;
      }

      const segments = this.selectedPurchase.itinerary?.segments || [];
      const flightNumbers = segments.map(s => s.flightNumber?.trim()).filter(Boolean);

      const passengersToCheck = this.passengers.map(p => ({
        namePassenger: p.namePassenger.trim(),
        lastnamesPassenger: p.lastnamesPassenger.trim(),
        country: p.country.trim()
      }));

      try {
        const res = await axios.post('http://localhost:5276/api/passengers/validate-duplicate', {
          passengers: passengersToCheck,
          flightNumbers: flightNumbers
        });
        
        console.log(res)

        const duplicates = res.data.duplicates || [];
        if (duplicates.length > 0) {
          const nombres = duplicates.join(', ');
          this.popupTitle = 'Pasajero(s) ya registrado(s)';
          this.popupMessage = `Los siguientes pasajeros ya existen en este vuelo: ${nombres}.`;
          this.popupType = 'error';
          this.showPopup = true;
          return;
        }
      } catch (error) {
        console.error('Error al validar pasajeros:', error);
        this.popupTitle = 'Error de conexión';
        this.popupMessage = 'No se pudo verificar la duplicidad de pasajeros. Intenta de nuevo.';
        this.popupType = 'error';
        this.showPopup = true;
        return;
      }

      const normalizedPassengers = this.normalizePassengers();
      sessionStorage.setItem('purchasePassengers', JSON.stringify(normalizedPassengers));
      this.$router.push({ name: 'luggage' });
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

.passenger-title span {
  align-items: center;
  display: flex;
  gap: 8px;
}

.passenger-title small {
  background: #e8f2ff;
  border-radius: 999px;
  color: #032056;
  font-size: 11px;
  font-weight: 700;
  padding: 3px 9px;
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

.form-field input,
.form-field select {
  border: 1px solid #c8d0de;
  border-radius: 8px;
  background: #ffffff;
  color: #032056;
  font-size: 14px;
  padding: 11px 12px;
  outline: none;
  transition: border 0.2s ease, box-shadow 0.2s ease;
}

.form-field input:focus,
.form-field select:focus {
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
