<template>
  <h1 class="display-6">Books series</h1>
  <div v-if="isLoading">Loading...</div>
  <div v-else>
    <router-link
      v-for="series in seriesList"
      :key="series.id"
      :to="'/series/' + series.id"
      style="text-decoration: none"
    >
      <SeriesListItem
        :name="series.name"
        :description="series.description"
        :books-count="series.booksCount"
      />
    </router-link>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSeriesListStore } from '@/stores/seriesList'
import type { SeriesListItemType } from '@/types/seriesTypes'
import SeriesListItem from '@/components/series/SeriesListItem.vue'

export default defineComponent({
  data() {
    return {
      isLoading: false,
      error: '',
      listStore: useSeriesListStore()
    }
  },
  components: {
    SeriesListItem
  },
  computed: {
    seriesList(): SeriesListItemType[] {
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
