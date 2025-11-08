import "./App.css";
import Sidebar from "./sidebar/Sidebar.tsx";
import { useState } from "react";
import GoogleMapsComponent from "./googleMaps/GoogleMapsComponent.tsx";

const App = () => {
  const [sidebarMinified, setSidebarMinified] = useState(false);
  const [_, setMobileSidebarOpen] = useState(false);

  return (
    <>
      {/*<Sidebar*/}
      {/*  appSidebarMinified={sidebarMinified}*/}
      {/*  onToggleMinified={() => setSidebarMinified(prev => !prev)}*/}
      {/*  onToggleMobile={() => setMobileSidebarOpen(prev => !prev)}*/}
      {/*/>*/}
      <GoogleMapsComponent />
    </>
  );
};

export default App;
