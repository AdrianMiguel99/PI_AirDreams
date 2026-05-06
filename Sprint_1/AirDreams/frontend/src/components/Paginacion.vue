<template>
  <div class="pagination">
    <button 
      v-for="page in totalPages" 
      :key="page"
      @click="$emit('changePage', page)"
    >
      {{ page }}
    </button>
  </div>
</template>

<script>
export default {
  props: {
    total: Number,
    perPage: Number
  },

  computed: {
    totalPages() {
      return Math.ceil(this.total / this.perPage);
    }
  }
};
</script>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 30px 0;
  gap: 8px;
}

.pagination button {
  padding: 8px 12px;
  border-radius: 8px;
  border: none;
  background: #2f3e5c;
  color: white;
  cursor: pointer;
  transition: background 0.2s;
}

.pagination button:hover {
  background: #032056;
}
</style>