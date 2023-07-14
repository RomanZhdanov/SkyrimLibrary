<template>
  <p>
    Page {{ pageResult.pageNumber }} of {{ pageResult.totalPages }}, from
    {{ pageResult.totalCount }} books
  </p>
  <div v-if="pageResult">
    <div class="col" v-for="book in pageResult.items" :key="book.id">
      <RouterLink :to="detailsLink(book.id)" class="book-card">
        <BookListItem
          :id="book.id"
          :title="book.title"
          :cover-url="book.coverImage"
          :description="book.description"
        />
      </RouterLink>
    </div>

    <base-pagination
      class="mt-4"
      :currentPage="page"
      :totalPages="pageResult.totalPages"
      :hasNext="pageResult.hasNextPage"
      :hasPrevious="pageResult.hasPreviousPage"
      @page-changed="selectPage"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useBooksPageStore } from '@/stores/booksPage'
import type { BookPageResult } from '@/types/bookTypes'
import BookListItem from './BookListItem.vue'
import BasePagination from '@/components/BasePagination.vue'

export default defineComponent({
  components: {
    BookListItem,
    BasePagination
  },
  props: {
    pageSize: {
      type: Number,
      required: false,
      default: 10
    }
  },
  data() {
    return {
      isLoading: false,
      pageStore: useBooksPageStore()
    }
  },
  computed: {
    page(): number {
      return this.pageStore.page
    },
    pageResult(): BookPageResult {
      return this.pageStore.pageResult
    }
  },
  methods: {
    async loadPage(): Promise<void> {
      await this.pageStore.loadPage(this.page, this.pageSize)
    },
    selectPage(page: number) {
      this.pageStore.page = page
      this.loadPage()
    },
    detailsLink(id: string) {
      return '/books/' + id
    }
  },
  created() {
    this.loadPage()
  }
})
</script>

<style scoped>
.book-card {
  text-decoration: none;
}
</style>
