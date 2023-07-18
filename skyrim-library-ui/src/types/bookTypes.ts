export interface BookPageResult {
  items: BookItem[]
  pageNumber: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface BookSearchResult {
  items: BookItem[]
  itemsCount: number
}

export interface BookItem {
  id: string
  title: string
  description: string
  author: string
  type: string
  snippets: string
  coverImage: string
}

export interface Book {
  id: string
  title: string
  author: string
  text: string
}

export interface BookDetails {
  id: string
  title: string
  description: string
  author: string
  type: string
  coverImage: string
}
