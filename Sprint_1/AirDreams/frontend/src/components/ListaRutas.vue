<template>
  <div class="routes-list-container">
    <div class="header-section">
      <h2>Rutas registradas</h2>
    </div>

    <div v-if="loading" class="state-message">
      Cargando rutas...
    </div>

    <div v-else-if="errorMessage" class="state-message error">
      {{ errorMessage }}
    </div>

    <div v-else-if="routes.length === 0" class="state-message">
      No hay rutas registradas.
    </div>

    <div v-else class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>ID Ruta</th>
            <th>Aeropuerto salida</th>
            <th>Aeropuerto llegada</th>
            <th>Duración</th>
            <th>Precio base</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="route in routes" :key="route.routeID">
            <td>{{ route.routeID }}</td>
            <td>{{ route.codeAirportSalida }}</td>
            <td>{{ route.codeAirportLlegada }}</td>
            <td>{{ route.flightDuration }} min</td>
            <td>${{ Number(route.basePrice).toFixed(2) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>


<script>
import axios from 'axios'
import AdminHeader from './AdminHeader.vue'

export default {
    name: 'ListaRutas',
    components: {
    AdminHeader
    },
data() {
    return {
    routes: [],
    loading: false,
    errorMessage: ''
    }
},
    methods: {
        irARegistro(){
            this.$router.push({ name: 'flightsRegister' });
        },
async fetchRoutes() {
    this.loading = true
    this.errorMessage = ''
    const token = localStorage.getItem("token")
        if (!token) {
            this.errorMessage = 'Debes iniciar sesión.'
            this.loading = false
            return
        }
        try {
        // usa la URL relativa para aprovechar proxy Vite si lo tienes: '/api/routes'
        const res = await axios.get('http://localhost:5276/api/routes', {
            headers: { Authorization: `Bearer ${token}` }
        })
        const raw = res.data || []

        console.log('RAW routes:', res.data);
        console.log('Primer route:', res.data?.[0]);
        console.log('Departure airport:', res.data?.[0]?.departureAirport);

        // Normalizar cada ruta a los campos que usa la UI
        this.routes = raw.map(r => {
        const routeID =  r.id || null
        const codeSalida = r.departureAirport.code + ' - ' + r.departureAirport.name  || ''
        const codeLlegada = r.arrivalAirport.code + ' - ' + r.arrivalAirport.name || ''
        // tomar duración desde stimatedTime (puede venir como "hh:mm:ss" o TimeSpan)
        let flightDuration = ''
        const st = r.stimatedTime || r.StimatedTime || r.duration || r.Duracion || ''
        if (st && typeof st === 'string') {
            const parts = st.split(':')
            if (parts.length >= 2) {
            const minutes = Number(parts[0]) * 60 + Number(parts[1])
            flightDuration = minutes
            } else {
            flightDuration = st
            }
        }
        const basePrice = r.turistClassPrice || r.touristPrice || r.basePrice || r.BasePrice || r.firstClassPrice || 0

        return {
            routeID,
            codeAirportSalida: codeSalida,
            codeAirportLlegada: codeLlegada,
            flightDuration,
            basePrice
        }
        })
        } catch (error) {
            console.error('Error al obtener rutas:', error)
            if (error.response?.status === 401 || error.response?.status === 403) {
            this.errorMessage = 'No tienes permisos o sesión expirada.'
            } else {
            this.errorMessage = 'No se pudieron cargar las rutas.'
            }
        } finally {
        this.loading = false
    }
}
    },
    created() {
    this.fetchRoutes()
    }
}
</script>

<style scoped>
.routes-list-container {
  padding: 10px 20px;
  width: 100%;
  max-width: 1150px;
  margin: 0 auto;
}

.header-section {
  margin-bottom: 20px;
}

.header-section h2 {
  margin: 0;
  font-family: 'Inter', sans-serif;
  font-size: 32px;
  font-weight: bold;
  color: #384467;
}

.subtitle {
  margin-top: 6px;
  margin-bottom: 0;
  font-family: 'Inter', sans-serif;
  color: #6b7280;
  font-size: 15px;
}

.state-message {
  text-align: center;
  color: #384467;
  padding: 20px;
  font-size: 16px;
  font-family: 'Inter', sans-serif;
}

.state-message.error {
  color: #b42318;
}

.table-wrapper {
  overflow-x: auto;
  background: #fff;
  border-radius: 16px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.10);
}

table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-size: 14px;
}

th,
td {
  text-align: left;
  padding: 10px 14px;
  border-bottom: 1px solid #f1f5f9;
}

th {
  background: #f8fafc;
  color: #384467;
  font-weight: bold;
}
</style>