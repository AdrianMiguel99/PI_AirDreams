<template>
  <div>
    <h2>Usuarios</h2>

    <div v-if="usuarios.length === 0">
      No hay usuarios disponibles
    </div>

    <div v-else>
      <TarjetaUsuario 
        v-for="usuario in usuarios" 
        :key="usuario.id"
        :usuario="usuario"
      />
    </div>
  </div>
</template>

<script>
import TarjetaUsuario from "./TarjetaUsuario.vue";

export default {
  props: ["usuarios"],
  components: {
    TarjetaUsuario
  }
};
</script>
