import { defineStore } from 'pinia'
import * as api from '@/api'
import type { SeriesListItemType } from '@/types/seriesTypes'

export const useSeriesListStore = defineStore('seriesList', {
  state: () => ({
    seriesList: [] as SeriesListItemType[]
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
