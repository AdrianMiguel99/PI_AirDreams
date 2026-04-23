<script setup>
import { ref } from 'vue'

const isUserMenuOpen = ref(false)

const toggleUserMenu = () => {
  isUserMenuOpen.value = !isUserMenuOpen.value
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
      <a href="#" class="menuItem">Perfil</a>
      <a href="#" class="menuItem">Cerrar sesión</a>
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
}

.menuItem {
  display: block;
  padding: 12px 16px;
  color: #032056;
  font-size: 14px;
  text-decoration: none;
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
