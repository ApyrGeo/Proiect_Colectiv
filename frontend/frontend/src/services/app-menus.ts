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
      ],
    },
  ];
}
