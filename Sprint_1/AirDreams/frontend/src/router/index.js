import { createRouter, createWebHistory } from 'vue-router'

import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'
import CompleteRegistration from '../views/CompleteRegistration.vue'
import LandingPageAdmin from '../components/LandingPageAdmin/LandingPageAdmin.vue'
import AddAirplaneView from '../views/AddAirplaneView.vue'
import ListAirplaneView from '../views/ListAirplane.vue'
import EditAirplane from '../views/EditAirplane.vue'
import ListaUsuarios from '../components/ListaUsuarios.vue'
import AirportManagement from '../components/AirportManagement.vue'
import FlightsRegister from '../components/FlightsRegister.vue'
import ListaRutas from '../components/ListaRutas.vue'

const routes = [
  
  {
    path: '/completar-registro',
    name: 'CompleteRegistration',
    component: CompleteRegistration
  },
  { path: '/', name: 'home', component: LandingPage },

  { path: '/login', name: 'login', component: Login },
  { path: '/registro', name: 'registro', component: RegistroUsuarios },

  { path: '/admin', name: 'admin', component: LandingPageAdmin },
  { path: '/adminHome', name: 'adminHome', component: LandingPageAdmin },

  { path: '/admin/flightsRegister', name: 'flightsRegister', component: FlightsRegister },
  { path: '/admin/routesList', name: 'routesList', component: ListaRutas },

  { path: '/admin/usuarios', name: 'adminUsuarios', component: ListaUsuarios },

  { path: '/addPlane', name: 'addPlane', component: AddAirplaneView },
  { path: '/listPlanes', name: 'listPlanes', component: ListAirplaneView },
  { path: '/editPlane/:plateNumber', name: 'editPlane', component: EditAirplane },

  { path: '/admin/airports', name: 'airports', component: AirportManagement }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router