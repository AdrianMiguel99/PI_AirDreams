<template>
  <HeaderLogo/>
  <div class="container">
    <ButtomNavigationAirports currentView="list" />
      <div class="d-flex justify-content-between align-items-center mb-5">
        <h1 class="title"> Aeropuertos Registrados </h1>
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
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="airport in airports" :key="airport.code">
            <td>{{ airport.code }}</td>
            <td>{{ airport.name }}</td>
            <td>{{ airport.city }}</td>
            <td>{{ airport.country }}</td>
            <td>{{ airport.timeZone || '-' }}</td>
            <td>
              <a
                href="javascript:void(0)"
                class="btn-editar"
                @click="editar(airport.code)"
              >
                <img
                  src="https://i.postimg.cc/FKTG53Hn/Chat-GPT-Image-5-may-2026-05-10-07-(1).png"
                  alt="editar"
                />
                <span>Editar</span>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import ButtomNavigationAirports from './ButtomNavigationAirports.vue'
import HeaderLogo from '../components/HeaderLogo.vue'

export default {
  name: 'AirportList',
  components: { ButtomNavigationAirports, HeaderLogo },
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
        const res = await axios.get('http://localhost:5276/api/airports')
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

.btn-editar:hover {
  filter: brightness(1.1);
}

.btn-editar img {
  width: 22px;
  height: 22px;
}

.title {
  text-align: left;
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 40px;
  margin: 0;
}
</style>