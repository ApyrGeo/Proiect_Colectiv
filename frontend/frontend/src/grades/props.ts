export interface SubjectProps {
  name: string;
  numberOfCredits: number;
}

export interface GradeItemProps {
  id: string;
  subject: SubjectProps;
  value: number;
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