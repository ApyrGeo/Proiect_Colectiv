import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type { PromotionOfUser, ScholarshipStatus } from "./props";
import type { SubjectGradesResponse, SubjectProps, TeacherProps, UserProps, StudentGroupProps } from "./props.ts";

const useGradesApi = () => {
  const { axios } = useApiClient();

  const gradesUrl = "/api/Grades";
  const userUrl = "/api/User";
  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";

  const getSubGroupsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get(`${timetableUrl}/subjects/${subjectId}/groups`);
      // Extragem toate subgrupurile
      const subGroups = response.data.flatMap((group: any) => group.studentSubGroups ?? []);
      return subGroups; // { id, name }[]
    },
    [axios]
  );

  // 📌 Enrollment pentru un student într-o subgrupă
  const getEnrollmentByStudentAndSubGroup = useCallback(
    async (userId: number, subGroupId: number) => {
      const response = await axios.get(`${academicsUrl}/enrollments`, { params: { userId } });
      const enrollment = response.data.find((e: any) => e.subGroupId === subGroupId);
      return enrollment ?? null;
    },
    [axios]
  );

  // 📌 Specializările la care este înscris studentul
  const getUserPromotions = useCallback(
    async (userId: number): Promise<PromotionOfUser[]> => {
      const response = await axios.get<{ promotions: PromotionOfUser[] }>(
        `${academicsUrl}/promotions/of-user/${userId}`
      );

      return response.data.promotions ?? [];
    },
    [axios]
  );

  // 📌 Status bursă student (poate fi null)
  const getScholarshipStatusForUser = useCallback(
    async (
      userId: number,
      specialization: number,
      studyYear: number,
      semester: number
    ): Promise<ScholarshipStatus | null> => {
      const response = await axios.get<ScholarshipStatus | null>(`${gradesUrl}/status`, {
        params: {
          userId,
          specialisationId: specialization,
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
    async (userId: number, specialization: number | null, studyYear?: number | null, semester?: number | null) => {
      const response = await axios.get(`${gradesUrl}`, {
        params: {
          userId,
          ...(specialization && { specialisationId: specialization }),
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
    async (subjectId: number, groupId: number): Promise<SubjectGradesResponse> => {
      const response = await axios.get<SubjectGradesResponse>(`${gradesUrl}/subject-groups`, {
        params: { subjectId, groupId },
      });
      return response.data;
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

  const updateGradeById = useCallback(
    async (gradeId: number, value: number | null, subjectId: number, enrollmentId: number, teacherId: number) => {
      await axios.put(
        `${gradesUrl}/${gradeId}`, // punem gradeId direct in URL
        { value, subjectId, enrollmentId }, // body
        { params: { teacherId } } // query param teacherId
      );
    },
    [axios]
  );

  const createGradeById = useCallback(
    async (
      value: number | null,
      subjectId: number,
      enrollmentId: number,
      teacherId: number // adăugăm teacherId
    ) => {
      await axios.post(
        `${gradesUrl}`, // ruta rămâne aceeași
        { value, subjectId, enrollmentId }, // body-ul
        { params: { teacherId } } // query parameter
      );
    },
    [axios]
  );

  return {
    getUserPromotions,
    getScholarshipStatusForUser,
    getGradesForUser,
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    getTeacherbyUserId,
    getTeacherById,
    getUserById,
    getStudentByGroup,
    getSpecialisationsByStudent,
    getSubGroupsBySubject,
    getEnrollmentByStudentAndSubGroup,
    updateGradeById,
    createGradeById,
  };
};

export default useGradesApi;
