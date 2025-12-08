import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import useCustomRouting from "./useCustomRouting.ts";
const PrivateRouter = () => {
  const { activeAccount, userProps } = useAuthContext();
  const { isRouteAvailable } = useCustomRouting();
  const location = useLocation();
  return activeAccount && isRouteAvailable(location.pathname, userProps) ? <Outlet /> : <Navigate to="/" />;
};

export default PrivateRouter;
