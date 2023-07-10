<template>
  <input
    v-model="searchInput"
    class="form-control form-control-lg"
    type="text"
    placeholder="search book by title"
  />
  <div v-if="message">{{ message }}</div>
  <ul v-if="results">
    <li v-for="book in results" :key="book.id">
      {{ book.title }}
    </li>
  </ul>
</template>

<script lang="ts">
import * as api from '@/api'
import { defineComponent } from 'vue'
import type { BookItem } from '@/types/bookTypes'
import type { AxiosError } from 'axios'

export default defineComponent({
  data() {
    return {
      searchInput: '',
      message: '',
      results: {} as BookItem[]
    }
  },
  watch: {
    searchInput() {
      if (this.searchInput.length > 2) {
        this.searchBooks()
      } else {
        this.results = {} as BookItem[]
      }
    }
  },
  methods: {
    async searchBooks() {
      try {
        const { data } = await api.search(this.searchInput)
        this.message = ''
        this.results = data
      } catch (err: unknown) {
        const error = err as AxiosError
        if (error.response) {
          this.message = error.response.data as string
        }
        this.results = {} as BookItem[]
      }
    }
  }
})
</script>
