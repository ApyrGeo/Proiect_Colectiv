export type LoggedUserInfo = {
  user: UserProps;
  enrollments: UserEnrollments[];
};

export interface UserProps {
  id: number;
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

export type UserEnrollments = {
  enrollmentId: number;
  subGroupId: number;
  groupId: number;
  promotionId: number;
  specializationId: number;
  facultyId: number;
};
