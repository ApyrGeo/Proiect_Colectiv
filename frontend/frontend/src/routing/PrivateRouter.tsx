import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import useCustomRouting from "./useCustomRouting.ts";
const PrivateRouter = () => {
  const { activeAccount, userInfo } = useAuthContext();
  const { isRouteAvailable } = useCustomRouting();
  const location = useLocation();
  return activeAccount && isRouteAvailable(location.pathname, userInfo) ? <Outlet /> : <Navigate to="/" />;
};

export default PrivateRouter;
