import { createRouter, createWebHistory } from 'vue-router'

import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'
import CompleteRegistration from '../views/CompleteRegistration.vue'
import LandingPageAdmin from '../components/LandingPageAdmin/LandingPageAdmin.vue'
import AirplaneForm from '../components/Airplane/AirplaneForm.vue'
import ListAirplaneView from '../views/ListAirplane.vue'
import EditAirplane from '../views/EditAirplane.vue'
import ListaUsuarios from '../components/ListaUsuarios.vue'
import AirportRegister from '../components/AirportRegister.vue'
import AirportList from '../components/AirportList.vue'
import FlightsRegister from '../components/RegistroVuelos.vue'
import ListaRutas from '../components/ListaRutas.vue'
import RegistroVuelos from '../components/RegistroVuelos.vue'
import AirplaneManagement from '../views/AirplaneManagement.vue'
import AddAirplaneView from '../views/AddAirplaneView.vue'
import AirportEdit from '../components/AirportEdit.vue'
import PurchaseSuccess from '../views/PurchaseSuccess.vue'
import LuggagePage from '../views/LuggagePage.vue'
import PassengerFormPage from '../views/PassengerFormPage.vue'
import PaymentPage from '../views/PaymentPage.vue'
import RoutesManagement from '../views/RoutesManagement.vue'

const routes = [
  
  {
    path: '/completar-registro',
    name: 'CompleteRegistration',
    component: CompleteRegistration
  },
  { path: '/', name: 'home', component: LandingPage },

  { path: '/login', name: 'login', component: Login },
  { path: '/registro', name: 'registro', component: RegistroUsuarios },

  { path: '/admin', name: 'admin', component: LandingPageAdmin, meta: { requiresAdmin: true }},
  { path: '/adminHome', name: 'adminHome', component: LandingPageAdmin, meta: { requiresAdmin: true }},

  { path: '/admin/routes', name: 'routesManagement', component: RoutesManagement },

  { path: '/admin/usuarios', name: 'adminUsuarios', component: ListaUsuarios, meta: { requiresAdmin: true }},
  { path: '/managementPlanes', name: 'managementPlanes', component: AirplaneManagement },
  { path: '/editPlane/:modelo', name: 'editPlane', component: EditAirplane },

  { path: '/admin/airports/register', name: 'airportRegister', component: AirportRegister },
  { path: '/admin/airports/list', name: 'airportList', component: AirportList },
  { path: '/admin/airports', redirect: '/admin/airports/register' },
  { path: '/admin/airports/edit/:code', name: 'airportEdit', component: AirportEdit },

  { path: '/purchased/:idCompra', name: 'PurchaseSuccess', component: PurchaseSuccess},
  { path: '/passengers', name: 'passengers', component: PassengerFormPage },
  { path: '/luggage', name: 'luggage', component: LuggagePage },
  { path: '/payment', name: 'payment', component: PaymentPage }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
