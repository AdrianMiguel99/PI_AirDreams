<template>
  <div class="container">
    <div v-if="showSuccessPopup" class="popup-overlay">
      <div class="popup-card">
        <h3>Ruta registrada exitosamente</h3>
        <p>La ruta y sus vuelos fueron creados correctamente.</p>

        <div class="popup-actions">
          <button
            type="button"
            class="btn boton_registrar"
            @click="showSuccessPopup = false"
          >
            Registrar otra ruta
          </button>

          <button
            type="button"
            class="btn boton_listar"
            @click="irALista"
          >
            Ver lista de rutas
          </button>
        </div>
      </div>
    </div>

    <div class="page">
      <div class="content">
        <h1 class="title">Registrar Ruta</h1>

        <div v-if="successMessage" class="alert alert-success">
          {{ successMessage }}
        </div>

        <div v-if="errorMessage" class="alert alert-danger">
          {{ errorMessage }}
        </div>

        <div class="card flight-card p-4 shadow-sm mb-5">
          <h3 class="d-flex section-title mb-3">Registrar Vuelo</h3>

          <form @submit.prevent="saveFlight">
            <div class="row">
              <div class="col-md-6 position-relative mb-3">
                <label for="originAirport" class="form-label">
                  Aeropuerto de origen
                </label>

                <input
                  v-model="originAirportQuery"
                  type="text"
                  id="originAirport"
                  @focus="showOriginResults = true"
                  @input="onOriginAirportInput"
                  placeholder="Ej: SJO o Juan Santamaría"
                  autocomplete="off"
                  class="form-control"
                  required
                />

                <ul
                  v-if="showOriginResults && filteredOriginAirports.length > 0"
                  class="list-group position-absolute w-100 shadow"
                  style="z-index: 1000; max-height: 200px; overflow-y: auto;"
                >
                  <li
                    v-for="airport in filteredOriginAirports"
                    :key="airport.id"
                    class="list-group-item list-group-item-action"
                    @click="selectOriginAirport(airport)"
                    style="cursor: pointer"
                  >
                    {{ airport.code }} - {{ airport.name }}
                  </li>
                </ul>
              </div>

              <div class="col-md-6 position-relative mb-3">
                <label for="destinationAirport" class="form-label">
                  Aeropuerto de destino
                </label>

                <input
                  v-model="destinationAirportQuery"
                  type="text"
                  id="destinationAirport"
                  @focus="showDestinationResults = true"
                  @input="onDestinationAirportInput"
                  placeholder="Ej: LIR o Daniel Oduber"
                  autocomplete="off"
                  class="form-control"
                  required
                />

                <ul
                  v-if="showDestinationResults && filteredDestinationAirports.length > 0"
                  class="list-group position-absolute w-100 shadow"
                  style="z-index: 1000; max-height: 200px; overflow-y: auto;"
                >
                  <li
                    v-for="airport in filteredDestinationAirports"
                    :key="airport.id"
                    class="list-group-item list-group-item-action"
                    @click="selectDestinationAirport(airport)"
                    style="cursor: pointer"
                  >
                    {{ airport.code }} - {{ airport.name }}
                  </li>
                </ul>
              </div>

              <div class="col-md-6 mb-3">
                <label for="departureTime" class="form-label">
                  Hora de salida
                </label>

                <input
                  v-model="formData.departureTime"
                  type="time"
                  id="departureTime"
                  class="form-control"
                  required
                />
              </div>

              <div class="col-md-6 mb-3">
                <label for="arrivalTime" class="form-label">
                  Hora de llegada
                </label>

                <input
                  v-model="formData.arrivalTime"
                  type="time"
                  id="arrivalTime"
                  class="form-control"
                  required
                />
              </div>

              <div class="col-md-4 mb-3">
                <label for="basePriceTurist" class="form-label">
                  Precio clase turista
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.basePriceTurist"
                    type="number"
                    id="basePriceTurist"
                    class="form-control"
                    min="0"
                    max="99999"
                    step="0.01"
                    required
                  />
                  <span class="input-group-text">$</span>
                </div>
              </div>

              <div class="col-md-4 mb-3">
                <label for="basePriceFirstClass" class="form-label">
                  Precio primera clase
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.basePriceFirstClass"
                    type="number"
                    id="basePriceFirstClass"
                    class="form-control"
                    min="0"
                    max="99999"
                    step="0.01"
                    required
                  />
                  <span class="input-group-text">$</span>
                </div>
              </div>

              <div class="col-md-6 mb-3">
                <label for="luggageMaxWeight" class="form-label">
                  Peso máximo de equipaje de cabina
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.luggageMaxWeight"
                    type="number"
                    id="luggageMaxWeight"
                    min="0"
                    max="9999"
                    class="form-control"
                    required
                  />
                  <span class="input-group-text">Kg</span>
                </div>
              </div>

              <div class="col-md-6 mb-3">
                <label for="LuggagePrice" class="form-label">
                  Precio por equipaje de cabina adicional
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.luggagePrice"
                    type="number"
                    id="LuggagePrice"
                    min="0"
                    step="0.01"
                    class="form-control"
                    required
                  />
                  <span class="input-group-text">$</span>
                </div>
              </div>

              <div class="col-md-6 mb-3">
                <label for="carryOnMaxWeight" class="form-label">
                  Peso máximo de equipaje carry-on de mano
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.carryOnMaxWeight"
                    type="number"
                    id="carryOnMaxWeight"
                    min="0"
                    max="9999"
                    class="form-control"
                    required
                  />
                  <span class="input-group-text">Kg</span>
                </div>
              </div>

              <div class="col-md-6 mb-3">
                <label for="carryOnPrice" class="form-label">
                  Precio por equipaje carry-on adicional
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.carryOnPrice"
                    type="number"
                    id="carryOnPrice"
                    min="0"
                    step="0.01"
                    class="form-control"
                    required
                  />
                  <span class="input-group-text">$</span>
                </div>
              </div>

              <div class="col-md-6 mb-3">
                <label for="porcentageMultiplier" class="form-label">
                  Porcentaje de aumento por equipaje adicional
                </label>

                <div class="input-group">
                  <input
                    v-model="formData.porcentageMultiplier"
                    type="number"
                    id="porcentageMultiplier"
                    min="0"
                    max="100"
                    step="0.01"
                    class="form-control"
                    required
                  />
                  <span class="input-group-text">%</span>
                </div>
              </div>

              <div class="col-12 mb-3">
                <label class="form-label">Frecuencia de vuelo</label>

                <div class="frequency-box">
                  <div
                    class="form-check form-check-inline"
                    v-for="day in weekDays"
                    :key="day.value"
                  >
                    <input
                      class="form-check-input"
                      type="checkbox"
                      :id="day.value"
                      :value="day.value"
                      v-model="formData.frequency"
                    />

                    <label class="form-check-label" :for="day.value">
                      {{ day.label }}
                    </label>
                  </div>
                </div>
              </div>

              <div class="col-12 mb-3">
                <label class="form-label">Duración del vuelo</label>

                <input
                  v-model="formData.flightDuration"
                  type="text"
                  id="flightDuration"
                  placeholder="HH:MM"
                  pattern="^([0-9]{1,2}):([0-5][0-9])$"
                  class="form-control"
                  required
                />
              </div>

              <div class="col-12 mb-3">
                <label class="form-label">Distancia(KM)</label>

                <input
                  v-model="formData.flightDistance"
                  type="number"
                  id="flightDistance"
                  min="0"
                  step="0.01"
                  class="form-control"
                  required
                />
              </div>

              <div class="col-12 position-relative mb-3">
                <label for="aircraftModel" class="form-label">
                  Aeronave
                </label>

                <input
                  v-model="aircraftQuery"
                  type="text"
                  id="aircraftModel"
                  @focus="showAircraftResults = true"
                  @input="onAircraftInput"
                  placeholder="Ej: Boeing 737"
                  autocomplete="off"
                  class="form-control"
                  required
                />

                <ul
                  v-if="showAircraftResults && filteredAircrafts.length > 0"
                  class="list-group position-absolute w-100 shadow"
                  style="z-index: 1000; max-height: 200px; overflow-y: auto;"
                >
                  <li
                    v-for="aircraft in filteredAircrafts"
                    :key="aircraft.id"
                    class="list-group-item list-group-item-action"
                    @click="selectAircraft(aircraft)"
                    style="cursor: pointer"
                  >
                    {{ aircraft.modelo }}
                  </li>
                </ul>
              </div>

              <div class="col-12 d-flex justify-content-end mt-2">
                <button type="submit" class="btn btn-primary boton_registrar">
                  Registrar Vuelo
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

