import { useCallback, useMemo } from "react";
import type { UserInfo } from "../core/props.ts";

const useCustomRouting = () => {
  const rolePermissions = useMemo(
    () =>
      new Map<string, string[]>([
        ["Student", ["grades", "timetable", "profile"]],
        ["Teacher", ["profile", "grades"]],
        ["Admin", []],
      ]),
    []
  );

  const isRouteAvailable = useCallback(
    (path: string, userInfo: UserInfo): boolean => {
      if (!userInfo || !userInfo.userRole) return false;

      const page = path.split("/").at(1);

      return !!(
        page &&
        rolePermissions.has(userInfo.userRole) &&
        rolePermissions.get(userInfo.userRole)?.includes(page)
      );
    },
    [rolePermissions]
  );

  return { isRouteAvailable };
};

export default useCustomRouting;
