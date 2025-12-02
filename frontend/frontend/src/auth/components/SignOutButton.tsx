import type { IPublicClientApplication, AccountInfo } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import useAuth from "../hooks/useAuth";

export interface SignOutButtonProps {
  text?: string;
}

const signOutClickHandler = (instance: IPublicClientApplication, account: AccountInfo | null) => {
  if (!account) {
    console.warn("MSAL: no account available to sign out");

    return;
  }

  const logoutRequest = {
    account: account,
    postLogoutRedirectUri: "http://localhost:5173/",
  };

  instance.logoutRedirect(logoutRequest);
};

const SignOutButton = ({ text }: SignOutButtonProps) => {
  const { instance } = useMsal();
  const { account } = useAuth();

  return <button onClick={() => signOutClickHandler(instance, account)}>{text ?? "Sign Out"}</button>;
};

export default SignOutButton;
