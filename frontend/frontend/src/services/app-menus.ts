export function getAppMenus() {
  return [
    {
      title: "Navigation",
      submenu: [
        {
          title: "Submenu ex",
          icon: "🧰",
          submenu: [
            { title: "Ex1", url: "/ex1" },
            { title: "Ex2", url: "/ex2" },
            { title: "Ex3", url: "/ex3" },
            { title: "Ex4", url: "/ex4" },
          ],
        },
        {
          title: "Orar",
          icon: "📆",
          url: "/timetable",
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
      ],
    },
  ];
}
