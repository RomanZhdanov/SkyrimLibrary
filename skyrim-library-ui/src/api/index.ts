import axios from 'axios'
import type { BookItem, BookDetails } from '@/types/bookTypes'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

export const get = async (id: string) => await http.get<BookDetails>(`/books/${id}`)

export const search = async (search: string) =>
  await http.get<BookItem[]>(`/search?input=${search}`)
