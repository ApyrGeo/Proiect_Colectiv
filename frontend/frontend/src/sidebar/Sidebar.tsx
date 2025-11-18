import React, { useEffect, useRef, useState } from "react";
import { useLocation, NavLink } from "react-router-dom";
import { getAppMenus } from "../services/app-menus";
import type { MenuItem } from "./MenuItem";
import type { SidebarProps } from "./SidebarProps";
import "./sidebar.css";

const LS_KEY = "app_sidebar_minified";

const Sidebar: React.FC<SidebarProps> = ({ appSidebarMinified = false }) => {
  const location = useLocation();

  const [menus] = useState<MenuItem[]>(getAppMenus);

  const readInitialMinified = () => {
    try {
      const raw = localStorage.getItem(LS_KEY);
      if (raw === "true") return true;
      if (raw === "false") return false;
    } catch {
      /* empty */
    }
    return appSidebarMinified;
  };

  const mountedRef = useRef(false);

  const [isMinified, setIsMinified] = useState<boolean>(() => readInitialMinified());

  const getInitialExpanded = () => {
    const activeParent = getAppMenus()
      .flatMap((section) => section.submenu ?? [])
      .find((item) => item.submenu?.some((sub) => location.pathname.startsWith(sub.url ?? "")));

    return activeParent?.title;
  };

  const [expanded, setExpanded] = useState<string | undefined>(getInitialExpanded);

  const [hoveredMenu, setHoveredMenu] = useState<MenuItem | null>(null);
  const [hoverPosition, setHoverPosition] = useState<number>(0);
  const hoverTimeout = useRef<number | null>(null);

  useEffect(() => {
    if (!mountedRef.current) {
      mountedRef.current = true;
      return;
    }
    localStorage.setItem(LS_KEY, isMinified ? "true" : "false");
  }, [isMinified]);

  const toggleSubmenu = (title: string) => {
    if (isMinified) return;
    setExpanded((prev) => (prev === title ? undefined : title));
  };

  const handleMouseEnter = (e: React.MouseEvent<HTMLDivElement>, item: MenuItem) => {
    if (!isMinified) return;
    const rect = e.currentTarget.getBoundingClientRect();
    setHoverPosition(rect.top);
    setHoveredMenu(item);
    if (hoverTimeout.current) {
      window.clearTimeout(hoverTimeout.current);
      hoverTimeout.current = null;
    }
  };

  const handleMouseLeave = () => {
    if (!isMinified) return;
    hoverTimeout.current = window.setTimeout(() => {
      setHoveredMenu(null);
      hoverTimeout.current = null;
    }, 200);
  };

  const handlePreviewEnter = () => {
    if (hoverTimeout.current) {
      window.clearTimeout(hoverTimeout.current);
      hoverTimeout.current = null;
    }
  };

  const handlePreviewLeave = () => {
    setHoveredMenu(null);
  };

  const isActiveUrl = (url?: string) => {
    if (!url) return false;
    return location.pathname === url || location.pathname.startsWith(url + "/");
  };

  const isChildActive = (item: MenuItem): boolean => {
    if (!item.submenu) return false;
    return item.submenu.some((sub) => isActiveUrl(sub.url));
  };

  useEffect(() => {
    if (!mountedRef.current) return;

    if (hoverTimeout.current) {
      window.clearTimeout(hoverTimeout.current);
      hoverTimeout.current = null;
    }

    const activeParent = menus
      .flatMap((section) => section.submenu ?? [])
      .find((item) => item.submenu?.some((sub) => isActiveUrl(sub.url)));

    setExpanded((prev) => activeParent?.title ?? prev);
    setHoveredMenu(null);
  }, [location.pathname]);

  return (
    <>
      <div className={`app-sidebar ${isMinified ? "minified" : ""}`}>
        <div className="app-sidebar-minify-btn-container">
          <button
            className="app-sidebar-minify-btn"
            onClick={() => setIsMinified((p) => !p)}
            title="Toggle sidebar"
            aria-pressed={isMinified}
          >
            {isMinified ? "»" : "«"}
          </button>
        </div>
        <div className="app-sidebar-content">
          {menus.map((section, sectionIndex) => (
            <div key={sectionIndex} className="menu">
              {section.submenu?.map((item) => {
                const active = item.submenu ? item.submenu.some((s) => isActiveUrl(s.url)) : isActiveUrl(item.url);

                return (
                  <div
                    key={item.id ?? item.title}
                    className={`menu-item ${item.submenu ? "has-sub" : ""} ${
                      expanded === item.title ? "expand active" : active ? "active" : ""
                    }`}
                    onMouseEnter={(e) => handleMouseEnter(e, item)}
                    onMouseLeave={handleMouseLeave}
                  >
                    {item.submenu ? (
                      <a
                        href="#!"
                        className={`menu-link ${isChildActive(item) ? "active-link" : ""}`}
                        onClick={(e) => {
                          e.preventDefault();
                          toggleSubmenu(item.title);
                        }}
                      >
                        <span className="menu-icon">{item.icon}</span>
                        {!isMinified && <span className="menu-text">{item.title}</span>}
                        {!isMinified && item.submenu && (
                          <span className="menu-caret">{expanded === item.title ? "▾" : "▸"}</span>
                        )}
                      </a>
                    ) : (
                      <NavLink
                        to={item.url ?? "#"}
                        className={({ isActive }) =>
                          `menu-link ${isActive || isActiveUrl(item.url) ? "active-link" : ""}`
                        }
                        onClick={() => {
                          setHoveredMenu(null);
                        }}
                      >
                        <span className="menu-icon">{item.icon}</span>
                        {!isMinified && <span className="menu-text">{item.title}</span>}
                      </NavLink>
                    )}

                    {item.submenu && expanded === item.title && (
                      <div className="menu-submenu" aria-hidden={isMinified}>
                        {item.submenu.map((sub) => (
                          <NavLink
                            key={sub.id ?? sub.title}
                            to={sub.url ?? "#"}
                            className={({ isActive }) =>
                              `submenu-link ${isActive || isActiveUrl(sub.url) ? "active-link" : ""}`
                            }
                            onClick={() => {
                              setHoveredMenu(null);
                            }}
                          >
                            {sub.title}
                          </NavLink>
                        ))}
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          ))}

          {isMinified && hoveredMenu && hoveredMenu.submenu && (
            <div
              className="sidebar-hover-preview"
              style={{ top: hoverPosition }}
              onMouseEnter={handlePreviewEnter}
              onMouseLeave={handlePreviewLeave}
            >
              <div className="hover-title">{hoveredMenu.title}</div>
              {hoveredMenu.submenu.map((sub) => (
                <NavLink
                  key={sub.id ?? sub.title}
                  to={sub.url ?? "#"}
                  className={({ isActive }) =>
                    `hover-submenu-link ${isActive || isActiveUrl(sub.url) ? "active-link" : ""}`
                  }
                  onClick={() => {
                    setHoveredMenu(null);
                  }}
                >
                  {sub.title}
                </NavLink>
              ))}
            </div>
          )}
        </div>
      </div>
      <div className={`app-sidebar-backdrop ${isMinified ? "" : "show"}`} />
    </>
  );
};

export default Sidebar;
