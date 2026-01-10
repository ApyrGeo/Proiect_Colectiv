export interface FacultyProps {
  id: number;
  name: string;
  specialisations: SpecialisationProps[];
}

export interface SemesterProps {
  id: number;
  semesterNumber: number;
}

export interface SpecialisationProps {
  id: number;
  name: string;
  promotions: GroupYearProps[];
}

export interface SubjectProps {
  id: number;
  code: string;
  name: string;
  numberOfCredits: number;
  type: string;
}

export interface GroupYearProps {
  id: number;
  startYear: number;
  endYear: number;
  studentGroups: GroupProps[];
  semesters: SemesterProps[];
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
