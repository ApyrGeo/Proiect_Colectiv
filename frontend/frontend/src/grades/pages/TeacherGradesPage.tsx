import React, { useEffect, useState } from "react";
import "../gradesTeacher.css";
import TableGlimmer from "../../components/loading/TableGlimmer";
import type { TeacherProps } from "../props";
import useGradesApi from "../GradesApi";
import type { SubjectGradesResponse, GradeEntry } from "../props.ts";

type Subject = { id: number; name: string };
type Group = { id: number; name: string };

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

  const {
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    getSubGroupsBySubject,
    getEnrollmentByStudentAndSubGroup,
    getStudentByGroup,
    updateGradeById,
    createGradeById,
  } = useGradesApi();

  // Load subjects for teacher
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

  // Load groups for selected subject
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

  // Load grades for selected subject & group
  const loadGrades = async (subjectId: number, groupId: number) => {
    setLoadingGrades(true);
    try {
      const data = await getStudentByGroup(subjectId, groupId);

      // Obținem subgrupurile materiei
      const subGroups = await getSubGroupsBySubject(subjectId);
      const subGroupId = subGroups[0]?.id;

      // Adăugăm enrollmentId și pregătim gradeId pentru fiecare student
      const gradesWithEnrollment: GradeEntry[] = await Promise.all(
        data.grades.map(async (g) => {
          const enrollment = await getEnrollmentByStudentAndSubGroup(g.user.id, subGroupId);
          const gradeId = g.grade?.id ?? 0; // 0 dacă nota nu există
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

  // Update grade local + adaug in pendingUpdates
  const onGradeChange = (studentId: number, value: string) => {
    if (!gradesData) return;

    const newValue = value === "" ? null : Number(value);

    setGradesData((prev) => {
      if (!prev) return prev;
      const newGrades = prev.grades.map((g) => {
        if (g.user.id === studentId) {
          // Actualizez pending updates
          setPendingUpdates((prevUpdates) => {
            const exists = prevUpdates.find(
              (p) => p.gradeId === g.grade.id && p.enrollmentId === g.grade.enrollment.id
            );
            if (exists) {
              return prevUpdates.map((p) =>
                p.gradeId === g.grade.id && p.enrollmentId === g.grade.enrollment.id ? { ...p, value: newValue } : p
              );
            } else {
              return [
                ...prevUpdates,
                {
                  gradeId: g.grade.id,
                  value: newValue,
                  enrollmentId: g.grade.enrollment.id,
                  subjectId: selectedSubjectId as number,
                },
              ];
            }
          });

          return { ...g, grade: { ...g.grade, value: newValue } };
        }
        return g;
      });
      return { ...prev, grades: newGrades };
    });
  };

  // Save grades: POST if gradeId === 0, PUT otherwise
  const saveAllGrades = async () => {
    for (const update of pendingUpdates) {
      if (!update.enrollmentId) continue;

      try {
        if (!update.gradeId || update.gradeId === 0) {
          // Folosim createGradeById pentru nota noua
          console.log(update);
          await createGradeById(update.value, update.subjectId, update.enrollmentId, id);
        } else {
          // Folosim updateGradeById pentru nota existenta
          console.log(update);
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

  // Handlers for select changes
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

  return (
    <div className="grades-page-teacher-container">
      <h1 className="grades-page-title">
        Profesor: {user.firstName} {user.lastName}
      </h1>

      {/* FILTRE */}
      <div className="grades-panel grades-filters-top">
        <div className="grades-filter-item">
          <label>Materie:</label>
          <select value={selectedSubjectId} onChange={onSubjectChange}>
            <option value="">Selectează materia</option>
            {loadingSubjects && <option disabled>Se încarcă...</option>}
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
            <label>Grupa:</label>
            <select value={selectedGroupId} onChange={onGroupChange} disabled={loadingGroups}>
              <option value="">{loadingGroups ? "Se încarcă..." : "Selectează grupa"}</option>
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
          <>
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
                        type="number"
                        min={1}
                        max={10}
                        value={g.grade.value ?? ""}
                        onChange={(e) => onGradeChange(g.user.id, e.target.value)}
                        className="grades-input"
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {pendingUpdates.length > 0 && (
              <button onClick={saveAllGrades} className="grades-update-button">
                Salvează modificările ({pendingUpdates.length})
              </button>
            )}
          </>
        )}

        {!loadingGrades && selectedGroupId && gradesData?.grades.length === 0 && (
          <div className="grades-empty-message">Nu există studenți în această grupă.</div>
        )}
      </div>
    </div>
  );
};

export default TeacherGradesPage;
