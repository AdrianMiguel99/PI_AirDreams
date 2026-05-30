<template>
  <div class="passenger-card">
    <div class="passenger-info">
      <span class="passenger-name">{{ passengerLabel }}</span>
      <span class="passenger-fullname">{{ fullName }}</span>
      <span class="passenger-total">Total: {{ formatCurrency(passengerTotal) }}</span>
    </div>

    <div class="luggage-options">
      <div class="luggage-item">
        <div class="luggage-details">
          <span class="luggage-type">Equipaje documentado</span>
          <span class="luggage-price">{{ formatCurrency(checkedPrice) }}</span>
          <span class="luggage-per">por pieza</span>
        </div>
        <div class="counter">
          <button class="counter-btn" @click="decrement('checked')" :disabled="checkedCount <= 0">−</button>
          <span class="counter-value">{{ checkedCount }}</span>
          <button class="counter-btn" @click="increment('checked')">+</button>
        </div>
      </div>

      <div class="luggage-item">
        <div class="luggage-details">
          <span class="luggage-type">Carry on</span>
          <span class="luggage-price">{{ formatCurrency(carryOnPrice) }}</span>
          <span class="luggage-per">por pieza</span>
        </div>
        <div class="counter">
          <button class="counter-btn" @click="decrement('carryOn')" :disabled="carryOnCount <= 0">−</button>
          <span class="counter-value">{{ carryOnCount }}</span>
          <button class="counter-btn" @click="increment('carryOn')">+</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'LuggagePassenger',

  props: {
    passengerIndex: {
      type: Number,
      required: true
    },
    fullName: {
      type: String,
      default: 'Nombre Apellidos'
    },
    checkedPrice: {
      type: Number,
      default: 0
    },
    carryOnPrice: {
      type: Number,
      default: 0
    }
  },

  data() {
    return {
      checkedCount: 0,
      carryOnCount: 0
    };
  },

  computed: {
    passengerLabel() {
      return `Pasajero ${this.passengerIndex}`;
    },

    passengerTotal() {
      return (this.checkedCount * this.checkedPrice) +
        (this.carryOnCount * this.carryOnPrice);
    }
  },

  methods: {
    increment(type) {
      if (type === 'checked') {
        this.checkedCount++;
      } else {
        this.carryOnCount++;
      }
      this.emitChange();
    },

    decrement(type) {
      if (type === 'checked' && this.checkedCount > 0) {
        this.checkedCount--;
      } else if (type === 'carryOn' && this.carryOnCount > 0) {
        this.carryOnCount--;
      }
      this.emitChange();
    },

    emitChange() {
      this.$emit('luggage-change', {
        passengerIndex: this.passengerIndex,
        checkedCount: this.checkedCount,
        carryOnCount: this.carryOnCount,
        passengerTotal: this.passengerTotal
      });
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(Number(value || 0));
    }
  }
};
</script>

<style scoped>
.passenger-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #ffffff;
  border: 1px solid #e8ecf4;
  border-radius: 12px;
  padding: 20px 28px;
  margin-bottom: 16px;
  box-shadow: 0 1px 4px rgba(3, 32, 86, 0.06);
  transition: box-shadow 0.2s ease;
}

.passenger-card:hover {
  box-shadow: 0 4px 16px rgba(3, 32, 86, 0.1);
}

.passenger-info {
  display: flex;
  flex-direction: column;
  min-width: 130px;
}

.passenger-name {
  font-weight: 700;
  font-size: 15px;
  color: #032056;
}

.passenger-fullname {
  font-size: 13px;
  color: #8a94a8;
  margin-top: 2px;
}

.passenger-total {
  color: #032056;
  font-size: 14px;
  font-weight: 700;
  margin-top: 8px;
}

.luggage-options {
  display: flex;
  gap: 24px;
  flex: 1;
  justify-content: flex-end;
}

.luggage-item {
  display: flex;
  align-items: center;
  gap: 20px;
  background: #f7f9fc;
  border: 1px solid #e8ecf4;
  border-radius: 10px;
  padding: 14px 20px;
  min-width: 220px;
}

.luggage-details {
  display: flex;
  flex-direction: column;
}

.luggage-type {
  font-size: 13px;
  font-weight: 600;
  color: #032056;
}

.luggage-price {
  font-size: 13px;
  font-weight: 700;
  color: #032056;
  margin-top: 2px;
}

.luggage-per {
  font-size: 11px;
  color: #8a94a8;
}

.counter {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-left: auto;
}

.counter-btn {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1.5px solid #032056;
  background: #ffffff;
  color: #032056;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  transition: background 0.15s ease, color 0.15s ease;
  padding: 0;
}

.counter-btn:hover:not(:disabled) {
  background: #032056;
  color: #ffffff;
}

.counter-btn:disabled {
  opacity: 0.35;
  cursor: not-allowed;
}

.counter-value {
  font-size: 15px;
  font-weight: 600;
  color: #032056;
  min-width: 16px;
  text-align: center;
}
</style>
