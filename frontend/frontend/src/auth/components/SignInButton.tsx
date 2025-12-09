import type { IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../authConfig";
import "../auth.css";
import { t } from "i18next";

export interface SignInButtonProps {
  text?: string;
}

const signInClickHandler = (instance: IPublicClientApplication) => {
  instance.loginRedirect(loginRequest);
};

const SignInButton = ({ text }: SignInButtonProps) => {
  const { instance } = useMsal();

  return (
    <button className="sign-in-button" onClick={() => signInClickHandler(instance)}>
      {text ?? t("Sign_In")}
    </button>
  );
};

export default SignInButton;
