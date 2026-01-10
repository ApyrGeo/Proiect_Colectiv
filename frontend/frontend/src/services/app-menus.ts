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
          title: "Academics",
          icon: "🏛️",
          url: "/admin/academics",
        },
        {
          title: "Timetable",
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
        {
          title: "Location",
          icon: "📍",
          url: "/admin/location",
        },
        {
          title: "Subject",
          icon: "📚",
          url: "/admin/subject-generation",
        },
      ],
    },
  ];
}
