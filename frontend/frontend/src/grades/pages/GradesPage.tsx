import React, { useState } from "react";
import "../grades.css";
import { mockGrades } from "../GradesApi.ts";
import GradeItem from "../components/GradeItem.tsx";

const years = ["2025/2026", "2024/2025", "2023/2024"];
const specializations = ["Informatica", "Matematica", "Fizica"];
const studyYears = [1, 2, 3, 4];
const semesters = [1, 2];

const GradesPage: React.FC = () => {
  const [selectedYear, setSelectedYear] = useState<string | "">("");
  const [selectedSpecialization, setSelectedSpecialization] = useState<string | "">("");
  const [selectedStudyYear, setSelectedStudyYear] = useState<number | "">("");
  const [selectedSemester, setSelectedSemester] = useState<number | "">("");

  const filteredGrades = mockGrades.result.filter((item) => {
    const matchYear = selectedYear === "" || item.academicYear === selectedYear;
    const matchSpec = selectedSpecialization === "" || item.specialization === selectedSpecialization;
    const matchStudyYear = selectedStudyYear === "" || item.studyYear === selectedStudyYear;
    const matchSemester = selectedSemester === "" || item.semester === selectedSemester;

    return matchYear && matchSpec && matchStudyYear && matchSemester;
  });

  return (
    <div className="grades-page">
      <div className="filters-container">
        <div className="filters-top">
          <div className="filter-item">
            <label>Academic Year:</label>
            <select value={selectedYear} onChange={(e) => setSelectedYear(e.target.value)}>
              <option value="">All</option>
              {years.map((y) => (
                <option key={y} value={y}>
                  {y}
                </option>
              ))}
            </select>
          </div>

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
        {filteredGrades.map((item) => (
          <GradeItem
            id={item.id}
            subject={item.subject}
            score={item.score}
            for_score={item.for_score}
            specialization={item.specialization}
            academicYear={item.academicYear}
            studyYear={item.studyYear}
            semester={item.semester}
          />
        ))}
      </div>

      <p className="average-score">
        <strong>Average score:</strong> {mockGrades.average_score}
      </p>
    </div>
  );
};

export default GradesPage;
