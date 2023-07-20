<template>
  <div v-if="book">
    <div class="text-center">
      <h1>{{ book.title }}</h1>
      <p v-if="book.author">by {{ book.author }}</p>
    </div>
    <div v-html="book.text" />
  </div>
  <div v-else="error">{{ error }}</div>
  <div v-else>Loading...</div>
</template>

<script lang="ts">
import * as api from '@/api'
import { defineComponent } from 'vue'
import type { Book } from '@/types/bookTypes'
import type { AxiosError } from 'axios'

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      book: {} as Book,
      error: ''
    }
  },
  methods: {
    async loadBook() {
      try {
        const { data } = await api.get(this.id)
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
  created() {
    this.loadBook()
  }
})
</script>
