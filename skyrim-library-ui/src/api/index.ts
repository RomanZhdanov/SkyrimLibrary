import axios from 'axios'
import type { BookPageResult, BookDetails, BookSearchResult } from '@/types/bookTypes'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

export const get = async (id: string) => await http.get<BookDetails>(`/books/${id}`)

export const getPage = async (page: number, pageSize: number) =>
  await http.get<BookPageResult>(`/books/?page=${page}&pageSize=${pageSize}`)

export const search = async (search: string) =>
  await http.get<BookSearchResult>(`/search?input=${search}`)
