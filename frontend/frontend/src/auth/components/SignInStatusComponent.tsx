import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import SignInButton from "./SignInButton";
import { useTranslation } from "react-i18next";
import SignOutButton from "./SignOutButton";
import useAuth from "../hooks/useAuth";

export default function SignInStatusComponent() {
  const { t } = useTranslation();
  const { userProps } = useAuth();

  return (
    <div style={{ display: "flex", justifyContent: "flex-end" }}>
      <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-end", height: "80px" }}>
        <AuthenticatedTemplate>
          <p>{t("You_are_logged_in_as") + ": " + (userProps ? userProps.firstName + " " + userProps?.lastName : "")}</p>
          <SignOutButton />
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <p>{t("You_are_not_logged_in")}</p>
          <SignInButton />
        </UnauthenticatedTemplate>
      </div>
    </div>
  );
}
