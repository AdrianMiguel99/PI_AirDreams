<template>
  <StepperLayout :currentStep="2">
    <h2 class="section-title">Equipaje por pasajero</h2>

    <LuggagePassenger
      v-for="passenger in passengers"
      :key="passenger.index"
      :passengerIndex="passenger.index"
      :fullName="passenger.fullName"
      :passengerTotal="getPassengerTotal(passenger.index)"
      :checkedPrice="baseCheckedPrice"
      :carryOnPrice="baseCarryOnPrice"
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
import axios from 'axios';
import StepperLayout from '../components/StepperLayout.vue';
import LuggagePassenger from '../components/LuggagePassenger.vue';
import PopupMessage from '../components/PopupMessage.vue';

export default {
  name: 'LuggagePage',
  components: { StepperLayout, LuggagePassenger, PopupMessage },
  data() {
    return {
      segmentsPricing: [],
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
      let txId = this.$route.query.transactionId ||
                 sessionStorage.getItem('transactionId');
      if (!txId || txId === 'TXN-DEFAULT') {
        txId = 'TXN-' + crypto.randomUUID().substring(0, 8);
        sessionStorage.setItem('transactionId', txId);
      }
      return txId;
    },
    baseCheckedPrice() {
      return this.segmentsPricing[0]?.checkedPrice || 0;
    },
    baseCarryOnPrice() {
      return this.segmentsPricing[0]?.carryOnPrice || 0;
    }
  },
  created() {
    this.loadSegmentsPricing();
    this.loadPassengers();
  },
  methods: {
    loadSegmentsPricing() {
      const savedPurchase = sessionStorage.getItem('selectedFlightPurchase');
      if (!savedPurchase) return;
      const purchase = JSON.parse(savedPurchase);
      const segments = purchase.itinerary?.segments || [];
      this.segmentsPricing = segments.map(seg => ({
        checkedPrice: Number(seg.checkedPrice) || 0,
        carryOnPrice: Number(seg.carryOnPrice) || 0,
        multiplier: Number(seg.porcentageMultiplier) || 0.2
      }));
    },
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
        const res = await axios.post('http://localhost:5276/api/luggage/calculate/total', {
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
        const luggageByPassenger = this.passengers.map((passenger) => {
          const sel = this.luggageSelections[passenger.index] || { passengerTotal: 0 };
          return {
            passenger,
            luggageItems: this.buildLuggageItems(passenger.index),
            passengerTotal: sel.passengerTotal   // total calculado por el backend
          };
        });
        sessionStorage.setItem('purchaseLuggage', JSON.stringify(luggageByPassenger));
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
.btn-back:hover { opacity: 0.7; }
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
.btn-continue:active { transform: translateY(0); }
</style>