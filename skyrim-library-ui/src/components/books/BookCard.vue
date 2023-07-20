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
          <div v-if="series">
            <small
              >Part of the series
              <router-link :to="'/series/' + series.id"
                ><strong>{{ series.name }}</strong></router-link
              >:</small
            >
            <ul>
              <li v-for="book in series.books" :key="book.id">
                <span v-if="book.current">{{ book.title }}</span>
                <router-link v-else :to="'/books/' + book.id">{{ book.title }}</router-link>
              </li>
            </ul>
          </div>
          <div>
            <router-link class="btn btn-primary shadow-sm" :to="readLink" role="button"
              >Read book</router-link
            >
            <router-link
              class="btn btn-primary ms-2 shadow-sm"
              :to="readLink"
              v-if="series"
              role="button"
              >Read series</router-link
            >
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { Series } from '@/types/bookTypes'

export default defineComponent({
  props: {
    id: String,
    coverSrc: String,
    title: String,
    description: String,
    author: String,
    series: Object as PropType<Series>
  },
  computed: {
    readLink() {
      return '/books/' + this.id + '/read'
    }
  }
})
</script>

<style scoped>
.img-fluid {
  width: 100%;
}
</style>
