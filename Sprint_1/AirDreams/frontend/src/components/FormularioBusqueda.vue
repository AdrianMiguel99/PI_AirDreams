<template>
  <div class="form-container">
    <div class="form-content">
      <div class="form-line">
        <div class="input-group">
          <label>Origen</label>
          <input 
            v-model="searchForm.origin"
            type="text"
            :class="['input-field', { 'input-error': errors.origin }]"
            @blur="validateField('origin')"
          />
          <span v-if="errors.origin" class="error-message">Espacio requerido</span>
        </div>
        <div class="input-group">
          <label>Destino</label>
          <input 
            v-model="searchForm.destination"
            type="text"
            :class="['input-field', { 'input-error': errors.destination }]"
            @blur="validateField('destination')"
          />
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
      errors: {
        origin: false,
        destination: false,
        departureDate: false,
        passengers: false
      }
    };
  },
  methods: {
    validateField(field) {
      const value = this.searchForm[field];
      if (field === 'passengers') {
        this.errors[field] = !value || value < 1;
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
