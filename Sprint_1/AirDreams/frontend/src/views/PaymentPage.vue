<template>
  <StepperLayout :currentStep="3">
    <div class="page-header">
      <h2 class="section-title">Pago</h2>
      <p class="section-subtitle">
        Revisa el detalle de tu compra y selecciona un método de pago.
      </p>
    </div>

    <div class="purchase-summary">
      <h3>Resumen de compra</h3>
      <div class="summary-grid">
        <div class="summary-item">
          <span class="summary-label">Vuelo</span>
          <strong>{{ flightDescription }}</strong>
        </div>
        <div class="summary-item">
          <span class="summary-label">Clase</span>
          <strong>{{ seatClassLabel }}</strong>
        </div>
        <div class="summary-item">
          <span class="summary-label">Pasajeros</span>
          <strong>{{ passengerCount }}</strong>
        </div>
        <div class="summary-item">
          <span class="summary-label">Precio por pasajero</span>
          <strong>{{ formatCurrency(pricePerPassenger) }}</strong>
        </div>
        <div class="summary-item">
          <span class="summary-label">Subtotal vuelo</span>
          <strong>{{ formatCurrency(flightSubtotal) }}</strong>
        </div>
        <div class="summary-item">
          <span class="summary-label">Equipaje</span>
          <strong>{{ formatCurrency(luggageTotal) }}</strong>
        </div>
        <div class="summary-item total">
          <span class="summary-label">Total a pagar</span>
          <strong>{{ formatCurrency(grandTotal) }}</strong>
        </div>
      </div>
    </div>

    <form @submit.prevent="processPayment">
      <div class="form-grid">
        <label class="form-field">
          Nombre del comprador
          <input v-model="payment.buyerName" type="text" required maxlength="100" />
        </label>
      </div>

      <div class="payment-methods">
        <h3>Método de pago</h3>
        <div class="method-options">
          <label v-for="method in paymentMethods" :key="method.value" class="method-label">
            <input type="radio" v-model="payment.paymentMethod" :value="method.value" @change="onMethodChange" />
            <span>{{ method.label }}</span>
          </label>
        </div>
      </div>

      <div v-if="payment.paymentMethod === 'Card'" class="card-fields">
        <label class="form-field">
          Número de tarjeta
          <input v-model="payment.cardNumber" type="text" maxlength="19" placeholder="0000 0000 0000 0000" @input="formatCardNumber" />
        </label>
        <div class="card-row">
          <label class="form-field">
            Vencimiento (MM/YY)
            <input v-model="payment.cardExpiry" type="text" maxlength="5" placeholder="MM/YY" @input="formatExpiry" />
          </label>
          <label class="form-field">
            CVV
            <input v-model="payment.cardCvv" type="password" maxlength="4" placeholder="123" />
          </label>
        </div>
      </div>

      <div class="footer-actions">
        <button type="button" class="btn-back" @click="$router.push({ name: 'luggage' })">← Volver a equipaje</button>
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
const API_BASE = import.meta.env.VITE_API_URL;
export default {
  name: 'PaymentPage',
  components: { StepperLayout, PopupMessage },
  data() {
    return {
      payment: {
        transactionId: '', buyerName: '', paymentMethod: '',
        cardNumber: '', cardExpiry: '', cardCvv: ''
      },
      paymentMethods: [
        { label: 'Tarjeta de crédito/débito', value: 'Card' },
        { label: 'PayPal', value: 'PayPal' },
        { label: 'Google Pay', value: 'GooglePay' },
        { label: 'Apple Pay', value: 'ApplePay' }
      ],
      processing: false,
      showPopup: false, popupType: 'success', popupTitle: '', popupMessage: '', popupActionText: '', popupAction: null,
      selectedPurchase: null, luggageData: []
    }
  },
  computed: {
    transactionId() { return this.payment.transactionId },
    seatClassLabel() {
      if (!this.selectedPurchase) return ''
      return this.selectedPurchase.seatClass === 'FirstClass' ? 'Primera clase' : 'Turista'
    },
    passengerCount() { return this.selectedPurchase?.passengerCount || 1 },
    pricePerPassenger() { return Number(this.selectedPurchase?.price || 0) },
    flightSubtotal() { return this.passengerCount * this.pricePerPassenger },
    luggageTotal() {
      return this.luggageData.reduce((total, item) => total + (item.passengerTotal || 0), 0)
    },
    grandTotal() { return this.flightSubtotal + this.luggageTotal },
    flightDescription() {
      const p = this.selectedPurchase
      if (p) {
        if (p.origin && p.destination) return `${p.origin} → ${p.destination}`
        if (p.flightNumber) return `Vuelo ${p.flightNumber}`
        if (p.itinerary?.itineraryId) return `Vuelo #${p.itinerary.itineraryId}`
      }
      return `#${this.transactionId}`
    }
  },
  created() { this.loadTransactionId(); this.loadPurchaseData(); },
  methods: {
    loadTransactionId() {
      let txId = this.$route.query.transactionId;
      if (!txId || txId === 'TXN-DEFAULT') {
        txId = 'TXN-' + crypto.randomUUID().substring(0, 8);
        sessionStorage.setItem('transactionId', txId);
      }
      this.payment.transactionId = txId;
    },
    loadPurchaseData() {
      const savedPurchase = sessionStorage.getItem('selectedFlightPurchase')
      if (savedPurchase) this.selectedPurchase = JSON.parse(savedPurchase)
      const savedLuggage = sessionStorage.getItem('purchaseLuggage')
      if (savedLuggage) this.luggageData = JSON.parse(savedLuggage)
    },
    onMethodChange() {
      if (this.payment.paymentMethod !== 'Card') {
        this.payment.cardNumber = ''; this.payment.cardExpiry = ''; this.payment.cardCvv = ''
      }
    },
    formatCardNumber(event) {
      let value = event.target.value.replace(/\D/g, '').substring(0, 16)
      this.payment.cardNumber = value.replace(/(.{4})/g, '$1 ').trim()
    },
    formatExpiry(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length > 2) value = value.substring(0,2) + '/' + value.substring(2,4)
      this.payment.cardExpiry = value
    },
    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(Number(value || 0))
    },

    normalizeSeatClass(value) {
      return value === 'FirstClass' || value === 'firstClass' ? 'FirstClass' : 'Turista'
    },
    
    formatDate(value) {
      const date = new Date(value)
      return date.toLocaleDateString('es-ES', { day: '2-digit', month: 'long', year: 'numeric' })
    },


    createStructForPage() {
      const purchase = JSON.parse(sessionStorage.getItem('selectedFlightPurchase') || '{}')
      const passengers = JSON.parse(sessionStorage.getItem('purchasePassengers') || '[]')
      const luggage = JSON.parse(sessionStorage.getItem('purchaseLuggage') || '[]')
      const seatClass = this.normalizeSeatClass(purchase.seatClass)

      return {
        transactionId: this.payment.transactionId,
        buyerName: this.payment.buyerName,
        paymentMethod: this.payment.paymentMethod,
        flightDescription: this.flightDescription,
        seatClassLabel: this.seatClassLabel,
        seatClass,
        passengerCount: this.passengerCount,
        pricePerPassenger: this.pricePerPassenger,
        flightSubtotal: this.flightSubtotal,
        luggageTotal: this.luggageTotal,
        grandTotal: this.grandTotal,
        purchase: purchase,
        passengers: passengers,
        luggage: luggage
      }
    },

    callPurchaseSuccess(purchasewindowData) {
      sessionStorage.setItem('purchaseSuccessData', JSON.stringify(purchasewindowData))
      this.$router.push({ name: 'PurchaseSuccess', params: { idCompra: this.transactionId } })
    },

    
    async processPayment() {
      this.processing = true
      if (!this.payment.paymentMethod) {
        this.showPopup = true;
        this.popupType = 'error';
        this.popupTitle = 'Método de pago requerido';
        this.popupMessage = 'Debes seleccionar un método de pago antes de continuar.';
        this.popupActionText = '';
        this.processing = false;
        return;
      }

    if (this.payment.paymentMethod === 'Card') {
      const cardNumberClean = this.payment.cardNumber?.replace(/\s/g, '') || '';
      if (!cardNumberClean || !this.payment.cardExpiry || !this.payment.cardCvv) {
        this.showPopup = true;
        this.popupType = 'error';
        this.popupTitle = 'Datos de tarjeta incompletos';
        this.popupMessage = 'Por favor, completa todos los campos de la tarjeta.';
        this.popupActionText = '';
        this.processing = false;
        return;
      }
      if (cardNumberClean.length < 13) {
        this.showPopup = true;
        this.popupType = 'error';
        this.popupTitle = 'Número de tarjeta inválido';
        this.popupMessage = 'El número de tarjeta es demasiado corto.';
        this.popupActionText = '';
        this.processing = false;
        return;
      }
    }
      try {
        const purchase = JSON.parse(sessionStorage.getItem('selectedFlightPurchase'))
        const passengers = JSON.parse(sessionStorage.getItem('purchasePassengers'))
        const luggage = JSON.parse(sessionStorage.getItem('purchaseLuggage'))
        const seatClass = this.normalizeSeatClass(purchase.seatClass)

        const payload = {
          transactionId: this.payment.transactionId,
          buyerName: this.payment.buyerName,
          paymentMethod: this.payment.paymentMethod,
          cardNumber: this.payment.cardNumber?.replace(/\s/g, '') || null,
          cardExpiry: this.payment.cardExpiry || null,
          cardCvv: this.payment.cardCvv || null,
          segments: purchase.itinerary.segments.map(s => ({
            flightNumber: (s.flightNumber || '').trim().substring(0, 20),
            routeId: s.routeId || s.idRoute || null,

            departureDate: s.departureDate || null,
            arrivalDate: s.arrivalDate || null,
            departureTime: s.departureTime || '',
            arrivalTime: s.arrivalTime || '',
            duration: s.duration || '',

            departureAirport: {
              name: s.departureAirport?.name || '',
              code: s.departureAirport?.code || '',
              city: s.departureAirport?.city || ''
            },

            arrivalAirport: {
              name: s.arrivalAirport?.name || '',
              code: s.arrivalAirport?.code || '',
              city: s.arrivalAirport?.city || ''
            },

            checkedPrice: s.checkedPrice || 0,
            carryOnPrice: s.carryOnPrice || 0,
            multiplier: s.porcentageMultiplier || 0.2
          })),
          seatClass,
          pricePerPassenger: purchase.price,
          passengerCount: purchase.passengerCount,
          passengers: passengers.map(p => ({
            namePassenger: p.namePassenger,
            lastnamesPassenger: p.lastnamesPassenger,
            birthDate: p.birthDate || null,
            emailPassenger: p.emailPassenger || '',
            telephone: p.telephone || '',
            country: p.country || '',
            birthDate: p.birthDate || ''
          })),
          luggage: luggage ? luggage.map(l => ({
            passengerIndex: l.passenger.index,
            luggageItems: l.luggageItems
          })) : []
        }

        for (const segment of payload.segments) {
          const availabilityResponse = await axios.post(`${API_BASE}/api/payment/check-availability`, {
            numberFlight: segment.flightNumber,
            seatClass: payload.seatClass,
            requestedSeats: payload.passengerCount
          })
          
          if (!availabilityResponse.data.isAvailable) {
            this.popupType = 'error'
            this.popupTitle = 'Asientos no disponibles'
            this.popupMessage = `No hay asientos disponibles para el vuelo ${segment.flightNumber} en clase ${payload.seatClass}. Por favor, regresa y selecciona otro vuelo o clase.`
            this.popupActionText = 'Volver a selección de vuelo'
            this.popupAction = () => {
              sessionStorage.removeItem('selectedFlightPurchase')
              sessionStorage.removeItem('purchasePassengers')
              sessionStorage.removeItem('purchaseLuggage')
              this.$router.push({ name: 'home' })
            }
            this.showPopup = true
            return
          };
        }
        await axios.post(`${API_BASE}/api/payment`, payload)
        await this.updateFlightWeight(this.payment.transactionId)
        const purchasewindowData = this.createStructForPage()
        sessionStorage.removeItem('transactionId')
        sessionStorage.removeItem('luggageWeights')

        this.callPurchaseSuccess(purchasewindowData)
      } catch (error) {
        this.popupType = 'error'
        this.popupTitle = 'Error en el pago'
        if (error.response?.data?.error) {
          this.popupMessage = error.response.data.error
        } else if (error.response?.data?.errors) {
          const messages = Object.values(error.response.data.errors).flat().join(', ')
          this.popupMessage = messages || 'Datos inválidos.'
        } else {
          this.popupMessage = 'No se pudo procesar el pago.'
        }
        this.popupActionText = ''
        this.showPopup = true
      } finally {
        this.processing = false
      }
    },

    async updateFlightWeight(transactionId) {
      const weights = JSON.parse(
        sessionStorage.getItem('luggageWeights') || '{}'
      )

      if (!weights.luggageWeight && !weights.carryOnWeight)
        return

      try {
        await axios.post(
          `${API_BASE}/api/luggage/update`,
          {
            transactionId,
            luggageWeight: weights.luggageWeight || 0,
            carryOnWeight: weights.carryOnWeight || 0
          }
        )
      } catch (error) {
        console.error('Ha ocurrido un error al actualizar el peso del equipaje:', error)
      }
    }
  }
}
</script>

<style scoped>
.page-header { margin-bottom: 24px; }
.section-title { font-size: 20px; font-weight: 700; color: #032056; margin: 0 0 6px; }
.section-subtitle { color: #667085; font-size: 14px; margin: 0; }

.purchase-summary {
  background: #ffffff;
  border: 1px solid #e8ecf4;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 1px 4px rgba(3, 32, 86, 0.06);
}
.purchase-summary h3 {
  color: #032056;
  margin: 0 0 16px;
  font-size: 16px;
}
.summary-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}
.summary-item {
  display: flex;
  justify-content: space-between;
  color: #334155;
  font-size: 14px;
}
.summary-label { color: #8a94a8; }
.summary-item.total { border-top: 1px solid #e8ecf4; padding-top: 12px; margin-top: 4px; font-weight: 700; }

.form-grid { display: grid; grid-template-columns: 1fr; gap: 16px; }

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
  .summary-grid,
  .card-row {
    grid-template-columns: 1fr;
  }

  .footer-actions {
    flex-direction: column;
    gap: 18px;
  }
}
</style>
