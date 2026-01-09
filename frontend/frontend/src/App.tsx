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
import FAQPopup from "./faq/components/FAQPopup.tsx";
import { faqsTimetable } from "./faq/FAQData.ts";
import ContractsPage from "./contracts/ContractsPage.tsx";
import ExamPage from "./exam/pages/ExamPage.tsx";
import PrivateRouter from "./routing/PrivateRouter.tsx";
import SignInStatusComponent from "./auth/components/SignInStatusComponent.tsx";
import TimetableGenerationPage from "./timetable-generation/pages/TimetableGenerationPage.tsx";
import AdminLocationPage from "./admin/location/AdminLocationPage.tsx";
import SubjectGenerationPage from "./admin/subject-generation/pages/SubjectGenerationPage.tsx";
import AdminAcademicsPage from "./admin-academics/AdminAcademicsPage.tsx";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);
  return (
    <>
      {/* Toaster permanent la nivel global */}
      <Toaster />
      <Sidebar appSidebarMinified={sidebarMinified} onToggleMinified={() => setSidebarMinified((prev) => !prev)} />

      <div className={`app-content`}>
        <SignInStatusComponent />

        <Routes>
          <Route index Component={Homepage} />
          <Route path={"/home"} Component={Homepage} />
          <Route Component={PrivateRouter}>
            <Route path={"/grades"} Component={GradesPage} />
            <Route path={"/timetable"} Component={TimetablePage} />
            <Route path={"/timetable/teacher/:id"} Component={TimetableTeacherPage} />
            <Route path={"/timetable/subject/:id"} Component={TimetableSubjectPage} />
            <Route path={"/timetable/classroom/:id"} Component={TimetableClassroomPage} />
            <Route path={"/contracts"} Component={ContractsPage} />
            <Route path={"/profile"} Component={ProfilePage} />
            <Route path={"/exam"} Component={ExamPage} />
            <Route path={"/admin/academics"} Component={AdminAcademicsPage} />
            <Route path={"/admin/timetable-generation"} Component={TimetableGenerationPage} />
            <Route path={"/admin/location"} Component={AdminLocationPage} />
            <Route path={"/admin/subject-generation"} Component={SubjectGenerationPage} />
          </Route>
        </Routes>

        <FAQPopup faqs={faqsTimetable} />
      </div>
    </>
  );
};

export default App;
