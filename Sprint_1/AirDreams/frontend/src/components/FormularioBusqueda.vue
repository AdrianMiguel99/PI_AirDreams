<template>
  <div class="form-container">
    <div class="form-content">
      <div class="form-line">
        <div class="input-group">
          <label>Origen</label>
          <input 
            v-model="originAirportQuery"
            type="text"
            :class="['input-field', { 'input-error': errors.origin }]"
            autocomplete="off"
            @focus="showOriginResults = true"
            @input="onOriginAirportInput"
            @blur="validateField('origin')"
          />
          <ul v-if="showOriginResults && filteredOriginAirports.length > 0" class="airport-results">
            <li
              v-for="airport in filteredOriginAirports"
              :key="airport.code"
              @mousedown.prevent="selectOriginAirport(airport)"
            >
              <strong>{{ airport.code }}</strong>
              <span>{{ airport.name }}</span>
              <small>{{ airport.country }}</small>
            </li>
          </ul>
          <span v-if="errors.origin" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Destino</label>
          <input 
            v-model="destinationAirportQuery"
            type="text"
            :class="['input-field', { 'input-error': errors.destination }]"
            autocomplete="off"
            @focus="showDestinationResults = true"
            @input="onDestinationAirportInput"
            @blur="validateField('destination')"
          />
          <ul v-if="showDestinationResults && filteredDestinationAirports.length > 0" class="airport-results">
            <li
              v-for="airport in filteredDestinationAirports"
              :key="airport.code"
              @mousedown.prevent="selectDestinationAirport(airport)"
            >
              <strong>{{ airport.code }}</strong>
              <span>{{ airport.name }}</span>
              <small>{{ airport.country }}</small>
            </li>
          </ul>
          <span v-if="errors.destination" class="error-message">Espacio requerido</span>
        </div>
      </div>

      <div class="form-line">
        <div class="input-group">
          <label>Fecha de salida</label>
          <input 
            v-model="searchForm.departureDate"
            type="date"
            :class="['input-field', { 'input-error': errors.departureDate }]"
            @blur="validateField('departureDate')"
          />
          <span v-if="errors.departureDate" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Pasajeros</label>
          <input 
            v-model="searchForm.passengers"
            type="number"
            min="1"
            max="10"
            :class="['input-field', { 'input-error': errors.passengers }]"
            @blur="validateField('passengers')"
          />
          <span v-if="errors.passengers" class="error-message">Espacio requerido</span>
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
      searchForm: {
        origin: '',
        destination: '',
        departureDate: '',
        passengers: 1
      },
      originAirportQuery: '',
      destinationAirportQuery: '',
      airports: [],
      showOriginResults: false,
      showDestinationResults: false,
      errors: {
        origin: false,
        destination: false,
        departureDate: false,
        passengers: false
      }
    };
  },
  computed: {
    filteredOriginAirports() {
      return this.filterAirports(this.originAirportQuery);
    },
    filteredDestinationAirports() {
      return this.filterAirports(this.destinationAirportQuery);
    }
  },
  created() {
    this.loadAirports();
  },
  methods: {
    async loadAirports() {
      try {
        const response = await fetch('http://localhost:5276/api/airports');
        if (!response.ok) return;

        const airports = await response.json();
        this.airports = airports.map((airport) => ({
          code: airport.code || airport.codeAirport || '',
          name: airport.name || airport.nameAirport || '',
          country: airport.country || ''
        }));
      } catch (error) {
        console.error('Error cargando aeropuertos:', error);
      }
    },
    filterAirports(queryValue) {
      const query = (queryValue || '').trim().toLowerCase();
      if (!query) return [];

      return this.airports.filter((airport) => {
        const code = String(airport.code || '').toLowerCase();
        const name = String(airport.name || '').toLowerCase();
        const country = String(airport.country || '').toLowerCase();
        return code.includes(query) || name.includes(query) || country.includes(query);
      }).slice(0, 8);
    },
    onOriginAirportInput() {
      this.showOriginResults = true;
      this.searchForm.origin = this.resolveAirportCode(this.originAirportQuery);
    },
    onDestinationAirportInput() {
      this.showDestinationResults = true;
      this.searchForm.destination = this.resolveAirportCode(this.destinationAirportQuery);
    },
    selectOriginAirport(airport) {
      this.originAirportQuery = `${airport.code} - ${airport.name}`;
      this.searchForm.origin = airport.code;
      this.showOriginResults = false;
      this.validateField('origin');
    },
    selectDestinationAirport(airport) {
      this.destinationAirportQuery = `${airport.code} - ${airport.name}`;
      this.searchForm.destination = airport.code;
      this.showDestinationResults = false;
      this.validateField('destination');
    },
    resolveAirportCode(value) {
      const rawValue = (value || '').trim();
      const possibleCode = rawValue.includes(' - ')
        ? rawValue.split(' - ')[0].trim()
        : rawValue;

      const airport = this.airports.find(
        item => item.code.toLowerCase() === possibleCode.toLowerCase()
      );

      return airport ? airport.code : '';
    },
    validateField(field) {
      if (field === 'origin') {
        this.searchForm.origin = this.resolveAirportCode(this.originAirportQuery);
      }

      if (field === 'destination') {
        this.searchForm.destination = this.resolveAirportCode(this.destinationAirportQuery);
      }

      const value = this.searchForm[field];
      if (field === 'passengers') {
        this.errors[field] = !value || value < 1 || value > 10;
      } else {
        this.errors[field] = !value || value.toString().trim() === '';
      }
    },
    validateForm() {
      const fields = ['origin', 'destination', 'departureDate', 'passengers'];
      
      let isValid = true;
      fields.forEach(field => {
        this.validateField(field);
        if (this.errors[field]) {
          isValid = false;
        }
      });
      return isValid;
    },
    formatDateWithTime(date, timeType) {
      return `${date}T${timeType === 'start' ? '00:00' : '23:59'}`;
    },
    async searchFlights() {
      if (Number(this.searchForm.passengers) < 1 || Number(this.searchForm.passengers) > 10) {
        alert('El número de pasajeros debe ser entre 1 y 10');
        return;
      }
      if (!this.validateForm()) {
        return;
      }

      const data = this.searchForm;

      try {
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
          console.error('Backend response:', result);
          alert(result.detail || result.description || 'Error al buscar vuelos');
          return;
        }

        this.$emit('search', {
          flights: result.flights || [],
          departureDate: data.departureDate,
          passengers: data.passengers
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
  position: relative;
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

.airport-results {
  position: absolute;
  top: 72px;
  left: 0;
  right: 0;
  z-index: 20;
  max-height: 220px;
  margin: 0;
  padding: 0;
  overflow-y: auto;
  list-style: none;
  background: white;
  border: 1px solid #d0d0d0;
  border-radius: 8px;
  box-shadow: 0 8px 18px rgba(3, 32, 86, 0.16);
}

.airport-results li {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 4px 8px;
  padding: 10px 12px;
  color: #1f2937;
  cursor: pointer;
}

.airport-results li:hover {
  background: #f0f5ff;
}

.airport-results strong {
  color: #032056;
}

.airport-results span {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.airport-results small {
  grid-column: 2;
  color: #667085;
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
