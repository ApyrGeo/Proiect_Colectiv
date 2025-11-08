import type { UserProps } from "../core/props.ts";

export interface HourProps {
  id?: number | null;
  day: string;
  hourInterval: string;
  frequency: string;
  location: LocationProps;
  locationUrl?: string | null;
  classroom: ClassroomProps;
  classroomUrl?: string | null;
  format: string;
  category?: string | null;
  subject: SubjectProps;
  subjectUrl?: string | null;
  teacher: TeacherProps;
  teacherUrl?: string | null;
  specialisation: string;
  isNext?: boolean;
  isCurrent?: boolean;
}

export interface LocationProps {
  id: number;
  name: string;
}

export interface ClassroomProps {
  id: number;
  name: string;
}

export interface SubjectProps {
  id: number;
  name: string;
  numberOfCredits: number;
}

export interface TeacherProps {
  id: number;
  userId?: number | null;
  user: UserProps;
}

export interface HourPropsDto {
  hours: HourProps[];
  calendarStartISODate: string;
}
