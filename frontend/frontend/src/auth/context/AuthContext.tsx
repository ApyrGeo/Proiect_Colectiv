import { createContext, useContext } from "react";
import type { AccountInfo } from "@azure/msal-browser";
import type { UserEnrollments, UserProps } from "../../core/props.ts";

export type AuthContextType = {
  accessToken: string | null;
  activeAccount: AccountInfo | null;
  loading: boolean;
  error: unknown | null;
  waitForAccessToken: () => Promise<string | null>;
  isFullfilled: boolean;
  userProps: UserProps | null;
  userEnrollments: UserEnrollments[] | null;
};

export const AuthContext = createContext<AuthContextType | null>(null);

export const useAuthContext = () => {
  const ctx = useContext(AuthContext);

  if (!ctx) {
    throw new Error("useAuthContext must be used inside <AuthProvider>");
  }

  return ctx;
};
