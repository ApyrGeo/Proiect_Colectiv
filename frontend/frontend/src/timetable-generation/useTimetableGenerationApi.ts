import type { SubjectProps, LocationProps, ClassroomProps, HourProps } from "../timetable/props";
import type { ComboOption } from "./components/ComboBox";
import type { SpecialisationProps, GroupProps, Semester, Frequency } from "./props";
import { UserRole } from "../core/props.ts";


let GENERATED_HOURS: HourProps[] = [];

/* ------------------------------------------------- */

const useTimetableGenerationApi = () => {

  const getSpecialisations = async (): Promise<SpecialisationProps[]> => [
    { id: 1, name: "Informatics (IR)" },
    { id: 2, name: "Computer Science" },
  ];

  const getGroups = async (specialisationId: number, year: number): Promise<GroupProps[]> => [
    { id: 216, name: "216", year, specialisationId },
    { id: 217, name: "217", year, specialisationId },
  ];

  const getSubjects = async (specialisationId: number, year: number, semester: Semester): Promise<SubjectProps[]> => [
    { id: 1, name: "Algebra liniară", numberOfCredits: 5 },
    { id: 2, name: "Introducere în domeniu", numberOfCredits: 4 },
    { id: 3, name: "Programare", numberOfCredits: 6 },
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
    selectedSpecialisation: ComboOption<number> | null,
    year: ComboOption<number> | null,
    semester: ComboOption<Semester> | null
  ): Promise<HourProps[]> => {
    console.log(selectedSpecialisation, year, semester);
    if (selectedSpecialisation?.value === 2 && year?.value === 1 && semester?.value === 2) {
      return [
        {
          id: 1,
          day: "Monday",
          hourInterval: "08-10",
          frequency: "Weekly",
          location: {
            id: 1,
            name: "Facultatea de Chimie",
            googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
          },
          classroom: { id: 2, name: "315" },
          format: "Course",
          category: "Lecture",
          subject: { id: 3, name: "Programare", numberOfCredits: 6 },
          teacher: {
            id: 2,
            user: {
              firstName: "Ionescu",
              lastName: "Maria",
              email: "",
              id: 0,
              phoneNumber: "",
              password: "",
              role: UserRole.STUDENT,
            },
          },
          specialisation: "Computer Science",
        },
        {
          id: 2,
          day: "Monday",
          hourInterval: "10-12",
          frequency: "Weekly",
          location: {
            id: 1,
            name: "Facultatea de Chimie",
            googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
          },
          classroom: { id: 1, name: "217" },
          format: "Lab",
          category: "Laboratory",
          subject: { id: 3, name: "Programare", numberOfCredits: 6 },
          teacher: {
            id: 1,
            user: {
              firstName: "Popa",
              lastName: "Edgar",
              email: "",
              id: 0,
              phoneNumber: "",
              password: "",
              role: UserRole.STUDENT,
            },
          },
          specialisation: "Computer Science",
        },
        {
          id: 3,
          day: "Tuesday",
          hourInterval: "12-14",
          frequency: "Weekly",
          location: {
            id: 1,
            name: "Facultatea de Chimie",
            googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
          },
          classroom: { id: 1, name: "217" },
          format: "Course",
          category: "Seminar",
          subject: { id: 1, name: "Algebra liniară", numberOfCredits: 5 },
          teacher: {
            id: 1,
            user: {
              firstName: "Popa",
              lastName: "Edgar",
              email: "",
              id: 0,
              phoneNumber: "",
              password: "",
              role: UserRole.STUDENT,
            },
          },
          specialisation: "Computer Science",
        },
        {
          id: 4,
          day: "Wednesday",
          hourInterval: "14-16",
          frequency: "Weekly",
          location: {
            id: 1,
            name: "Facultatea de Chimie",
            googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
          },
          classroom: { id: 2, name: "315" },
          format: "Course",
          category: "Lecture",
          subject: {
            id: 2,
            name: "Introducere în domeniu",
            numberOfCredits: 4,
          },
          teacher: {
            id: 2,
            user: {
              firstName: "Ionescu",
              lastName: "Maria",
              email: "",
              id: 0,
              phoneNumber: "",
              password: "",
              role: UserRole.STUDENT,
            },
          },
          specialisation: "Computer Science",
        },
      ];
    }

    return GENERATED_HOURS;
  };

  const generateTimetable = async (subjects: SubjectProps[], frequency: Frequency): Promise<void> => {
    GENERATED_HOURS = subjects.map((subject, index) => ({
      id: index + 1,
      day: "Monday",
      hourInterval: `${8 + index * 2}-${10 + index * 2}`,
      frequency,
      location: {
        id: 1,
        name: "Facultatea de Chimie",
        googleMapsData: { id: "1", latitude: 46.77, longitude: 23.59 },
      },
      classroom: { id: 1, name: "217" },
      format: "Course",
      category: "Lecture",
      subject,
      teacher: {
        id: 1,
        user: {
          firstName: "Popa",
          lastName: "Edgar",
          email: "",
        },
      },
      specialisation: "IR",
    }));
  };

  return {
    getSpecialisations,
    getGroups,
    getSubjects,
    getLocations,
    getClassrooms,

    getGeneratedTimetable,
    generateTimetable,
  };
};

export default useTimetableGenerationApi;
