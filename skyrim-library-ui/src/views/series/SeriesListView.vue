<template>
  <h1 class="display-6">Books Series</h1>
  <div v-if="isLoading">Loading...</div>
  <div v-else class="list-group">
    <router-link
      v-for="series in seriesList"
      :key="series.id"
      :to="'/series/' + series.id"
      class="list-group-item list-group-item-action"
    >
      <div class="d-flex w-100 justify-content-between">
        <div>{{ series.name }}</div>
        <small>{{ series.booksCount }} tomes</small>
      </div>
    </router-link>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSeriesListStore } from '@/stores/seriesList'
import type { SeriesListItem } from '@/types/seriesTypes'

export default defineComponent({
  data() {
    return {
      isLoading: false,
      error: '',
      listStore: useSeriesListStore()
    }
  },
  computed: {
    seriesList(): SeriesListItem[] {
      return this.listStore.seriesList
    }
  },
  methods: {
    async loadList(): Promise<void> {
      this.isLoading = true
      await this.listStore.loadList()
      this.isLoading = false
    }
  },
  created() {
    this.loadList()
  }
})
</script>
