<template>
  <div class="form-container">
    <div class="trip-type">
      <button 
        :class="['trip-btn', { active: tripType === 'roundTrip' }]"
        @click="tripType = 'roundTrip'"
      >
        Ida y vuelta
      </button>
      <button 
        :class="['trip-btn', { active: tripType === 'oneWay' }]"
        @click="tripType = 'oneWay'"
      >
        Solo ida
      </button>
    </div>

    <div v-show="tripType === 'roundTrip'" class="form-content">
      <div class="form-line">
        <div class="input-group">
          <label>Origen</label>
          <input 
            v-model="roundTrip.origin"
            type="text"
            :class="['input-field', { 'input-error': errors.roundTrip.origin }]"
            @blur="validateField('roundTrip', 'origin')"
          />
          <span v-if="errors.roundTrip.origin" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Destino</label>
          <input 
            v-model="roundTrip.destination"
            type="text"
            :class="['input-field', { 'input-error': errors.roundTrip.destination }]"
            @blur="validateField('roundTrip', 'destination')"
          />
          <span v-if="errors.roundTrip.destination" class="error-message">Espacio requerido</span>
        </div>
      </div>

      <div class="form-line">
        <div class="input-group">
          <label>Fecha de salida</label>
          <input 
            v-model="roundTrip.departureDate"
            type="date"
            :class="['input-field', { 'input-error': errors.roundTrip.departureDate }]"
            @blur="validateField('roundTrip', 'departureDate')"
          />
          <span v-if="errors.roundTrip.departureDate" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Fecha de regreso</label>
          <input 
            v-model="roundTrip.returnDate"
            type="date"
            :class="['input-field', { 'input-error': errors.roundTrip.returnDate }]"
            @blur="validateField('roundTrip', 'returnDate')"
          />
          <span v-if="errors.roundTrip.returnDate" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Pasajeros</label>
          <input 
            v-model="roundTrip.passengers"
            type="number"
            min="1"
            :class="['input-field', { 'input-error': errors.roundTrip.passengers }]"
            @blur="validateField('roundTrip', 'passengers')"
          />
          <span v-if="errors.roundTrip.passengers" class="error-message">Espacio requerido</span>
        </div>
      </div>
    </div>

    <div v-show="tripType === 'oneWay'" class="form-content">
      <div class="form-line">
        <div class="input-group">
          <label>Origen</label>
          <input 
            v-model="oneWay.origin"
            type="text"
            :class="['input-field', { 'input-error': errors.oneWay.origin }]"
            @blur="validateField('oneWay', 'origin')"
          />
          <span v-if="errors.oneWay.origin" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Destino</label>
          <input 
            v-model="oneWay.destination"
            type="text"
            :class="['input-field', { 'input-error': errors.oneWay.destination }]"
            @blur="validateField('oneWay', 'destination')"
          />
          <span v-if="errors.oneWay.destination" class="error-message">Espacio requerido</span>
        </div>
      </div>

      <div class="form-line">
        <div class="input-group">
          <label>Fecha de salida</label>
          <input 
            v-model="oneWay.departureDate"
            type="date"
            :class="['input-field', { 'input-error': errors.oneWay.departureDate }]"
            @blur="validateField('oneWay', 'departureDate')"
          />
          <span v-if="errors.oneWay.departureDate" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Pasajeros</label>
          <input 
            v-model="oneWay.passengers"
            type="number"
            min="1"
            :class="['input-field', { 'input-error': errors.oneWay.passengers }]"
            @blur="validateField('oneWay', 'passengers')"
          />
          <span v-if="errors.oneWay.passengers" class="error-message">Espacio requerido</span>
        </div>
      </div>
    </div>

    <button class="search-btn" @click="searchFlights">
      Buscar Vuelos
    </button>
  </div>
</template>