const API_BASE = import.meta.env.VITE_API_URL;

export default {
  name: "FlightRegister",

  data() {
    return {
      successMessage: "",
      errorMessage: "",
      showSuccessPopup: false,

      aircraftQuery: "",
      showAircraftResults: false,
      selectedAircraft: null,
      aircrafts: [],

      originAirportQuery: "",
      destinationAirportQuery: "",
      showOriginResults: false,
      showDestinationResults: false,
      selectedOriginAirport: null,
      selectedDestinationAirport: null,
      airports: [],

      formData: {
        originAirport: "",
        destinationAirport: "",
        departureTime: "",
        arrivalTime: "",
        basePriceTurist: 0,
        basePriceFirstClass: 0,
        flightDuration: "",
        flightDistance: 0,
        aircraftModel: "",
        luggageMaxWeight: 0,
        luggagePrice: 0,
        carryOnPrice: 0,
        carryOnMaxWeight: 0,
        porcentageMultiplier: 0,
        frequency: []
      },

      weekDays: [
        { value: "Monday", label: "Lunes" },
        { value: "Tuesday", label: "Martes" },
        { value: "Wednesday", label: "Miércoles" },
        { value: "Thursday", label: "Jueves" },
        { value: "Friday", label: "Viernes" },
        { value: "Saturday", label: "Sábado" },
        { value: "Sunday", label: "Domingo" }
      ]
    };
  },

  computed: {
    filteredOriginAirports() {
      return this.filterAirports(this.originAirportQuery);
    },

    filteredDestinationAirports() {
      return this.filterAirports(this.destinationAirportQuery);
    },

    filteredAircrafts() {
      return this.filterAircrafts(this.aircraftQuery);
    }
  },

  mounted() {
    this.loadAircrafts();
    this.loadAirports();
  },

  methods: {
    getToken() {
      return localStorage.getItem("token");
    },

    getArrayResponse(data) {
      if (Array.isArray(data)) return data;
      if (Array.isArray(data?.data)) return data.data;
      if (Array.isArray(data?.result)) return data.result;
      if (Array.isArray(data?.items)) return data.items;
      if (Array.isArray(data?.airports)) return data.airports;
      if (Array.isArray(data?.aircrafts)) return data.aircrafts;
      if (Array.isArray(data?.planes)) return data.planes;

      return [];
    },

    async loadAirports() {
      try {
        const token = this.getToken();

        const res = await axios.get(`${API_BASE}/api/airports`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });

        console.log("Airports response:", res.data);

        const airportsRaw = this.getArrayResponse(res.data);

        this.airports = airportsRaw
          .filter(a => a.isActive !== false && a.active !== false)
          .map((a, i) => ({
            id: a.id || a.idAirport || i + 1,
            code: a.code || a.Code || a.codeAirport || a.CodeAirport || "",
            name: a.name || a.Name || a.nameAirport || a.NameAirport || ""
          }))
          .filter(a => a.code && a.name);
      } catch (e) {
        console.error("No se pudieron cargar aeropuertos", {
          status: e.response?.status,
          data: e.response?.data,
          message: e.message
        });
      }
    },

    async loadAircrafts() {
      try {
        const token = this.getToken();

        const res = await axios.get(`${API_BASE}/api/Airplane`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });

        console.log("Aircraft response:", res.data);

        const aircraftsRaw = this.getArrayResponse(res.data);

        this.aircrafts = aircraftsRaw
          .filter(a => a.isActive !== false && a.active !== false)
          .map((a, i) => ({
            id: a.id || a.idAircraft || i + 1,
            modelo:
              a.aircraftModel ||
              a.modelo ||
              a.model ||
              a.Model ||
              a.Modelo ||
              ""
          }))
          .filter(a => a.modelo);
      } catch (e) {
        console.error("No se pudieron cargar los aviones", {
          status: e.response?.status,
          data: e.response?.data,
          message: e.message
        });
      }
    },

    filterAirports(queryValue) {
      const query = String(queryValue || "").trim().toLowerCase();
      if (!query) return [];

      return this.airports.filter((airport) => {
        const code = String(airport.code || "").toLowerCase();
        const name = String(airport.name || "").toLowerCase();

        return code.includes(query) || name.includes(query);
      });
    },

    filterAircrafts(queryValue) {
      const query = String(queryValue || "").trim().toLowerCase();
      if (!query) return [];

      return this.aircrafts.filter((aircraft) => {
        const aircraftModel = String(aircraft.modelo || "").toLowerCase();

        return aircraftModel.includes(query);
      });
    },

    onAircraftInput() {
      this.showAircraftResults = true;
      this.formData.aircraftModel = "";
    },

    selectAircraft(aircraft) {
      this.selectedAircraft = aircraft;
      this.aircraftQuery = aircraft.modelo;
      this.formData.aircraftModel = aircraft.modelo;
      this.showAircraftResults = false;
    },

    onOriginAirportInput() {
      this.showOriginResults = true;
      this.formData.originAirport = "";
    },

    onDestinationAirportInput() {
      this.showDestinationResults = true;
      this.formData.destinationAirport = "";
    },

    selectOriginAirport(airport) {
      this.selectedOriginAirport = airport;
      this.originAirportQuery = `${airport.code} - ${airport.name}`;
      this.formData.originAirport = airport.code;
      this.showOriginResults = false;
    },

    selectDestinationAirport(airport) {
      this.selectedDestinationAirport = airport;
      this.destinationAirportQuery = `${airport.code} - ${airport.name}`;
      this.formData.destinationAirport = airport.code;
      this.showDestinationResults = false;
    },

    irALista() {
      this.$emit("change-view", "list");
    },

    volverAlPanel() {
      this.$router.push("/admin");
    },

    async saveFlight() {
      console.log("Formulario a guardar:", this.formData);

      this.errorMessage = "";
      this.successMessage = "";

      if (this.formData.originAirport === this.formData.destinationAirport) {
        this.errorMessage = "El aeropuerto de origen y destino no pueden ser el mismo.";
        return;
      }

      if (!this.formData.aircraftModel) {
        this.errorMessage = "Debe seleccionar una aeronave desde la lista.";
        return;
      }

      const extractCode = (value) => {
        if (!value) return "";
        return value.includes(" - ") ? value.split(" - ")[0].trim() : value.trim();
      };

      const toTime = (timeValue) => {
        if (!timeValue) return "";
        return timeValue.length === 5 ? `${timeValue}:00` : timeValue;
      };

      const toDate = (date) => date.toISOString().split("T")[0];

      const codeSalida = extractCode(
        this.formData.originAirport || this.originAirportQuery
      );

      const codeLlegada = extractCode(
        this.formData.destinationAirport || this.destinationAirportQuery
      );

      let stimatedTime = this.formData.flightDuration;

      if (stimatedTime && stimatedTime.length === 5) {
        stimatedTime = `${stimatedTime}:00`;
      }

      const routeCreationDate = new Date();

      const routeFrequencyEndDate = new Date(
        routeCreationDate.getFullYear(),
        routeCreationDate.getMonth() + 6,
        routeCreationDate.getDate()
      );

      const payload = {
        adminID: 1,
        codeAirportSalida: codeSalida,
        codeAirportLlegada: codeLlegada,
        modelo: this.formData.aircraftModel,
        firstClassPrice: parseFloat(this.formData.basePriceFirstClass),
        turistClassPrice: parseFloat(this.formData.basePriceTurist),
        stimatedTime,
        luggageMaxWeight: parseFloat(this.formData.luggageMaxWeight),
        luggagePrice: parseFloat(this.formData.luggagePrice),
        carryOnMaxWeight: parseFloat(this.formData.carryOnMaxWeight),
        carryOnPrice: parseFloat(this.formData.carryOnPrice),
        porcentageMultiplier: parseFloat(this.formData.porcentageMultiplier),
        distance: parseFloat(this.formData.flightDistance),
        Frequencies: this.formData.frequency.map(day => ({
          dayOfWeek: day,
          departureTime: toTime(this.formData.departureTime),
          estimatedArrivalTime: toTime(this.formData.arrivalTime),
          startingDate: toDate(routeCreationDate),
          endingDate: toDate(routeFrequencyEndDate),
          active: true
        }))
      };

      try {
        const token = this.getToken();

        if (!token) {
          this.errorMessage = "Debes iniciar sesión.";
          return;
        }

        const res = await axios.post(`${API_BASE}/api/routes`, payload, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
          }
        });

        console.log("Creada ruta:", res.data);

        this.successMessage = "Ruta creada exitosamente.";
        this.errorMessage = "";
        this.showSuccessPopup = true;
      } catch (err) {
        console.error("Status:", err.response?.status);
        console.error("Backend response:", err.response?.data);
        console.error("Payload enviado:", payload);

        this.errorMessage =
          err.response?.data?.message ||
          "Error registrando vuelo. Revise los datos e inténtelo de nuevo.";

        this.successMessage = "";
        this.showSuccessPopup = false;
      }
    }
  }
};
</script>

<style scoped>
.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.popup-card {
  background: white;
  width: min(420px, 90vw);
  border-radius: 12px;
  padding: 28px;
  text-align: center;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.25);
}

.popup-card h3 {
  color: #032056;
  margin-bottom: 10px;
}

.popup-card p {
  color: #5f6b7a;
  margin-bottom: 24px;
}

.popup-actions {
  display: flex;
  justify-content: center;
  gap: 12px;
  flex-wrap: wrap;
}

.boton_listar {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: 600;
  border: 2px solid #384467;
  background: transparent;
}

.boton_listar:hover {
  background-color: #384467;
  color: white;
}

.frequency-box {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
}

.boton_registrar {
  background-color: #384467;
  border-color: #384467;
  color: white;
  font-family: 'Inter', sans-serif;
  font-weight: 600;
}

.container {
  padding: 20px;
  width: 100%;
}

.content {
  width: 100%;
  max-width: 1150px;
  margin: 0 auto;
}

.title {
  text-align: left;
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 40px;
  margin-bottom: 24px;
}

.flight-card {
  border-radius: 20px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.10);
}

.section-title {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
}

.form-label {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: 600;
}

.form-control {
  font-family: 'Inter', sans-serif;
  color: #384467;
}
</style>