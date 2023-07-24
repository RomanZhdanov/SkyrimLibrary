export interface SeriesRead {
  id: number
  name: string
  author: string
  description: string
  books: BookSeriesText[]
}

export interface BookSeriesText {
  id: string
  title: string
  text: string
}

export interface SeriesDetails {
  id: number
  name: string
  author: string
  description: string
  books: BookSeriesItem[]
}

export interface BookSeriesItem {
  id: string
  title: string
  description: string
  coverImage: string
}

export interface SeriesListItemType {
  id: number
  name: string
  description: string
  booksCount: number
}
