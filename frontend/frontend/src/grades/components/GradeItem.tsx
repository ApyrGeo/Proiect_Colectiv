import React from "react";
import "../grades.css";
import type { GradeItem } from "../props";

interface GradeItemProps {
  item: GradeItem;
}

const GradeItemComponent: React.FC<GradeItemProps> = ({ item }) => {
  let rowClass = "";
  if (!item.for_score) rowClass = "not-counted";
  else rowClass = item.score >= 5 ? "pass" : "fail";

  return (
    <div key={item.id} className={`grade-card ${rowClass}`}>
      <div className="subject-name">{item.subject.name}</div>
      <div className="credit-badge">
        <i>🎓</i> Credite: {item.subject.credits}
      </div>
      <div className={`grade-badge ${rowClass}`}>Nota: {item.score}</div>
    </div>
  );
};

export default GradeItemComponent;
