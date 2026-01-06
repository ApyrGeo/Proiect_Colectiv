import type {
  SubjectProps,
  LocationProps,
  ClassroomProps,
} from "../timetable/props";

export interface SpecialisationProps {
  id: number;
  name: string;
}

export interface GroupProps {
  id: number;
  name: string;
  year: number;
  specialisationId: number;
}

export type Semester = 1 | 2;

export type Frequency = "Weekly" | "FirstWeek" | "SecondWeek";


export interface TimetableGenerationInput {
  specialisationId: number;
  year: number;
  semester: Semester;
  groupId: number;
  frequency: Frequency;

  locations: LocationProps[];
  classrooms: ClassroomProps[];
}

export interface TimetableGenerationPreview {
  subjects: SubjectProps[];
}

export interface EditableHourRow {
  id: string;

  day?: string;
  interval?: string;
  frequency?: Frequency;

  formationGroupId?: number;

  subjectId?: number;
  type?: "Lecture" | "Seminar" | "Laboratory";
  teacherId?: number;

  classroomId?: number;
  locationId?: number;
}

