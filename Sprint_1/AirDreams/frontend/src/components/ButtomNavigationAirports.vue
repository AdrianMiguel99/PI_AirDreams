<template>
  <div class="navigation-container">
    <button
      :class="['nav-btn', { active: currentRoute === 'list' }]"
      @click="$router.push('/admin/airports/list')"
    >
      Lista de Aeropuertos
    </button>

    <button
      :class="['nav-btn', { active: currentRoute === 'register' }]"
      @click="$router.push('/admin/airports/register')"
    >
      Registro de Aeropuertos
    </button>

    <button
      class="nav-btn"
      @click="$router.push('/admin')"
    >
      Regresar a Menu
    </button>
  </div>
</template>

<script>
export default {
  name: 'ButtomNavigationAirports',
  props: {
    currentView: {
      type: String,
      default: 'list' 
    }
  },
  computed: {
    currentRoute() {
      if (this.$route.path.includes('register')) return 'register'
      return 'list'
    }
  }
}
</script>

<style scoped>
.navigation-container {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-top: 10px;
  margin-bottom: 30px;
}

.nav-btn {
  padding: 7px 50px;
  border-radius: 30px;
  border: none;
  background-color: #f5dddd;
  color: #3d4b74;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  font-family: 'Inter', sans-serif;
  transition: all 0.3s ease;
}

.nav-btn:hover {
  transform: translateY(-2px);
}

.nav-btn.active {
  background-color: white;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}
</style>