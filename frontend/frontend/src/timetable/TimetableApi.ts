import axios from "axios";
import type { ClassroomProps, HourProps, SubjectProps, TeacherProps } from "./props.ts";

const baseUrl = "http://localhost:3000";
const hourUrl = `${baseUrl}/hours`;
const teacherUrl = `${baseUrl}/teachers`;
const subjectUrl = `${baseUrl}/subjects`;
const classroomUrl = `${baseUrl}/classrooms`;

interface ResponseProps<T> {
  data: T;
}

async function withLogs<T>(promise: Promise<ResponseProps<T>>, fnName: string): Promise<T> {
  console.log(fnName);
  try {
    const res = await promise;
    return res.data;
  } catch (err) {
    return await Promise.reject(err);
  }
}

const config = {
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "localhost:3000",
  },
};

export const getHours: () => Promise<HourProps[]> = () => {
  return withLogs(axios.get(hourUrl, config), "getHours");
};

export const getUserHours: (userId: number) => Promise<HourProps[]> = (userId: number) => {
  return withLogs(axios.get(hourUrl + "?userId=" + userId, config), "getUserHours");
};

export const getGroupYearHours: (groupYearId: number) => Promise<HourProps[]> = (groupYearId: number) => {
  return withLogs(axios.get(hourUrl + "?groupYearId=" + groupYearId, config), "getGroupYearHours");
};

export const getTeacherHours: (teacherId: number) => Promise<HourProps[]> = (teacherId: number) => {
  return withLogs(axios.get(hourUrl + "?teacherId=" + teacherId, config), "getTeacherHours");
};

export const getSubjectHours: (subjectId: number) => Promise<HourProps[]> = (subjectId: number) => {
  return withLogs(axios.get(hourUrl + "?subjectId=" + subjectId, config), "getSubjectHours");
};

export const getClassroomHours: (classroomId: number) => Promise<HourProps[]> = (classroomId: number) => {
  return withLogs(axios.get(hourUrl + "?classRoomId=" + classroomId, config), "getClassroomHours");
};

export const getSpecialisationHours: (specialisationId: number) => Promise<HourProps[]> = (
  specialisationId: number
) => {
  return withLogs(axios.get(hourUrl + "?specialisationId=" + specialisationId, config), "getSpecialisationHours");
};

export const getFacultyHours: (facultyId: number) => Promise<HourProps[]> = (facultyId: number) => {
  return withLogs(axios.get(hourUrl + "?facultyId=" + facultyId, config), "getFacultyHours");
};

export const getTeacher: (teacherId: number) => Promise<TeacherProps> = (teacherId: number) => {
  return withLogs(axios.get(teacherUrl + "?id=" + teacherId, config), "getTeacher");
};

export const getSubject: (subjectId: number) => Promise<SubjectProps> = (subjectId: number) => {
  return withLogs(axios.get(subjectUrl + "?id=" + subjectId, config), "getSubject");
};

export const getClassroom: (classroomId: number) => Promise<ClassroomProps> = (classroomId: number) => {
  return withLogs(axios.get(classroomUrl + "?id=" + classroomId, config), "getClassroom");
};
