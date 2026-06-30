<template>
  <HeaderLogo />
  <div class="container">
    
    <div class="page">
      <div class="content">
        <!-- Barra de navegación unificada -->
        <ButtomNavigationAirports currentView="register" />
        <div class="d-flex justify-content-between align-items-center mb-5">
          <h1 class="title">Registrar Aeropuerto</h1>
        </div>
        <section class="form-section">
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
          </form>
        </section>
      </div>
    </div>

    <!-- Popup de éxito / error -->
    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="goToListFromPopup"
    />
  </div>
</template>

<script>
import axios from 'axios'
import HeaderLogo from '../components/HeaderLogo.vue'
import PopupMessage from './PopupMessage.vue'
import ButtomNavigationAirports from './ButtomNavigationAirports.vue'

const API_BASE = import.meta.env.VITE_API_URL

export default {
  name: 'AirportRegister',
  components: { HeaderLogo, PopupMessage, ButtomNavigationAirports },
  data() {
    return {
      paises: [],
      ciudades: [],
      form: {
        code: '',
        name: '',
        country: '',
        city: '',
        timeZone: ''
      },
      // Popup
      showPopup: false,
      popupType: 'success',
      popupTitle: '',
      popupMessage: '',
      popupActionText: ''
    }
  },
  methods: {
    getToken() {
      return localStorage.getItem('token')
    },
    async cargarPaises() {
      try {
        const res = await axios.get(`${API_BASE}/api/locations/countries`)
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
        const res = await axios.get(`${API_BASE}/api/locations/cities`, {
          params: { country: this.form.country }
        })
        this.ciudades = res.data
        this.form.city = ''
      } catch (e) {
        console.error('Error al cargar ciudades:', e)
      }
    },
    showSuccessPopup() {
      this.popupType = 'success'
      this.popupTitle = 'Aeropuerto registrado exitosamente'
      this.popupMessage = `El aeropuerto ${this.form.code || ''} fue creado correctamente.`
      this.popupActionText = 'Ver lista'
      this.showPopup = true
    },
    showErrorPopup(message) {
      this.popupType = 'error'
      this.popupTitle = 'Error al registrar aeropuerto'
      this.popupMessage = message
      this.popupActionText = ''
      this.showPopup = true
    },
    goToListFromPopup() {
      this.showPopup = false
      this.$router.push('/admin/airports/list')
    },
    async registrarAeropuerto() {
      // Validación local
      if (!/^[A-Z]{3}$/.test(this.form.code)) {
        this.showErrorPopup('El código debe ser exactamente 3 letras mayúsculas.')
        return
      }
      try {
        const token = this.getToken()
        if (!token) {
          this.showErrorPopup('Debes iniciar sesión.')
          return
        }
        const res = await axios.post(`${API_BASE}/api/airports`, this.form, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          }
        })
        // Éxito: popup y limpiar formulario
        this.showSuccessPopup()
        this.form.code = ''
        this.form.name = ''
        this.form.country = ''
        this.form.city = ''
        this.form.timeZone = ''
        this.ciudades = []
      } catch (e) {
        if (e.response?.status === 401) {
          this.showErrorPopup('Debes iniciar sesión.')
        } else if (e.response?.status === 403) {
          this.showErrorPopup('No tienes permisos de administrador.')
        } else {
          const errMsg = e.response?.data?.error || 'Error al registrar.'
          this.showErrorPopup(errMsg)
        }
      }
    }
  },
  created() {
    this.cargarPaises()
  }
}
</script>

<style scoped>
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
.page {
  flex: 1;
}
.content {
  padding: 40px;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}

.title {
  text-align: left;
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
  font-size: 40px;
  margin: 0;
}
.subtitle {
  color: #6b7280;
  margin-bottom: 32px;
}
.form-section {
  background: white;
  padding: 30px;
  border-radius: 20px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.10);
}
</style>