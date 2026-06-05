export interface TableAction<T> {
  icon: string;      
  label: string;
  colorClass: string;
  callback: (row: T) => void;
}