<script>
export default {
  data() {
    return {
      tripType: 'roundTrip',
      roundTrip: {
        origin: '',
        destination: '',
        departureDate: '',
        returnDate: '',
        passengers: 1
      },
      oneWay: {
        origin: '',
        destination: '',
        departureDate: '',
        passengers: 1
      },
      errors: {
        roundTrip: {
          origin: false,
          destination: false,
          departureDate: false,
          returnDate: false,
          passengers: false
        },
        oneWay: {
          origin: false,
          destination: false,
          departureDate: false,
          passengers: false
        }
      }
    };
  },
  methods: {
    validateField(tripType, field) {
      const value = this[tripType][field];
      if (field === 'passengers') {
        this.errors[tripType][field] = !value || value < 1;
      } else {
        this.errors[tripType][field] = !value || value.toString().trim() === '';
      }
    },
    validateForm() {
      const currentTripType = this.tripType;
      const fields = currentTripType === 'roundTrip' 
        ? ['origin', 'destination', 'departureDate', 'returnDate', 'passengers']
        : ['origin', 'destination', 'departureDate', 'passengers'];
      
      let isValid = true;
      fields.forEach(field => {
        this.validateField(currentTripType, field);
        if (this.errors[currentTripType][field]) {
          isValid = false;
        }
      });
      return isValid;
    },
    formatDateWithTime(date, timeType) {
      return `${date}T${timeType === 'start' ? '00:00' : '23:59'}`;
    },
    async searchFlights() {
  if (!this.validateForm()) {
    return;
  }

  const data = this.tripType === 'roundTrip' ? this.roundTrip : this.oneWay;

  try {
    if (this.tripType === 'oneWay') {
      const params = new URLSearchParams({
        origin: data.origin,
        destination: data.destination,
        departureDate: this.formatDateWithTime(data.departureDate, 'start'),
        returnDate: this.formatDateWithTime(data.departureDate, 'end'),
        passengers: data.passengers
      });

      const response = await fetch(`http://localhost:5276/api/flights/search?${params.toString()}`);
      const result = await response.json();

      if (!response.ok) {
        alert(result.description || 'Error al buscar vuelos');
        return;
      }

      this.$emit('search', {
        type: 'oneWay',
        outboundFlights: result.flights || [],
        returnFlights: [],
        departureDate: data.departureDate
      });

      return;
    }

    const outboundParams = new URLSearchParams({
      origin: data.origin,
      destination: data.destination,
      departureDate: this.formatDateWithTime(data.departureDate, 'start'),
      returnDate: this.formatDateWithTime(data.departureDate, 'end'),
      passengers: data.passengers
    });

    const returnParams = new URLSearchParams({
      origin: data.destination,
      destination: data.origin,
      departureDate: this.formatDateWithTime(data.returnDate, 'start'),
      returnDate: this.formatDateWithTime(data.returnDate, 'end'),
      passengers: data.passengers
    });

    const outboundResponse = await fetch(`http://localhost:5276/api/flights/search?${outboundParams.toString()}`);
    const returnResponse = await fetch(`http://localhost:5276/api/flights/search?${returnParams.toString()}`);

    const outboundResult = await outboundResponse.json();
    const returnResult = await returnResponse.json();

    if (!outboundResponse.ok) {
      alert(outboundResult.description || 'Error al buscar vuelos de ida');
      return;
    }

    if (!returnResponse.ok) {
      alert(returnResult.description || 'Error al buscar vuelos de regreso');
      return;
    }

    this.$emit('search', {
      type: 'roundTrip',
      outboundFlights: outboundResult.flights || [],
      returnFlights: returnResult.flights || [],
      departureDate: data.departureDate,
      returnDate: data.returnDate
    });

  } catch (error) {
    console.error('Error:', error);
    alert('Error de conexión al buscar vuelos');
  }
  }
  }
};
</script>

<style scoped>
.form-container {
  width: 80%;
  margin: auto;
  margin-top: 20px;
  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.2);
}

.trip-type {
  display: flex;
  gap: 15px;
  margin-bottom: 25px;
  justify-content: center;
}

.trip-btn {
  padding: 12px 25px;
  border-radius: 25px;
  border: none;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: #e0e0e0;
  color: #032056;
}

.trip-btn.active {
  background-color: white;
  color: #032056;
  box-shadow: 0 2px 8px rgba(3, 32, 86, 0.15);
}

.form-content {
  margin-bottom: 20px;
}

.form-line {
  display: flex;
  gap: 15px;
  margin-bottom: 20px;
}

.input-group {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.input-group label {
  font-size: 16px;
  font-weight: 700;
  color: #032056;
  margin-bottom: 5px;
  text-align: left;
}

.error-message {
  color: #e74c3c;
  font-size: 12px;
  margin-top: 3px;
  display: block;
}

.input-field {
  padding: 12px 15px;
  border-radius: 8px;
  border: 1px solid #d0d0d0;
  font-size: 14px;
  background: white;
  color: #333;
  transition: all 0.3s ease;
}

.input-field.input-error {
  border-color: #e74c3c;
  background-color: #ffe6e6;
}

.input-field:focus {
  outline: none;
  background: white;
  border-color: #032056;
  box-shadow: 0 0 5px rgba(3, 32, 86, 0.1);
}

.input-field.input-error:focus {
  border-color: #e74c3c;
  box-shadow: 0 0 5px rgba(231, 76, 60, 0.3);
}

.input-field::placeholder {
  color: #999;
}

.search-btn {
  width: 100%;
  padding: 15px 30px;
  border-radius: 8px;
  border: none;
  background: #032056;
  color: white;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.search-btn:hover {
  background: #1a4d8f;
  box-shadow: 0 4px 12px rgba(3, 32, 86, 0.2);
}

.search-btn:active {
  transform: translateY(1px);
}
</style>