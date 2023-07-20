import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import BooksView from '@/views/BooksView.vue'
import SeriesDetails from '@/views/SeriesDetials.vue'
import BookDetails from '@/views/BookDetails.vue'
import BookRead from '@/views/BookRead.vue'

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
      path: '/series/:id',
      component: SeriesDetails,
      props: true
    },
    {
      path: '/books/:id/read',
      component: BookRead,
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
