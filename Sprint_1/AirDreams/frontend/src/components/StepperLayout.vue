<template>
  <div>
    <BarraNavegacion />

    <div class="page-wrapper">
      <div class="stepper">
        <div class="step" :class="{ active: currentStep >= 1, done: currentStep > 1 }">
          <div class="step-circle">1</div>
          <span class="step-label">Pasajeros</span>
        </div>
        <div class="step-line" :class="{ done: currentStep > 1 }"></div>

        <div class="step" :class="{ active: currentStep >= 2, done: currentStep > 2 }">
          <div class="step-circle">2</div>
          <span class="step-label">Equipaje</span>
        </div>
        <div class="step-line" :class="{ done: currentStep > 2 }"></div>

        <div class="step" :class="{ active: currentStep >= 3 }">
          <div class="step-circle">3</div>
          <span class="step-label">Pago</span>
        </div>
      </div>

      <div class="content-box">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script>
import BarraNavegacion from './BarraNavegacion.vue';

export default {
  name: 'StepperLayout',

  components: {
    BarraNavegacion
  },

  props: {
    currentStep: {
      type: Number,
      required: true,
      validator: (value) => value >= 1 && value <= 3
    }
  }
};
</script>

<style scoped>
.page-wrapper {
  max-width: 860px;
  margin: 0 auto;
  padding: 36px 24px 60px;
}

.stepper {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0;
  margin-bottom: 40px;
}

.step {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
}

.step-circle {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  border: 2px solid #c8d0de;
  background: #ffffff;
  color: #8a94a8;
  font-size: 14px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.step.active .step-circle {
  background: #032056;
  border-color: #032056;
  color: #ffffff;
}

.step.done .step-circle {
  background: #032056;
  border-color: #032056;
  color: #ffffff;
}

.step-label {
  font-size: 12px;
  color: #8a94a8;
  font-weight: 500;
}

.step.active .step-label {
  color: #032056;
  font-weight: 700;
}

.step-line {
  flex: 1;
  height: 2px;
  background: #c8d0de;
  min-width: 60px;
  margin-bottom: 18px;
  transition: background 0.2s ease;
}

.step-line.done {
  background: #032056;
}

.content-box {
  background: #f7f9fc;
  border-radius: 16px;
  padding: 32px 36px;
}
</style>
