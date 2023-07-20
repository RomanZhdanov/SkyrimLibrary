<template>
  <div v-if="book">
    <BookCard
      :id="book.id"
      :title="book.title"
      :author="book.author"
      :description="book.description"
      :cover-src="book.coverImage"
      :series="book.series"
    />
  </div>
  <div v-else="error">{{ error }}</div>
  <div v-else>Loading...</div>
</template>

<script lang="ts">
import * as api from '@/api'
import { defineComponent } from 'vue'
import BookCard from '@/components/BookCard.vue'
import type { BookDetails } from '@/types/bookTypes'
import type { AxiosError } from 'axios'

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  components: {
    BookCard
  },
  data() {
    return {
      book: {} as BookDetails,
      error: ''
    }
  },
  methods: {
    async loadBook() {
      try {
        const { data } = await api.getDetails(this.id)
        this.book = data
      } catch (err: unknown) {
        const error = err as AxiosError
        if (error.response) {
          this.error = error.response.data as string
        } else {
          this.error = 'Something went wrong...'
        }
      }
    }
  },
  watch: {
    id: function (newVal) {
      this.loadBook()
    }
  },
  created() {
    this.loadBook()
  }
})
</script>
