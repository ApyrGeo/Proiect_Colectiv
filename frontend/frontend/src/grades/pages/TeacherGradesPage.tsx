import React, { useEffect, useState } from "react";
import "../gradesTeacher.css";
import TableGlimmer from "../../components/loading/TableGlimmer";
import type { TeacherProps } from "../props";
import useGradesApi from "../GradesApi";
import type { SubjectGradesResponse, GradeEntry } from "../props";
import { useTranslation } from "react-i18next";

type Subject = {
  id: number;
  name: string;
};

type Group = {
  id: number;
  name: string;
};

type GradeUpdate = {
  gradeId: number; // id-ul notei din backend (0 dacă nota nu există)
  value: number | null;
  enrollmentId: number;
  subjectId: number;
};

const TeacherGradesPage: React.FC<TeacherProps> = ({ id, user }) => {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [groups, setGroups] = useState<Group[]>([]);
  const [gradesData, setGradesData] = useState<SubjectGradesResponse | null>(null);

  const [selectedSubjectId, setSelectedSubjectId] = useState<number | "">("");
  const [selectedGroupId, setSelectedGroupId] = useState<number | "">("");

  const [loadingSubjects, setLoadingSubjects] = useState(false);
  const [loadingGroups, setLoadingGroups] = useState(false);
  const [loadingGrades, setLoadingGrades] = useState(false);

  const [pendingUpdates, setPendingUpdates] = useState<GradeUpdate[]>([]);

  const { t } = useTranslation();

  const {
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    getSubGroupsBySubject,
    getEnrollmentByStudentAndSubGroup,
    getStudentByGroup,
    updateGradeById,
    createGradeById,
  } = useGradesApi();

  /* ===================== LOAD SUBJECTS ===================== */
  useEffect(() => {
    const loadSubjects = async () => {
      setLoadingSubjects(true);
      try {
        const data = await getSubjectsByTeacher(id);
        setSubjects(data.map((s) => ({ id: s.id, name: s.name })));
      } catch {
        setSubjects([]);
      } finally {
        setLoadingSubjects(false);
      }
    };

    loadSubjects();
  }, [id, getSubjectsByTeacher]);

  /* ===================== LOAD GROUPS ===================== */
  const loadGroups = async (subjectId: number) => {
    setLoadingGroups(true);
    try {
      const data = await getStudentGroupsBySubject(subjectId);
      setGroups(data.map((g) => ({ id: g.id, name: g.name })));
    } catch {
      setGroups([]);
    } finally {
      setLoadingGroups(false);
    }
  };

  /* ===================== LOAD GRADES ===================== */
  const loadGrades = async (subjectId: number, groupId: number) => {
    setLoadingGrades(true);
    try {
      const data = await getStudentByGroup(subjectId, groupId);
      const subGroups = await getSubGroupsBySubject(subjectId);
      const subGroupId = subGroups[0]?.id;

      const gradesWithEnrollment: GradeEntry[] = await Promise.all(
        data.grades.map(async (g) => {
          const enrollment = await getEnrollmentByStudentAndSubGroup(g.user.id, subGroupId);

          const gradeId = g.grade?.id ?? 0;
          const value = g.grade?.value ?? null;

          return {
            user: g.user,
            grade: {
              ...g.grade,
              id: gradeId,
              value,
              enrollment: enrollment!,
            },
          };
        })
      );

      setGradesData({ ...data, grades: gradesWithEnrollment });
      setPendingUpdates([]);
    } catch {
      setGradesData(null);
    } finally {
      setLoadingGrades(false);
    }
  };

  /* ===================== GRADE CHANGE ===================== */
  const onGradeChange = (studentId: number, value: string) => {
    if (!gradesData) return;

    const newValue = value === "" ? null : Number(value);

    setGradesData((prev) => {
      if (!prev) return prev;

      const newGrades = prev.grades.map((g) => {
        if (g.user.id === studentId) {
          setPendingUpdates((prevUpdates) => {
            const exists = prevUpdates.find(
              (p) => p.gradeId === g.grade.id && p.enrollmentId === g.grade.enrollment.id
            );

            if (exists) {
              return prevUpdates.map((p) =>
                p.gradeId === g.grade.id && p.enrollmentId === g.grade.enrollment.id ? { ...p, value: newValue } : p
              );
            }

            return [
              ...prevUpdates,
              {
                gradeId: g.grade.id,
                value: newValue,
                enrollmentId: g.grade.enrollment.id,
                subjectId: selectedSubjectId as number,
              },
            ];
          });

          return {
            ...g,
            grade: { ...g.grade, value: newValue },
          };
        }

        return g;
      });

      return { ...prev, grades: newGrades };
    });
  };

  /* ===================== SAVE ALL ===================== */
  const saveAllGrades = async () => {
    for (const update of pendingUpdates) {
      if (!update.enrollmentId) continue;

      try {
        if (!update.gradeId || update.gradeId === 0) {
          await createGradeById(update.value, update.subjectId, update.enrollmentId, id);
        } else {
          await updateGradeById(update.gradeId, update.value, update.subjectId, update.enrollmentId, id);
        }
      } catch (err) {
        console.error("Eroare la salvarea notei:", err);
      }
    }

    setPendingUpdates([]);

    if (selectedSubjectId && selectedGroupId) {
      loadGrades(selectedSubjectId as number, selectedGroupId as number);
    }
  };

  /* ===================== SELECT HANDLERS ===================== */
  const onSubjectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;

    if (!val) {
      setSelectedSubjectId("");
      setSelectedGroupId("");
      setGroups([]);
      setGradesData(null);
      setPendingUpdates([]);
      return;
    }

    const subjectId = Number(val);
    setSelectedSubjectId(subjectId);
    setSelectedGroupId("");
    setGradesData(null);
    setPendingUpdates([]);
    loadGroups(subjectId);
  };

  const onGroupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;
    if (!val || !selectedSubjectId) return;

    const groupId = Number(val);
    setSelectedGroupId(groupId);
    loadGrades(selectedSubjectId as number, groupId);
  };

  /* ===================== RENDER ===================== */
  return (
    <div className="grades-page-teacher-container">
      <h1 className="grades-page-title">
        {t("Professor")}: {user.firstName} {user.lastName}
      </h1>

      {/* FILTRE */}
      <div className="grades-panel grades-filters-top">
        <div className="grades-filter-item">
          <label>{t("Subject")}:</label>
          <select value={selectedSubjectId} onChange={onSubjectChange}>
            {!selectedSubjectId && <option value="">{t("Select_a_subject")}</option>}

            {loadingSubjects && <option disabled>{t("Loading")}</option>}

            {!loadingSubjects &&
              subjects.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
          </select>
        </div>

        {selectedSubjectId && (
          <div className="grades-filter-item">
            <label>{t("Group")}:</label>
            <select value={selectedGroupId} onChange={onGroupChange} disabled={loadingGroups}>
              {!selectedGroupId && <option value="">{loadingGroups ? t("Loading") : t("Select_a_group")}</option>}

              {!loadingGroups &&
                groups.map((g) => (
                  <option key={g.id} value={g.id}>
                    {g.name}
                  </option>
                ))}
            </select>
          </div>
        )}
      </div>

      {/* TABEL */}
      <div className="grades-table">
        {loadingGrades && (
          <table className="grades-table-inner">
            <tbody>
              <TableGlimmer no_lines={6} no_cols={2} />
            </tbody>
          </table>
        )}

        {!loadingGrades && gradesData && gradesData.grades.length > 0 && (
          <table className="grades-table-inner">
            <thead>
              <tr>
                <th>Student</th>
                <th>Notă</th>
              </tr>
            </thead>
            <tbody>
              {gradesData.grades.map((g) => (
                <tr key={g.user.id}>
                  <td>
                    {g.user.firstName} {g.user.lastName}
                  </td>
                  <td>
                    <input
                      type="text"
                      inputMode="numeric"
                      pattern="[0-9]*"
                      value={g.grade.value ?? ""}
                      className="grades-input"
                      onKeyDown={(e) => {
                        const allowedKeys = ["Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab"];

                        if (!allowedKeys.includes(e.key) && !/^[0-9]$/.test(e.key)) {
                          e.preventDefault();
                        }
                      }}
                      onPaste={(e) => {
                        const pasted = e.clipboardData.getData("text");
                        if (!/^\d+$/.test(pasted)) {
                          e.preventDefault();
                        }
                      }}
                      onChange={(e) => {
                        const val = e.target.value;

                        if (val === "") {
                          onGradeChange(g.user.id, "");
                          return;
                        }

                        const numeric = Number(val);
                        if (!Number.isNaN(numeric) && numeric >= 1 && numeric <= 10) {
                          onGradeChange(g.user.id, val);
                        }
                      }}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        <button className="grades-update-button" onClick={saveAllGrades} disabled={pendingUpdates.length === 0}>
          {t("SaveModifications")}
        </button>

        {!loadingGrades && selectedGroupId && gradesData?.grades.length === 0 && (
          <div className="grades-empty-message">{t("NoStudentsInGroup")}</div>
        )}
      </div>
    </div>
  );
};

export default TeacherGradesPage;
