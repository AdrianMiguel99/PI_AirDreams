<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isUserMenuOpen = ref(false)

const toggleUserMenu = () => {
  isUserMenuOpen.value = !isUserMenuOpen.value
}

const navigateTo = (path) => {
  router.push(path)
  isUserMenuOpen.value = false
}
</script>

<template>
  <header class="headerContainer">
    <img src="../assets/logo.png" alt="Logo" class="logo" />
    
    <button class="userButton" @click="toggleUserMenu">
      Usuario
      <span class="arrow">›</span>
    </button>

    <div v-if="isUserMenuOpen" class="userMenuDropdown">
      <button @click="navigateTo('/admin')" class="menuItem">Perfil</button>
      <button @click="navigateTo('/login')" class="menuItem">Cerrar sesión</button>
    </div>
  </header>
</template>

<style scoped>
.headerContainer {
  background-color: #ffffff;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  position: relative;
  z-index: 100;
}

.logo {
  height: 40px;
  width: auto;
}

.userButton {
  background-color: transparent;
  border: none;
  color: #032056;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  padding: 8px 16px;
}

.userButton:hover {
  color: #1a4d8f;
}

.userMenuDropdown {
  position: absolute;
  top: 65px;
  right: 32px;
  background-color: white;
  border-radius: 4px;
  min-width: 150px;
  margin-top: 8px;
  z-index: 1000;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.menuItem {
  display: block;
  padding: 12px 16px;
  color: #032056;
  font-size: 14px;
  text-decoration: none;
  background: none;
  border: none;
  cursor: pointer;
  width: 100%;
  text-align: left;
}

.menuItem:hover {
  background-color: #F4F5F9;
}

.arrow {
    margin-left: 8px;
    font-size: 20px;
    color: #94a3b8;
}
</style>
