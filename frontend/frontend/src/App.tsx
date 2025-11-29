import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";
import { Route, Routes } from "react-router-dom";
import Grades from "./grades/pages/GradesPage.tsx";
import TimetablePage from "./timetable/pages/TimetablePage.tsx";
import TimetableTeacherPage from "./timetable/pages/TimetableTeacherPage.tsx";
import TimetableSubjectPage from "./timetable/pages/TimetableSubjectPage.tsx";
import TimetableClassroomPage from "./timetable/pages/TimetableClassroomPage.tsx";
import ProfilePage from "./profile/pages/ProfilePage.tsx";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);

  return (
    <>
      <Sidebar appSidebarMinified={sidebarMinified} onToggleMinified={() => setSidebarMinified((prev) => !prev)} />

      <div className={`app-content`}>
        <Routes>
          <Route path={"/grades"} Component={Grades} />
          <Route path={"/timetable"} Component={TimetablePage} />
          <Route path={"/timetable/teacher/:id"} Component={TimetableTeacherPage} />
          <Route path={"/timetable/subject/:id"} Component={TimetableSubjectPage} />
          <Route path={"/timetable/classroom/:id"} Component={TimetableClassroomPage} />
          <Route path={"/profile"} Component={ProfilePage} />
        </Routes>
      </div>
    </>
  );
};

export default App;
