export enum FieldCategory {
  TEXT = "text",
  DATE = "date",
  NUMBER = "number",
  SELECT = "select",
  CHECKBOX = "checkbox",
  PHONE = "tel",
  EMAIL = "email",
}

type ContractField = { name: string; label: string } & (
  | { category: FieldCategory.TEXT }
  | { category: FieldCategory.DATE }
  | { category: FieldCategory.CHECKBOX }
  | { category: FieldCategory.NUMBER }
  | { category: FieldCategory.PHONE }
  | { category: FieldCategory.EMAIL }
  | { category: FieldCategory.SELECT; options: string[] }
);

export type ContractStructure = {
  title: string;
  fields: ContractField[];
  signature: boolean;
};

export const exampleStructures: ContractStructure[] = [
  {
    title: "My Contract 1",
    fields: [
      { name: "full-name", label: "Full name", category: FieldCategory.TEXT },
      { name: "email", label: "Email", category: FieldCategory.EMAIL },
      { name: "phone", label: "Phone number", category: FieldCategory.PHONE },
      { name: "opt1", label: "Optional 1", category: FieldCategory.SELECT, options: ["Geometry", "IOC", "Robotics"] },
      { name: "opt2", label: "Optional 2", category: FieldCategory.SELECT, options: ["VR", "POP", "Blockchain"] },
      { name: "agree", label: "I agree to the Terms and Conditions", category: FieldCategory.CHECKBOX },
    ],
    signature: false,
  },
  {
    title: "My Contract 2",
    fields: [
      { name: "full-name", label: "Full name", category: FieldCategory.TEXT },
      { name: "date", label: "Date of Birth", category: FieldCategory.DATE },
      { name: "opt1", label: "Optional 1", category: FieldCategory.SELECT, options: ["Geometry", "IOC", "Robotics"] },
      { name: "agree", label: "I agree to the Terms and Conditions", category: FieldCategory.CHECKBOX },
    ],
    signature: true,
  },
];
