import React from "react";
import "../grades.css";
import type { GradeItemProps } from "../props";
import { useTranslation } from "react-i18next";

const GradeItemComponent: React.FC<GradeItemProps> = ({ id, subject, score, for_score }) => {
  const { t } = useTranslation();

  let rowClass = "";
  if (!for_score) rowClass = "not-counted";
  else rowClass = score >= 5 ? "pass" : "fail";

  return (
    <div key={id} className={`grade-card ${rowClass}`}>
      <div className="subject-name">{subject.name}</div>
      <div className="credit-badge">
        <i>🎓</i> {t("Credits")}: {subject.credits}
      </div>
      <div className={`grade-badge ${rowClass}`}>
        {t("Grade")}: {score}
      </div>
    </div>
  );
};

export default GradeItemComponent;
