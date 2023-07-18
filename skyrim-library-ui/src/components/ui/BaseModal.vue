<template>
  <div>
    <!-- Modal -->
    <div
      v-if="OpenClose"
      class="modal fade show"
      tabindex="-1"
      aria-labelledby="exampleModalLabel"
      aria-modal="true"
      role="dialog"
      style="display: block"
    >
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
          <!-- <div class="modal-header">
            <button type="button" v-on:click="$emit('onClose')" class="btn-close"></button>
          </div> -->
          <div class="modal-body">
            <slot></slot>
          </div>
          <div class="modal-footer">
            <button type="button" v-on:click="$emit('onClose')" :class="'btn btn-' + variant">
              {{ 'Close' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  emits: ['onClose'],
  props: {
    visible: Boolean,
    variant: String
  },
  data() {
    return {
      OpenClose: this.visible
    }
  },
  watch: {
    visible: function (newVal, oldVal) {
      this.OpenClose = newVal
    }
  }
})
</script>
