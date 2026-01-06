import type { HourProps } from "../timetable/props.ts";

export type Semester = 1 | 2;

export interface TimeTableGenerationProps {
  hours: HourProps[];
  calendarStartISODate: string;
}

export interface SpecialisationProps {
  id: number;
  name: string;
  groupYears: GroupYearProps[];
}

export interface GroupYearProps {
  id: number;
  startYear: number;
  endYear: number;
  studentGroups: GroupProps[];
}

export interface FacultyProps {
  id: number;
  name: string;
  specialisations: SpecialisationProps[];
}

export interface GroupProps {
  id: number;
  name: string;
  studentSubGroups: StudentSubGroupProps[];
}

export interface StudentSubGroupProps {
  id: number;
  name: string;
}

export interface EditableHourRow {
  id: string;
  day?: string;
  interval?: string;
  frequency?: string;
  type?: string;
  formationGroupId?: number;
  locationId?: number;
  classroomId?: number;
  subjectId?: number;
  teacherId?: number;
}
