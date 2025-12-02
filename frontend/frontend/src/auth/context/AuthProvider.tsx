import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from "@azure/msal-browser";
import { AuthContext } from "./AuthContext";
import useAuth from "../hooks/useAuth";

interface AuthProviderProps {
  instance: PublicClientApplication;
  children: React.ReactNode;
}

export const AuthProvider = ({ instance, children }: AuthProviderProps) => {
  return (
    <MsalProvider instance={instance}>
      <InnerAuthProvider>{children}</InnerAuthProvider>
    </MsalProvider>
  );
};

const InnerAuthProvider = ({ children }: { children: React.ReactNode }) => {
  const auth = useAuth();

  return <AuthContext.Provider value={auth}>{children}</AuthContext.Provider>;
};
