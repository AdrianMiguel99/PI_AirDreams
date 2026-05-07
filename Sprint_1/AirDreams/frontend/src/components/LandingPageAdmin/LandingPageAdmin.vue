<script setup>
import { useRouter } from 'vue-router'
import AdminHeader from '../AdminHeader.vue'
import airplaneIcon from '../../assets/icons/airplane.svg'
import routeIcon from '../../assets/icons/route.svg'
import airportIcon from '../../assets/icons/airport.svg'
import usersIcon from '../../assets/icons/users.svg'
import reportsIcon from '../../assets/icons/reports.svg'

const router = useRouter()

const navigateTo = (path) => {
    router.push(path)
}
</script>

<template>
    <div class="container">
        <AdminHeader />
        <div class="page">
            <div class="content">
        <h1>Bienvenido a Air Dreams</h1>
        <p class="subtitle">
            Seleccione una opción para gestionar el sistema
        </p>

        <button class="back-button" type="button" @click="navigateTo('/')">
            Volver al inicio
        </button>


        <section>
            <h2>Administración de vuelos</h2>
            <div class="cards">
            <div class="card" @click="navigateTo('/addPlane')">
                <img :src="airplaneIcon" alt="Airplane" class="icon blue">
                <div>
                <h3>Tipos de aeronave</h3>
                <p>Gestiona y crea nuevas aeronaves.</p>
                </div>
                <span class="arrow">›</span>
            </div>

            <div class="card" @click="navigateTo('/admin/flightsRegister')">
                <img :src="routeIcon" alt="Route" class="icon blue">
                <div>
                <h3>Crear ruta</h3>
                <p>Gestiona y crea nuevas rutas de vuelo.</p>
                </div>
                <span class="arrow">›</span>
            </div>

            <div class="card" @click="navigateTo('/admin/airports/register')">
                <img :src="airportIcon" alt="Airport" class="icon blue">
                <div>
                <h3>Aeropuertos</h3>
                <p>Gestiona y crea nuevos aeropuertos.</p>
                </div>
                <span class="arrow">›</span>
            </div>
            </div>
        </section>

        <section>
            <h2>Administración usuarios</h2>
            <div class="cards">
            <div class="card" @click="navigateTo('/admin/Usuarios')">
                <img :src="usersIcon" alt="Users" class="icon blue">
                <div>
                <h3>Usuarios</h3>
                <p>Gestiona y crea nuevos usuarios.</p>
                </div>
                <span class="arrow">›</span>
            </div>
            </div>
        </section>

        <section>
            <h2>Reportes</h2>
            <div class="cards">
            <div class="card" @click="navigateTo('/admin')">
                <img :src="reportsIcon" alt="Reports" class="icon blue">
                <div>
                <h3>Reportes</h3>
                <p>Consulta y genera reportes del sistema.</p>
                </div>
                <span class="arrow">›</span>
            </div>
            </div>
        </section>
            </div>
        </div>
    </div>
</template>

<style scoped>

.back-button {
    border: 1px solid #032056;
    background: white;
    color: #032056;
    border-radius: 8px;
    padding: 10px 16px;
    cursor: pointer;
    font-weight: 600;
}

.back-button:hover {
    background: #e0ecff;
}

.container {
    width: 100%;
    display: flex;
    flex-direction: column;
    background-color: #F4F5F9;
}

.page {
    flex: 1;
}

.content {
    padding: 40px;
    max-width: 1200px;
    margin: 0 auto;
    width: 100%;
}

h1 {
    color: #032056;
    margin-top: none;
    margin-bottom: 8px;
}

.subtitle {
    color: #6b7280;
    margin-bottom: 32px;
}

section {
    margin-bottom: 40px;
}

h2 {
    color: #032056;
    margin-bottom: 16px;
    font-size: 18px;
}

.cards {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 18px;
}

.card {
    background: white;
    border-radius: 12px;
    padding: 20px;
    display: flex;
    align-items: center;
    gap: 16px;
    cursor: pointer;
    transition: all 0.2s;
}

.card:hover {
    transform: translateY(-2px);
}

.icon {
    width: 50px;
    height: 50px;
    border-radius: 12px;
    padding: 8px;
}

.blue { background: #e0ecff; }

.card h3 {
    margin: 0;
    color: #032056;
    font-size: 16px;
}

.card p {
    margin: 4px 0 0;
    color: #6b7280;
    font-size: 14px;
}

.arrow {
    margin-left: auto;
    font-size: 20px;
    color: #94a3b8;
}
</style>