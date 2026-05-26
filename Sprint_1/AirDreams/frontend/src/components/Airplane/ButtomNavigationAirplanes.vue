<template>
  <div class="navigation-container">
    <button
      :class="['nav-btn', { active: activeButton === 'list' }]"
      @click="selectButton('list')"
    >
      Lista de aeronaves
    </button>

    <button
      :class="['nav-btn', { active: activeButton === 'register' }]"
      @click="selectButton('register')"
    >
      Registro de Aeronave
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
  data() {
    return {
      activeButton: 'list'
    }
  },

  methods: {
    selectButton(view) {
      this.activeButton = view
      this.$emit('change-view', view)
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