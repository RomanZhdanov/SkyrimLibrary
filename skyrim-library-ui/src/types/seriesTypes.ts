export interface SeriesDetails {
  id: number
  name: string
  books: BookSeriesItem[]
}

export interface BookSeriesItem {
  id: string
  title: string
  coverImage: string
}

export interface SeriesListItem {
  id: number
  name: string
  booksCount: number
}
