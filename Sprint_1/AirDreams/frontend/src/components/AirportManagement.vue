<template>
  <div class="container">
    <AdminHeader />
    <div class="page">
      <div class="content">
        <h1>Gestión de Aeropuertos</h1>
        <p class="subtitle">Registra nuevos aeropuertos y consulta los existentes.</p>

        <!-- Formulario de registro -->
        <section class="form-section">
          <h2>Registrar Aeropuerto</h2>
          <form @submit.prevent="registrarAeropuerto" class="airport-form">
            <div class="row">
              <!-- Código -->
              <div class="col-md-4 mb-3">
                <label for="codigo" class="form-label letra_semibold">Código (3 mayúsculas)</label>
                <input
                  type="text"
                  id="codigo"
                  v-model="form.code"
                  maxlength="3"
                  class="form-control"
                  placeholder="Ejm: SJO"
                  required
                  pattern="[A-Z]{3}"
                  title="Tres letras mayúsculas"
                  @input="form.code = form.code.toUpperCase()"
                />
              </div>
              <!-- Nombre -->
              <div class="col-md-8 mb-3">
                <label for="nombre" class="form-label letra_semibold">Nombre del Aeropuerto</label>
                <input
                  type="text"
                  id="nombre"
                  v-model="form.name"
                  maxlength="200"
                  class="form-control"
                  placeholder="Ejm: Aeropuerto Internacional Juan Santamaría"
                  required
                />
              </div>
            </div>

            <div class="row">
              <!-- País (dropdown) -->
              <div class="col-md-4 mb-3">
                <label for="pais" class="form-label letra_semibold">País</label>
                <select id="pais" v-model="form.country" @change="cargarCiudades" class="form-select" required>
                  <option disabled value="">Seleccione un país</option>
                  <option v-for="p in paises" :key="p" :value="p">{{ p }}</option>
                </select>
              </div>
              <!-- Ciudad (dropdown dependiente) -->
              <div class="col-md-4 mb-3">
                <label for="ciudad" class="form-label letra_semibold">Ciudad</label>
                <select id="ciudad" v-model="form.city" class="form-select" required :disabled="!form.country">
                  <option disabled value="">Seleccione una ciudad</option>
                  <option v-for="c in ciudades" :key="c" :value="c">{{ c }}</option>
                </select>
              </div>
              <!-- Zona horaria -->
              <div class="col-md-4 mb-3">
                <label for="zona" class="form-label letra_semibold">Zona Horaria (UTC)</label>
                <input
                  type="text"
                  id="zona"
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
import { ref, reactive, onMounted } from 'vue'
import AdminHeader from './AdminHeader.vue'

export default {
  components: { AdminHeader },
  setup() {
    const paises = ref([])
    const ciudades = ref([])
    const aeropuertos = ref([])
    const cargando = ref(false)
    const mensaje = ref('')
    const tipoMensaje = ref('alert-success')

    const form = reactive({
      code: '',
      name: '',
      country: '',
      city: '',
      timeZone: ''
    })

    const API_BASE = import.meta.env.VITE_API_URL

    // Cargar países al montar
    onMounted(async () => {
      await cargarPaises()
      await obtenerAeropuertos()
    })

    const cargarPaises = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/locations/countries`)
        paises.value = await res.json()
      } catch (e) {
        console.error('Error al cargar países:', e)
      }
    }

    const cargarCiudades = async () => {
      if (!form.country) {
        ciudades.value = []
        return
      }
      try {
        const res = await fetch(`${API_BASE}/api/locations/cities?country=${encodeURIComponent(form.country)}`)
        ciudades.value = await res.json()
        form.city = '' 
      } catch (e) {
        console.error('Error al cargar ciudades:', e)
      }
    }

    const obtenerAeropuertos = async () => {
      cargando.value = true
      try {
        const res = await fetch(`${API_BASE}/api/airports`, {
          headers: { 'Admin-ID': '1' } 
        })
        if (res.ok) {
          aeropuertos.value = await res.json()
        } else {
          console.error('Error al obtener aeropuertos')
        }
      } catch (e) {
        console.error('Error de conexión:', e)
      } finally {
        cargando.value = false
      }
    }

    const registrarAeropuerto = async () => {
      mensaje.value = ''
      // Validación adicional
      if (!/^[A-Z]{3}$/.test(form.code)) {
        mensaje.value = 'El código debe ser exactamente 3 letras mayúsculas.'
        tipoMensaje.value = 'alert-danger'
        return
      }
      try {
        const res = await fetch(`${API_BASE}/api/airports`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Admin-ID': '1'
          },
          body: JSON.stringify(form)
        })
        if (res.ok) {
          const data = await res.json()
          mensaje.value = `Aeropuerto ${data.code} registrado con éxito.`
          tipoMensaje.value = 'alert-success'
          // Limpiar formulario
          form.code = ''
          form.name = ''
          form.country = ''
          form.city = ''
          form.timeZone = ''
          ciudades.value = []
          obtenerAeropuertos() // actualizar lista
        } else {
          const err = await res.json()
          mensaje.value = err.error || 'Error al registrar.'
          tipoMensaje.value = 'alert-danger'
        }
      } catch (e) {
        mensaje.value = 'Error de conexión.'
        tipoMensaje.value = 'alert-danger'
      }
    }

    return {
      paises,
      ciudades,
      aeropuertos,
      cargando,
      mensaje,
      tipoMensaje,
      form,
      cargarCiudades,
      registrarAeropuerto,
      obtenerAeropuertos
    }
  }
}
</script>

<style scoped>
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