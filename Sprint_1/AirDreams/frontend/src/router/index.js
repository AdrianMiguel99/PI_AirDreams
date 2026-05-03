import { createRouter, createWebHistory } from 'vue-router'

import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'
import LandingPageAdmin from '../components/LandingPageAdmin/LandingPageAdmin.vue'
import AddAirplane from '../components/AddAirplaneForm.vue'
import AirportManagement from '../components/AirportManagement.vue'
import FlightsRegister from '../components/FlightsRegister.vue'

const routes = [
  //Pagina principal
  { path: '/', name: 'home', component: LandingPage },

  //Login y registro
  { path: '/login', name: 'login', component: Login },
  { path: '/registro', name: 'registro', component: RegistroUsuarios },

  //Panel administrador
  { path: '/admin', name: 'admin', component: LandingPageAdmin },
  { path: '/adminHome', name: 'adminHome', component: LandingPageAdmin },

  { path: '/flights-register', name: 'flightsRegister', component: FlightsRegister },
  
  //Agregar y visualizar aeronaves
  { path: '/addPlane', name: 'addPlane', component: AddAirplane },

  { path: '/admin/airports', name: 'airports', component: AirportManagement }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router