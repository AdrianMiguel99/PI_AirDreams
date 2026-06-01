<template>
  <div>
    <HeaderLogo />

    <ButtomNavigationAirplanes
      :mode="currentView"
      @change-view="currentView = $event"
    />

    <div class="mt-4">
      <AddAirplaneView
        v-if="currentView === 'register'"
        @change-view="currentView = $event"
      />
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