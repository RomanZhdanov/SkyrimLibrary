<template>
  <BaseModal variant="secondary" :visible="visible" @on-close="$emit('onClose')">
    <div v-if="isLoading">Loading...</div>
    <div v-else-if="error">{{ error }}</div>
    <BookCard
      v-else-if="book"
      :id="book.id"
      :title="book.title"
      :description="book.description"
      :coverSrc="book.coverImage"
    />
  </BaseModal>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import * as api from '@/api'
import type { AxiosError } from 'axios'
import BaseModal from '@/components/ui/BaseModal.vue'
import BookCard from '@/components/BookCard.vue'
import type { BookDetails } from '@/types/bookTypes'

export default defineComponent({
  emits: ['onClose'],
  components: {
    BaseModal,
    BookCard
  },
  props: {
    bookId: String,
    visible: Boolean
  },
  data() {
    return {
      isLoading: false,
      error: '',
      localBookId: '',
      book: {} as BookDetails
    }
  },
  methods: {
    async loadBook() {
      this.isLoading = true
      try {
        const { data } = await api.getDetails(this.localBookId)
        this.book = data
      } catch (err: unknown) {
        const error = err as AxiosError
        if (error.response) {
          this.error = error.response.data as string
        } else {
          this.error = 'Something went wrong...'
        }
      }

      this.isLoading = false
    }
  },
  watch: {
    bookId: async function (newVal) {
      this.localBookId = newVal
      if (newVal) {
        this.loadBook()
      } else {
        this.book = {} as BookDetails
      }
    }
  }
})
</script>
