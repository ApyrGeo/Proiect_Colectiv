import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";
import { Route, Routes } from "react-router-dom";
import GradesPage from "./grades/pages/GradesPage.tsx";
import TimetablePage from "./timetable/pages/TimetablePage.tsx";
import TimetableTeacherPage from "./timetable/pages/TimetableTeacherPage.tsx";
import TimetableSubjectPage from "./timetable/pages/TimetableSubjectPage.tsx";
import TimetableClassroomPage from "./timetable/pages/TimetableClassroomPage.tsx";
import ProfilePage from "./profile/pages/ProfilePage.tsx";
import { Toaster } from "react-hot-toast";

import "../i18n.ts";
import Homepage from "./homepage/HomePage.tsx";
import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import SignOutButton from "./auth/components/SignOutButton.tsx";
import SignInButton from "./auth/components/SignInButton.tsx";
import ContractsPage from "./contracts/ContractsPage.tsx";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);

  return (
    <>
      {/* Toaster permanent la nivel global */}
      <Toaster />
      <Sidebar appSidebarMinified={sidebarMinified} onToggleMinified={() => setSidebarMinified((prev) => !prev)} />

      <div className={`app-content`}>
        <div style={{ display: "flex", justifyContent: "flex-end" }}>
          <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-end" }}>
            <AuthenticatedTemplate>
              <p>This will only render if a user is signed-in.</p>
              <SignOutButton />
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
              <p>This will only render if a user is not signed-in.</p>
              <SignInButton />
            </UnauthenticatedTemplate>
          </div>
        </div>

        <Routes>
          <Route path="/" Component={Homepage} />
          <Route path={"/home"} Component={Homepage} />
          <Route path={"/grades"} Component={GradesPage} />
          <Route path={"/timetable"} Component={TimetablePage} />
          <Route path={"/timetable/teacher/:id"} Component={TimetableTeacherPage} />
          <Route path={"/timetable/subject/:id"} Component={TimetableSubjectPage} />
          <Route path={"/timetable/classroom/:id"} Component={TimetableClassroomPage} />
          <Route path={"/contracts"} Component={ContractsPage}></Route>
          <Route path={"/profile"} Component={ProfilePage} />
        </Routes>
      </div>
    </>
  );
};

export default App;
