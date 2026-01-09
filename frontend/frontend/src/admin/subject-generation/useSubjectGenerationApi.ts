import useApiClient from "../../core/useApiClient.ts";
import type { TeacherProps } from "../../exam/props.ts";
import { useCallback } from "react";
import type { FacultyProps } from "./props.ts";

export type AddSubjectDto = {
  name: string;
  code: string;
  numberOfCredits: number;
  holderTeacherId: number;
  semesterId: number;
  type: string;
  formationType: string;
  optionalPackage: number;
};

const useSubjectGenerationApi = () => {
  const { axios } = useApiClient();

  const getFaculties = useCallback(async (): Promise<FacultyProps[]> => {
    const response = await axios.get(`/api/Academics/faculties/`);
    return response.data;
  }, [axios]);

  const getTeachers = useCallback(
    async (facultyId: number): Promise<TeacherProps[]> => {
      const response = await axios.get(`/api/Academics/teachers/faculty/${facultyId}`);
      return response.data;
    },
    [axios]
  );

  const addSubject = useCallback(
    async (subject: AddSubjectDto): Promise<void> => {
      await axios.post(`/api/Timetable/subjects/`, subject);
    },
    [axios]
  );

  return {
    getFaculties,
    getTeachers,
    addSubject,
  };
};

export default useSubjectGenerationApi;
