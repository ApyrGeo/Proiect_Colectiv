import axios from "axios";
import type { ClassroomProps, HourPropsDto, SubjectProps, TeacherProps } from "./props.ts";

const timetableUrl = "http://localhost:5206/api/Timetable";
const academicsUrl = "http://localhost:5206/api/Academics";
const hourUrl = `${timetableUrl}/hours`;
const teacherUrl = `${academicsUrl}/teachers`;
const subjectUrl = `${timetableUrl}/subjects`;
const classroomUrl = `${timetableUrl}/classrooms`;

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
    "Access-Control-Allow-Origin": "localhost:5206",
  },
};

export const getHours: () => Promise<HourPropsDto> = () => {
  return withLogs(axios.get(hourUrl, config), "getHours");
};

export const getUserHours: (userId: number) => Promise<HourPropsDto> = (userId: number) => {
  return withLogs(axios.get(hourUrl + "?userId=" + userId, config), "getUserHours");
};

export const getUserCurrentWeekHours: (userId: number) => Promise<HourPropsDto> = (userId: number) => {
  return withLogs(
    axios.get(hourUrl + "?userId=" + userId + "&currentWeekTimetable=true", config),
    "getUserCurrentWeekHours"
  );
};

export const getGroupYearHours: (groupYearId: number) => Promise<HourPropsDto> = (groupYearId: number) => {
  return withLogs(axios.get(hourUrl + "?groupYearId=" + groupYearId, config), "getGroupYearHours");
};

export const getTeacherHours: (teacherId: number) => Promise<HourPropsDto> = (teacherId: number) => {
  return withLogs(axios.get(hourUrl + "?teacherId=" + teacherId, config), "getTeacherHours");
};

export const getSubjectHours: (subjectId: number) => Promise<HourPropsDto> = (subjectId: number) => {
  return withLogs(axios.get(hourUrl + "?subjectId=" + subjectId, config), "getSubjectHours");
};

export const getClassroomHours: (classroomId: number) => Promise<HourPropsDto> = (classroomId: number) => {
  return withLogs(axios.get(hourUrl + "?classRoomId=" + classroomId, config), "getClassroomHours");
};

export const getSpecialisationHours: (specialisationId: number) => Promise<HourPropsDto> = (
  specialisationId: number
) => {
  return withLogs(axios.get(hourUrl + "?specialisationId=" + specialisationId, config), "getSpecialisationHours");
};

export const getFacultyHours: (facultyId: number) => Promise<HourPropsDto> = (facultyId: number) => {
  return withLogs(axios.get(hourUrl + "?facultyId=" + facultyId, config), "getFacultyHours");
};

export const getTeacher: (teacherId: number) => Promise<TeacherProps> = (teacherId: number) => {
  return withLogs(axios.get(teacherUrl + "/" + teacherId, config), "getTeacher");
};

export const getSubject: (subjectId: number) => Promise<SubjectProps> = (subjectId: number) => {
  return withLogs(axios.get(subjectUrl + "/" + subjectId, config), "getSubject");
};

export const getClassroom: (classroomId: number) => Promise<ClassroomProps> = (classroomId: number) => {
  return withLogs(axios.get(classroomUrl + "/" + classroomId, config), "getClassroom");
};
