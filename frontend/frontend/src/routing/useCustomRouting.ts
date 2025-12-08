import { useCallback, useMemo } from "react";
import { type UserProps, UserRole } from "../core/props.ts";

const useCustomRouting = () => {
  const rolePermissions = useMemo(
    () =>
      new Map<UserRole, string[]>([
        [UserRole.STUDENT, ["grades", "timetable", "contracts", "profile"]],
        [UserRole.TEACHER, ["profile", "grades"]],
        [UserRole.ADMIN, []],
      ]),
    []
  );

  const isRouteAvailable = useCallback(
    (path: string, userProps: UserProps): boolean => {
      if (!userProps || userProps.role == undefined) return false;
      const page = path.split("/").at(1);
      return !!(page && rolePermissions.has(userProps.role) && rolePermissions.get(userProps.role)?.includes(page));
    },
    [rolePermissions]
  );

  return { isRouteAvailable };
};

export default useCustomRouting;
