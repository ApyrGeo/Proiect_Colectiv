import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type {Location, ClassroomDetails, Teacher, StudentGroup, Subject, Exam, Enrollment, User} from "./props";

const useExamApi = () => {
  const { axios } = useApiClient();

  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";
  const examUrl = "/api/Exam";
  const userUrl = "/api/User";

  // 📌 Obține locații
  const getLocations = useCallback(async () => {
    const response = await axios.get<Location[]>(`${timetableUrl}/locations`);
    return response.data ?? [];
  }, [axios]);

  // 📌 Detalii clasă
  const getClassroomDetails = useCallback(
    async (classroomId: number) => {
      const response = await axios.get<ClassroomDetails>(`${timetableUrl}/classrooms/${classroomId}`);
      return response.data;
    },
    [axios]
  );

  // 📌 Profesor după ID (404 => null)
  const getTeacherById = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<Teacher>(`${academicsUrl}/teachers/${teacherId}`);
      return response.data;
    },
    [axios]
  );

  const getUserById = useCallback(
    async (userId: number) => {
      const response = await axios.get<User>(`${userUrl}/${userId}`);
      return response.data;
    },
    [axios]
  );

  // 📌 Grupuri la o materie
  const getStudentGroupsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<StudentGroup[]>(`${timetableUrl}/subjects/${subjectId}/groups`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Materii predate de profesor
  const getSubjectsByTeacher = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<Subject[]>(`${timetableUrl}/subjects/holder-teacher/${teacherId}`);
      return response.data ?? [];
    },
    [axios]
  );

  // 📌 Examene pentru o materie
  const getExamsBySubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<Exam[]>(`${examUrl}/subject/${subjectId}`);
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
      console.log(examData);
      const response = await axios.put(`${examUrl}`, [examData]);
      console.log(response);
      return response.data;
    },
    [axios]
  );
  return {
    getLocations,
    getClassroomDetails,
    getTeacherById,
    getStudentGroupsBySubject,
    getSubjectsByTeacher,
    getExamsBySubject,
    updateExam,
    getUserById,
  };
};

export default useExamApi;
