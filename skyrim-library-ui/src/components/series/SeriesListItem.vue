<template>
  <div class="card mb-3 shadow">
    <div class="card-body">
      <div class="d-flex w-100 justify-content-between">
        <div>
          <div class="fw-bold">{{ name }}</div>
          <i>{{ description }}</i>
        </div>
        <small class="text-secondary">{{ booksCount }} tomes</small>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  props: {
    name: String,
    description: String,
    booksCount: Number
  }
})
</script>
