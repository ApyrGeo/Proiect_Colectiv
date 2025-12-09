import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import useCustomRouting from "./useCustomRouting.ts";
import { useNavigate } from "react-router";
import { useEffect } from "react";

const PrivateRouter = () => {
  const { userProps, isFullfilled, activeAccount } = useAuthContext();
  const { isRouteAvailable } = useCustomRouting();
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isFullfilled)
      return
    if (!activeAccount || !isRouteAvailable(location.pathname, userProps))
      navigate("/");
  }, [isFullfilled, userProps, activeAccount])

  return <Outlet />
};

export default PrivateRouter;
