import React, { useRef, useState, useEffect } from "react";
import "../profile.css";
import Signature from "../components/Signature.tsx";
import { useTranslation } from "react-i18next";
import { fetchUserProfile, updateUserSignature } from "../ProfileApi.ts";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";

const ProfilePage: React.FC = () => {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");

  const { t } = useTranslation();
  const { userProps } = useAuthContext();

  const sigRef = useRef<any>(null);

  useEffect(() => {
    if (!userProps?.id) return;

    const loadProfile = async () => {
      try {
        const profile = await fetchUserProfile(userProps.id as number);
        setFullName(`${profile.firstName} ${profile.lastName}`);
        setEmail(profile.email);
        setPhone(profile.phoneNumber ?? "");
      } catch (err) {
        console.error("Failed to load profile", err);
      }
    };

    loadProfile();
  }, [userProps?.id]);

  const uploadSignature = async () => {
    if (!userProps?.id) {
      alert("User not loaded");
      return;
    }

    const blob: Blob | null = await sigRef.current?.toBlob("image/png", 0.92);
    if (!blob) return alert("No signature drawn");

    const reader = new FileReader();

    reader.onloadend = async () => {
      const base64 = (reader.result as string).split(",")[1];

      await updateUserSignature(userProps.id as number, base64);
      alert("Signature saved");
    };

    reader.readAsDataURL(blob);
  };
  return (
    <div className="profile-page">
      <h1 className="profile-title">{t("ProfileSettings")}</h1>
      <p className="profile-subtext">{t("ManageAccountDetails")}</p>

      <div className="profile-grid">
        <div className="profile-card">
          <div className="profile-avatar">
            <img src="src/assets/UBB_Logo.png" alt="UBB LOGO" />
          </div>
          <h2>Full Name</h2>
        </div>

        <div className="profile-form">
          <div className="form-group">
            <label>{t("Name")}</label>
            <input type="text" value={fullName} readOnly={true} />
          </div>

          <div className="form-group">
            <label>Email</label>
            <input type="email" value={email} readOnly={true} />
          </div>

          <div className="form-group">
            <label>{t("Phone")}</label>
            <input type="text" value={phone} readOnly={true} />
          </div>

          <div className="form-group">
            <label>{t("Signature")}</label>
            <div className="signature-container">
              <Signature ref={sigRef} />
            </div>
          </div>

          <div className="button-container">
            <button className="btn-save" onClick={uploadSignature}>
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
