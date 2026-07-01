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
            <th>Acciones</th>
          </tr>
        </thead>

        <tbody>
          <tr v-for="route in routes" :key="route.routeID">
            <td>{{ route.routeID }}</td>
            <td>{{ route.codeAirportSalida }}</td>
            <td>{{ route.codeAirportLlegada }}</td>
            <td>{{ route.flightDuration }} min</td>
            <td>${{ Number(route.basePrice).toFixed(2) }}</td>
            <td>

            
              <button
                class="delete-button"
                
                :disabled="deletingRouteId === route.routeID"
                @click="confirmarEliminacion(route)"
              >
              <img 
                src="https://i.ibb.co/FkMhvPdS/Chat-GPT-Image-5-may-2026-05-11-58-1.png"
                alt="eliminar"
                style="width: 20px; height: 22px;"
                >
                {{ deletingRouteId === route.routeID ? 'Eliminando...' : 'Eliminar' }}
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <PopupMessage
      :show="showConfirmPopup"
      type="warning"
      title="¿Eliminar ruta?"
      :message="deleteRouteMessage"
      actionText="Eliminar"
      @close="showConfirmPopup = false"
      @action="eliminarRuta"
    />

    <PopupMessage
      :show="showResultPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      @close="showResultPopup = false"
    />
  </div>
</template>


<script>
import axios from 'axios'
import AdminHeader from './AdminHeader.vue'
import PopupMessage from './PopupMessage.vue'
const API_BASE = import.meta.env.VITE_API_URL;
export default {
    name: 'ListaRutas',
    components: {
    AdminHeader,
    PopupMessage
    },
data() {
    return {
	    routes: [],
	    loading: false,
	    errorMessage: '',
	    deletingRouteId: null,
	    showConfirmPopup: false,
	    routeToDelete: null,
	    showResultPopup: false,
	    popupType: 'success',
	    popupTitle: '',
	    popupMessage: ''
    }
},
    computed: {
        deleteRouteMessage() {
            if (!this.routeToDelete) {
                return '¿Estás seguro de que deseas eliminar esta ruta? Esta acción no se puede deshacer.'
            }

            return `¿Estás seguro de que deseas eliminar la ruta ${this.routeToDelete.codeAirportSalida} a ${this.routeToDelete.codeAirportLlegada}? Esta acción no se puede deshacer.`
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
        const res = await axios.get(`${API_BASE}/api/routes`, {
            headers: { Authorization: `Bearer ${token}` }
        })
        const raw = res.data || []

        console.log('RAW routes:', res.data);
        console.log('Primer route:', res.data?.[0]);
        console.log('Departure airport:', res.data?.[0]?.departureAirport);

        // Normalizar cada ruta a los campos que usa la UI
        this.routes = raw.map(r => {
        const routeID =  r.id || null
        const departure = r.departureAirport || {}
        const arrival = r.arrivalAirport || {}
        const codeSalida = `${departure.code || ''} - ${departure.name || ''}`
        const codeLlegada = `${arrival.code || ''} - ${arrival.name || ''}`
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
            codeAirportSalida: `${departure.code || ''} - ${departure.name || ''}`,
            codeAirportLlegada: `${arrival.code || ''} - ${arrival.name || ''}`,
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
,
    confirmarEliminacion(route) {
        if (!route?.routeID) {
            this.showRoutePopup('error', 'Ruta inválida', 'No se pudo identificar la ruta seleccionada.')
            return
        }

        this.routeToDelete = route
        this.showConfirmPopup = true
    },
	async eliminarRuta() {
        this.showConfirmPopup = false

	    if (!this.routeToDelete?.routeID) {
	        this.showRoutePopup('error', 'Ruta inválida', 'No se pudo identificar la ruta seleccionada.')
	        return
	    }

	    const token = localStorage.getItem("token")
	    if (!token) {
	        this.showRoutePopup('error', 'Sesión requerida', 'Debes iniciar sesión.')
	        return
	    }
	
	    this.deletingRouteId = this.routeToDelete.routeID
	
	    try {
	        const res = await axios.delete(`${API_BASE}/api/routes/${this.routeToDelete.routeID}`, {
	            headers: { Authorization: `Bearer ${token}` }
	        })

        const message = res.data?.message || 'La ruta fue eliminada correctamente.'

        this.showRoutePopup('success', 'Ruta eliminada', message)
        await this.fetchRoutes()
    } catch (error) {
        console.error('Error al eliminar ruta:', error)
        const message = error.response?.data?.message || 'No se pudo eliminar la ruta.'
        this.showRoutePopup('error', 'Error al eliminar ruta', message)
	    } finally {
	        this.deletingRouteId = null
            this.routeToDelete = null
	    }
	},
	showRoutePopup(type, title, message) {
	    this.popupType = type
	    this.popupTitle = title
	    this.popupMessage = message
	    this.showResultPopup = true
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

.delete-button {
  border: none;
  border-radius: 999px;
  background: #ff0000;
  color: #fff;
  padding: 7px 16px;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  cursor: pointer;
  transition: background 0.2s ease, opacity 0.2s ease;
}

.delete-button:hover:not(:disabled) {
  background: #b91c1c;
}

.delete-button:disabled {
  cursor: not-allowed;
  opacity: 0.65;
}
</style>
