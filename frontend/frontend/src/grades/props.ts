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

export interface TeacherProps {
  id: number;
  userId: number;
  user: UserProps;
  facultyId: number;
}

export interface UserProps {
  id: number;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  password: string | null;
  role: number;
}
