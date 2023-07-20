<template>
  <h3>{{ name }}</h3>
  <ul>
    <li v-for="book in books" :key="book.id">
      <router-link :to="'/books/' + book.id">{{ book.title }}</router-link>
    </li>
  </ul>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { BookSeriesItem } from '@/types/seriesTypes'

export default defineComponent({
  props: {
    id: Number,
    name: String,
    books: Array as PropType<BookSeriesItem[]>
  }
})
</script>
