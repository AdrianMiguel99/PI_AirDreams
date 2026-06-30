<template>
  <article class="passenger-luggage-card">
    <section class="passenger-info">
      <h3>Pasajero {{ passengerIndex }}</h3>
      <p>{{ fullName }}</p>

      <div class="price-info">
        <span>Precio anterior: {{ formatCurrency(previousTotal) }}</span>
        <span>Total nuevo: {{ formatCurrency(newTotal) }}</span>
        <strong>Total a pagar: {{ formatCurrency(extraTotal) }}</strong>
      </div>
    </section>

    <section class="luggage-option">
      <div class="luggage-text">
        <h4>Equipaje documentado</h4>
        <strong>{{ formatCurrency(checkedPrice) }}</strong>
        <small>por pieza</small>
        <p>+50% por cada pieza extra</p>
        <span class="current-text">Actual: {{ initialCheckedCount }}</span>
      </div>

      <div class="counter">
        <button
          type="button"
          :disabled="checkedLocal <= initialCheckedCount"
          @click="decreaseChecked"
        >
          −
        </button>

        <span>{{ checkedLocal }}</span>

        <button type="button" @click="increaseChecked">
          +
        </button>
      </div>
    </section>

    <section class="luggage-option">
      <div class="luggage-text">
        <h4>Carry on</h4>
        <strong>{{ formatCurrency(carryOnPrice) }}</strong>
        <small>por pieza</small>
        <p>+50% por cada pieza extra</p>
        <span class="current-text">Actual: {{ initialCarryOnCount }}</span>
      </div>

      <div class="counter">
        <button
          type="button"
          :disabled="carryOnLocal <= initialCarryOnCount"
          @click="decreaseCarryOn"
        >
          −
        </button>

        <span>{{ carryOnLocal }}</span>

        <button type="button" @click="increaseCarryOn">
          +
        </button>
      </div>
    </section>
  </article>
</template>

<script>
export default {
  name: 'ExtraLuggagePassenger',

  props: {
    passengerIndex: {
      type: [Number, String],
      required: true
    },
    fullName: {
      type: String,
      required: true
    },

    initialCheckedCount: {
      type: Number,
      default: 0
    },
    initialCarryOnCount: {
      type: Number,
      default: 0
    },

    checkedCount: {
      type: Number,
      default: 0
    },
    carryOnCount: {
      type: Number,
      default: 0
    },

    checkedPrice: {
      type: Number,
      default: 0
    },
    carryOnPrice: {
      type: Number,
      default: 0
    },

    previousTotal: {
      type: Number,
      default: 0
    },
    newTotal: {
      type: Number,
      default: 0
    },
    extraTotal: {
      type: Number,
      default: 0
    }
  },

  emits: ['luggage-change'],

  data() {
    return {
      checkedLocal: this.checkedCount,
      carryOnLocal: this.carryOnCount
    };
  },

  watch: {
    checkedCount(newValue) {
      this.checkedLocal = Number(newValue || 0);
    },

    carryOnCount(newValue) {
      this.carryOnLocal = Number(newValue || 0);
    }
  },

  methods: {
    increaseChecked() {
      this.checkedLocal += 1;
      this.emitChange();
    },

    decreaseChecked() {
      if (this.checkedLocal <= this.initialCheckedCount) return;

      this.checkedLocal -= 1;
      this.emitChange();
    },

    increaseCarryOn() {
      this.carryOnLocal += 1;
      this.emitChange();
    },

    decreaseCarryOn() {
      if (this.carryOnLocal <= this.initialCarryOnCount) return;

      this.carryOnLocal -= 1;
      this.emitChange();
    },

    emitChange() {
      this.$emit('luggage-change', {
        passengerIndex: this.passengerIndex,
        checkedCount: this.checkedLocal,
        carryOnCount: this.carryOnLocal
      });
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        maximumFractionDigits: 0
      }).format(Number(value || 0));
    }
  }
};
</script>

<style scoped>
.passenger-luggage-card {
  background: #ffffff;
  border: 1px solid #dbe3ef;
  border-radius: 10px;
  padding: 20px 28px;
  margin-bottom: 18px;
  display: grid;
  grid-template-columns: 130px 1fr 1fr;
  gap: 24px;
  align-items: center;
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.08);
}

.passenger-info h3 {
  margin: 0 0 6px;
  color: #032056;
  font-size: 17px;
  font-weight: 800;
}

.passenger-info p {
  margin: 0 0 14px;
  color: #7b8496;
  font-size: 14px;
}

.price-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  color: #032056;
  font-size: 13px;
  font-weight: 700;
}

.price-info strong {
  font-size: 13px;
  font-weight: 800;
}

.luggage-option {
  min-height: 145px;
  border: 1px solid #dbe3ef;
  border-radius: 10px;
  background: #f8fafc;
  padding: 18px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.luggage-text h4 {
  margin: 0 0 8px;
  color: #032056;
  font-size: 14px;
  font-weight: 800;
}

.luggage-text strong {
  display: block;
  color: #032056;
  font-size: 16px;
  font-weight: 800;
}

.luggage-text small {
  display: block;
  color: #7b8496;
  font-size: 12px;
  margin-top: 2px;
}

.luggage-text p {
  margin: 8px 0 0;
  color: #032056;
  font-size: 14px;
  line-height: 1.35;
}

.current-text {
  display: block;
  margin-top: 8px;
  color: #384467;
  font-size: 12px;
  font-weight: 700;
}

.counter {
  display: flex;
  align-items: center;
  gap: 12px;
}

.counter button {
  width: 30px;
  height: 30px;
  border: 1px solid #032056;
  border-radius: 7px;
  background: #ffffff;
  color: #032056;
  font-size: 18px;
  font-weight: 800;
  cursor: pointer;
}

.counter button:hover:not(:disabled) {
  background: #032056;
  color: #ffffff;
}

.counter button:disabled {
  border-color: #9aa7bd;
  color: #9aa7bd;
  cursor: not-allowed;
  background: #f1f5f9;
}

.counter span {
  min-width: 18px;
  text-align: center;
  color: #032056;
  font-weight: 800;
  font-size: 16px;
}

@media (max-width: 900px) {
  .passenger-luggage-card {
    grid-template-columns: 1fr;
  }

  .luggage-option {
    align-items: flex-start;
  }
}
</style>