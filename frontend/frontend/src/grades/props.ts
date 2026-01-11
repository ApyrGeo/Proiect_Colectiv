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

export interface UserProps {
  id: number;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  email?: string;
  owner?: string;
  tenantEmail?: string;
  password?: string | null;
  role?: number;
}

export interface TeacherProps {
  id: number;
  userId: number;
  user: UserProps;
  facultyId: number;
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

// Enrollment / SubGroup
export interface EnrollmentProps {
  id: number;
  userId: number;
  user: UserProps;
  subGroupId: number;
  subGroup: StudentSubGroupProps;
}

// Promotion / Semester
export interface PromotionProps {
  id: number;
  startYear: number;
  endYear: number;
  studentGroups: StudentGroupProps[];
  semesters: string[];
}

export interface PromotionYearProps {
  id: number;
  yearNumber: number;
  promotion: PromotionProps;
}

export interface SemesterProps {
  id: number;
  semesterNumber: number;
  promotionYear: PromotionYearProps;
}

// Grade
export interface GradeDetailProps {
  id: number;
  value: number | null;
  subject: SubjectProps;
  semester: SemesterProps;
  enrollment: EnrollmentProps;
}

// Grade entry for a student
export interface GradeEntry {
  user: {
    id: number;
    firstName: string;
    lastName: string;
  };
  grade: GradeDetailProps;
}

// Full response for a subject
export interface SubjectGradesResponse {
  subject: SubjectProps;
  studentGroup: StudentGroupProps;
  grades: GradeEntry[];
}

export interface SubjectProps {
  name: string;
  numberOfCredits: number;
}

export interface GradeItemProps {
  id: string;
  subject: SubjectProps;
  value: number;
}
