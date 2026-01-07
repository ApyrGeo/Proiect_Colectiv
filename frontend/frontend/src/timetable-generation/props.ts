import type { TeacherProps } from "../exam/props.ts";

export type Semester = 1 | 2;

export interface PutTimeTableGenerationDto {
  id: number;
  day: string;
  hourInterval: string;
  frequency?: string;
  category: string;
  classroomId: number | null;
  subjectId?: number;
  teacherId: number | null;
  studentGroupId: number | null;
  studentSubGroupId: number | null;
  groupYearId: number | null;
}

export interface LocationProps {
  id: number;
  name: string;
  address: string;
  classrooms: ClassroomProps[];
}

export interface ClassroomProps {
  id: number;
  name: string;
  locationId: number;
}

export interface TimeTableGenerationProps {
  hours: EditableHourRow[];
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
  category?: string;
  classroom?: ClassroomProps;
  day: string;
  frequency: string;
  format: string;
  hourInterval: string;
  id?: number;
  location?: LocationProps;
  promotion?: GroupYearProps;
  studentGroup?: GroupProps;
  studentSubGroup?: StudentSubGroupProps;
  subject?: SubjectProps;
  teacher?: TeacherProps;
}

export interface SubjectProps {
  id: number;
  code: string;
  name: string;
  numberOfCredits: number;
  type: string;
}


