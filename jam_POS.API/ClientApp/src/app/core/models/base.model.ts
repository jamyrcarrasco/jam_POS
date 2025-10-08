export interface BaseEntity {
  id: number;
  createdAt?: Date;
  updatedAt?: Date;
  isActive?: boolean;
}

export interface BaseFilter {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
