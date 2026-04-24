<template>
  <div class="form-container">

    <div class="trip-type">
      <button class="active">Ida y vuelta</button>
      <button>Solo ida</button>
    </div>

    <div class="inputs">
      <input placeholder="Origen" />
      <input placeholder="Destino" />
      <input type="date" />
      <input type="date" />
      <input type="number" placeholder="Pasajeros" />
    </div>

    <button class="search-btn">Buscar</button>

  </div>
</template>

<style>
.form-container {
  width: 80%;
  margin: auto;
  margin-top: 20px;
  background: white;
  padding: 25px;
  border-radius: 15px;
  box-shadow: 0px 5px 20px rgba(0,0,0,0.2);
  text-align: center;
}

.trip-type button {
  margin: 5px;
  padding: 8px 15px;
  border-radius: 20px;
  border: none;
}

.trip-type .active {
  background: #2f3e5c;
  color: white;
}

.inputs {
  display: flex;
  gap: 10px;
  margin-top: 15px;
}

.inputs input {
  flex: 1;
  padding: 10px;
  border-radius: 8px;
  border: none;
  background: #2f3e5c;
  color: white;
}

.search-btn {
  margin-top: 20px;
  padding: 10px 30px;
  border-radius: 20px;
  border: none;
  background: #2f3e5c;
  color: white;
}
</style>