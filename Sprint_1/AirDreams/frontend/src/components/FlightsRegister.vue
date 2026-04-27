    <template>
    <div class="d-flex justify-content-center align-items-center vh-100">
        <div class="card p-4 shadow" style="max-width: 600px; width: 100%">
        <h3 class="text-center mb-3">Formulario de creación de vuelos</h3>

        <form @submit.prevent="saveFlight">
            <div class="position-relative mb-3">
            <label for="originAirport" class="form-label">Aeropuerto de origen</label>
            <input
                v-model="airportQuery"
                type="text"
                id="originAirport"
                @focus="showResults = true"
                @input="onAirportInput"
                placeholder="Escriba el código o el nombre del aeropuerto"
                autocomplete="off"
                class="form-control"
                required
            />

            <ul
                v-if="showResults && filteredAirports.length > 0"
                class="list-group position-absolute w-100 shadow"
                style="z-index: 1000; max-height: 200px; overflow-y: auto;"
            >
                <li
                v-for="airport in filteredAirports"
                :key="airport.id"
                class="list-group-item list-group-item-action"
                @click="selectAirport(airport)"
                style="cursor: pointer"
                >
                {{ airport.code }} - {{ airport.name }}
                </li>
            </ul>
            </div>

            <div class="form-group mb-3">
            <label for="destinationAirport">Aeropuerto de destino</label>
            <input
                v-model="formData.destinationAirport"
                type="text"
                id="destinationAirport"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="departureTime">Hora de salida</label>
            <input
                v-model="formData.departureTime"
                type="datetime-local"
                id="departureTime"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="arrivalTime">Hora de llegada</label>
            <input
                v-model="formData.arrivalTime"
                type="datetime-local"
                id="arrivalTime"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="basePriceTurist">Precio base, para clase turista</label>
            <input
                v-model="formData.basePriceTurist"
                type="number"
                id="basePriceTurist"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="basePriceFirstClass">Precio base, para primera clase</label>
            <input
                v-model="formData.basePriceFirstClass"
                type="number"
                id="basePriceFirstClass"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="maxWeightLuggage">Peso máximo permitido para equipaje (kg)</label>
            <input
                v-model="formData.maxWeightLuggage"
                type="number"
                id="maxWeightLuggage"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="priceLuggage">Precio por equipaje adicional</label>
            <input
                v-model="formData.priceLuggage"
                type="number"
                id="priceLuggage"
                class="form-control"
                required
            />
            </div>

            <div class="form-group mb-3">
            <label for="status">Estado</label>
            <select
                v-model="formData.status"
                id="status"
                class="form-control"
                required
            >
                <option value="" disabled>Seleccione un estado</option>
                <option>A tiempo</option>
                <option>Abordando</option>
                <option>Retrasado</option>
                <option>Cancelado</option>
                <option>En vuelo</option>
                <option>Aterrizó</option>
            </select>
            </div>

            <div class="form-group mb-3">
            <label class="form-label">Frecuencia de vuelo</label>
            <div class="form-check" v-for="day in weekDays" :key="day.value">
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

            <button type="submit" class="btn btn-success w-100">Guardar</button>
        </form>
        </div>
    </div>
    </template>

    <script>
    import axios from "axios";

    export default {
    name: "FlightRegister",
    data() {
        return {
        airportQuery: "",
        showResults: false,
        selectedAirport: null,
        airports: [
            { id: 1, code: "SJO", name: "Juan Santamaría" },
            { id: 2, code: "LIR", name: "Daniel Oduber" },
            { id: 3, code: "MAD", name: "Madrid-Barajas" },
            { id: 4, code: "JFK", name: "John F. Kennedy" }
        ],
        formData: {
            originAirport: "",
            destinationAirport: "",
            departureTime: "",
            arrivalTime: "",
            basePriceTurist: 0,
            basePriceFirstClass: 0,
            maxWeightLuggage: 0,
            priceLuggage: 0,
            status: "",
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
        filteredAirports() {
            const query = this.airportQuery.trim().toLowerCase();
            if (!query) return [];

            return this.airports.filter(
                (airport) =>
                airport.code.toLowerCase().includes(query) ||
                airport.name.toLowerCase().includes(query)
            );
        }
    },
    methods: {
        onAirportInput() {
            this.showResults = true;
            this.formData.originAirport = "";
        },
        selectAirport(airport) {
            this.selectedAirport = airport;
            this.airportQuery = `${airport.code} - ${airport.name}`;
            this.formData.originAirport = airport.code;
            this.showResults = false;
        },
        saveFlight() {
            console.log("Formulario a guardar:", this.formData);

        // axios
        //     .post("https://localhost:7019/api/Flight", this.formData)
        //     .then(() => {
        //     this.$router.push("/");
        //     })
        //     .catch((error) => {
        //     console.log(error);
        //     });
        }
    }
};
</script>