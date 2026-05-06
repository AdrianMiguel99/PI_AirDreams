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
      vuelos: [],
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
      // Aquí iría la llamada a la API de vuelos
    },

    cambiarPagina(pagina) {
      this.paginaActual = pagina;
    }
  }
};
</script>