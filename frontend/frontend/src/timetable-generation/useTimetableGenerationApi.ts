import type { TeacherProps } from "../exam/props";
import type { LocationProps, FacultyProps, TimeTableGenerationProps, PutTimeTableGenerationDto } from "./props";
import { useCallback } from "react";
import useApiClient from "../core/useApiClient.ts";

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
    async (groupYearId: number, semesterNumber: number): Promise<TimeTableGenerationProps> => {
      const params = { semesterNumber: 2 - (semesterNumber % 2), groupYearId: groupYearId };
      const response = await axios.get<TimeTableGenerationProps>(`/api/Timetable/hours`, {
        params,
      });
      return response.data;
    },
    [axios]
  );

  const generateTimetable = useCallback(
    async (specialisationId: number, semesterId: number) => {
      console.log(specialisationId, semesterId);
      const response = await axios.post("/api/Timetable/hours/generate-timetable", {
        specialisationId,
        semesterId,
      });
      return response.data;
    },
    [axios]
  );

  const updateHour = useCallback(
    async (hourId: number, payload: PutTimeTableGenerationDto) => {
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
