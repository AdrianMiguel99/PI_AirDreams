<template>
  <div v-if="show" class="popup-overlay">
    <div class="popup-card">
      <h2 :class="type">{{ title }}</h2>

      <p>{{ message }}</p>

      <div class="popup-actions">
        <button class="btn-secondary" @click="$emit('close')">
          Cerrar
        </button>

        <button
          v-if="actionText"
          class="btn-primary"
          @click="$emit('action')"
        >
          {{ actionText }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "PopupMessage",

  props: {
    show: {
      type: Boolean,
      required: true
    },
    type: {
      type: String,
      default: "success"
    },
    title: {
      type: String,
      required: true
    },
    message: {
      type: String,
      required: true
    },
    actionText: {
      type: String,
      default: ""
    }
  }
};
</script>

<style scoped>
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.35);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.popup-card {
  background: white;
  border-radius: 12px;
  padding: 28px;
  width: 360px;
  text-align: center;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
}

.popup-card h2 {
  font-size: 22px;
  margin-bottom: 12px;
}

.success {
  color: #0b1f4d;
}

.error {
  color: #b00020;
}

.popup-card p {
  color: #6c6c6c;
  font-weight: 600;
  margin-bottom: 24px;
}

.popup-actions {
  display: flex;
  justify-content: center;
  gap: 12px;
}

.btn-primary {
  background: #071b4d;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
}

.btn-secondary {
  background: white;
  color: #071b4d;
  border: 1px solid #071b4d;
  padding: 10px 18px;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
}
</style>