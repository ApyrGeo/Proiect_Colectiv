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
          title: "Location",
          icon: "📍",
          url: "/admin/location",
        },
        {
          title: "Subject",
          icon: "📚",
          url: "/admin/subject-generation",
        },
        {
          title: "Exam",
          icon: "📋",
          url: "/admin/exam-generation",
        },
        {
          title: "AddUsers",
          icon: "➕",
          submenu: [
            {
              title: "ImportUsers",
              url: "/admin/import-users",
            },
            {
              title: "ImportPromotion",
              url: "/admin/import-promotion",
            },
            {
              title: "AddTeacher",
              url: "/admin/add-teacher",
            },
          ],
        },
      ],
    },
  ];
}
