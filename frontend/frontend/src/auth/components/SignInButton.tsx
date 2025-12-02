import type { IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../authConfig";

export interface SignInButtonProps {
  text?: string;
}

const signInClickHandler = (instance: IPublicClientApplication) => {
  instance.loginRedirect(loginRequest);
};

const SignInButton = ({ text }: SignInButtonProps) => {
  const { instance } = useMsal();

  return <button onClick={() => signInClickHandler(instance)}>{text ?? "Sign In"}</button>;
};

export default SignInButton;
