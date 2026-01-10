import { useCallback } from "react";
import useApiClient from "../../core/useApiClient.ts";

export type FacultyResult = {
  id: number;
  name: string;
};

export type UserResult = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  owner: string;
  tenantEmail: string;
  role: number;
};

export type TeacherResult = {
  id: number;
  userId: number;
  user: UserResult;
  subGroupId: number;
  subGroup: {
    id: number;
    name: string;
  };
};

const useAddTeacherApi = () => {
  const { axios } = useApiClient();

  const academicsUrl = "/api/Academics";
  const usersUrl = "/api/User";

  const getAllFaculties = useCallback(async () => {
    const response = await axios.get<FacultyResult[]>(`${academicsUrl}/faculties`);
    return response.data;
  }, [axios]);

  const getUsersByEmail = useCallback(
    async (email: string) => {
      const response = await axios.get<UserResult[]>(usersUrl, {
        params: { email },
      });
      return response.data;
    },
    [axios]
  );

  const createTeacher = useCallback(
    async (params: { userId: number; facultyId: number }) => {
      const response = await axios.post<TeacherResult>(`${academicsUrl}/teachers`, {
        userId: params.userId,
        facultyId: params.facultyId,
      });

      return response.data;
    },
    [axios]
  );

  return { getAllFaculties, getUsersByEmail, createTeacher };
};

export default useAddTeacherApi;
