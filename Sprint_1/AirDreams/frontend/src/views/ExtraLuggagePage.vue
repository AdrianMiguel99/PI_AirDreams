<template>
  <main class="add-luggage-page">
    <h2 class="section-title">Agregar maletas</h2>

    <LuggagePassenger
      v-for="passenger in passengers"
      :key="passenger.idPassenger"
      :passengerIndex="passenger.idPassenger"
      :fullName="passenger.fullName"
      :passengerTotal="getPassengerTotal(passenger.idPassenger)"
      :checkedPrice="baseCheckedPrice"
      :carryOnPrice="baseCarryOnPrice"
      @luggage-change="handleLuggageChange"
    />

    <div class="footer-actions">
      <button class="btn-back" @click="goBack">
        ← Volver
      </button>

      <button
        class="btn-continue"
        :disabled="loading"
        @click="continueToPay"
      >
        Continuar al pago
      </button>
    </div>

    <PopupMessage
      :show="showPopup"
      :title="popupTitle"
      :message="popupMessage"
      :type="popupType"
      @close="showPopup = false"
    />
  </main>
</template>

<script>
import axios from 'axios';
import LuggagePassenger from '../components/LuggagePassenger.vue';
import PopupMessage from '../components/PopupMessage.vue';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5276/api';

export default {
  name: 'AddLuggagePage',

  components: {
    LuggagePassenger,
    PopupMessage
  },

  data() {
    return {
      passengers: [],
      segments: [],
      segmentsPricing: [],
      luggageSelections: {},

      checkedBagWeight: 23,
      carryOnBagWeight: 10,

      loading: false,
      showPopup: false,
      popupMessage: '',
      popupTitle: 'Agregar maletas',
      popupType: 'success'
    };
  },

  computed: {
    reservationCode() {
      return String(this.$route.query.reservationCode || '').trim();
    },

    baseCheckedPrice() {
      return this.segmentsPricing[0]?.checkedPrice || 0;
    },

    baseCarryOnPrice() {
      return this.segmentsPricing[0]?.carryOnPrice || 0;
    },

    totalCheckedWeight() {
      return Object.values(this.luggageSelections).reduce((total, selection) => {
        return total + selection.checkedCount * this.checkedBagWeight;
      }, 0);
    },

    totalCarryOnWeight() {
      return Object.values(this.luggageSelections).reduce((total, selection) => {
        return total + selection.carryOnCount * this.carryOnBagWeight;
      }, 0);
    }
  },

  created() {
    this.loadReservation();
  },

  methods: {
    async loadReservation() {
      if (!this.reservationCode) {
        this.showError('No se recibió el código de reserva.');
        return;
      }

      try {
        const response = await axios.get(
          `${API_URL}/Reservation/${encodeURIComponent(this.reservationCode)}`
        );

        const reservation = response.data;

        this.passengers = reservation.passengers.map((passenger) => ({
          ...passenger,
          idPassenger: passenger.idPassenger,
          fullName: passenger.fullName ||
            `${passenger.namePassenger} ${passenger.lastnamesPassenger}`
        }));

        this.segments = reservation.segments || reservation.flights || [];

        this.segmentsPricing = this.segments.map((segment) => ({
          checkedPrice: Number(segment.checkedPrice) || 0,
          carryOnPrice: Number(segment.carryOnPrice) || 0,
          multiplier: Number(segment.porcentageMultiplier) || 0.5
        }));
      } catch (error) {
        console.error('Error al cargar la reserva:', error);
        this.showError('No se pudo cargar la reserva.');
      }
    },

    async handleLuggageChange({ passengerIndex, checkedCount, carryOnCount }) {
      if (checkedCount === 0 && carryOnCount === 0) {
        this.luggageSelections[passengerIndex] = {
          checkedCount: 0,
          carryOnCount: 0,
          passengerTotal: 0
        };
        return;
      }

      try {
        const res = await axios.post(`${API_URL}/luggage/calculate/total`, {
          checkedQuantity: checkedCount,
          carryOnQuantity: carryOnCount,
          segments: this.segmentsPricing
        });

        const total = (res.data.checkedTotal || 0) + (res.data.carryOnTotal || 0);

        this.luggageSelections[passengerIndex] = {
          checkedCount,
          carryOnCount,
          passengerTotal: total
        };
      } catch (e) {
        console.error('Error al calcular equipaje:', e);

        this.luggageSelections[passengerIndex] = {
          checkedCount,
          carryOnCount,
          passengerTotal: 0
        };
      }
    },

    getPassengerTotal(passengerIndex) {
      const sel = this.luggageSelections[passengerIndex];
      return sel ? sel.passengerTotal : 0;
    },

    buildLuggageItems(passengerIndex) {
      const selection = this.luggageSelections[passengerIndex];
      const items = [];

      if (!selection) return items;

      if (selection.checkedCount > 0) {
        items.push({
          type: 'checked',
          quantity: selection.checkedCount
        });
      }

      if (selection.carryOnCount > 0) {
        items.push({
          type: 'carryOn',
          quantity: selection.carryOnCount
        });
      }

      return items;
    },

    async continueToPay() {
      this.loading = true;

      try {
        const totalCheckedWeight = this.totalCheckedWeight;
        const totalCarryOnWeight = this.totalCarryOnWeight;

        if (totalCheckedWeight === 0 && totalCarryOnWeight === 0) {
          this.showError('Debes seleccionar al menos una maleta.');
          return;
        }

        for (const segment of this.segments) {
          const flightId = (segment.flightNumber || '').trim();
          const routeId = segment.routeId;

          const validation = await this.validateWeights(
            flightId,
            routeId,
            totalCheckedWeight,
            totalCarryOnWeight
          );

          if (!validation.success) {
            this.popupType = 'error';
            this.popupTitle = 'No hay espacio suficiente';
            this.popupMessage = `${flightId}: ${validation.message}`;
            this.showPopup = true;
            return;
          }
        }

        const luggageByPassenger = this.passengers.map((passenger) => {
          const sel = this.luggageSelections[passenger.idPassenger] || {
            passengerTotal: 0
          };

          return {
            idPassenger: passenger.idPassenger,
            passenger,
            luggageItems: this.buildLuggageItems(passenger.idPassenger),
            passengerTotal: sel.passengerTotal
          };
        });

        sessionStorage.setItem(
          'extraLuggage',
          JSON.stringify({
            reservationCode: this.reservationCode,
            passengers: luggageByPassenger,
            luggageWeight: totalCheckedWeight,
            carryOnWeight: totalCarryOnWeight
          })
        );

        this.$router.push({
          name: 'payment',
          query: {
            reservationCode: this.reservationCode,
            extraLuggage: true
          }
        });
      } catch (error) {
        this.showError(error.message || 'Error al validar equipaje.');
      } finally {
        this.loading = false;
      }
    },

    async validateWeights(flightId, routeId, checkedWeight, carryOnWeight) {
      try {
        const response = await fetch(`${API_URL}/luggage/availability`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            flightId,
            routeId,
            luggageWeight: checkedWeight || 0,
            carryOnWeight: carryOnWeight || 0
          })
        });

        const data = await response.json();

        if (!response.ok) {
          return {
            success: false,
            message: data.message || 'Error en la validación'
          };
        }

        return {
          success: data.success,
          message: data.message || 'Validación exitosa'
        };
      } catch (error) {
        return {
          success: false,
          message: 'Error de conexión: ' + error.message
        };
      }
    },

    showError(message) {
      this.popupType = 'error';
      this.popupTitle = 'Agregar maletas';
      this.popupMessage = message;
      this.showPopup = true;
    },

    goBack() {
      this.$router.push({
        name: 'ReservationDetails',
        query: {
          reservationCode: this.reservationCode
        }
      });
    }
  }
};
</script>

<style scoped>
.add-luggage-page {
  width: min(1000px, calc(100% - 32px));
  margin: 0 auto;
  padding: 32px 0 48px;
}

.section-title {
  font-size: 20px;
  font-weight: 700;
  color: #032056;
  margin: 0 0 24px;
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
}

.btn-continue:hover {
  background: #0a3a7a;
}

.btn-continue:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>