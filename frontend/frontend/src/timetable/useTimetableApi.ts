import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type { ClassroomProps, HourPropsDto, SubjectProps, TeacherProps } from "./props";

type HourFilter = {
  userId?: number;
  classroomId?: number;
  subjectId?: number;
  teacherId?: number;
  facultyId?: number;
  specialisationId?: number;
  groupYearId?: number;
  currentWeekOnly?: boolean;
  semesterNumber?: number;
};

const useTimetableApi = () => {
  const { axios } = useApiClient();

  const timetableUrl = "/api/Timetable";
  const academicsUrl = "/api/Academics";

  const getHours = useCallback(async () => {
    const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`);
    return response.data;
  }, [axios]);

  const getUserHours = useCallback(
    async (userId: number) => {
      const params: HourFilter = { userId };
      const response = await axios.get(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getUserCurrentWeekHours = useCallback(
    async (userId: number) => {
      const params: HourFilter = { userId, currentWeekOnly: true };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getGroupYearHours = useCallback(
    async (groupYearId: number) => {
      const params: HourFilter = { groupYearId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getTeacherHours = useCallback(
    async (teacherId: number) => {
      const params: HourFilter = { teacherId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getSubjectHours = useCallback(
    async (subjectId: number) => {
      const params: HourFilter = { subjectId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getClassroomHours = useCallback(
    async (classroomId: number) => {
      const params: HourFilter = { classroomId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getSpecialisationHours = useCallback(
    async (specialisationId: number) => {
      const params: HourFilter = { specialisationId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getFacultyHours = useCallback(
    async (facultyId: number) => {
      const params: HourFilter = { facultyId };
      const response = await axios.get<HourPropsDto>(`${timetableUrl}/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const getTeacher = useCallback(
    async (teacherId: number) => {
      const response = await axios.get<TeacherProps>(`${academicsUrl}/teachers/${teacherId}`);
      return response.data;
    },
    [axios]
  );

  const getSubject = useCallback(
    async (subjectId: number) => {
      const response = await axios.get<SubjectProps>(`${academicsUrl}/subjects/${subjectId}`);
      return response.data;
    },
    [axios]
  );

  const getClassroom = useCallback(
    async (classroomId: number) => {
      const response = await axios.get<ClassroomProps>(`${academicsUrl}/classrooms/${classroomId}`);
      return response.data;
    },
    [axios]
  );

  return {
    getHours,
    getUserHours,
    getUserCurrentWeekHours,
    getGroupYearHours,
    getTeacherHours,
    getSubjectHours,
    getClassroomHours,
    getSpecialisationHours,
    getFacultyHours,
    getTeacher,
    getSubject,
    getClassroom,
  };
};

export default useTimetableApi;
