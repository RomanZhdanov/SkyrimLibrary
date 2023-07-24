<template>
  <h3>{{ name }}</h3>
  <p v-if="author">
    <small>by {{ author }}</small>
  </p>
  <p>
    <i v-if="description">{{ description }}</i>
  </p>
  <router-link
    v-for="book in books"
    :key="book.id"
    :to="'/books/' + book.id"
    style="text-decoration: none"
  >
    <book-list-item
      :id="book.id"
      :title="book.title"
      :description="book.description"
      :cover-url="book.coverImage"
    />
  </router-link>
  <div>
    <router-link
      class="btn btn-primary ms-2 shadow-sm"
      :to="'/series/' + id + '/read'"
      role="button"
      >Read series</router-link
    >
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { BookSeriesItem } from '@/types/seriesTypes'
import BookListItem from '@/components/books/BookListItem.vue'

export default defineComponent({
  props: {
    id: Number,
    name: String,
    author: String,
    description: String,
    books: Array as PropType<BookSeriesItem[]>
  },
  components: {
    BookListItem
  }
})
</script>
