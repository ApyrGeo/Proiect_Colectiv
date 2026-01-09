import type { TeacherProps } from "../exam/props";
import type {
  LocationProps,
  FacultyProps,
  TimeTableGenerationProps,
  PutTimeTableGenerationDto,
} from "./props";
import { useCallback } from "react";
import useApiClient from "../core/useApiClient.ts";

type HourFilter = {
  userId?: number;
  classroomId?: number;
  subjectId?: number;
  teacherId?: number;
  facultyId?: number;
  specialisationId?: number;
  groupYearId?: number;
  currentWeekTimetable?: boolean;
  semesterNumber?: number;
};

export const updateHour = async (
  hourId: number,
  patch: Partial<{
    day: string;
    hourInterval: string;
    frequency: string;
    category: string;
    classroomId: number;
    subjectId: number;
    teacherId: number;
    studentGroupId: number;
  }>
): Promise<void> => {
  await fetch(`/api/Timetable/hours/${hourId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(patch),
  });
};

const useTimetableGenerationApi = () => {
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

  const getLocations = useCallback(async (): Promise<LocationProps[]> => {
    const response = await axios.get(`/api/Timetable/locations`);
    return response.data;
  }, [axios]);

  const getGeneratedTimetable = useCallback(
    async (specialization: number, _year: number, semester: number): Promise<TimeTableGenerationProps> => {
      const params: HourFilter = { semesterNumber: semester, specialisationId: specialization };
      const response = await axios.get<TimeTableGenerationProps>(`/api/Timetable/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const generateTimetable = useCallback(
    async (specialisationId: number, year: number, semester: number) => {
      const response = await axios.post("/api/Timetable/hours/generate-timetable", {
        specialisationId,
        year,
        semester,
      });
      return response.data;
    },
    [axios]
  );

  const updateHour = useCallback(
    async (hourId: number, payload: PutTimeTableGenerationDto) => {
      console.log(payload);
      const response = await axios.put(`/api/Timetable/hours/${hourId}`, payload);

      return response.data;
    },
    [axios]
  );

  return {
    getTeachers,
    getFaculties,
    getLocations,

    getGeneratedTimetable,
    generateTimetable,
    updateHour,
  };
};

export default useTimetableGenerationApi;
