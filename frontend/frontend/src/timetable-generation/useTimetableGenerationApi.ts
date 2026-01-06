import type { SubjectProps, LocationProps, ClassroomProps, HourProps } from "../timetable/props";
import type { ComboOption } from "./components/ComboBox";
import type {
  SpecialisationProps,
  GroupProps,
  Semester,
  Frequency,
  FacultyProps,
  TimeTableGenerationProps,
} from "./props";
import TimetableGenerationPage from "./pages/TimetableGenerationPage.tsx";
import { useCallback } from "react";
import useApiClient from "../core/useApiClient.ts";

let GENERATED_HOURS: HourProps[] = [];

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

const updateHour = async (
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

  const getLocations = async (): Promise<LocationProps[]> => [
    {
      id: 1,
      name: "Facultatea de Chimie",
      googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
    },
  ];

  const getClassrooms = async (): Promise<ClassroomProps[]> => [
    { id: 1, name: "217" },
    { id: 2, name: "315" },
  ];

  const getGeneratedTimetable = useCallback(
    async (specialization: number, year: number, semester: number): Promise<TimeTableGenerationProps> => {
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
    async (
      hourId: number,
      payload: {
        day?: string;
        hourInterval?: string;
        frequency?: string;
        category?: string;
        classroomId?: number;
        subjectId?: number;
        teacherId?: number;
        studentGroupId?: number;
      }
    ) => {
      const response = await axios.put(`/api/Timetable/hours/${hourId}`, payload);

      return response.data;
    },
    [axios]
  );

  return {
    getFaculties,
    getLocations,
    getClassrooms,

    getGeneratedTimetable,
    generateTimetable,
    updateHour,
  };
};

export default useTimetableGenerationApi;
