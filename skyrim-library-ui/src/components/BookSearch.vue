<template>
  <div class="mb-4">
    <input
      v-model="searchInput"
      class="form-control form-control-lg"
      type="text"
      placeholder="search book by title"
    />
  </div>
  <div v-if="message">{{ message }}</div>
  <div v-if="books">
    <p>Found {{ booksCount }} books</p>
    <!-- <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4 g-3"> -->
    <div class="col" v-for="book in books" :key="book.id">
      <RouterLink :to="detailsLink(book.id)" class="book-card">
        <BookListItem
          :id="book.id"
          :title="book.title"
          :description="book.description"
          :cover-url="book.coverImage"
        />
      </RouterLink>
    </div>
    <!-- </div> -->
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useBookSearchStore } from '@/stores/bookSearch'
import type { BookItem, BookSearchResult } from '@/types/bookTypes'
import BookListItem from '@/components/BookListItem.vue'

export default defineComponent({
  components: {
    BookListItem
  },
  data() {
    return {
      searchInput: '',
      message: '',
      searchStore: useBookSearchStore()
    }
  },
  computed: {
    books(): BookItem[] {
      return this.searchStore.result.items
    },
    booksCount(): number {
      return this.searchStore.result.itemsCount
    }
  },
  watch: {
    searchInput() {
      if (this.searchInput.length > 0) {
        this.searchBooks()
      } else {
        this.searchStore.searchInput = ''
        this.searchStore.result = {} as BookSearchResult
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

<style scoped>
.book-card {
  text-decoration: none;
}
</style>
