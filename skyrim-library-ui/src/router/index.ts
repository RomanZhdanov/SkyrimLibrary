import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BooksView from '@/views/books/BooksView.vue'
import BookDetails from '@/views/books/BookDetails.vue'
import BookRead from '@/views/books/BookRead.vue'
import SeriesListView from '@/views/series/SeriesListView.vue'
import SeriesDetailsView from '@/views/series/SeriesDetialsView.vue'
import SeriesReadView from '@/views/series/SeriesReadView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/books/',
      component: BooksView
    },
    {
      path: '/books/:id',
      component: BookDetails,
      props: true
    },
    {
      path: '/books/:id/read',
      component: BookRead,
      props: true
    },
    {
      path: '/series/',
      component: SeriesListView
    },
    {
      path: '/series/:id',
      component: SeriesDetailsView,
      props: true
    },
    {
      path: '/series/:id/read',
      component: SeriesReadView,
      props: true
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    }
  ]
})

export default router
