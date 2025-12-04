import React, { useRef, useState } from "react";
import "../profile.css";
import Signature from "../components/Signature.tsx";

const ProfilePage: React.FC = () => {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const sigRef = useRef<any>(null);

  const uploadSignature = async () => {
    const blob: Blob | null = await sigRef.current?.toBlob("image/png", 0.92);
    if (!blob) return alert("No signature drawn");
    const fd = new FormData();
    fd.append("signature", blob, `signature-${Date.now()}.png`);
    // u
  };

  return (
    <div className="profile-page">
      <h1 className="profile-title">Profile Settings</h1>
      <p className="profile-subtext">Manage your account details and personal information.</p>

      <div className="profile-grid">
        <div className="profile-card">
          <div className="profile-avatar">
            <img src="src/assets/UBB_Logo.png" alt="UBB LOGO" />
          </div>
          <h2>Full Name</h2>
        </div>

        <div className="profile-form">
          <div className="form-group">
            <label>Name</label>
            <input type="text" value={fullName} readOnly={true} />
          </div>

          <div className="form-group">
            <label>Email Address</label>
            <input type="email" value={email} readOnly={true} />
          </div>

          <div className="form-group">
            <label>Phone</label>
            <input type="text" value={phone} readOnly={true} />
          </div>

          <div className="form-group">
            <label>Change Password</label>
            <input type="password" placeholder="Enter a new password" />
          </div>

          <div className="form-group">
            <label>Signature</label>
            <div className="signature-container">
              <Signature ref={sigRef} />
            </div>
          </div>

          <div className="button-container">
            <button className="btn-save">Save Changes</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
