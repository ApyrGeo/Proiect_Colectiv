import React, { useEffect, useState } from "react";
import "../grades.css";
import useGradesApi from "../GradesApi.ts";
import GradeItem from "../components/GradeItem.tsx";
import ScholarshipStatusComponent from "../components/ScholarshipStatusComponent.tsx";
import type { GradeItemProps, PromotionOfUser, ScholarshipStatus } from "../props.ts";
import { toast } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import Circular from "../../components/loading/Circular.tsx";

interface StudentProps {
  id: number;
}

const StudentGradesPage: React.FC<StudentProps> = ({ id }) => {
  const { t } = useTranslation();
  const { getUserPromotions, getGradesForUser, getScholarshipStatusForUser } = useGradesApi();

  const [selectedPromotion, setSelectedPromotion] = useState<PromotionOfUser | null>(null);
  const [selectedStudyYear, setSelectedStudyYear] = useState<number | "">("");
  const [selectedSemester, setSelectedSemester] = useState<number | "">("");
  const [promotions, setPromotions] = useState<PromotionOfUser[]>([]);
  const [grades, setGrades] = useState<GradeItemProps[]>([]);
  const [status, setStatus] = useState<ScholarshipStatus | null>(null);
  const [error, setError] = useState<Error | null>(null);
  const [loading, setLoading] = useState(false);

  const semesters = [1, 2];

  // Fetch specializations
  useEffect(() => {
    const fetchSpecializations = async () => {
      try {
        const proms = await getUserPromotions(id);
        setPromotions(proms);
        if (proms.length > 0 && !selectedPromotion) {
          setSelectedPromotion(proms[0]);
        }
      } catch (err) {
        setError(err as Error);
      }
    };

    fetchSpecializations();
  }, [getUserPromotions, id, selectedPromotion]);

  // Fetch grades + scholarship status
  useEffect(() => {
    if (!selectedPromotion) return;

    const prom = selectedPromotion;
    const year = selectedStudyYear === "" ? null : selectedStudyYear;
    const sem = selectedSemester === "" ? null : selectedSemester;

    // Scholarship status
    if (prom && year && sem) {
      (async () => {
        try {
          const stat = await getScholarshipStatusForUser(id, prom.id, year, sem);
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
        const result = await getGradesForUser(id, prom.id, year, sem);
        setGrades(result);
        setError(null);
      } catch (err) {
        setError(err as Error);
      } finally {
        setLoading(false);
      }
    })();
  }, [id, selectedPromotion, selectedStudyYear, selectedSemester, getScholarshipStatusForUser, getGradesForUser]);

  // Toast errors
  useEffect(() => {
    if (error) {
      toast.error(t("Select_a_specialization"));
    }
  }, [error, t]);

  return (
    <div className="grades-page">
      <div className="filters-container">
        <div className="filters-top">
          <div className="filter-item">
            <label>{t("Promotion")}:</label>
            <select onChange={(e) => setSelectedPromotion(promotions[Number(e.target.value)])}>
              {promotions.map((p, index) => (
                <option key={index} value={index}>
                  {p.prettyName}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-item">
            <label>{t("YearOfStudy")}:</label>
            <select value={selectedStudyYear} onChange={(e) => setSelectedStudyYear(Number(e.target.value) || "")}>
              <option value="">{t("All")}</option>
              {Array.from({ length: selectedPromotion?.yearDuration ?? 1 }).map((_, i) => (
                <option value={i + 1} key={`year-${i}`}>
                  {i + 1}
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
