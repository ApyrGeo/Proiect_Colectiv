export interface UserProps {
  id?: number | null;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  password: string;
  role: string;
}

export type UserInfo = {
  userId: number;
  userRole: string;
  groupYearId?: number;
  facultyId?: number;
  specialisationId?: number;
};
