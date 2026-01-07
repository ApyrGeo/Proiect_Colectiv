export interface SubjectProps {
  id: number;
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

type GradeEntry = {
  user: {
    id: number;
    firstName: string;
    lastName: string;
  };
  grade: number | null;
};

export type SubjectGradesResponse = {
  subject: {
    id: number;
    name: string;
    numberOfCredits: number;
    code: string;
    type: string;
  };
  studentGroup: {
    id: number;
    name: string;
  };
  grades: GradeEntry[];
};

export interface StudentGroupProps {
  id: number;
  name: string;
}
