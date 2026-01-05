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
  | (ContractFieldBase & { category: FieldCategory.NUMAR });

type StudyContractCall = (userId: number) => Promise<string>;
type ContractAPICall = StudyContractCall;

export const structure: ContractStructure[] = [
  {
    title: "Study Contract",
    fields: [
      {
        name: "fullName",
        label: "Full name",
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
        label: "Phone number",
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
        label: "Serie CI",
        category: FieldCategory.SERIE,
      },
      {
        name: "numar",
        label: "Număr CI",
        category: FieldCategory.NUMAR,
      },
      {
        name: "opt1",
        label: "Optional 1",
        category: FieldCategory.SELECT,
        options: ["Geometry", "IOC", "Robotics"],
      },
      {
        name: "agree",
        label: "I agree to the Terms",
        category: FieldCategory.CHECKBOX,
      },
    ],
  },
];
