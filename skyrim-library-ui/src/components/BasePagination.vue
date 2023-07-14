<template>
  <nav aria-label="Page navigation example">
    <ul class="pagination">
      <li v-if="hasPrevious" class="page-item">
        <a class="page-link" href="#" @click="prevPage">Previous</a>
      </li>
      <li class="page-item" v-if="showFirstPage">
        <a class="page-link" href="#" @click="pageClick(1)">1</a>
      </li>
      <li class="page-item disabled" v-if="showFirstPage">
        <a class="page-link" href="#">...</a>
      </li>

      <li v-for="page in pages" :key="page.name" class="page-item">
        <a
          class="page-link"
          :class="{ active: page.isActive }"
          href="#"
          @click="pageClick(page.name)"
        >
          {{ page.name }}
        </a>
      </li>

      <li class="page-item disabled" v-if="showLastPage">
        <a class="page-link" href="#">...</a>
      </li>
      <li class="page-item" v-if="showLastPage">
        <a class="page-link" href="#" @click="pageClick(totalPages)">{{ totalPages }}</a>
      </li>

      <li v-if="hasNext" class="page-item">
        <a class="page-link" href="#" @click="nextPage"> Next </a>
      </li>
    </ul>
  </nav>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  emits: ['page-changed'],
  props: {
    maxVisibleButtons: {
      type: Number,
      required: false,
      default: 5
    },
    currentPage: {
      type: Number,
      required: true
    },
    totalPages: {
      type: Number,
      required: true
    },
    hasPrevious: {
      type: Boolean,
      requred: true
    },
    hasNext: {
      type: Boolean,
      required: true
    }
  },
  computed: {
    startPage() {
      if (this.currentPage === 1) {
        return 1
      }

      if (this.currentPage === this.totalPages) {
        return this.totalPages - this.maxVisibleButtons
      }

      return this.currentPage - 1
    },
    pages() {
      const range = []

      for (
        let i = this.startPage;
        i <= Math.min(this.startPage + this.maxVisibleButtons - 1, this.totalPages);
        i++
      ) {
        range.push({
          name: i,
          isActive: i === this.currentPage
        })
      }

      return range
    },
    showFirstPage() {
      return this.currentPage > 2
    },
    showLastPage() {
      return this.currentPage < this.totalPages - this.maxVisibleButtons
    }
  },
  methods: {
    pageClick(page: number) {
      if (this.currentPage === page) return
      this.$emit('page-changed', page)
    },
    nextPage() {
      this.$emit('page-changed', this.currentPage + 1)
    },
    prevPage() {
      this.$emit('page-changed', this.currentPage - 1)
    }
  }
})
</script>
