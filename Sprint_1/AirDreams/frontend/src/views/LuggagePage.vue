<template>
  <StepperLayout :currentStep="2">
    <h2 class="section-title">Equipaje por pasajero</h2>

    <LuggagePassenger
      v-for="passenger in passengers"
      :key="passenger.index"
      :passengerIndex="passenger.index"
      :fullName="passenger.fullName"
      :checkedPrice="checkedBagPrice"
      :carryOnPrice="carryOnBagPrice"
      @luggage-change="handleLuggageChange"
    />

    <div class="footer-actions">
      <button class="btn-back" @click="$router.push({ name: 'passengers' })">← Volver</button>
      <button class="btn-continue" @click="continueToPay" :disabled="loading">
        {{ loading ? 'Guardando equipaje...' : 'Continuar al pago' }}
      </button>
    </div>

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
import LuggagePassenger from '../components/LuggagePassenger.vue';
import PopupMessage from '../components/PopupMessage.vue';

export default {
  name: 'LuggagePage',

  components: {
    StepperLayout,
    LuggagePassenger,
    PopupMessage
  },

  data() {
    return {
      checkedBagPrice: '25.00',
      carryOnBagPrice: '15.00',

      passengers: [],

      luggageSelections: {},
      loading: false,
      showPopup: false,
      popupMessage: '',
      popupTitle: 'Equipaje',
      popupType: 'success'
    };
  },

  computed: {
    transactionIdItinerary() {
      return this.$route.query.transactionId || sessionStorage.getItem('transactionId') || 'TXN-DEFAULT';
    }
  },

  created() {
    this.loadPassengers();
  },

  methods: {
    loadPassengers() {
      const savedPassengers = sessionStorage.getItem('purchasePassengers');

      if (!savedPassengers) {
        this.popupType = 'error';
        this.popupTitle = 'No hay pasajeros';
        this.popupMessage = 'Primero debes registrar los pasajeros.';
        this.showPopup = true;
        return;
      }

      this.passengers = JSON.parse(savedPassengers).map((passenger, index) => ({
        ...passenger,
        index: passenger.index || index + 1,
        fullName: passenger.fullName ||
          `${passenger.namePassenger} ${passenger.lastnamesPassenger}`
      }));
    },

    handleLuggageChange({ passengerIndex, checkedCount, carryOnCount }) {
      this.luggageSelections[passengerIndex] = { checkedCount, carryOnCount };
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
        const luggageByPassenger = this.passengers.map((passenger) => ({
          passenger,
          luggageItems: this.buildLuggageItems(passenger.index)
        }));

        sessionStorage.setItem(
          'purchaseLuggage',
          JSON.stringify(luggageByPassenger)
        );

        this.popupType = 'success';
        this.popupTitle = 'Equipaje registrado';
        this.popupMessage = 'Equipaje guardado exitosamente. Continuando al pago...';
        this.showPopup = true;

        setTimeout(() => {
          this.$router.push({ 
            name: 'payment', 
            query: { transactionId: this.transactionIdItinerary } 
          });
        }, 2000);
      } catch (error) {
        this.popupType = 'error';
        this.popupTitle = 'Error';
        this.popupMessage = error.message || 'Error de conexión al registrar equipaje';
        this.showPopup = true;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
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
</style>
