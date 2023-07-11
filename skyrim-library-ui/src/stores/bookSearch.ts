import { defineStore } from 'pinia'
import * as api from '@/api'
import type { BookItem } from '@/types/bookTypes'
import type { AxiosError } from 'axios'

export const useBookSearchStore = defineStore('bookSearch', {
  state: () => ({
    searchInput: '',
    results: {} as BookItem[]
  }),
  actions: {
    async searchBooks(input: string) {
      this.searchInput = input
      try {
        const { data } = await api.search(input)
        this.results = data
      } catch (err: unknown) {
        const error = err as AxiosError
        let message = 'There was an exception during search...'

        if (error.response) {
          message = error.response.data as string
        }

        this.results = {} as BookItem[]
        throw new Error(message)
      }
    }
  }
})
