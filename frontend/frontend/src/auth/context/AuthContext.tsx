import { createContext, useContext } from "react";
import type { AccountInfo } from "@azure/msal-browser";

export type AuthContextType = {
  accessToken: string | null;
  activeAccount: AccountInfo | null;
  loading: boolean;
  error: unknown | null;
  acquireToken: () => Promise<string | null>;
};

export const AuthContext = createContext<AuthContextType | null>(null);

export const useAuthContext = () => {
  const ctx = useContext(AuthContext);

  if (!ctx) {
    throw new Error("useAuthContext must be used inside <AuthProvider>");
  }

  return ctx;
};
