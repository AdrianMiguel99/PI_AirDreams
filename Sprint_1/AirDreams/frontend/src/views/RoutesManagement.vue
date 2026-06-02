<template>
  <div>
    <HeaderLogo />

    <ButtomNavigationRoutes
      :currentView="currentView"
      @change-view="currentView = $event"
    />

    <div class="mt-4">
      <RegistroVuelos
        v-if="currentView === 'register'"
        @change-view="currentView = $event"
      />

      <ListaRutas
        v-if="currentView === 'list'"
      />
    </div>
  </div>
</template>

<script>
import HeaderLogo from '../components/HeaderLogo.vue'
import ButtomNavigationRoutes from '../components/ButtomNavigationRoutes.vue'
import RegistroVuelos from '../components/RegistroVuelos.vue'
import ListaRutas from '../components/ListaRutas.vue'

export default {
  name: 'RoutesManagement',

  components: {
    HeaderLogo,
    ButtomNavigationRoutes,
    RegistroVuelos,
    ListaRutas
  },

  data() {
    return {
      currentView: 'list'
    }
  }
}
</script>