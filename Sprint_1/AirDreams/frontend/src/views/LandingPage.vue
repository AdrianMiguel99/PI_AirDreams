<template>
  <div>
    <BarraNavegacion />
    <SeccionHero />
    <FormularioBusqueda />
    
    <ListaVuelos 
    v-if="busquedaRealizada"
    :vuelos="vuelosPaginados" 
    />

    <Paginacion 
    v-if="busquedaRealizada"
    :total="vuelos.length" 
    :porPagina="10"
    @cambiarPagina="cambiarPagina"
    />
  </div>
</template>

<script>
import BarraNavegacion from "../components/BarraNavegacion.vue";
import SeccionHero from "../components/SeccionHero.vue";
import FormularioBusqueda from "../components/FormularioBusqueda.vue";
import ListaVuelos from "../components/ListaVuelos.vue";
import Paginacion from "../components/Paginacion.vue";

export default {
  components: {
    BarraNavegacion,
    SeccionHero,
    FormularioBusqueda,
    ListaVuelos,
    Paginacion
  },

  data() {
    return {
      paginaActual: 1,
      vuelos: [
        {
          id: 1,
          numero: "AD101",
          origen: "SJO",
          destino: "MEX",
          salida: "2026-05-01 08:00",
          llegada: "2026-05-01 12:00",
          duracion: "4h",
          conexion: "Directo"
        },
        {
          id: 2,
          numero: "AD202",
          origen: "SJO",
          destino: "USA",
          salida: "2026-05-02 10:00",
          llegada: "2026-05-02 15:00",
          duracion: "5h",
          conexion: "1 escala"
        }
      ],
      busquedaRealizada: false
    };
  },

  computed: {
    vuelosPaginados() {
      const inicio = (this.paginaActual - 1) * 10;
      return this.vuelos.slice(inicio, inicio + 10);
    }
  },

methods: {
  buscarVuelos() {
    this.busquedaRealizada = true;

    this.vuelos = [
      {
        id: 1,
        numero: "AD101",
        origen: "SJO",
        destino: "MEX",
        salida: "2026-05-01 08:00",
        llegada: "2026-05-01 12:00",
        duracion: "4h",
        conexion: "Directo"
      }
    ];
  },

  cambiarPagina(pagina) {
    this.paginaActual = pagina;
  }
}
}
</script>