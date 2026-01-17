import type { AccountInfo, IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import useAuth from "../hooks/useAuth";
import "../auth.css";
import { t } from "i18next";

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
    postLogoutRedirectUri: import.meta.env.VITE_ENTRA_REDIRECT_URI,
  };

  instance.logoutRedirect(logoutRequest);
};

const SignOutButton = ({ text }: SignOutButtonProps) => {
  const { instance } = useMsal();
  const { activeAccount } = useAuth();

  return (
    <button className="sign-in-button" onClick={() => signOutClickHandler(instance, activeAccount)}>
      {text ?? t("Sign_Out")}
    </button>
  );
};

export default SignOutButton;
