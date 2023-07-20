<template>
  <div v-if="isLoading">Loading...</div>
  <div v-if="error" class="text-danger">{{ error }}</div>
  <SeriesCard v-else :id="series.id" :name="series.name" :books="series.books" />
</template>

<script lang="ts">
import * as api from '@/api'
import type { AxiosError } from 'axios'
import { defineComponent } from 'vue'
import type { SeriesDetails } from '@/types/seriesTypes'
import SeriesCard from '@/components/series/SeriesCard.vue'

export default defineComponent({
  props: {
    id: String
  },
  data() {
    return {
      isLoading: false,
      error: '',
      series: {} as SeriesDetails
    }
  },
  components: {
    SeriesCard
  },
  methods: {
    async loadSeries() {
      console.log('are we loading?')
      this.isLoading = true
      try {
        const { data } = await api.getSeries(Number(this.id))
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
