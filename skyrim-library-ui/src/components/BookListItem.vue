<template>
  <div class="card mb-3">
    <div class="card-body">
      <div class="d-flex">
        <div class="flex-shrink-0">
          <img :src="coverUrl" alt="cover" style="width: 60px" />
        </div>
        <div class="flex-grow-1 ms-3">
          <h5 class="card-title">{{ title }}</h5>
          <p class="card-text">
            {{ description }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    description: {
      type: String,
      required: true
    },
    coverUrl: {
      type: String,
      required: true
    }
  }
})
</script>
