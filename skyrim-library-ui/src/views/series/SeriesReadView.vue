<template>
  <div v-if="isLoading">Loading...</div>
  <div v-else-if="error">{{ error }}</div>
  <div v-else>
    <div class="text-center">
      <h1>{{ series.name }}</h1>
      <p v-if="series.author">by {{ series.author }}</p>
      <p v-if="series.description">
        <i>{{ series.description }}</i>
      </p>
    </div>
    <div v-for="book in series.books" :key="book.id">
      <div class="text-center">
        <h3>{{ book.title }}</h3>
      </div>
      <div v-html="book.text" />
    </div>
  </div>
</template>

<script lang="ts">
import * as api from '@/api'
import { defineComponent } from 'vue'
import type { SeriesRead } from '@/types/seriesTypes'
import type { AxiosError } from 'axios'

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      series: {} as SeriesRead,
      error: '',
      isLoading: false
    }
  },
  methods: {
    async loadSeries() {
      this.isLoading = true
      try {
        const { data } = await api.getSeriesRead(Number(this.id))
        this.series = data
      } catch (err: unknown) {
        const error = err as AxiosError
        if (error.response) {
          this.error = error.response.data as string
        } else {
          this.error = 'Something went wrong...'
        }
      }
      this.isLoading = false
    }
  },
  created() {
    this.loadSeries()
  }
})
</script>
