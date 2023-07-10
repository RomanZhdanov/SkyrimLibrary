<template>
  <input
    v-model="searchInput"
    class="form-control form-control-lg"
    type="text"
    placeholder="search book by title"
  />
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

export default defineComponent({
  data() {
    return {
      searchInput: '',
      results: {} as BookItem[]
    }
  },
  watch: {
    searchInput() {
      if (this.searchInput.length > 2) {
        console.log('now we can search')
        this.searchBooks()
      } else {
        this.results = {} as BookItem[]
      }
    }
  },
  methods: {
    async searchBooks() {
      const { data } = await api.search(this.searchInput)
      this.results = data
    }
  }
})
</script>
