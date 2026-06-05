export interface TableColumn {
  key: string;
  label: string;
  type?: 'text' | 'badge' | 'custom';
  badgeClass?: (value: any) => string;
}
