import { createApp } from 'vue'
import App from './App.vue'
import { createRouter, createWebHistory } from 'vue-router';
import LandingPageAdmin from './components/LandingPageAdmin/LandingPageAdmin.vue';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', name: "Home", component: LandingPageAdmin },
        { path: '/adminHome', name: "Admin Home", component: LandingPageAdmin },
    ]
});

createApp(App).use(router).mount('#app')