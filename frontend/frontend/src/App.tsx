import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";
import { Route, Routes } from "react-router-dom";
import GradesPage from "./grades/pages/GradesPage.tsx";
import TimetablePage from "./timetable/pages/TimetablePage.tsx";
import TimetableTeacherPage from "./timetable/pages/TimetableTeacherPage.tsx";
import TimetableSubjectPage from "./timetable/pages/TimetableSubjectPage.tsx";
import TimetableClassroomPage from "./timetable/pages/TimetableClassroomPage.tsx";
import { ToastContainer } from "react-toastify";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);
  const [, setMobileSidebarOpen] = useState(false);

  return (
    <>
      {/* Toaster permanent la nivel global */}
      <ToastContainer aria-label={undefined} />

      <Sidebar
        appSidebarMinified={sidebarMinified}
        onToggleMinified={() => setSidebarMinified((prev) => !prev)}
        onToggleMobile={() => setMobileSidebarOpen((prev) => !prev)}
      />

      <Routes>
        <Route path="/grades" Component={GradesPage} />
        <Route path="/timetable" Component={TimetablePage} />
        <Route path="/timetable/teacher/:id" Component={TimetableTeacherPage} />
        <Route path="/timetable/subject/:id" Component={TimetableSubjectPage} />
        <Route path="/timetable/classroom/:id" Component={TimetableClassroomPage} />
      </Routes>
    </>
  );
};

export default App;
