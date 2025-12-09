import { useTranslation } from "react-i18next";
import logoUrl from '/src/assets/UBB_Logo.png'
import "./homepage.css";

export default function Homepage() {
  const { t } = useTranslation();
  return (
    <div className="App">
      <header className="header">
        <img src={logoUrl} className="logo" alt="UBB Logo" />
        <h1>{t("Welcome")} Track for UBB</h1>
        <p>{t("Paragraf")}</p>
      </header>
      <footer className="footer">© 2025 Track for UBB. All rights reserved.</footer>
    </div>
  );
}
