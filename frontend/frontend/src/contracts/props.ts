export type StudyContractPayload = {
  promotionId: number;
  fields: Record<string, any>;
};

export type SubjectProps = {
  id: number;
  name: string;
  numberOfCredits: number;
  code: string;
  type: string;
};

export type OptionalPackageProps = {
  packageId: number;
  yearNumber: number;
  semesterNumber: number;
  subjects: SubjectProps[];
  semester1or2: number;
};

export type PromotionProps = {
  id: number;
  startYear: number;
  endYear: number;
};

export type PromotionSelect = {
  id: number;
  yearDuration: number;
  prettyName: string;
}