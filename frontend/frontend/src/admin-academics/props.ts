export type PromotionPostData = {
  startYear: number;
  endYear: number;
  specialisationId: number;
};
export type FacultyPostData = {
  name: string;
};

export type SpecialisationPostData = {
  name: string;
  facultyId: number;
};

export type StudentGroupPostData = {
  name: string;
  groupYearId: number; // == promotionId
};

export type StudentSubGroupPostData = {
  name: string;
  studentGroupId: number;
};

export type Promotion = {
  id: number;
  startYear: number;
  endYear: number;
};
export type Faculty = {
  id: number;
  name: string;
  specialisations: Specialisation[];
};

export type Specialisation = {
  id: number;
  name: string;
  groupYears: Promotion[];
};

export type FacultiesResponse = { faculties: Faculty[] };

export type StudentGroup = {
  id: number;
  name: string;
  groupYearId: number; // == promotionId
};
//
// export type StudentSubGroup = {
//   name: string;
//   studentGroupId: number;
// };
