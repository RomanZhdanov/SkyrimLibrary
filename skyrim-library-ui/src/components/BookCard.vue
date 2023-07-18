<template>
  <div class="card border-0">
    <div class="row g-0">
      <div class="col-md-4">
        <img :src="coverSrc" alt="cover image" class="img-fluid" />
      </div>
      <div class="col-md-8">
        <div class="card-body">
          <h4 class="card-title">{{ title }}</h4>
          <p class="card-text" v-if="author">
            <small class="text-body-secondary">by {{ author }}</small>
          </p>
          <p class="card-text">{{ description }}</p>
          <router-link class="btn btn-primary shadow-sm" :to="readLink">Read</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  props: {
    id: String,
    coverSrc: String,
    title: String,
    description: String,
    author: String
  },
  computed: {
    readLink() {
      return '/books/' + this.id + '/read'
    }
  }
})
</script>
