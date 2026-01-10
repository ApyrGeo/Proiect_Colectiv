import React, { useEffect, useState } from "react";
import "../gradesTeacher.css";
import TableGlimmer from "../../components/loading/TableGlimmer";
import type { TeacherProps } from "../../exam/props";
import useGradesApi from "../GradesApi";
import type { SubjectGradesResponse } from "../props.ts";

type Subject = { id: number; name: string };
type Group = { id: number; name: string };

const TeacherGradesPage: React.FC<TeacherProps> = ({ id, user }) => {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [groups, setGroups] = useState<Group[]>([]);
  const [gradesData, setGradesData] = useState<SubjectGradesResponse | null>(null);

  const [selectedSubjectId, setSelectedSubjectId] = useState<number | "">("");
  const [selectedGroupId, setSelectedGroupId] = useState<number | "">("");

  const [loadingSubjects, setLoadingSubjects] = useState(false);
  const [loadingGroups, setLoadingGroups] = useState(false);
  const [loadingGrades, setLoadingGrades] = useState(false);

  const { getSubjectsByTeacher, getStudentGroupsBySubject, getStudentByGroup } = useGradesApi();

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

  const loadGrades = async (subjectId: number, groupId: number) => {
    setLoadingGrades(true);
    try {
      const data = await getStudentByGroup(subjectId, groupId);
      setGradesData(data);
    } catch {
      setGradesData(null);
    } finally {
      setLoadingGrades(false);
    }
  };

  const onSubjectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;
    if (!val) {
      setSelectedSubjectId("");
      setSelectedGroupId("");
      setGroups([]);
      setGradesData(null);
      return;
    }
    const subjectId = Number(val);
    setSelectedSubjectId(subjectId);
    setSelectedGroupId("");
    setGradesData(null);
    loadGroups(subjectId);
  };

  const onGroupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;
    if (!val || selectedSubjectId === "") return;
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

        {selectedSubjectId !== "" && (
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

      {/* TITLU */}
      {gradesData && (
        <h3 className="grades-page-subtitle">
          {gradesData.subject.name} – {gradesData.studentGroup.name}
        </h3>
      )}

      {/* TABEL */}
      <div className="grades-table">
        {loadingGrades && <TableGlimmer no_lines={6} no_cols={2} />}

        {!loadingGrades && gradesData && gradesData.grades.length > 0 && (
          <table className="grades-table-inner">
            <thead>
              <tr>
                <th>Student</th>
                <th>Notă</th>
              </tr>
            </thead>
            <tbody>
              {gradesData.grades.map((g) => {
                const status = g.grade == null ? "grades-not-counted" : g.grade >= 5 ? "grades-pass" : "grades-fail";
                return (
                  <tr key={g.user.id} className={status}>
                    <td>
                      {g.user.firstName} {g.user.lastName}
                    </td>
                    <td>{g.grade ?? "—"}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}

        {!loadingGrades && selectedGroupId !== "" && gradesData?.grades.length === 0 && (
          <div className="grades-empty-message">Nu există studenți în această grupă.</div>
        )}
      </div>
    </div>
  );
};

export default TeacherGradesPage;
