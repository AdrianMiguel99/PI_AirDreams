<template>
  <main class="cancel-page">
    <section class="cancel-card">
      <div class="plane-icon">✈</div>

      <h1>Cancelar reserva</h1>

      <p v-if="!confirmed && !errorMessage" class="description">
        ¿Está seguro de que desea cancelar esta reserva?
      </p>

      <p v-if="!confirmed && !errorMessage" class="warning">
        Esta acción es irreversible y no se realizará devolución de dinero.
      </p>

      <button
        v-if="!confirmed && !errorMessage"
        class="primary-button"
        @click="confirmCancellation"
        :disabled="loading"
      >
        {{ loading ? 'Cancelando...' : 'Confirmar cancelación' }}
      </button>

      <p v-if="successMessage" class="success">
        {{ successMessage }}
      </p>

      <p v-if="errorMessage" class="error">
        {{ errorMessage }}
      </p>

      <router-link to="/" class="back-link">
        Volver al inicio
      </router-link>
    </section>
  </main>
</template>

<script>
export default {
  name: 'CancelReservation',

  data() {
    return {
      loading: false,
      confirmed: false,
      successMessage: '',
      errorMessage: ''
    };
  },

  methods: {
    async confirmCancellation() {
      this.loading = true;
      this.errorMessage = '';

      const token = String(this.$route.query.token || '').trim();

      if (!token) {
        this.errorMessage = 'Token de cancelación inválido.';
        this.loading = false;
        return;
      }

      try {
        const response = await fetch(
          'http://localhost:5276/api/cancellations/confirm',
          {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ token })
          }
        );

        const data = await response.json();

        if (!response.ok) {
          throw new Error(data.message || 'No se pudo cancelar la reserva.');
        }

        this.confirmed = true;
        this.successMessage =
          data.message || 'Reserva cancelada correctamente.';
      } catch (error) {
        this.errorMessage =
          error.message || 'Error al cancelar la reserva.';
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.cancel-page {
  min-height: 100vh;
  background: #ffffff;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 40px;
  font-family: Arial, sans-serif;
}

.cancel-card {
  width: 100%;
  max-width: 620px;
  background: #ffffff;
  border: 1px solid #d9dde5;
  border-radius: 18px;
  padding: 48px 54px;
  text-align: center;
  box-shadow: 0 12px 32px rgba(15, 23, 42, 0.08);
}

.plane-icon {
  width: 42px;
  height: 42px;
  margin: 0 auto 18px;
  border: 2px solid #2f3e5c;
  border-radius: 50%;
  color: #2f3e5c;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  font-weight: bold;
}

h1 {
  color: #1f2933;
  font-size: 34px;
  font-weight: 700;
  margin-bottom: 20px;
}

.description {
  color: #1f2933;
  font-size: 20px;
  margin-bottom: 18px;
}

.warning {
  color: #999;
  font-size: 20px;
  font-weight: 700;
  margin: 22px 0 32px;
  line-height: 1.4;
}

.primary-button {
  width: 100%;
  padding: 18px 24px;
  border: none;
  border-radius: 14px;
  background: #2f3e5c;
  color: #ffffff;
  cursor: pointer;
  font-weight: 700;
  font-size: 20px;
}

.primary-button:hover {
  background: #2f3e5c;
}

.primary-button:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.success,
.error {
  font-size: 20px;
  font-weight: 700;
  margin-top: 20px;
}

.success {
  color: #15803d;
}

.error {
  color: #b91c1c;
}

.back-link {
  display: inline-block;
  margin-top: 26px;
  color: #2f3e5c;
  text-decoration: none;
  font-size: 17px;
  font-weight: 700;
}

.back-link:hover {
  text-decoration: underline;
}
</style>