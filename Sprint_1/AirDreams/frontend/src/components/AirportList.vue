<template>
  <HeaderLogo />
  <div class="container">
    <ButtomNavigationAirports currentView="list" />
    <div class="d-flex justify-content-between align-items-center mb-5">
      <h1 class="title">Aeropuertos Registrados</h1>
    </div>

    <div v-if="loading" class="state-message">Cargando aeropuertos...</div>
    <div v-else-if="errorMessage" class="state-message error">{{ errorMessage }}</div>

    <div v-else-if="filteredAirports.length === 0" class="state-message">
      No hay aeropuertos registrados.
    </div>

    <div v-else class="table-wrapper">
      <table>
        <thead>
          <tr>
            <th>Código</th>
            <th>Nombre</th>
            <th>Ciudad</th>
            <th>País</th>
            <th>Zona horaria</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="airport in filteredAirports" :key="airport.code">
            <td>{{ airport.code }}</td>
            <td>{{ airport.name }}</td>
            <td>{{ airport.city }}</td>
            <td>{{ airport.country }}</td>
            <td>{{ airport.timeZone || '-' }}</td>
            <td>
              <a href="javascript:void(0)" class="btn-editar" @click="editar(airport.code)">
                <img src="https://i.postimg.cc/FKTG53Hn/Chat-GPT-Image-5-may-2026-05-10-07-(1).png" alt="editar" />
                <span>Editar</span>
              </a>

              <button class="btn-eliminar" @click="confirmarEliminacion(airport.code)">
                <img src="https://i.ibb.co/FkMhvPdS/Chat-GPT-Image-5-may-2026-05-11-58-1.png" alt="eliminar" />
                Eliminar
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <PopupMessage
      :show="showConfirmPopup"
      type="warning"
      title="¿Eliminar aeropuerto?"
      :message="`¿Estás seguro de que deseas eliminar el aeropuerto ${airportToDelete}? Esta acción no se puede deshacer.`"
      actionText="Eliminar"
      @close="showConfirmPopup = false"
      @action="eliminarAeropuerto"
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
import ButtomNavigationAirports from './ButtomNavigationAirports.vue'
import HeaderLogo from '../components/HeaderLogo.vue'
import PopupMessage from '../components/PopupMessage.vue'

const API_BASE = import.meta.env.VITE_API_URL

export default {
  name: 'AirportList',
  components: { ButtomNavigationAirports, HeaderLogo, PopupMessage },
  data() {
    return {
      airports: [],
      loading: false,
      errorMessage: '',
      showConfirmPopup: false,
      airportToDelete: '',
      showResultPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: ''
    }
  },
  computed: {
    filteredAirports() {
      return this.airports.filter(a => a.isActive !== false);
    }
  },
  methods: {
    async fetchAirports() {
      this.loading = true
      this.errorMessage = ''
      try {
        const res = await axios.get(`${API_BASE}/api/airports`)
        this.airports = res.data
      } catch (error) {
        console.error('Error al obtener aeropuertos:', error)
        this.errorMessage = 'No se pudieron cargar los aeropuertos.'
      } finally {
        this.loading = false
      }
    },
    editar(code) {
      this.$router.push(`/admin/airports/edit/${code}`)
    },
    confirmarEliminacion(code) {
      this.airportToDelete = code
      this.showConfirmPopup = true
    },
    async eliminarAeropuerto() {
      this.showConfirmPopup = false
      try {
        const token = localStorage.getItem('token')
        if (!token) {
          this.showResultPopup = true
          this.popupType = 'error'
          this.popupTitle = 'No autorizado'
          this.popupMessage = 'Debes iniciar sesión como administrador.'
          return
        }
        const res = await axios.delete(`${API_BASE}/api/airports/${this.airportToDelete}`, {
          headers: { Authorization: `Bearer ${token}` }
        })
        this.showResultPopup = true
        this.popupType = 'success'
        this.popupTitle = 'Aeropuerto eliminado'
        this.popupMessage = res.data.message || 'Operación completada.'
        this.fetchAirports()
      } catch (error) {
        console.error('Error al eliminar:', error)
        this.showResultPopup = true
        this.popupType = 'error'
        this.popupTitle = 'Error'
        this.popupMessage = error.response?.data?.error || error.response?.data?.message || 'Error al eliminar el aeropuerto.'
      }
    }
  },
  created() {
    this.fetchAirports()
  }
}
</script>

<style scoped>
.container {
  padding: 20px;
}
.header-section {
  margin-bottom: 24px;
}
.header-section h2 {
  margin: 0;
  font-size: 28px;
  color: #333;
}
.state-message {
  text-align: center;
  color: #666;
  padding: 20px;
  font-size: 16px;
}
.state-message.error {
  color: #b42318;
}
.table-wrapper {
  overflow-x: auto;
  background: #fff;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  text-align: left;
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
}
th {
  background: #f8fafc;
  color: #334155;
  font-weight: 600;
}
.btn-editar {
  background-color: #3E4B78;
  color: white;
  border: none;
  border-radius: 999px;
  padding: 6px 16px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 400;
  transition: 0.3s;
  cursor: pointer;
}
.btn-editar:hover { filter: brightness(1.1); }
.btn-editar img { width: 22px; height: 22px; }

.btn-eliminar {
  background-color: #ff0000;
  color: white;
  border: none;
  border-radius: 999px;
  padding: 6px 16px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-family: 'Inter', sans-serif;
  font-size: 14px;
  font-weight: 400;
  transition: 0.3s;
  cursor: pointer;
  margin-left: 10px;
}
.btn-eliminar:hover { filter: brightness(1.1); }
.btn-eliminar img { width: 20px; height: 22px; }
.title {
  text-align: left;
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 40px;
  margin: 0;
}
</style>