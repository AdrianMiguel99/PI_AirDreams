<template>
  <div class="pagination">
    <button 
      v-for="pagina in totalPaginas" 
      :key="pagina"
      @click="$emit('cambiarPagina', pagina)"
    >
      {{ pagina }}
    </button>
  </div>
</template>

<script>
export default {
  props: {
    total: Number,
    porPagina: Number
  },

  computed: {
    totalPaginas() {
      return Math.ceil(this.total / this.porPagina);
    }
  }
};
</script>

<style>
.pagination {
  text-align: center;
  margin: 20px;
}

.pagination button {
  margin: 5px;
  padding: 8px 12px;
  border-radius: 8px;
  border: none;
  background: #2f3e5c;
  color: white;
}
</style>