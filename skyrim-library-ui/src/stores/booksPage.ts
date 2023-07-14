import { defineStore } from 'pinia'
import * as api from '@/api'
import type { BookPageResult } from '@/types/bookTypes'

export const useBooksPageStore = defineStore('booksPage', {
  state: () => ({
    page: 1,
    pageResult: {} as BookPageResult
  }),
  actions: {
    async loadPage(page: number, pageSize: number) {
      try {
        const { data } = await api.getPage(page, pageSize)
        this.pageResult = data
      } catch (err: unknown) {
        throw new Error(err as string)
      }
    }
  }
})
