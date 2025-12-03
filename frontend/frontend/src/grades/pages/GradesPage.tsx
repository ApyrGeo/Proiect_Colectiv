import React, { useEffect, useState } from "react";
import "../grades.css";
import { fetchUserSpecializations, fetchStatusForUser, fetchGradesForUser } from "../GradesApi.ts";
import GradeItem from "../components/GradeItem.tsx";
import ScholarshipStatusComponent from "../components/ScholarshipStatusComponent.tsx";
import type { GradeItemProps, ScholarshipStatus } from "../props.ts";
import FAQPopup from "../../faq/components/FAQPopup.tsx";
import { faqsGrades } from "../../faq/FAQData.ts";
import { toast } from "react-hot-toast";
import { useTranslation } from "react-i18next";

const USER_ID = 27273;

const GradesPage: React.FC = () => {
  const { t } = useTranslation();

  const [selectedSpecialization, setSelectedSpecialization] = useState<string | "">("");
  const [selectedStudyYear, setSelectedStudyYear] = useState<number | "">("");
  const [selectedSemester, setSelectedSemester] = useState<number | "">("");
  const [specializations, setSpecializations] = useState<string[]>([]);
  const [grades, setGrades] = useState<GradeItemProps[]>([]);
  const [status, setStatus] = useState<ScholarshipStatus | null>(null);
  const [error, setError] = useState<Error | null>(null);

  // Fetch specializations la mount
  useEffect(() => {
    const fetchSpecializations = async () => {
      try {
        const specs = await fetchUserSpecializations(USER_ID);
        setSpecializations(specs);
      } catch (err) {
        const e = err as Error;
        setError(e);
      }
    };
    fetchSpecializations();
  }, []);

  // Fetch grades și status când se schimbă filtrele
  useEffect(() => {
    const spec = selectedSpecialization === "" ? null : selectedSpecialization;
    const year = selectedStudyYear === "" ? null : selectedStudyYear;
    const sem = selectedSemester === "" ? null : selectedSemester;

    // Fetch status
    if (spec && year && sem) {
      (async () => {
        try {
          const stat = await fetchStatusForUser(USER_ID, spec, year, sem);
          setStatus(stat);
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
        } catch (err) {
          setStatus(null);
        }
      })();
    } else {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setStatus(null);
    }

    // Fetch grades
    (async () => {
      try {
        const result = await fetchGradesForUser(USER_ID, spec, year, sem);
        setGrades(result);
        setError(null);
      } catch (err) {
        setError(err as Error);
      }
    })();
  }, [selectedSpecialization, selectedStudyYear, selectedSemester]);

  // Toast pentru erori – folosit în useEffect și cu setTimeout pentru siguranță
  useEffect(() => {
    if (error) {
      // Delay zero pentru a ne asigura că Toaster e montat
      toast.error(`Error: ${error.message}`);
    }
  }, [error]);

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
              <option value="">All</option>
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
          <GradeItem key={item.id} id={item.id} subject={item.subject} value={item.value} />
        ))}
      </div>

      {status && <ScholarshipStatusComponent status={status} />}
      <FAQPopup faqs={faqsGrades} />
    </div>
  );
};

export default GradesPage;
