// import { useState } from "react";
// import reactLogo from "./assets/react.svg";
// import viteLogo from "/vite.svg";

import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";

const App = () => {
  // const [count, setCount] = useState(0);
  const [sidebarMinified, setSidebarMinified] = useState(false);
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);

  return (
    <>
      <Sidebar
        appSidebarMinified={sidebarMinified}
        onToggleMinified={() => setSidebarMinified((prev) => !prev)}
        onToggleMobile={() => setMobileSidebarOpen((prev) => !prev)}
      />
    </>
  );
};

export default App;
