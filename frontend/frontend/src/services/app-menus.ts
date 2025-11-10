export function getAppMenus() {
  return [
    {
      title: "Navigation",
      submenu: [
        {
          title: "Maintenance",
          icon: "🧰",
          submenu: [
            { title: "Servers", url: "/servers" },
            { title: "Logs", url: "/logs" },
            { title: "Backups", url: "/backups" },
            { title: "Settings", url: "/settings" },
          ],
        },
        {
          title: "WebDesign",
          icon: "💻",
          submenu: [
            { title: "HTML", url: "/html" },
            { title: "CSS", url: "/css" },
          ],
        },
        {
          title: "Orar",
          icon: "📆",
          url: "/timetable",
        },
        {
          title: "WebHosting",
          icon: "🏠",
          submenu: [
            { title: "Plans", url: "/plans" },
            { title: "Domains", url: "/domains" },
          ],
        },
        {
          title: "Design",
          icon: "🎨",
          url: "/designs",
        },
        {
          title: "Grades",
          icon: "🎓",
          url: "/grades",
        },
      ],
    },
  ];
}
