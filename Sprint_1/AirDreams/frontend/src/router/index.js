import { createRouter, createWebHistory } from 'vue-router'
import LandingPage from '../views/LandingPage.vue'
import Login from '../views/Login.vue'

const routes = [
  { path: '/', component: LandingPage },
  { path: '/login', component: Login }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
