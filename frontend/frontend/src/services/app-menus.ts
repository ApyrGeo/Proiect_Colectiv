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
          title: "Create Users",
          icon: "➕",
          url: "/create-users",
        },
      ],
    },
  ];
}
