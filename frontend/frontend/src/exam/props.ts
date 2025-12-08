export interface StudentGroup {
  id: number;
  name: string;
}

export interface StudentGroupDetails {
  id: number;
  name: string;
  studentSubGroups: never[];
}

export interface Subject {
  id: number;
  name: string;
}

export interface Classroom {
  id: number;
  name: string;
}

export interface ClassroomDetails {
  id: number;
  name: string;
  capacity?: number;
}

export interface Location {
  id: number;
  name: string;
  address: string;
  classrooms: Classroom[];
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  password: string | null;
  role: number;
}

export interface Teacher {
  id: number;
  userId: number;
  user: User;
  facultyId: number;
}

export interface Exam {
  id: number;
  date: string | null;
  duration: number | null;
  classroom: Classroom | null;
  subject: Subject;
  studentGroup: StudentGroup;
}

export interface Enrollment {
  id: number;
  userId: number;
  user: User;
  subGroupId: number;
  subGroup: SubGroup;
}

export interface SubGroup {
  id: number;
  name: string;
}