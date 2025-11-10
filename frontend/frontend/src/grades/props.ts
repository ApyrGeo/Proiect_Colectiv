export interface Subject {
  name: string;
  credits: number;
}

export interface GradeItem {
  id: string;
  subject: Subject;
  score: number;
  for_score: boolean;
  academicYear: string;
  specialization: string;
  studyYear: number;
  semester: number;
}

export interface GradesData {
  average_score: number;
  result: GradeItem[];
}
