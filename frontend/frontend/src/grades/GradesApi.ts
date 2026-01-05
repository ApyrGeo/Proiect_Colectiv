import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type { ScholarshipStatus, SpecializationResponse } from "./props";
import type {StudentGroupProps, SubjectProps, TeacherProps, UserProps} from "../exam/props.ts";

const useGradesApi = () => {
  const { axios } = useApiClient();

  const gradesUrl = "/api/Grades";
  const userUrl = "/api/User";
  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";

  // 📌 Specializările la care este înscris studentul
  const getUserSpecializations = useCallback(
    async (userId: number): Promise<string[]> => {
      const response = await axios.get<SpecializationResponse[]>(`${userUrl}/${userId}/enrolled-specialisations`);

      return response.data?.map((s) => s.name) ?? [];
    },
    [axios]
  );

  // 📌 Status bursă student (poate fi null)
  const getScholarshipStatusForUser = useCallback(
    async (
      userId: number,
      specialization: string,
      studyYear: number,
      semester: number
    ): Promise<ScholarshipStatus | null> => {
      const response = await axios.get<ScholarshipStatus | null>(`${gradesUrl}/status`, {
        params: {
          userId,
          specialisation: specialization,
          yearOfStudy: studyYear,
          semester,
        },
        validateStatus: (status) => status === 200 || status === 204,
      });

      return response.status === 204 ? null : response.data;
    },
    [axios]
  );

  // 📌 Notele studentului (cu filtre opționale)
  const getGradesForUser = useCallback(
    async (userId: number, specialization?: string | null, studyYear?: number | null, semester?: number | null) => {
      const response = await axios.get(`${gradesUrl}`, {
        params: {
          userId,
          ...(specialization && { specialisation: specialization }),
          ...(studyYear && { yearOfStudy: studyYear }),
          ...(semester && { semester }),
        },
        validateStatus: (status) => status === 200 || status === 204,
      });

      if (response.status === 204) return [];
      return Array.isArray(response.data) ? response.data : [];
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

  const getTeacherbyUserId = useCallback(
    async (userId: number) => {
      const response = await axios.get<TeacherProps>(`${academicsUrl}/teacher/user/${userId}`);
      return response.data ?? [];
    },
    [axios]
  );

  const getStudentByGroup = useCallback(
    async (groupId: number) => {
      const response = await axios.get<TeacherProps>(`${academicsUrl}/student-groups/${groupId}/students`);
      return response.data ?? [];
    },
    [axios]
  );

  const getTeacherById = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<TeacherProps>(`${academicsUrl}/teachers/${teacherId}`);
      return response.data;
    },
    [axios]
  );

  const getUserById = useCallback(
    async (userId: number) => {
      const response = await axios.get<UserProps>(`${userUrl}/${userId}`);
      return response.data;
    },
    [axios]
  );

  const getSpecialisationsByStudent = useCallback(
    async (userId: number) => {
      const response = await axios.get<UserProps>(`${userUrl}/${userId}/enrolled-specialisations`);
      return response.data;
    },
    [axios]
  );

  return {
    getUserSpecializations,
    getScholarshipStatusForUser,
    getGradesForUser,
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    getTeacherbyUserId,
    getTeacherById,
    getUserById,
    getStudentByGroup,
    getSpecialisationsByStudent,
  };
};

export default useGradesApi;
