<template>
  <div class="card mb-3 shadow">
    <div class="card-body">
      <div class="d-flex">
        <div class="flex-shrink-0">
          <img :src="coverUrl" alt="cover" style="width: 60px" />
        </div>
        <div class="flex-grow-1 ms-3">
          <h5 class="card-title" v-html="title"></h5>
          <p class="card-text text-secondary" v-html="description"></p>
          <p class="card-text" v-if="snippets" v-html="snippets"></p>
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
    snippets: {
      type: String,
      required: false
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
