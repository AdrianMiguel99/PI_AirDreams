import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import './style.css'
import FlightsRegister from './components/FlightsRegister.vue'
import App from './App.vue'

const routes = [
    {
        path: '/flights',
        component: FlightsRegister
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

createApp(App).mount('#app')
