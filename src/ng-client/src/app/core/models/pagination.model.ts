export interface IPagination<T> {
    items: T[];
    offset: number;
    limit: number;
    totalCount: number;
    hasMore: boolean;
}