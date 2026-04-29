<template>
  <div class="card">
    <div class="top">
      <h3>{{ user.fullName }}</h3>
      <span class="role">{{ user.role }}</span>
    </div>

    <div class="middle">
      <div class="email-section">
        <p>Email</p>
        <small>{{ user.email }}</small>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    user: Object
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
  align-items: center;
}

.role {
  background: #e3f2fd;
  padding: 5px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  color: #1976d2;
  font-weight: 500;
}

.middle {
  margin-top: 15px;
}

.email-section p {
  margin: 0;
  font-size: 0.9rem;
  color: #666;
}

.email-section small {
  display: block;
  margin-top: 5px;
  font-size: 1rem;
  color: #333;
}
</style>
