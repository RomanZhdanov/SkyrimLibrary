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
      <RouterLink :to="detailsLink(book.id)">{{ book.title }}</RouterLink>
    </li>
  </ul>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useBookSearchStore } from '@/stores/bookSearch'
import type { BookItem } from '@/types/bookTypes'

export default defineComponent({
  data() {
    return {
      searchInput: '',
      message: '',
      searchStore: useBookSearchStore()
    }
  },
  computed: {
    results(): BookItem[] {
      return this.searchStore.results
    }
  },
  watch: {
    searchInput() {
      if (this.searchInput.length > 2) {
        this.searchBooks()
      } else {
        this.searchStore.searchInput = ''
        this.searchStore.results = {} as BookItem[]
      }
    }
  },
  methods: {
    async searchBooks() {
      try {
        await this.searchStore.searchBooks(this.searchInput)
        this.message = ''
      } catch (error: unknown) {
        this.message = error as string
      }
    },
    detailsLink(id: string) {
      return '/books/' + id
    }
  },
  created() {
    this.searchInput = this.searchStore.searchInput
  }
})
</script>
