<template>
  <!-- Logo -->
  <div class="d-flex align-items-center">
    <img 
      src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png"
      class="img-fluid"
      alt="imagen de aerolinea"
      style="width:50px"
    >

    <h2 
      class="letra_semibold"
      style="font-size: 1.3rem; padding-top: 10px;"
    >
      Air Dreams
    </h2>
  </div>

  <div class="container mt-5">
    <div class="d-flex justify-content-between align-items-center mb-5">
      <h1 class="title">
        Aeronaves Registradas
      </h1>

      <a href="/addPlane"class="btn-volver-lista">
        Regresar
      </a>

    </div>

    <table class="table-wrapper" v-if="airplanes.length > 0">

      <thead>
        <tr>
          <th class="letra_semibold">MATRICULA</th>
          <th class="letra_semibold">MODELO</th>
          <th class="letra_semibold">CANTIDAD DE PASAJEROS</th>
          <th class="letra_semibold">PESO MAXIMO</th>
          <th class="letra_semibold">ACCIONES</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="airplane in airplanes":key="airplane.plateNumber">
          <td>{{ airplane.plateNumber }}</td>
          <td>{{ airplane.modelo }}</td>
          <td>{{ airplane.cantPasajeros }}</td>
          <td>{{ airplane.maxWeight }}</td>

          <td>
            <a :href="`/editPlane/${airplane.plateNumber}`" class="btn-editar me-4" style="background-color: #3E4B78;">
              <img src="https://i.postimg.cc/FKTG53Hn/Chat-GPT-Image-5-may-2026-05-10-07-(1).png" alt="editar">
              <span>Editar</span>
            </a>

            <button 
              @click="deleteAirplane(airplane.plateNumber)"
              class="btn-editar"
              style="background-color: #ff0000;"
            >
              <img 
                src="https://i.ibb.co/FkMhvPdS/Chat-GPT-Image-5-may-2026-05-11-58-1.png"
                alt="eliminar"
                style="width: 20px; height: 22px;"
              >

              Eliminar
            </button>

          </td>
        </tr>
      </tbody>

    </table>

  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'ListAirplane',

  data() {
    return {
      airplanes: []
    };
  },

  methods: {

    getAirplanes() {
      axios.get('http://localhost:5276/api/Airplane')

        .then(response => {
          this.airplanes = response.data;
        })

        .catch(error => {
          console.error("ERROR:", error);
        });
    },

    deleteAirplane(plateNumber) {

      axios.delete(`http://localhost:5276/api/Airplane/${plateNumber}`)

        .then(() => {

          alert('Aeronave eliminada con éxito');

          this.getAirplanes();
        })

        .catch(error => {

          console.error("ERROR:", error.response?.data);

          alert('Error al eliminar la aeronave');
        });
    }
  },

  created() {
    this.getAirplanes();
  }
};
</script>

<style scoped>

.btn-editar {
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

.btn-editar img {
  width: 22px;
  height: 22px;
}

.letra_bold {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
}

.letra_semibold {
  font-family: 'Inter', sans-serif;
  font-weight: bold;
  color: #384467;
}

.title {
  text-align: left;

  font-family: 'Inter', sans-serif;
  color: #384467;

  font-weight: bold;
  font-size: 40px;

  margin: 0;
}

.container {
  padding: 20px;
}

.table-wrapper {
  width: 100%;

  border-radius: 20px;
  overflow: hidden;

  background: #fff;

  border: 1px solid #e5e7eb;

  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);

  color: #384467;
}

table {
  width: 100%;
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

.btn-volver-lista {

  display: inline-block;

  background-color: #384467;
  color: white;

  padding: 10px 20px;

  text-decoration: none;

  border-radius: 8px;

  font-family: 'Inter', sans-serif;
  font-weight: 500;

  transition: 0.3s;
}

.btn-volver-lista:hover {
  opacity: 0.9;
}

</style>