import type { IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";

export interface SignInButtonProps {
  text?: string;
}

const signInClickHandler = (instance: IPublicClientApplication) => {
  instance.loginRedirect();
};

const SignInButton = ({ text }: SignInButtonProps) => {
  const { instance } = useMsal();

  return <button onClick={() => signInClickHandler(instance)}>{text ?? "Sign In"}</button>;
};

export default SignInButton;
