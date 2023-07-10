import axios from 'axios'
import type { BookItem } from '@/types/bookTypes'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

export const search = async (search: string) =>
  await http.get<BookItem[]>(`/search?input=${search}`)
