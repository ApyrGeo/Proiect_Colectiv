import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";
import { Route, Routes } from "react-router-dom";
import TimetablePage from "./timetable/TimetablePage.tsx";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);
  const [_, setMobileSidebarOpen] = useState(false);

  return (
    <>
      <Sidebar
        appSidebarMinified={sidebarMinified}
        onToggleMinified={() => setSidebarMinified((prev) => !prev)}
        onToggleMobile={() => setMobileSidebarOpen((prev) => !prev)}
      />

      {/*temp routing*/}
      <Routes>
        <Route path={"/timetable"} Component={TimetablePage} />
      </Routes>
    </>
  );
};

export default App;
