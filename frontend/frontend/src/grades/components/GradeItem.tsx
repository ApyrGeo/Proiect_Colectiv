import React from "react";
import "../grades.css";
import type { GradeItemProps } from "../props";
import { useTranslation } from "react-i18next";

const GradeItemComponent: React.FC<GradeItemProps> = ({ id, subject, value }) => {
  const { t } = useTranslation();
  let rowClass = "";
  rowClass = value >= 5 ? "pass" : "fail";

  return (
    <div key={id} className={`grade-card ${rowClass}`}>
      <div className="subject-name">{subject.name}</div>
      <div className="credit-badge">
        <i>🎓</i> {t("Credits")}: {subject.numberOfCredits}
      </div>
      <div className={`grade-badge ${rowClass}`}>
        {t("Grade")}: {value}
      </div>
    </div>
  );
};

export default GradeItemComponent;
