import { createRouter, createWebHistory } from 'vue-router'
import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'
import CompleteRegistration from '../views/CompleteRegistration.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: LandingPage
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/registro',
    name: 'Registro',
    component: RegistroUsuarios
  },
  {
    path: '/completar-registro',
    name: 'CompleteRegistration',
    component: CompleteRegistration
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router