<template>
  <div>
    <HeaderLogo />

    <ButtomNavigationAirplanes @change-view="currentView = $event" />

    <div class="mt-4">
      <AddAirplaneView v-if="currentView === 'register'" />
      <ListAirplaneView v-if="currentView === 'list'" />
    </div>
  </div>
</template>

<script>
import HeaderLogo from '../components/HeaderLogo.vue'
import ButtomNavigationAirplanes from '../components/Airplane/ButtomNavigationAirplanes.vue'
import AddAirplaneView from './AddAirplaneView.vue'
import ListAirplaneView from './ListAirplane.vue'

export default {
  name: 'AirplaneManagement',

  components: {
    HeaderLogo,
    ButtomNavigationAirplanes,
    AddAirplaneView,
    ListAirplaneView
  },

  data() {
    return {
      currentView: 'list'
    }
  }
}
</script>