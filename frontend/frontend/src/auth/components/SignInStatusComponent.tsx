import { AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import SignInButton from "./SignInButton";
import { useTranslation } from "react-i18next";
import SignOutButton from "./SignOutButton";
import useAuth from "../hooks/useAuth";
import "../auth.css";
import { UserRole } from "../../core/props";

export default function SignInStatusComponent() {
  const { t } = useTranslation();
  const { userProps } = useAuth();

  return (
    <div style={{ display: "flex", justifyContent: "flex-end" }}>
      <div className="sign-in-status-card" style={{ display: "flex", flexDirection: "column", alignItems: "flex-end" }}>
        <AuthenticatedTemplate>
          <div style={{ display: "flex", alignItems: "center", gap: "12px", width: "100%" }}>
            <p className="sign-in-status-icon">
              {userProps?.role === UserRole.ADMIN && `🧑🏿‍🏫`}
              {userProps?.role === UserRole.TEACHER && `🧑‍🏫`}
              {userProps?.role === UserRole.STUDENT && `🧑‍🎓`}
            </p>
            <p className="sign-in-status-text">
              {t("You_are_logged_in_as") + ": "}
              <strong>{userProps ? userProps.firstName + " " + userProps?.lastName : ""}</strong>
            </p>
          </div>
          <SignOutButton />
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <p className="sign-in-status-text">{t("You_are_not_logged_in")}</p>
          <SignInButton />
        </UnauthenticatedTemplate>
      </div>
    </div>
  );
}
