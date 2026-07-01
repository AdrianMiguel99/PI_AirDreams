<template>
  <div class="container">
    <AdminHeader />
    <div class="page">
      <div class="content">
        <h1>Editar Aeropuerto</h1>
        <p class="subtitle">Modifica el nombre del aeropuerto.</p>

        <div v-if="loading" class="state-message">Cargando datos...</div>
        <div v-else-if="errorLoad" class="state-message error">{{ errorLoad }}</div>

        <div v-else class="form-section">
          <form @submit.prevent="guardarCambios">
            <div class="mb-3">
              <label class="form-label letra_semibold">Código</label>
              <input type="text" v-model="airport.code" class="form-control" disabled />
            </div>

            <div class="mb-3">
              <label class="form-label letra_semibold">Nombre del Aeropuerto</label>
              <input type="text" v-model="airport.name" class="form-control" required maxlength="200" />
            </div>

            <div class="mb-3">
              <label class="form-label letra_semibold">Ciudad</label>
              <input type="text" v-model="airport.city" class="form-control" disabled />
            </div>

            <div class="mb-3">
              <label class="form-label letra_semibold">País</label>
              <input type="text" v-model="airport.country" class="form-control" disabled />
            </div>

            <div class="mb-3">
              <label class="form-label letra_semibold">Zona Horaria</label>
              <input type="text" v-model="airport.timeZone" class="form-control" disabled />
            </div>

            <div class="d-flex justify-content-between">
              <button type="button" class="btn btn-outline-secondary" @click="volver">
                ← Regresar a la lista
              </button>
              <button type="submit" class="btn colores_invertidos">Guardar Cambios</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <PopupMessage
      :show="showPopup"
      :type="popupType"
      :title="popupTitle"
      :message="popupMessage"
      :actionText="popupActionText"
      @close="showPopup = false"
      @action="goToList"
    />
  </div>
</template>

<script>
import axios from 'axios'
import AdminHeader from './AdminHeader.vue'         
import PopupMessage from './PopupMessage.vue' 

const API_BASE = import.meta.env.VITE_API_URL

export default {
  name: 'AirportEdit',
  components: { AdminHeader, PopupMessage },
  data() {
    return {
      airport: {
        code: '',
        name: '',
        city: '',
        country: '',
        timeZone: ''
      },
      loading: false,
      errorLoad: '',
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
    async cargarAeropuerto() {
      const code = this.$route.params.code
      if (!code) {
        this.errorLoad = 'Código de aeropuerto no proporcionado.'
        return
      }
      this.loading = true
      try {
        const res = await axios.get(`${API_BASE}/api/airports/${code}`)
        this.airport = res.data
      } catch (e) {
        console.error('Error al cargar aeropuerto:', e)
        this.errorLoad = 'No se pudo cargar la información del aeropuerto.'
      } finally {
        this.loading = false
      }
    },
    async guardarCambios() {
      const token = this.getToken()
      if (!token) {
        this.showPopup = true
        this.popupType = 'error'
        this.popupTitle = 'Error de autenticación'
        this.popupMessage = 'Debes iniciar sesión como administrador.'
        this.popupActionText = ''
        return
      }

      try {
        await axios.put(
          `${API_BASE}/api/airports/${this.airport.code}`, 
          { name: this.airport.name },
          {
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${token}`
            }
          }
        )
        this.showPopup = true
        this.popupType = 'success'
        this.popupTitle = 'Aeropuerto actualizado'
        this.popupMessage = 'El nombre del aeropuerto fue modificado correctamente.'
        this.popupActionText = 'Ir a la lista'
      } catch (e) {
        console.error('Error al actualizar:', e)
        this.showPopup = true
        this.popupType = 'error'
        this.popupTitle = 'Error al actualizar'
        if (e.response?.status === 401) {
          this.popupMessage = 'Debes iniciar sesión.'
        } else if (e.response?.status === 403) {
          this.popupMessage = 'No tienes permisos de administrador.'
        } else {
          this.popupMessage = e.response?.data?.error || 'Ocurrió un error inesperado.'
        }
        this.popupActionText = ''
      }
    },
    volver() {
      this.$router.push('/admin/airports/list')
    },
    goToList() {
      this.showPopup = false
      this.$router.push('/admin/airports/list')
    }
  },
  created() {
    this.cargarAeropuerto()
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
.state-message {
  text-align: center;
  color: #666;
  padding: 20px;
  font-size: 16px;
}
.state-message.error {
  color: #b42318;
}
</style>