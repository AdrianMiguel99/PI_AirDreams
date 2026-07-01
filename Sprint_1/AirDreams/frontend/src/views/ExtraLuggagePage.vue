<template>
  <main class="add-luggage-page">
    <h2 class="section-title">Agregar maletas</h2>

    <section class="summary-card">
      <div>
        <span>Precio anterior</span>
        <strong>{{ formatCurrency(previousLuggageTotal) }}</strong>
      </div>

      <div>
        <span>Precio nuevo</span>
        <strong>{{ formatCurrency(newLuggageTotal) }}</strong>
      </div>

      <div>
        <span>Diferencia a pagar</span>
        <strong>{{ formatCurrency(extraLuggageTotal) }}</strong>
      </div>
    </section>

    <ExtraLuggagePassenger
      v-for="passenger in pageState.passengers"
      :key="passenger.idPassenger"
      :passengerIndex="passenger.idPassenger"
      :fullName="passenger.fullName"
      :initialCheckedCount="passenger.luggage.initial.checkedCount"
      :initialCarryOnCount="passenger.luggage.initial.carryOnCount"
      :checkedCount="passenger.luggage.current.checkedCount"
      :carryOnCount="passenger.luggage.current.carryOnCount"
      :checkedPrice="baseCheckedPrice"
      :carryOnPrice="baseCarryOnPrice"
      :previousTotal="passenger.luggage.initial.totalPrice"
      :newTotal="passenger.luggage.current.totalPrice"
      :extraTotal="passenger.luggage.extra.totalPrice"
      @luggage-change="handleLuggageChange"
    />

    <div class="footer-actions">
      <button class="btn-back" @click="goBack">
        ← Volver
      </button>

      <button class="btn-continue" @click="continueToPay">
        Pagar
      </button>
    </div>

    <PopupMessage
      :show="showPopup"
      :title="popupTitle"
      :message="popupMessage"
      :type="popupType"
      @close="showPopup = false"
    />
  </main>
</template>

<script>
import axios from 'axios';
import ExtraLuggagePassenger from '../components/reservations/ExtraLuggagePassenger.vue';
import PopupMessage from '../components/PopupMessage.vue';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5276/api';

