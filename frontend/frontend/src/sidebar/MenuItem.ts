export interface MenuItem {
  id?: string;
  title: string;
  url?: string;
  icon?: string;
  img?: string;
  label?: string;
  badge?: string;
  caret?: boolean | string;
  highlight?: boolean;
  hide?: boolean;
  state?: "expand" | "collapse" | "closed";
  submenu?: MenuItem[];
  expanded?: boolean;
}
