import React from "react";
import "../grades.css";
import type { GradeItemProps } from "../props";

const GradeItemComponent: React.FC<GradeItemProps> = ({ id, subject, value }) => {
  let rowClass = "";
  rowClass = value >= 5 ? "pass" : "fail";

  return (
    <div key={id} className={`grade-card ${rowClass}`}>
      <div className="subject-name">{subject.name}</div>
      <div className="credit-badge">
        <i>🎓</i> Credite: {subject.numberOfCredits}
      </div>
      <div className={`grade-badge ${rowClass}`}>Nota: {value}</div>
    </div>
  );
};

export default GradeItemComponent;
