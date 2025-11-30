export interface SubjectProps {
  name: string;
  credits: number;
}

export interface GradeItemProps {
  id: string;
  subject: SubjectProps;
  score: number;
  for_score: boolean;
  academicYear: string;
  specialization: string;
  studyYear: number;
  semester: number;
}

export interface GradesDataProps {
  average_score: number;
  result: GradeItemProps[];
}

export interface SpecializationResponse {
  name: string;
  id: number;
}

export interface ScholarshipStatus {
  averageScore: number;
  rank: number;
  totalStudents: number;
  isEligible: boolean;
  scholarshipType?: string | null;
}
