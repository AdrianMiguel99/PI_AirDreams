<template>
  <div class="container">
    <AdminHeader />

    <div class="header-section">
      <h2>Aeropuertos registrados</h2>
    </div>

    <div class="mb-3">
      <button class="btn btn-outline-secondary btn-volver" @click="volver">
        ← Volver al registro
      </button>
    </div>

    <div v-if="loading" class="state-message">Cargando aeropuertos...</div>
    <div v-else-if="errorMessage" class="state-message error">{{ errorMessage }}</div>

    <div v-else-if="airports.length === 0" class="state-message">
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
          </tr>
        </thead>
        <tbody>
          <tr v-for="airport in airports" :key="airport.code">
            <td>{{ airport.code }}</td>
            <td>{{ airport.name }}</td>
            <td>{{ airport.city }}</td>
            <td>{{ airport.country }}</td>
            <td>{{ airport.timeZone || '-' }}</td>
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
  name: 'AirportList',
  components: {
    AdminHeader
  },
  data() {
    return {
      airports: [],
      loading: false,
      errorMessage: ''
    }
  },
  methods: {
    async fetchAirports() {
      this.loading = true
      this.errorMessage = ''
      try {
        const res = await axios.get('http://localhost:5276/api/airports', {
          headers: { 'Admin-ID': '1' }
        })
        this.airports = res.data
      } catch (error) {
        console.error('Error al obtener aeropuertos:', error)
        this.errorMessage = 'No se pudieron cargar los aeropuertos.'
      } finally {
        this.loading = false
      }
    },
    volver() {
      this.$router.push('/admin/airports/register')
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

th,
td {
  text-align: left;
  padding: 12px 16px;
  border-bottom: 1px solid #f1f5f9;
}

th {
  background: #f8fafc;
  color: #334155;
  font-weight: 600;
}

.btn-volver {
  font-family: 'Inter', sans-serif;
  color: #384467;
  border-color: #384467;
  font-weight: 600;
}
.btn-volver:hover {
  background-color: #384467;
  color: white;
}
</style>