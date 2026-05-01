<template>
<!-- Icono de aerolinea y texto de la aerolinea-->
  <div class="d-flex align-items-center">
    <img 
        src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png" class="img-fluid" alt="imagen de aerolinea" style="width:50px"
    >
    <h2 class="letra_semibold" style="font-size: 1.3rem; padding-top: 10px;">
      Air Dreams
    </h2>
  </div>

 <!-- Titulo principal y boton listar-->
  <div class="container mt-5">
    <h1 class="letra_bold text-center" style="font-size: 2.5rem;">
            Registro de Aeronave
    </h1>

    <div class="row justify-content-end letra_bold">
      <div class="col-2">
        <a href="/listPlanes">
                    <button type="button" class="btn btn-outline-secondary boton_listar">
                        Listar Aeronaves
                    </button>
        </a>
      </div>
    </div>
  </div>

  <!-- Formulario -->
  <AirplaneForm />
</template>

<script>
import AirplaneForm from '../components/AirplaneForm.vue'

export default {
  name: 'AddAirplaneView',
  components: {
    AirplaneForm
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

.boton_listar {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: 600;
  border: 2px solid #384467;
}

.boton_listar:hover {
  background-color: #384467;
  color: white;
}
</style>