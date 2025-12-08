export type Point = { x: number; y: number };

export type SignatureHandle = {
  clear: () => void;
  undo: () => void;
  toBlob: (type?: string, quality?: number) => Promise<Blob | null>;
};

export interface Profile {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber: string;
  signatureUrl?: string;
}
