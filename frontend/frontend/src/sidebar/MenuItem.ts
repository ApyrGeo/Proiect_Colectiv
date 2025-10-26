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
  state?: "expanded" | "collapsed" | "closed";
  submenu?: MenuItem[];
  expanded?: boolean;
}
