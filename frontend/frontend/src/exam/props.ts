export interface StudentGroupProps {
  id: number;
  name: string;
}

export interface SubjectProps {
  id: number;
  name: string;
}

export interface ClassroomProps {
  id: number;
  name: string;
}

export interface LocationProps {
  id: number;
  name: string;
  address: string;
  classrooms: ClassroomProps[];
}

export interface StudentIdProps {
  id: number;
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

export interface TeacherProps {
  id: number;
  userId: number;
  user: UserProps;
  facultyId: number;
}

export interface ExamProps {
  id: number;
  date: string | null;
  duration: number | null;
  classroom: ClassroomProps | null;
  subject: SubjectProps | null;
  studentGroup: StudentGroupProps | null;
}

export interface GroupRowProps {
  id: number;
  name: string;
  selectedLocationId: number | null;
  selectedClassroomId: number | null;
  examDate: string | null;
  examDuration: number | null;
  examId: number;
  selectedGroupId: number | null;
}

export interface StudentExamRowProps {
  examId: number;
  subjectName: string;
  examDate: string | null;
  examDuration: number | null;
  locationName: string | null;
  classroomName: string | null;
}
