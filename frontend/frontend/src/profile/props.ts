export type Point = { x: number; y: number };

export type SignatureHandle = {
  clear: () => void;
  undo: () => void;
  toBlob: (type?: string, quality?: number) => Promise<Blob | null>;
};
