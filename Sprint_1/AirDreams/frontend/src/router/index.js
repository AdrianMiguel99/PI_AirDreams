import { createRouter, createWebHistory } from 'vue-router'

import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'
import LandingPageAdmin from '../components/LandingPageAdmin/LandingPageAdmin.vue'
import AddAirplaneView from '../views/AddAirplaneView.vue'
import ListAirplaneView from '../views/ListAirplane.vue'
import EditAirplane from '../views/EditAirplane.vue'

const routes = [
  { path: '/', name: 'home', component: LandingPage },

  { path: '/login', name: 'login', component: Login },
  { path: '/registro', name: 'registro', component: RegistroUsuarios },

  { path: '/admin', name: 'admin', component: LandingPageAdmin },
  { path: '/adminHome', name: 'adminHome', component: LandingPageAdmin },

  { path: '/addPlane', name: 'addPlane', component: AddAirplaneView },
  { path: '/listPlanes', name: 'listPlanes', component: ListAirplaneView },
  { path: '/editPlane/:plateNumber', name: 'editPlane', component: EditAirplane }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router