export default {
  name: 'ExtraLuggagePage',

  components: {
    ExtraLuggagePassenger,
    PopupMessage
  },

  data() {
    return {
      pageState: {
        passengers: [],
        flights: []
      },

      checkedBagWeight: 23,
      carryOnBagWeight: 10,

      loading: false,
      showPopup: false,
      popupMessage: '',
      popupTitle: 'Agregar maletas',
      popupType: 'success'
    };
  },

  computed: {
    reservationCode() {
      return String(this.$route.query.reservationCode || '').trim();
    },

    baseCheckedPrice() {
      return this.pageState.flights[0]?.checkedPrice || 0;
    },

    baseCarryOnPrice() {
      return this.pageState.flights[0]?.carryOnPrice || 0;
    },

    previousLuggageTotal() {
      return this.pageState.passengers.reduce((total, passenger) => {
        return total + Number(passenger.luggage.initial.totalPrice || 0);
      }, 0);
    },

    newLuggageTotal() {
      return this.pageState.passengers.reduce((total, passenger) => {
        return total + Number(passenger.luggage.current.totalPrice || 0);
      }, 0);
    },

    extraLuggageTotal() {
      return this.pageState.passengers.reduce((total, passenger) => {
        return total + Number(passenger.luggage.extra.totalPrice || 0);
      }, 0);
    },

    totalCheckedWeight() {
      return this.pageState.passengers.reduce((total, passenger) => {
        return total + Number(passenger.luggage.extra.checkedCount || 0) * this.checkedBagWeight;
      }, 0);
    },

    totalCarryOnWeight() {
      return this.pageState.passengers.reduce((total, passenger) => {
        return total + Number(passenger.luggage.extra.carryOnCount || 0) * this.carryOnBagWeight;
      }, 0);
    }
  },

  created() {
    this.loadPageData();
  },

  methods: {
    async loadPageData() {
      if (!this.reservationCode) {
        this.showError('No se recibió el código de reserva.');
        return;
      }

      try {
        const reservationResponse = await axios.get(
          `${API_URL}/Reservation/${encodeURIComponent(this.reservationCode)}`
        );

        const reservation = reservationResponse.data || {};

        this.pageState.passengers = (reservation.passengers || []).map((passenger) => ({
          idPassenger: passenger.idPassenger,
          fullName:
            passenger.fullName ||
            passenger.passengerName ||
            `${passenger.namePassenger || ''} ${passenger.lastnamesPassenger || ''}`.trim(),

          luggage: {
            initial: {
              checkedCount: 0,
              carryOnCount: 0,
              totalPrice: 0
            },

            current: {
              checkedCount: 0,
              carryOnCount: 0,
              totalPrice: 0
            },

            extra: {
              checkedCount: 0,
              carryOnCount: 0,
              totalPrice: 0
            }
          }
        }));

        const luggageResponse = await axios.get(
          `${API_URL}/Luggage/reservation/${encodeURIComponent(this.reservationCode)}`
        );

        const luggageData = luggageResponse.data || {};
        const reservationLuggage = luggageData.luggage || [];

        this.pageState.flights = (luggageData.segments || []).map((segment) => ({
          id: String(segment.flightNumber || '').trim(),
          routeId: segment.routeId,
          checkedPrice: Number(segment.checkedPrice || 0),
          carryOnPrice: Number(segment.carryOnPrice || 0),
          multiplier: Number(segment.multiplier || 0.5)
        }));

        for (const item of reservationLuggage) {
          const passenger = this.pageState.passengers.find((currentPassenger) => {
            return String(currentPassenger.idPassenger) === String(item.idPassenger);
          });

          if (!passenger) continue;

          const type = String(item.type || '').toLowerCase();
          const quantity = Number(item.quantity || 0);

          if (type === 'checked') {
            passenger.luggage.initial.checkedCount += quantity;
            passenger.luggage.current.checkedCount += quantity;
          }

          if (type === 'carryon' || type === 'carry-on' || type === 'carry_on') {
            passenger.luggage.initial.carryOnCount += quantity;
            passenger.luggage.current.carryOnCount += quantity;
          }
        }

        for (const passenger of this.pageState.passengers) {
          const initialTotal = await this.calculateLuggageTotal(
            passenger.luggage.initial.checkedCount,
            passenger.luggage.initial.carryOnCount
          );

          passenger.luggage.initial.totalPrice = initialTotal;
          passenger.luggage.current.totalPrice = initialTotal;
          passenger.luggage.extra.totalPrice = 0;
        }
      } catch (error) {
        console.error('Error al cargar datos de equipaje:', error);
        this.showError('No se pudo cargar la información de la reserva.');
      }
    },

    async calculateLuggageTotal(checkedQuantity, carryOnQuantity) {
      if (this.pageState.flights.length === 0) return 0;

      const response = await axios.post(`${API_URL}/luggage/calculate/total`, {
        checkedQuantity,
        carryOnQuantity,
        segments: this.pageState.flights.map((flight) => ({
          checkedPrice: Number(flight.checkedPrice || 0),
          carryOnPrice: Number(flight.carryOnPrice || 0),
          multiplier: Number(flight.multiplier || 0.5)
        }))
      });

      return Number(response.data.checkedTotal || 0) +
        Number(response.data.carryOnTotal || 0);
    },

    async handleLuggageChange({ passengerIndex, checkedCount, carryOnCount }) {
      try {
        const passenger = this.pageState.passengers.find((currentPassenger) => {
          return String(currentPassenger.idPassenger) === String(passengerIndex);
        });

        if (!passenger) {
          this.showError('No se encontró el pasajero.');
          return;
        }

        const initial = passenger.luggage.initial;

        const newTotal = await this.calculateLuggageTotal(
          checkedCount,
          carryOnCount
        );

        passenger.luggage.current = {
          checkedCount,
          carryOnCount,
          totalPrice: newTotal
        };

        passenger.luggage.extra = {
          checkedCount: Math.max(
            0,
            Number(checkedCount || 0) - Number(initial.checkedCount || 0)
          ),
          carryOnCount: Math.max(
            0,
            Number(carryOnCount || 0) - Number(initial.carryOnCount || 0)
          ),
          totalPrice: Math.max(
            0,
            Number(newTotal || 0) - Number(initial.totalPrice || 0)
          )
        };
      } catch (error) {
        console.error('Error al calcular equipaje:', error);
        this.showError('No se pudo calcular el precio del equipaje.');
      }
    },

    buildExtraLuggageByPassenger() {
      return this.pageState.passengers
        .map((passenger) => {
          const luggageItems = [];

          if (passenger.luggage.extra.checkedCount > 0) {
            luggageItems.push({
              type: 'checked',
              quantity: passenger.luggage.extra.checkedCount
            });
          }

          if (passenger.luggage.extra.carryOnCount > 0) {
            luggageItems.push({
              type: 'carryOn',
              quantity: passenger.luggage.extra.carryOnCount
            });
          }

          return {
            idPassenger: passenger.idPassenger,
            passenger,
            luggageItems,
            previousTotal: Number(passenger.luggage.initial.totalPrice || 0),
            newTotal: Number(passenger.luggage.current.totalPrice || 0),
            passengerTotal: Number(passenger.luggage.extra.totalPrice || 0)
          };
        })
        .filter((item) => item.luggageItems.length > 0);
    },

    async continueToPay() {
      this.loading = true;

      try {
        const totalCheckedWeight = this.totalCheckedWeight;
        const totalCarryOnWeight = this.totalCarryOnWeight;

        if (totalCheckedWeight === 0 && totalCarryOnWeight === 0) {
          this.showError('Debes agregar al menos una maleta adicional.');
          return;
        }

        for (const flight of this.pageState.flights) {
          if (!flight.id || !flight.routeId) continue;

          const validation = await this.validateWeights(
            flight.id,
            flight.routeId,
            totalCheckedWeight,
            totalCarryOnWeight
          );

          if (!validation.success) {
            this.popupType = 'error';
            this.popupTitle = 'No hay espacio suficiente';
            this.popupMessage = `${flight.id}: ${validation.message}`;
            this.showPopup = true;
            return;
          }
        }

        const luggageByPassenger = this.buildExtraLuggageByPassenger();

        if (luggageByPassenger.length === 0) {
          this.showError('Debes agregar al menos una maleta adicional.');
          return;
        }

        for (const passengerLuggage of luggageByPassenger) {
          const response = await fetch(`${API_URL}/Luggage/register`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
              idPassenger: passengerLuggage.idPassenger,
              transactionIdItinerary: this.reservationCode,
              luggageItems: passengerLuggage.luggageItems
            })
          });

          const data = await response.json();

          if (!response.ok) {
            throw new Error(data.message || 'No se pudo registrar el equipaje.');
          }
        }

        const updateResponse = await fetch(`${API_URL}/Luggage/update`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            transactionId: this.reservationCode,
            luggageWeight: totalCheckedWeight || 0,
            carryOnWeight: totalCarryOnWeight || 0
          })
        });

        const updateData = await updateResponse.json();

        if (!updateResponse.ok) {
          throw new Error(updateData.message || 'No se pudo actualizar el peso del vuelo.');
        }

        this.popupType = 'success';
        this.popupTitle = 'Pago exitoso';
        this.popupMessage = `El equipaje adicional fue pagado y registrado correctamente. Total pagado: ${this.formatCurrency(this.extraLuggageTotal)}.`;
        this.showPopup = true;

        await this.loadPageData();
      } catch (error) {
        console.error('Error al procesar equipaje extra:', error);
        this.showError(error.message || 'Ocurrió un error al procesar el equipaje adicional.');
      } finally {
        this.loading = false;
      }
    },

    async validateWeights(flightId, routeId, checkedWeight, carryOnWeight) {
      try {
        const response = await fetch(`${API_URL}/luggage/availability`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            flightId,
            routeId,
            luggageWeight: checkedWeight || 0,
            carryOnWeight: carryOnWeight || 0
          })
        });

        const data = await response.json();

        if (!response.ok) {
          return {
            success: false,
            message: data.message || 'Error en la validación'
          };
        }

        return {
          success: data.success,
          message: data.message || 'Validación exitosa'
        };
      } catch (error) {
        return {
          success: false,
          message: 'Error de conexión: ' + error.message
        };
      }
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        maximumFractionDigits: 0
      }).format(Number(value || 0));
    },

    showError(message) {
      this.popupType = 'error';
      this.popupTitle = 'Agregar maletas';
      this.popupMessage = message;
      this.showPopup = true;
    },

    goBack() {
      this.$router.push({
        name: 'ReservationDetails',
        query: {
          reservationCode: this.reservationCode
        }
      });
    }
  }
};
</script>

<style scoped>
.add-luggage-page {
  width: min(1000px, calc(100% - 32px));
  margin: 0 auto;
  padding: 32px 0 48px;
}

.section-title {
  font-size: 20px;
  font-weight: 700;
  color: #032056;
  margin: 0 0 24px;
}

.summary-card {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 18px 20px;
  margin-bottom: 22px;
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.08);
}

.summary-card div {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.summary-card span {
  color: #64748b;
  font-size: 13px;
  font-weight: 600;
}

.summary-card strong {
  color: #032056;
  font-size: 20px;
  font-weight: 800;
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
}

.btn-continue:hover {
  background: #0a3a7a;
}

.btn-continue:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

@media (max-width: 760px) {
  .summary-card {
    grid-template-columns: 1fr;
  }

  .footer-actions {
    flex-direction: column-reverse;
    gap: 14px;
    align-items: stretch;
  }

  .btn-continue {
    width: 100%;
  }
}
</style>