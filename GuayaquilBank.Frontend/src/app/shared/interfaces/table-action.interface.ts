export interface TableAction<T> {
  icon: string;
  label: string | ((row: T) => string);
  colorClass: string | ((row: T) => string);
  callback: (row: T) => void;
}
