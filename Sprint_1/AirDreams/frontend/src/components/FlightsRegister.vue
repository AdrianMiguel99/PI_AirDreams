<template>
    <div class="container">
        <AdminHeader />
        <div class="page">
            <div class="content">
        <h1 class="page-title mb-2">Gestión de Vuelos</h1>
        <p class="page-subtitle mb-4">
            Registra nuevos vuelos y consulta los vuelos existentes.
        </p>

        <div class="card flight-card p-4 shadow-sm mb-5">
        <h3 class="section-title mb-3">Registrar Vuelo</h3>

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
                Fecha y hora de salida
            </label>
            <input
                v-model="formData.departureTime"
                type="datetime-local"
                id="departureTime"
                class="form-control"
                required
            />
            </div>

            <div class="col-md-6 mb-3">
            <label for="arrivalTime" class="form-label">
                Fecha y hora de llegada
            </label>
            <input
                v-model="formData.arrivalTime"
                type="datetime-local"
                id="arrivalTime"
                class="form-control"
                required
            />
            </div>

            <div class="col-md-4 mb-3">
            <label for="basePriceTurist" class="form-label">
                Precio clase turista
            </label>
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
            </div>

            <div class="col-md-4 mb-3">
                <label for="basePriceFirstClass" class="form-label">
                Precio primera clase
                </label>
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
            </div>

            <div class="col-md-4 mb-3">
            <label for="status" class="form-label">
                Estado
            </label>
            <select
                v-model="formData.status"
                id="status"
                class="form-control"
                required
            >
                <option value="" disabled>Seleccione un estado</option>
                <option
                    v-for="option in StatusOptions"
                    :key="option.value"
                    :value="option.value"
                >
                    {{ option.label }}
                </option>
            </select>
            </div>

            <div class="col-md-6 mb-3">
            <label for="maxWeightLuggage" class="form-label">
                Peso máximo de equipaje, kg
            </label>
            <input
                v-model="formData.maxWeightLuggage"
                type="number"
                id="maxWeightLuggage"
                min="0"
                max="9999"
                class="form-control"
                required
            />
            </div>

            <div class="col-md-6 mb-3">
            <label for="priceLuggage" class="form-label">
                Precio por equipaje adicional
            </label>
            <input
                v-model="formData.priceLuggage"
                type="number"
                id="priceLuggage"
                min="0"
                step="0.01"
                class="form-control"
                required
            />
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
                type="Time"
                id="flightDuration"
                min="00:00"
                step="0.01"
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
                <label for="aircraftModel" class="form-label"> Aeronave </label>

                <input
                v-model="aircraftQuery"
                type="text"
                id="aircraftModel"
                @focus="showAircraftResults = true"
                @input="onAircraftInput"
                placeholder="Ej: TI-BFJ o Boeing 737"
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
                    {{ aircraft.plateNumber }} - {{ aircraft.modelo }}
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
    import AdminHeader from "./AdminHeader.vue";

    export default {
    components: { AdminHeader },
    name: "FlightRegister",
    data() {
        return {
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
            maxWeightLuggage: 0,
            priceLuggage: 0,
            status: "",
                frequency: []
        },
        StatusOptions: [
            { label: "A tiempo", value: "On-Time" },
            { label: "Abordando", value: "Boarding" },
            { label: "Retrasado", value: "Delayed" },
            { label: "Cancelado", value: "Cancelled" },
            { label: "En vuelo", value: "In-Flight" },
            { label: "Aterrizó", value: "Landed" }
        ],
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
        filteredAircrafts(){
            return this.filterAircrafts(this.aircraftQuery);
        }
    },
    mounted(){
        this.loadAircrafts();
        this.loadAirports();
    },
    methods: {
        getToken() {
            return localStorage.getItem("token");
        },

        async loadAirports(){
            try {
                const token = localStorage.getItem("token");
                const res = await axios.get('/api/airports', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            // adapter según la forma del DTO que devuelva el backend
            this.airports = res.data.map((a, i) => ({ id: i+1, code: a.code || a.Code || a.CodeAirport, name: a.name || a.Name || a.NameAirport }));
            } catch (e) {
            console.error('No se pudieron cargar aeropuertos', e);
            }
        },
        async loadAircrafts(){
            try {
            const token = localStorage.getItem("token");
            const res = await axios.get('/api/Airplane', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            // adapter según la forma del DTO que devuelva el backend
            this.aircrafts = res.data.map((a, i) => ({
                id: i+1,
                plateNumber: a.plateNumber || a.PlateNumber,
                modelo: a.aircraftModel || a.modelo || a.Model
            }));
            } catch (e) {
                console.error('No se pudieron cargar los aviones', e);
            }
        },
        filterAirports(queryValue) {
            const query = queryValue.trim().toLowerCase();
            if (!query) return [];

            return this.airports.filter(
                (airport) => {
                const code = String(airport.code || "").toLowerCase();
                const name = String(airport.name || "").toLowerCase();
                return code.includes(query) || name.includes(query);
                }
            );
        },
        filterAircrafts(){
            const query = this.aircraftQuery.trim().toLowerCase();
            if (!query) return [];
            
            return this.aircrafts.filter((aircraft) => {
                const plateNumber = String(aircraft.plateNumber || "").toLowerCase();
                const aircraftModel = String(aircraft.modelo || "").toLowerCase();
                return plateNumber.includes(query) || aircraftModel.includes(query);
                }
            );
        },
        onAircraftInput(){
            this.showAircraftResults = true;
            this.formData.aircraftModel = "";
        },
        selectAircraft(aircraft) {
            this.selectedAircraft = aircraft;
            this.aircraftQuery = `${aircraft.plateNumber} - ${aircraft.modelo}`;
            this.formData.aircraftModel = aircraft.plateNumber;
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
        async saveFlight() {
            console.log("Formulario a guardar:", this.formData);

            
            if (this.formData.originAirport === this.formData.destinationAirport) {
                alert("El aeropuerto de origen y destino no pueden ser el mismo.");
                return;
            }

            if (!this.formData.aircraftModel) {
                alert("Debe seleccionar una aeronave desde la lista.");
                return;
            }
            const dtDepart = new Date(this.formData.departureTime);
            const dtArrive = new Date(this.formData.arrivalTime);

            const extractCode = s => {
                if (!s) return '';
                return s.includes(' - ') ? s.split(' - ')[0].trim() : s.trim();
            };

            // formato HH:MM:SS
            const toTime = (date) => date.toTimeString().split(" ")[0]; 
            // YYYY-MM-DD
            const toDate = (date) => date.toISOString().split("T")[0];

            const codeSalida = extractCode(this.formData.originAirport || this.originAirportQuery);
            const codeLlegada = extractCode(this.formData.destinationAirport);

            const pad = (n) => String(n).padStart(2, "0");

            // stimatedTime: usa input o calcula diferencia
            let stimatedTime = this.formData.flightDuration;
            if (!stimatedTime) {
                const diffMs = dtArrive - dtDepart;
                const h = Math.floor(diffMs / 3600000);
                const m = Math.floor((diffMs % 3600000) / 60000);
                stimatedTime = `${pad(h)}:${pad(m)}:00`;
            } else if (stimatedTime.length === 5) {
                stimatedTime = stimatedTime + ":00";
            }

            const payload = {
                adminID: 1, // cambiar según admin real
                codeAirportSalida: codeSalida,
                codeAirportLlegada: codeLlegada,
                plateNumber: this.formData.aircraftModel || 'TEST123', // debe existir en Aircraft
                firstClassPrice: parseFloat(this.formData.basePriceFirstClass),
                turistClassPrice: parseFloat(this.formData.basePriceTurist),
                stimatedTime: stimatedTime,
                routeState: this.formData.status,
                maxWeightLuggage: parseFloat(this.formData.maxWeightLuggage),
                priceLuggage: parseFloat(this.formData.priceLuggage),
                distance: parseFloat(this.formData.flightDistance),
                Frequencies: this.formData.frequency.map(day => ({
                dayOfWeek: day,
                departureTime: toTime(dtDepart),
                estimatedArrivalTime: toTime(dtArrive),
                startingDate: toDate(dtDepart),
                endingDate: toDate(new Date(dtDepart.getFullYear(), dtDepart.getMonth()+6, dtDepart.getDate())), // 6 meses por defecto
                active: true
                }))
            };

            try {
                const token = this.getToken()
                    if (!token) {
                        this.mensaje = 'Debes iniciar sesión.'
                        this.tipoMensaje = 'alert-danger'
                        return
                    }
                
                const res = await axios.post("/api/routes", payload, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                });
                console.log("Creada ruta:", res.data);
                this.$router.push("/");
            } catch (err) {
                console.error(err);
                alert("Error registrando vuelo (ver consola).");
            }
        }
    }
};
</script>

<style scoped>
    .frequency-box {
        display: flex;
        flex-wrap: wrap;
        gap: 15px;
    }

    .boton_registrar {
        background-color: #384467;
        border-color: #384467;
    }
</style>
