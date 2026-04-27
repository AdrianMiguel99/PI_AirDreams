<template>
  <div class="card">
    <div class="top">
      <h3>{{ vuelo.numero }}</h3>
      <span>{{ vuelo.conexion }}</span>
    </div>

    <div class="middle">
      <div>
        <p>{{ vuelo.origen }}</p>
        <small>{{ vuelo.salida }}</small>
      </div>

      <div>✈</div>

      <div>
        <p>{{ vuelo.destino }}</p>
        <small>{{ vuelo.llegada }}</small>
      </div>
    </div>

    <div class="bottom">
      <p>Duración: {{ vuelo.duracion }}</p>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    vuelo: Object
  }
};
</script>

<style>
.card {
  background: white;
  padding: 20px;
  margin: 15px;
  border-radius: 15px;
  box-shadow: 0px 3px 10px rgba(0,0,0,0.1);
}

.top {
  display: flex;
  justify-content: space-between;
}

.middle {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 15px;
}

.bottom {
  margin-top: 10px;
}
</style>