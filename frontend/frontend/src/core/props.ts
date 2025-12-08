export interface UserProps {
  id?: number | null;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  password: string;
  role: UserRole;
}

export enum UserRole {
  STUDENT,
  TEACHER,
  ADMIN,
}
