export enum FieldCategory {
  TEXT = "text",
  NUMBER = "number",
  SELECT = "select",
  CHECKBOX = "checkbox",
  PHONE = "tel",
  EMAIL = "email",
  SERIE = "serie",
  CNP = "cnp",
  NUMAR = "numar",
  STUDY_YEAR = "studyYear",
}

export type ContractFieldBase = {
  name: string;
  label: string;
  autoFill?: boolean;
};

export type ContractStructure = {
  title: string;
  fields: ContractField[];
  apiCall?: ContractAPICall;
};

type ContractField =
  | (ContractFieldBase & { category: FieldCategory.TEXT })
  | (ContractFieldBase & { category: FieldCategory.CHECKBOX })
  | (ContractFieldBase & { category: FieldCategory.NUMBER })
  | (ContractFieldBase & { category: FieldCategory.PHONE })
  | (ContractFieldBase & { category: FieldCategory.EMAIL })
  | (ContractFieldBase & { category: FieldCategory.SELECT; options: string[] })
  | (ContractFieldBase & { category: FieldCategory.CNP })
  | (ContractFieldBase & { category: FieldCategory.SERIE })
  | (ContractFieldBase & { category: FieldCategory.NUMAR })
  | (ContractFieldBase & {
      category: FieldCategory.STUDY_YEAR;
      options: { label: string; value: number }[];
    });

type StudyContractCall = (userId: number) => Promise<string>;
type ContractAPICall = StudyContractCall;

export const structure: ContractStructure[] = [
  {
    title: "Study_Contract",
    fields: [
      {
        name: "fullName",
        label: "Full_name",
        category: FieldCategory.TEXT,
        autoFill: true,
      },
      {
        name: "email",
        label: "Email",
        category: FieldCategory.EMAIL,
        autoFill: true,
      },
      {
        name: "phone",
        label: "Phone",
        category: FieldCategory.PHONE,
        autoFill: true,
      },
      {
        name: "cnp",
        label: "CNP",
        category: FieldCategory.CNP,
      },
      {
        name: "serie",
        label: "ID_series",
        category: FieldCategory.SERIE,
      },
      {
        name: "numar",
        label: "ID_Number",
        category: FieldCategory.NUMAR,
      },
      {
        name: "agree",
        label: "Terms",
        category: FieldCategory.CHECKBOX,
      },
    ],
  },
];

export type OptionalField = ContractFieldBase & {
  category: FieldCategory.SELECT;
  options: { label: string; value: number }[];
  packageId: number;
};

export type StudyYearField = ContractFieldBase & {
  category: FieldCategory.STUDY_YEAR;
  options: { label: string; value: number }[];
};
