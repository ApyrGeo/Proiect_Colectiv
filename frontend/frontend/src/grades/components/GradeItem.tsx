import React from "react";
import "../grades.css";
import type { GradeItemProps } from "../props";

const GradeItemComponent: React.FC<GradeItemProps> = ({ id, subject, score, for_score }) => {
  let rowClass = "";
  if (!for_score) rowClass = "not-counted";
  else rowClass = score >= 5 ? "pass" : "fail";

  return (
    <div key={id} className={`grade-card ${rowClass}`}>
      <div className="subject-name">{subject.name}</div>
      <div className="credit-badge">
        <i>🎓</i> Credite: {subject.credits}
      </div>
      <div className={`grade-badge ${rowClass}`}>Nota: {score}</div>
    </div>
  );
};

export default GradeItemComponent;
