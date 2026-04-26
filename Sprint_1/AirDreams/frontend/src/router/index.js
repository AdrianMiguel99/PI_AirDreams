import { createRouter, createWebHistory } from 'vue-router'
import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'
import RegistroUsuarios from '../views/RegistroUsuarios.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login },
  { path: '/registro', component: RegistroUsuarios }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
