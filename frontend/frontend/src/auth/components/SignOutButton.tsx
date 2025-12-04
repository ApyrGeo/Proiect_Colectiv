import type { AccountInfo, IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import useAuth from "../hooks/useAuth";

export interface SignOutButtonProps {
  text?: string;
}

const signOutClickHandler = (instance: IPublicClientApplication, activeAccount: AccountInfo | null) => {
  if (!activeAccount) {
    console.warn("MSAL: no account available to sign out");

    return;
  }

  const logoutRequest = {
    account: activeAccount,
    postLogoutRedirectUri: "http://localhost:5173/",
  };

  instance.logoutRedirect(logoutRequest);
};

const SignOutButton = ({ text }: SignOutButtonProps) => {
  const { instance } = useMsal();
  const { activeAccount } = useAuth();

  return <button onClick={() => signOutClickHandler(instance, activeAccount)}>{text ?? "Sign Out"}</button>;
};

export default SignOutButton;
