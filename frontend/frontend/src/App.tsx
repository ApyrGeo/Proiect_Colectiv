import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);
  const [_, setMobileSidebarOpen] = useState(false);

  return (
    <>
      <Sidebar
        appSidebarMinified={sidebarMinified}
        onToggleMinified={() => setSidebarMinified(prev => !prev)}
        onToggleMobile={() => setMobileSidebarOpen(prev => !prev)}
      />
    </>
  );
};

export default App;
