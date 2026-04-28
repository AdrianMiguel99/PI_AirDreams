import { createApp } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
import 'bootstrap/dist/css/bootstrap.min.css';
import './style.css';
import FlightsRegister from './components/FlightsRegister.vue';
import App from './App.vue';

const routes = [
    {
        path: '/',
        component: FlightsRegister,
    },
    {
        path: '/flights-register',
        component: FlightsRegister,
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

const app = createApp(App);
app.use(router);
app.mount('#app'); 