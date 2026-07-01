<template>
    <div class="d-flex align-items-center gap-2 w-100 position-relative">

        <div class="logoContainer" @click="$router.push('/')">
            <img 
                src="https://i.ibb.co/MxwJ1Y9m/Chat-GPT-Image-7-abr-2026-01-52-40.png"
                class="img-fluid"
                alt="imagen de aerolinea"
                style="width:50px"
            >

            <h2 class="letra_logo m-0" style="font-size: 1.3rem;">
                Air Dreams
            </h2>
        </div>
    </div>
</template>

<script>
export default {
  data() {
    return {
      isUserMenuOpen: false
    }
  },

  methods: {
    toggleUserMenu() {
      this.isUserMenuOpen = !this.isUserMenuOpen
    },

    navigateTo(route) {
      this.$router.push(route)
    },
  }
}
</script>

<style scoped>
.userButton {
  background-color: transparent;
  border: none;
  color: #032056;
  font-size: 20px;
  font-weight: 500;
  padding: 8px 16px;
}

.userMenuDropdown {
  position: absolute;
  top: 65px;
  right: 32px;
  background-color: white;
  border-radius: 4px;
  min-width: 150px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.menuItem {
  display: block;
  padding: 12px 16px;
  color: #032056;
  font-size: 14px;
  background: none;
  border: none;
}

.userButton:hover {
  color: #1a4d8f;
}

.letra_logo {
  font-family: 'Inter', sans-serif;
  color: #384467;
  font-weight: bold;
}

.logoContainer {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
</style>