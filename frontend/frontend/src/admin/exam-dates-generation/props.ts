import type {UserProps} from "../../grades/props.ts";

export interface FacultyProps {
  id: number;
  name: string;
  specialisations: SpecialisationProps[];
}

export interface SpecialisationProps {
  id: number;
  name: string;
  promotions: GroupYearProps[];
}

export interface GroupYearProps {
  id: number;
  startYear: number;
  endYear: number;
  studentGroups: GroupProps[];
  semesters: SemesterProps[];
}

export interface SemesterProps {
  id: number;
  semesterNumber: number;
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

export interface TeacherProps {
  id: number;
  userId: number;
  user: UserProps;
  facultyId: number;
}

export interface SubjectProps {
  id: number;
  name: string;
  numberOfCredits: number;
  code: string;
  type: string;
}

export interface StudentSubGroupProps {
  id: number;
  name: string;
}

export interface StudentGroupProps {
  id: number;
  name: string;
  studentSubGroups: StudentSubGroupProps[];
}
