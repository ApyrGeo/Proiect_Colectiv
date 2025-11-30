import React, { useEffect, useState } from "react";
import "../grades.css";
import { fetchUserSpecializations, fetchStatusForUser, fetchGradesForUser } from "../GradesApi.ts";
import GradeItem from "../components/GradeItem.tsx";
import ScholarshipStatusComponent from "../components/ScholarshipStatusComponent.tsx";
import type { GradeItemProps, ScholarshipStatus } from "../props.ts";
import FAQPopup from "../../faq/components/FAQPopup.tsx";
import { faqsGrades } from "../../faq/FAQData.ts";

const USER_ID = 21011;

const GradesPage: React.FC = () => {
  const [selectedSpecialization, setSelectedSpecialization] = useState<string | "">("");
  const [selectedStudyYear, setSelectedStudyYear] = useState<number | "">("");
  const [selectedSemester, setSelectedSemester] = useState<number | "">("");
  const [specializations, setSpecializations] = useState<string[]>([]);
  const [grades, setGrades] = useState<GradeItemProps[]>([]);
  const [status, setStatus] = useState<ScholarshipStatus | null>(null);

  useEffect(() => {
    const fetchSpecializations = async () => {
      const specs = await fetchUserSpecializations(USER_ID);
      setSpecializations(specs);
    };
    fetchSpecializations();
  }, []);

  useEffect(() => {
    if (selectedSpecialization && selectedStudyYear && selectedSemester) {
      const fetchData = async () => {
        // fetch grades
        const result = await fetchGradesForUser(USER_ID, selectedSpecialization, selectedStudyYear, selectedSemester);
        setGrades(result);

        // fetch scholarship status
        const stat = await fetchStatusForUser(USER_ID, selectedSpecialization, selectedStudyYear, selectedSemester);
        setStatus(stat);
      };

      fetchData();
    }
  }, [selectedSpecialization, selectedStudyYear, selectedSemester]);

  const studyYears = [1, 2, 3];
  const semesters = [1, 2];

  return (
    <div className="grades-page">
      <div className="filters-container">
        <div className="filters-top">
          <div className="filter-item">
            <label>Specialization:</label>
            <select value={selectedSpecialization} onChange={(e) => setSelectedSpecialization(e.target.value)}>
              <option value="">All</option>
              {specializations.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-item">
            <label>Year of Study:</label>
            <select value={selectedStudyYear} onChange={(e) => setSelectedStudyYear(Number(e.target.value) || "")}>
              <option value="">All</option>
              {studyYears.map((y) => (
                <option key={y} value={y}>
                  {y}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-item">
            <label>Semester:</label>
            <select value={selectedSemester} onChange={(e) => setSelectedSemester(Number(e.target.value) || "")}>
              <option value="">All</option>
              {semesters.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <div className="grades-table">
        {grades.map((item) => (
          <GradeItem id={item.id} subject={item.subject} value={item.value} />
        ))}
      </div>

      {status && <ScholarshipStatusComponent status={status} />}

      <FAQPopup faqs={faqsGrades} />
    </div>
  );
};

export default GradesPage;
