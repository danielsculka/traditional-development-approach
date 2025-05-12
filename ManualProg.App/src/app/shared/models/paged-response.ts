export interface IPagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
