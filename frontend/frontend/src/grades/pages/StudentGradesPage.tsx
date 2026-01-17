import React, { useEffect, useState } from "react";
import "../grades.css";
import useGradesApi from "../GradesApi.ts";
import GradeItem from "../components/GradeItem.tsx";
import ScholarshipStatusComponent from "../components/ScholarshipStatusComponent.tsx";
import type { GradeItemProps, ScholarshipStatus } from "../props.ts";
import { toast } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import Circular from "../../components/loading/Circular.tsx";

interface StudentProps {
  id: number;
}

const StudentGradesPage: React.FC<StudentProps> = ({ id }) => {
  const { t } = useTranslation();
  const { getUserSpecializations, getGradesForUser, getScholarshipStatusForUser } = useGradesApi();

  const [selectedSpecialization, setSelectedSpecialization] = useState<string>("");
  const [selectedStudyYear, setSelectedStudyYear] = useState<number | "">("");
  const [selectedSemester, setSelectedSemester] = useState<number | "">("");
  const [specializations, setSpecializations] = useState<string[]>([]);
  const [grades, setGrades] = useState<GradeItemProps[]>([]);
  const [status, setStatus] = useState<ScholarshipStatus | null>(null);
  const [error, setError] = useState<Error | null>(null);
  const [loading, setLoading] = useState(false);

  // Fetch specializations
  useEffect(() => {
    const fetchSpecializations = async () => {
      try {
        const specs = await getUserSpecializations(id);
        setSpecializations(specs);
        if (specs.length > 0 && !selectedSpecialization) {
          setSelectedSpecialization(specs[0]);
        }
      } catch (err) {
        setError(err as Error);
      }
    };

    fetchSpecializations();
  }, [getUserSpecializations, id, selectedSpecialization]);

  // Fetch grades + scholarship status
  useEffect(() => {
    if (!selectedSpecialization) return;

    const spec = selectedSpecialization;
    const year = selectedStudyYear === "" ? null : selectedStudyYear;
    const sem = selectedSemester === "" ? null : selectedSemester;

    // Scholarship status
    if (spec && year && sem) {
      (async () => {
        try {
          const stat = await getScholarshipStatusForUser(id, spec, year, sem);
          setStatus(stat);
        } catch {
          setStatus(null);
        }
      })();
    } else {
      setStatus(null);
    }

    // Grades
    (async () => {
      setLoading(true);
      try {
        const result = await getGradesForUser(id, spec, year, sem);
        setGrades(result);
        setError(null);
      } catch (err) {
        setError(err as Error);
      } finally {
        setLoading(false);
      }
    })();
  }, [id, selectedSpecialization, selectedStudyYear, selectedSemester, getScholarshipStatusForUser, getGradesForUser]);

  // Toast errors
  useEffect(() => {
    if (error) {
      toast.error(t("Select_a_specialization"));
    }
  }, [error, t]);

  const studyYears = [1, 2, 3];
  const semesters = [1, 2];

  return (
    <div className="grades-page">
      <div className="filters-container">
        <div className="filters-top">
          <div className="filter-item">
            <label>{t("Specialization")}:</label>
            <select value={selectedSpecialization} onChange={(e) => setSelectedSpecialization(e.target.value)}>
              <option value="" hidden>
                -
              </option>
              {specializations.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-item">
            <label>{t("YearOfStudy")}:</label>
            <select value={selectedStudyYear} onChange={(e) => setSelectedStudyYear(Number(e.target.value) || "")}>
              <option value="">{t("All")}</option>
              {studyYears.map((y) => (
                <option key={y} value={y}>
                  {y}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-item">
            <label>{t("Semester")}:</label>
            <select value={selectedSemester} onChange={(e) => setSelectedSemester(Number(e.target.value) || "")}>
              <option value="">{t("All")}</option>
              {semesters.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {loading ? (
        <Circular />
      ) : (
        <div className="grades-table">
          {grades.map((item) => (
            <GradeItem key={item.id} id={item.id} subject={item.subject} value={item.value} />
          ))}
        </div>
      )}

      {status && <ScholarshipStatusComponent status={status} />}
    </div>
  );
};

export default StudentGradesPage;
