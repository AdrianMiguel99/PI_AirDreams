<template>
  <StepperLayout :currentStep="3">
    <div class="page-header">
      <h2 class="section-title">Pago</h2>
      <p class="section-subtitle">
        Ingresa los datos del comprador y elige el método de pago.
      </p>
    </div>

    <form @submit.prevent="processPayment">
      <div class="form-grid">
        <label class="form-field">
          Nombre del comprador
          <input v-model="payment.buyerName" type="text" required maxlength="100" />
        </label>

        <label class="form-field">
          Correo electrónico
          <input v-model="payment.buyerEmail" type="email" required maxlength="100" />
        </label>

        <label class="form-field">
          Teléfono
          <input v-model="payment.buyerPhone" type="tel" maxlength="20" />
        </label>
      </div>

      <div class="payment-methods">
        <h3>Método de pago</h3>
        <div class="method-options">
          <label
            v-for="method in paymentMethods"
            :key="method.value"
            class="method-label"
          >
            <input
              type="radio"
              v-model="payment.paymentMethod"
              :value="method.value"
              @change="onMethodChange"
            />
            <span>{{ method.label }}</span>
          </label>
        </div>
      </div>

      <div v-if="payment.paymentMethod === 'Card'" class="card-fields">
        <label class="form-field">
          Número de tarjeta
          <input
            v-model="payment.cardNumber"
            type="text"
            maxlength="19"
            placeholder="0000 0000 0000 0000"
            @input="formatCardNumber"
          />
        </label>

        <div class="card-row">
          <label class="form-field">
            Vencimiento (MM/YY)
            <input
              v-model="payment.cardExpiry"
              type="text"
              maxlength="5"
              placeholder="MM/YY"
              @input="formatExpiry"
            />
          </label>

          <label class="form-field">
            CVV
            <input
              v-model="payment.cardCvv"
              type="password"
              maxlength="4"
              placeholder="123"
            />
          </label>
        </div>
      </div>

      <div class="footer-actions">
        <button type="button" class="btn-back" @click="$router.push({ name: 'luggage' })">
          ← Volver a equipaje
        </button>
        <button type="submit" class="btn-continue" :disabled="processing">
          {{ processing ? 'Procesando...' : 'Confirmar pago' }}
        </button>
      </div>
    </form>

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="popupAction"
    />
  </StepperLayout>
</template>

<script>
import axios from 'axios'
import StepperLayout from '../components/StepperLayout.vue'
import PopupMessage from '../components/PopupMessage.vue'

export default {
  name: 'PaymentPage',
  components: { StepperLayout, PopupMessage },
  data() {
    return {
      payment: {
        transactionId: '',
        buyerName: '',
        buyerEmail: '',
        buyerPhone: '',
        paymentMethod: '',
        cardNumber: '',
        cardExpiry: '',
        cardCvv: ''
      },
      paymentMethods: [
        { label: 'Tarjeta de crédito/débito', value: 'Card' },
        { label: 'PayPal', value: 'PayPal' },
        { label: 'Google Pay', value: 'GooglePay' },
        { label: 'Apple Pay', value: 'ApplePay' }
      ],
      processing: false,
      showPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: '',
      popupActionText: '',
      popupAction: null
    }
  },
  created() {
    this.loadTransactionId()
  },
  methods: {
    loadTransactionId() {
      this.payment.transactionId =
        this.$route.query.transactionId ||
        sessionStorage.getItem('transactionId') ||
        'TXN-DEFAULT'
    },
    onMethodChange() {
      if (this.payment.paymentMethod !== 'Card') {
        this.payment.cardNumber = ''
        this.payment.cardExpiry = ''
        this.payment.cardCvv = ''
      }
    },
    formatCardNumber(event) {
      let value = event.target.value.replace(/\D/g, '')
      value = value.substring(0, 16)
      value = value.replace(/(.{4})/g, '$1 ').trim()
      this.payment.cardNumber = value
    },
    formatExpiry(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 2) value = value.substring(0,2) + '/' + value.substring(2,4)
      this.payment.cardExpiry = value
    },
    async processPayment() {
      this.processing = true
      try {
        const payload = { ...this.payment }
        if (payload.paymentMethod === 'Card') {
          payload.cardNumber = payload.cardNumber.replace(/\s/g, '')
        }
        await axios.post('http://localhost:5276/api/payment', payload)
        this.popupType = 'success'
        this.popupTitle = 'Pago exitoso'
        this.popupMessage = 'Tu compra ha sido confirmada. ¡Gracias por volar con Air Dreams!'
        this.popupActionText = 'Volver al inicio'
        this.popupAction = () => this.$router.push({ name: 'home' })
        this.showPopup = true
      } catch (error) {
        console.error(error)
        this.popupType = 'error'
        this.popupTitle = 'Error en el pago'
        this.popupMessage = error.response?.data?.error || 'No se pudo procesar el pago.'
        this.popupActionText = ''
        this.showPopup = true
      } finally {
        this.processing = false
      }
    }
  }
}
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

.payment-methods {
  margin: 24px 0;
}

.payment-methods h3 {
  font-size: 16px;
  font-weight: 600;
  color: #032056;
  margin-bottom: 12px;
}

.method-options {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.method-label {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #334155;
  font-weight: 500;
  cursor: pointer;
}

.method-label input[type="radio"] {
  accent-color: #032056;
}

.card-fields {
  background: #f8fafc;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 24px;
}

.card-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-top: 16px;
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
  .form-grid,
  .card-row {
    grid-template-columns: 1fr;
  }

  .footer-actions {
    flex-direction: column;
    gap: 18px;
  }
}
</style>