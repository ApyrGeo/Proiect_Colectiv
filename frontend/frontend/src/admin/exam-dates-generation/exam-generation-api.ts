// useExamGenerationApi.ts
import { useCallback } from "react";
import type { FacultyProps } from "./props";
import type { TeacherProps } from "../../exam/props";
import useApiClient from "../../core/useApiClient";
import type { StudentGroupProps, SubjectProps } from "./props.ts";

const useExamGenerationApi = () => {
  const { axios } = useApiClient();

  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";
  const examUrl = "/api/Exam";

  const getFaculties = useCallback(async (): Promise<FacultyProps[]> => {
    const response = await axios.get(`${academicsUrl}/faculties/`);
    return response.data;
  }, [axios]);

  const getTeachers = useCallback(
    async (facultyId: number): Promise<TeacherProps[]> => {
      const response = await axios.get(`${academicsUrl}/teachers/faculty/${facultyId}`);
      return response.data;
    },
    [axios]
  );

  const getSubjectsByTeacher = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<SubjectProps[]>(`${timetableUrl}/subjects/holder-teacher/${teacherId}`);
      return response.data ?? [];
    },
    [axios]
  );

  const getStudentGroupsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<StudentGroupProps[]>(`${timetableUrl}/subjects/${subjectId}/groups`);
      return response.data ?? [];
    },
    [axios]
  );

  const generateExamEntries = useCallback(
    async (subjectId: number, studentGroupIds: number[]) => {
      const response = await axios.post(`${examUrl}/generate-exam-entries`, {
        subjectId,
        studentGroupIds,
      });
      return response.data;
    },
    [axios]
  );

  return {
    getFaculties,
    getTeachers,
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    generateExamEntries,
  };
};

export default useExamGenerationApi;
