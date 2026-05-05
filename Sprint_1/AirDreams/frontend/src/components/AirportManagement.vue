<template>
  <div class="container">
    <AdminHeader />
    <div class="page">
      <div class="content">
        <h1>Gestión de Aeropuertos</h1>
        <p class="subtitle">Registra nuevos aeropuertos y consulta los existentes.</p>

        <section class="form-section">
          <h2>Registrar Aeropuerto</h2>
          <form @submit.prevent="registrarAeropuerto" class="airport-form">
            <div class="row">
              <div class="col-md-4 mb-3">
                <label class="form-label letra_semibold">Código (3 mayúsculas)</label>
                <input
                  type="text"
                  v-model="form.code"
                  maxlength="3"
                  class="form-control"
                  placeholder="Ejm: SJO"
                  required
                  @input="form.code = form.code.toUpperCase()"
                />
              </div>
              <div class="col-md-8 mb-3">
                <label class="form-label letra_semibold">Nombre del Aeropuerto</label>
                <input
                  type="text"
                  v-model="form.name"
                  maxlength="200"
                  class="form-control"
                  placeholder="Ejm: Aeropuerto Internacional Juan Santamaría"
                  required
                />
              </div>
            </div>

            <div class="row">
              <div class="col-md-4 mb-3">
                <label class="form-label letra_semibold">País</label>
                <select v-model="form.country" @change="cargarCiudades" class="form-select" required>
                  <option disabled value="">Seleccione un país</option>
                  <option v-for="p in paises" :key="p" :value="p">{{ p }}</option>
                </select>
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label letra_semibold">Ciudad</label>
                <select v-model="form.city" class="form-select" required :disabled="!form.country">
                  <option disabled value="">Seleccione una ciudad</option>
                  <option v-for="c in ciudades" :key="c" :value="c">{{ c }}</option>
                </select>
              </div>
              <div class="col-md-4 mb-3">
                <label class="form-label letra_semibold">Zona Horaria (UTC)</label>
                <input
                  type="text"
                  v-model="form.timeZone"
                  class="form-control"
                  placeholder="Ejm: -06:00:00"
                />
              </div>
            </div>

            <div class="text-end">
              <button type="submit" class="btn colores_invertidos">Registrar Aeropuerto</button>
            </div>
            <div v-if="mensaje" :class="['alert mt-3', tipoMensaje]">{{ mensaje }}</div>
          </form>
        </section>

        <!-- Tabla de aeropuertos -->
        <section class="list-section mt-5">
          <h2>Aeropuertos Registrados</h2>
          <div v-if="cargando" class="text-center">Cargando...</div>
          <table v-else class="table table-striped table-hover">
            <thead>
              <tr>
                <th>Código</th>
                <th>Nombre</th>
                <th>Ciudad</th>
                <th>País</th>
                <th>Zona Horaria</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="a in aeropuertos" :key="a.code">
                <td>{{ a.code }}</td>
                <td>{{ a.name }}</td>
                <td>{{ a.city }}</td>
                <td>{{ a.country }}</td>
                <td>{{ a.timeZone || '-' }}</td>
              </tr>
              <tr v-if="aeropuertos.length === 0">
                <td colspan="5" class="text-center">No hay aeropuertos registrados.</td>
              </tr>
            </tbody>
          </table>
          <button class="btn boton_listar mt-3" @click="obtenerAeropuertos">Actualizar lista</button>
        </section>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import AdminHeader from './AdminHeader.vue'

//const API_BASE = import.meta.env.VITE_API_URL

export default {
  name: 'AirportManagement',
  components: {
    AdminHeader
  },
  data() {
    return {
      paises: [],
      ciudades: [],
      aeropuertos: [],
      cargando: false,
      mensaje: '',
      tipoMensaje: 'alert-success',
      form: {
        code: '',
        name: '',
        country: '',
        city: '',
        timeZone: ''
      }
    }
  },
  methods: {
    async cargarPaises() {
      try {
        const res = await axios.get(`http://localhost:5276/api/locations/countries`)
        this.paises = res.data
      } catch (e) {
        console.error('Error al cargar países:', e)
      }
    },
    async cargarCiudades() {
      if (!this.form.country) {
        this.ciudades = []
        return
      }
      try {
        const res = await axios.get(`http://localhost:5276/api/locations/cities`, {
          params: { country: this.form.country }
        })
        this.ciudades = res.data
        this.form.city = ''
      } catch (e) {
        console.error('Error al cargar ciudades:', e)
      }
    },
    async obtenerAeropuertos() {
      this.cargando = true
      try {
        const res = await axios.get(`http://localhost:5276/api/airports`, {
          headers: { 'Admin-ID': '1' } 
        })
        this.aeropuertos = res.data
      } catch (e) {
        console.error('Error al obtener aeropuertos:', e)
      } finally {
        this.cargando = false
      }
    },
    async registrarAeropuerto() {
      this.mensaje = ''
      // Validación extra
      if (!/^[A-Z]{3}$/.test(this.form.code)) {
        this.mensaje = 'El código debe ser exactamente 3 letras mayúsculas.'
        this.tipoMensaje = 'alert-danger'
        return
      }
      try {
        const res = await axios.post(`http://localhost:5276/api/airports`, this.form, {
          headers: {
            'Content-Type': 'application/json',
            'Admin-ID': '1'
          }
        })
        this.mensaje = `Aeropuerto ${res.data.code} registrado con éxito.`
        this.tipoMensaje = 'alert-success'
        // Limpiar formulario
        this.form.code = ''
        this.form.name = ''
        this.form.country = ''
        this.form.city = ''
        this.form.timeZone = ''
        this.ciudades = []
        this.obtenerAeropuertos() 
      } catch (e) {
        const errMsg = e.response?.data?.error || 'Error al registrar.'
        this.mensaje = errMsg
        this.tipoMensaje = 'alert-danger'
      }
    }
  },
  created() {
    this.cargarPaises()
    this.obtenerAeropuertos()
  }
}
</script>

<style scoped>
/* Los mismos estilos que ya tenías */
.letra_bold {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
}
.letra_semibold {
  font-family: 'Inter', sans-serif;
  font-weight: 600;
  color: #384467;
}
.colores_invertidos {
  background-color: #384467;
  color: white;
  border: none;
  font-weight: 600;
}
.colores_invertidos:hover {
  background-color: #2d3756;
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
.page {
  flex: 1;
  background-color: #F4F5F9;
}
.content {
  padding: 40px;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}
h1, h2 {
  color: #032056;
}
.subtitle {
  color: #6b7280;
  margin-bottom: 32px;
}
.form-section {
  background: white;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}
.list-section {
  background: white;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}
</style>