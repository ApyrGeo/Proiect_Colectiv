export function getAppMenus() {
  return [
    {
      title: "Navigation",
      submenu: [
        {
          title: "Timetable",
          icon: "📆",
          url: "/timetable",
        },
        {
          title: "Contracts",
          icon: "📝",
          url: "/contracts",
        },
        {
          title: "Grades",
          icon: "🎓",
          url: "/grades",
        },
        {
          title: "Profile",
          icon: "👤",
          url: "/profile",
        },
        {
          title: "Exam",
          icon: "📋",
          url: "/exam",
        },
        {
          title: "Timetable Generation",
          icon: "📆",
          url: "/admin/timetable-generation",
        },
        {
          title: "Add users",
          icon: "➕",
          submenu: [
            {
              title: "Import users",
              url: "/admin/import-users",
            },
            {
              title: "Import promotion",
              url: "/admin/import-promotion",
            },
            {
              title: "Add teacher",
              url: "/admin/add-teacher",
            },
          ],
        },
      ],
    },
  ];
}
