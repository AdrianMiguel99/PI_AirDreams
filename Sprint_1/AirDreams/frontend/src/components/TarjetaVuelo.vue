<template>
  <div class="card">
    <div class="card-content">
      <div class="card-main">
        <div class="left-section">
          <div class="flight-number">
            <span class="label">Vuelo:</span>
            <span class="value">#{{ flight.flightGUID }}</span>
          </div>

          <div class="times-section">
            <div class="time-item">
              <span class="time">{{ flight.route.departureTime }}</span>
              <span class="date">{{ formatDate(departureDate) }}</span>
            </div>
            <div class="time-item">
              <span class="time">{{ flight.route.arrivalTime }}</span>
              <span class="date">{{ formatDate(flight.departureDate) }}</span>
            </div>
          </div>

          <div class="flight-duration">
            <span class="label">Duración:</span>
            <span class="value">{{ flight.route.duration }}</span>
          </div>
        </div>

        <div class="right-section">
          <div class="airports">
            <span class="airport-code">{{ flight.route.departureAirport.code }}</span>
            <span class="arrow">→</span>
            <span class="airport-code">{{ flight.route.arrivalAirport.code }}</span>
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
  methods: {
    formatDate(dateStr) {
      if (!dateStr) return '';
      const date = new Date(dateStr);
      return date.toLocaleDateString('es-ES', { 
        day: '2-digit', 
        month: '2-digit', 
        year: 'numeric' 
      });
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
  position: relative;
}

.left-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  flex: 1;
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

.times-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.time-item {
  display: flex;
  flex-direction: column;
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

.flight-duration {
  display: flex;
  gap: 8px;
  align-items: center;
  padding-top: 8px;
  border-top: 1px solid #e0e0e0;
}

.flight-duration .label {
  font-size: 14px;
  font-weight: 600;
  color: #666;
}

.flight-duration .value {
  font-size: 14px;
  font-weight: 600;
  color: #032056;
}

.right-section {
  position: absolute;
  top: 0;
  right: 0;
  display: flex;
  align-items: center;
  justify-content: flex-end;
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
</style>