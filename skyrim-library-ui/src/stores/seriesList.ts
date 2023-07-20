import { defineStore } from 'pinia'
import * as api from '@/api'
import type { SeriesListItem } from '@/types/seriesTypes'

export const useSeriesListStore = defineStore('seriesList', {
  state: () => ({
    seriesList: [] as SeriesListItem[]
  }),
  actions: {
    async loadList() {
      try {
        const { data } = await api.getSeriesList()
        this.seriesList = data
      } catch (err: unknown) {
        throw new Error(err as string)
      }
    }
  }
})
