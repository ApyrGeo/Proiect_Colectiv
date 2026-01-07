export type StudyContractPayload = {
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