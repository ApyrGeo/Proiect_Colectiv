import type { SubjectProps, LocationProps, ClassroomProps, HourProps } from "../timetable/props";
import type { ComboOption } from "./components/ComboBox";
import type { SpecialisationProps, GroupProps, Semester, Frequency, FacultyProps } from "./props";


let GENERATED_HOURS: HourProps[] = [];

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

  const getFaculties = async (): Promise<FacultyProps[]> => [
    {
      id: 20,
      name: "Facultatea de Matematica si Informatica",
      specialisations: [
        {
          id: 73,
          name: "Informatics Romana",
          groupYears: [],
        },
        {
          id: 74,
          name: "Informatica Engleza",
          groupYears: [],
        },
      ],
    },
  ];

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


  const getGeneratedTimetable = async (
    specialisation: ComboOption<number> | null,
    year: ComboOption<number> | null,
    semester: ComboOption<Semester> | null
  ): Promise<HourProps[]> => {
    if (!specialisation || !year || !semester) return [];

    const res = await fetch(
      `/api/Timetable/hours?specialisationId=${specialisation.value}&groupYearId=${year.value}&semesterNumber=${semester.value}`
    );

    if (!res.ok) return [];
    return res.json();
  };

  const generateTimetable = async (
    specialisationId: number,
    year: number,
    semester: Semester
  ): Promise<void> => {
    await fetch("/api/Timetable/hours/generate-timetable", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        specialisationId,
        year,
        semester,
      }),
    });
  };

  const updateHour = async (
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
  ): Promise<void> => {
    await fetch(`/api/Timetable/hours/${hourId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
  };

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
