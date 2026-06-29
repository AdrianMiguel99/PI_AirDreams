<template>
  <div class="navigation-container">
    <button
      :class="['nav-btn', { active: currentView === 'sales' }]"
      @click="$emit('change-view', 'sales')"
    >
      Reporte de ingresos
    </button>

    <button
      :class="['nav-btn', { active: currentView === 'flights' }]"
      @click="$emit('change-view', 'flights')"
    >
      Reporte de vuelos
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
  name: 'NavigationReports',

  props: {
    currentView: {
      type: String,
      default: 'sales'
    }
  }
}
</script>

<style scoped>
.navigation-container {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-top: 30px;
  margin-bottom: 30px;
  flex-wrap: wrap;
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
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

@media (max-width: 768px) {
  .navigation-container {
    padding: 0 20px;
  }

  .nav-btn {
    width: 100%;
    max-width: 320px;
    padding: 9px 24px;
  }
}
</style>
