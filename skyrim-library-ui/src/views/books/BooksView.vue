<template>
  <h1 class="display-6">All books</h1>
  <BooksPage />
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import BooksPage from '@/components/books/BooksPage.vue'

export default defineComponent({
  components: {
    BooksPage
  }
})
</script>
