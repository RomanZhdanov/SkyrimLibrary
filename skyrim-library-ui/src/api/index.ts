import axios from 'axios'
import type {
  Book,
  BookPageResult,
  BookDetails,
  SeriesDetails,
  BookSearchResult
} from '@/types/bookTypes'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

export const get = async (id: string) => await http.get<Book>(`/books/${id}`)

export const getSeries = async (id: number) => await http.get<SeriesDetails>(`/series/${id}`)

export const getDetails = async (id: string) => await http.get<BookDetails>(`/books/${id}/details`)

export const getPage = async (page: number, pageSize: number) =>
  await http.get<BookPageResult>(`/books/?page=${page}&pageSize=${pageSize}`)

export const search = async (search: string) =>
  await http.get<BookSearchResult>(`/search?input=${search}`)
