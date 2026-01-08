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
  subjects: SubjectProps[];
};

export type PromotionProps = {
  id: number;
  startYear: number;
  endYear: number;
};