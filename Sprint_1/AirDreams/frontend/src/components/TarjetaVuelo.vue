<template>
  <div class="card">
    <div class="card-content">
      <div class="card-main">
        <div class="left-section">

          <div class="stops-info">
            <span v-if="flight.stops === 0">Directo</span>
            <span v-else-if="flight.stops === 1">1 escala</span>
            <span v-else>{{ flight.stops }} escalas</span>
          </div>

          <div class="segments">
            <div
              v-for="(segment, index) in flight.segments"
              :key="index"
              class="segment"
            >
              <div class="segment-header">
                <strong>Vuelo #{{ segment.flightNumber }}</strong>
              </div>

              <div class="segment-body">
                <div class="segment-left">
                  <div class="airports">
                    <span class="airport-code">
                      {{ segment.departureAirport.code }}
                    </span>

                    <span class="arrow">→</span>

                    <span class="airport-code">
                      {{ segment.arrivalAirport.code }}
                    </span>
                  </div>

                  <div class="times-section">
                    <div class="time-item">
                      <span class="label">Salida:</span>
                      <span class="time">{{ segment.departureTime }}</span>
                      <span class="date">
                        {{ formatDate(segment.departureDate) }}
                      </span>
                    </div>

                    <div class="time-item">
                      <span class="label">Llegada:</span>
                      <span class="time">{{ segment.arrivalTime }}</span>
                      <span class="date">
                        {{ formatDate(segment.arrivalDate) }}
                      </span>
                    </div>
                  </div>
                </div>

                <div class="prices">
                  <span class="price tourist-price">
                    Turista: {{ formatCurrency(flight.touristPrice) }}
                  </span>

                  <span class="price first-class-price">
                    Primera clase: {{ formatCurrency(flight.firstClassPrice) }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="card-actions">
            <button
              type="button"
              class="buy-button tourist-button"
              @click="buyFlight('Tourist', flight.touristPrice)"
            >
              Comprar turista
            </button>

            <button
              type="button"
              class="buy-button first-class-button"
              @click="buyFlight('FirstClass', flight.firstClassPrice)"
            >
              Comprar primera clase
            </button>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    flight: Object,
    departureDate: String
  },
  emits: ['buy'],
  methods: {
    buyFlight(seatClass, price) {
      this.$emit('buy', {
        flight: this.flight,
        seatClass,
        price
      });
    },

    formatDate(dateStr) {
      if (!dateStr) return '';

      const date = new Date(dateStr);

      return date.toLocaleDateString('es-ES', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(value);
    }
  }
};
</script>

<style scoped>
.card {
  background: white;
  padding: 20px;
  margin: 15px 0;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #032056;
  transition: all 0.3s ease;
  position: relative;
}

.card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.card-content {
  display: flex;
  flex-direction: column;
}

.card-main {
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;
  width: 100%;
}

.left-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  flex: 1;
  width: 100%;
}

.flight-number {
  display: flex;
  align-items: center;
  gap: 8px;
}

.flight-number .label {
  font-size: 14px;
  font-weight: 600;
  color: #666;
}

.flight-number .value {
  font-size: 16px;
  font-weight: 700;
  color: #032056;
}

.stops-info {
  font-size: 14px;
  font-weight: 600;
  color: #2e7d32;
}

.segments {
  display: flex;
  flex-direction: column;
  gap: 16px;
  width: 100%;
}

.segment {
  width: 100%;
  padding-top: 12px;
  border-top: 1px solid #e0e0e0;
}

.segment-header {
  margin-bottom: 10px;
  font-size: 14px;
  color: #032056;
}

.segment-body {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;
  width: 100%;
}

.segment-left {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.airports {
  display: flex;
  align-items: center;
  gap: 12px;
}

.airport-code {
  font-size: 24px;
  font-weight: 700;
  color: #032056;
}

.arrow {
  font-size: 24px;
  font-weight: bold;
  color: #032056;
}

.times-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.time-item {
  display: flex;
  flex-direction: column;
}

.time-item .label {
  font-size: 12px;
  font-weight: 600;
  color: #666;
}

.time-item .time {
  font-size: 14px;
  font-weight: 600;
  color: #032056;
}

.time-item .date {
  font-size: 12px;
  color: #666;
  margin-top: 2px;
  display: block;
}

.prices {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 8px;
  margin-left: auto;
  color: #2e7d32;
  min-width: 180px;
}

.price {
  font-size: 15px;
  font-weight: 700;
  color: #2e7d32;
  white-space: nowrap;
}

.card-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  margin-top: 8px;
  flex-wrap: wrap;
}

.buy-button {
  border: none;
  border-radius: 8px;
  background: #032056;
  color: white;
  cursor: pointer;
  font-size: 14px;
  font-weight: 700;
  padding: 10px 18px;
  transition: background 0.2s ease, transform 0.2s ease;
}

.buy-button:hover {
  transform: translateY(-1px);
}

.tourist-button {
  background: #032056;
}

.tourist-button:hover {
  background: #06357e;
}

.first-class-button {
  background: #2e7d32;
}

.first-class-button:hover {
  background: #256829;
}
</style>
