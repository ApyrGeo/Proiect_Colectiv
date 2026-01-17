import React, { useRef, useState, useEffect } from "react";
import "../profile.css";
import Signature from "../components/Signature.tsx";
import { useTranslation } from "react-i18next";
import logoUrl from "/src/assets/UBB_Logo.png";
import useUserApi from "../ProfileApi.ts";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";
import toast from "react-hot-toast";

const ProfilePage: React.FC = () => {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [, setSignatureBase64] = useState<string | null>(null);

  const { t } = useTranslation();
  const { userProps } = useAuthContext();
  const { fetchUserProfile, updateUserSignature } = useUserApi();

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const sigRef = useRef<any>(null);

  useEffect(() => {
    if (!userProps?.id) return;

    const loadProfile = async () => {
      try {
        const profile = await fetchUserProfile(userProps.id as number);
        setFullName(`${profile.firstName} ${profile.lastName}`);
        setEmail(profile.email);
        setPhone(profile.phoneNumber ?? "");
        if (profile.signatureUrl) {
          setSignatureBase64(profile.signatureUrl);
          setTimeout(() => {
            sigRef.current?.fromBase64(profile.signatureUrl);
          }, 100);
        }
      } catch (err) {
        toast.error(t("Error_loading_profile"));
        console.error("Failed to load profile", err);
      }
    };

    loadProfile();
  }, [userProps?.id]);

  const uploadSignature = async () => {
    if (!userProps?.id) {
      toast.error(t("User_not_loaded"));
      return;
    }

    const blob: Blob | null = await sigRef.current?.toBlob("image/png", 0.92);
    if (!blob) {
      toast.error(t("No_signature_to_save"));
      return;
    }

    const reader = new FileReader();

    reader.onloadend = async () => {
      const base64 = (reader.result as string).split(",")[1];

      await updateUserSignature(userProps.id as number, base64);
      setSignatureBase64(base64);
      toast.success(t("Signature_saved_successfully"));
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
            <img src={logoUrl} alt="UBB LOGO" />
          </div>
          <h2>{fullName}</h2>
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
