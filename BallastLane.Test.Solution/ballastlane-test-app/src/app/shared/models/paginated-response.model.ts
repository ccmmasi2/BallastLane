export interface PaginatedResponse<T> {
  items: T[];
  totalRecords: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
