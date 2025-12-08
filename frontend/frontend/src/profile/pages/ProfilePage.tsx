import React, { useRef, useState } from "react";
import "../profile.css";
import Signature from "../components/Signature.tsx";
import { useTranslation } from "react-i18next";
import logoUrl from '/src/assets/UBB_Logo.png'

const ProfilePage: React.FC = () => {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");

  const { t } = useTranslation();

  const sigRef = useRef<any>(null);

  const uploadSignature = async () => {
    console.log("Signature", sigRef.current);
    const blob: Blob | null = await sigRef.current?.toBlob("image/png", 0.92);
    if (!blob) return alert("No signature drawn");
    const fd = new FormData();
    fd.append("signature", blob, `signature-${Date.now()}.png`);
    console.log("signature", blob);
    console.log(fd);
  };

  const saveTest = async () => {
    const blob = await sigRef.current?.toBlob();
    if (!blob) return;

    const reader = new FileReader();
    reader.onloadend = () => {
      console.log("BASE64:", reader.result);
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
            <button className="btn-save" onClick={saveTest}>{t("SaveChanges")}</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
