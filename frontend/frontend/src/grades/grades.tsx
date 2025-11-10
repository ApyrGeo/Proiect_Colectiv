import React, { useState } from "react";
import "./grades.css";

type Subject = {
  name: string;
  credits: number;
};

type GradeItem = {
  id: string;
  subject: Subject;
  score: number;
  for_score: boolean;
  academicYear: string;
  specialization: string;
  studyYear: number;
  semester: number;
};

type GradesData = {
  average_score: number;
  result: GradeItem[];
};

const mockGrades: GradesData = {
  average_score: 4.2,
  result: [
    {
      id: "1",
      subject: { name: "Analiza", credits: 5 },
      score: 10,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "2",
      subject: { name: "Algebra", credits: 4 },
      score: 2,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "3",
      subject: { name: "ASC", credits: 4 },
      score: 6,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "4",
      subject: { name: "Fizica", credits: 3 },
      score: 4,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Fizica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "5",
      subject: { name: "Chimie", credits: 3 },
      score: 5,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "6",
      subject: { name: "Sport", credits: 2 },
      score: 10,
      for_score: false,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "7",
      subject: { name: "Informatica", credits: 5 },
      score: 8,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "8",
      subject: { name: "Matematica", credits: 4 },
      score: 3,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "9",
      subject: { name: "Logica", credits: 3 },
      score: 7,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "10",
      subject: { name: "Engleza", credits: 2 },
      score: 0,
      for_score: true,
      academicYear: "2023/2024",
      specialization: "Fizica",
      studyYear: 1,
      semester: 2,
    },
    {
      id: "11",
      subject: { name: "Desen", credits: 2 },
      score: 10,
      for_score: false,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
  ],
};

const years = ["2025/2026", "2024/2025", "2023/2024"];
const specializations = ["Informatica", "Matematica", "Fizica"];
const studyYears = [1, 2, 3, 4];
const semesters = [1, 2];

const Grades: React.FC = () => {
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

  const gradedForAverage = filteredGrades.filter((g) => g.for_score && g.score > 0);

  const totalCredits = gradedForAverage.reduce((sum, g) => sum + g.subject.credits, 0);

  const weightedSum = gradedForAverage.reduce((sum, g) => sum + g.score * g.subject.credits, 0);

  const averageScore = totalCredits > 0 ? (weightedSum / totalCredits).toFixed(2) : "—";

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
        {filteredGrades.map((item) => {
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
        })}
      </div>

      <p className="average-score">
        <strong>Average score:</strong> {averageScore}
      </p>
    </div>
  );
};

export default Grades;
