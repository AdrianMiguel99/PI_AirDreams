<template>
  <div class="hero">
    <div class="overlay">
      <h1>Air Dreams</h1>
    </div>
  </div>
</template>

<style>
.hero {
  height: 350px;
  background-image: url("/images/avion.png");
  background-size: cover;
  background-position: center;
}

.overlay {
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  color: white;
}

.overlay h1 {
  font-size: 40px;
  color:white;
}
</style>
