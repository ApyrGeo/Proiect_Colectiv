import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type { LocationProps, TeacherProps, StudentGroupProps, SubjectProps, ExamProps, UserProps } from "./props";

const useExamApi = () => {
  const { axios } = useApiClient();

  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";
  const examUrl = "/api/Exam";
  const userUrl = "/api/User";

  // 📌 Obține locații
  const getLocations = useCallback(async () => {
    const response = await axios.get<LocationProps[]>(`${timetableUrl}/locations`);
    return response.data ?? [];
  }, [axios]);

  const getLocation = useCallback(
    async (locationId: number) => {
      const response = await axios.get<LocationProps>(`${timetableUrl}/locations/${locationId}`);
      return response.data;
    },
    [axios]
  );

  // 📌 Profesor după ID (404 => null)
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

  const getTeacherbyUserId = useCallback(
    async (userId: number) => {
      const response = await axios.get<TeacherProps>(`${academicsUrl}/teacher/user/${userId}`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Grupuri la o materie
  const getStudentGroupsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<StudentGroupProps[]>(`${timetableUrl}/subjects/${subjectId}/groups`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Materii predate de profesor
  const getSubjectsByTeacher = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<SubjectProps[]>(`${timetableUrl}/subjects/holder-teacher/${teacherId}`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Examene pentru o materie
  const getExamsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<ExamProps[]>(`${examUrl}/subject/${subjectId}`);
      return response.data ?? [];
    },
    [axios]
  );

  const getExamsByStudent = useCallback(
    async (studentId: number) => {
      const response = await axios.get<ExamProps[]>(`${examUrl}/student/${studentId}`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Update exam
  const updateExam = useCallback(
    async (examData: {
      id: number;
      date: string;
      duration: number;
      classroomId: number;
      subjectId: number;
      studentGroupId: number;
    }) => {
      const response = await axios.put(`${examUrl}`, [examData]);
      return response.data;
    },
    [axios]
  );
  return {
    getLocations,
    getTeacherById,
    getStudentGroupsBySubject,
    getSubjectsByTeacher,
    getExamsBySubject,
    updateExam,
    getUserById,
    getTeacherbyUserId,
    getExamsByStudent,
    getLocation,
  };
};

export default useExamApi;
