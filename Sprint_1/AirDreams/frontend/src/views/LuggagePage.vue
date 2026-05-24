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
      <button class="btn-back" @click="$router.push({ name: 'home' })">← Volver</button>
      <button class="btn-continue" @click="$router.push({ name: 'home' })">Continuar al pago</button>
    </div>
  </StepperLayout>
</template>

<script>
import StepperLayout from '../components/StepperLayout.vue';
import LuggagePassenger from '../components/LuggagePassenger.vue';

export default {
  name: 'LuggagePage',

  components: {
    StepperLayout,
    LuggagePassenger
  },

  data() {
    return {
      checkedBagPrice: '25.00',
      carryOnBagPrice: '15.00',

      passengers: [
        { index: 1, fullName: 'Nombre Apellidos' },
        { index: 2, fullName: 'Nombre Apellidos' },
        { index: 3, fullName: 'Nombre Apellidos' },
        { index: 4, fullName: 'Nombre Apellidos' }
      ],

      luggageSelections: {}
    };
  },

  methods: {
    handleLuggageChange({ passengerIndex, checkedCount, carryOnCount }) {
      this.luggageSelections[passengerIndex] = { checkedCount, carryOnCount };
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