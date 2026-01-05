import React, { useEffect, useState } from "react";
import "../grades.css";
import TableGlimmer from "../../components/loading/TableGlimmer";
import type { TeacherProps } from "../../exam/props.ts";
import useGradesApi from "../GradesApi"; // <-- use your API hook

type Subject = {
  id: number;
  name: string;
};

type Group = {
  id: number;
  name: string;
  // optional fields used in table display
  location?: string;
  className?: string;
  date?: string;
  duration?: string;
};

const TeacherGradesPage: React.FC<TeacherProps> = ({ id, user }) => {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [groups, setGroups] = useState<Group[]>([]);
  const [loadingSubjects, setLoadingSubjects] = useState(false);
  const [loadingGroups, setLoadingGroups] = useState(false);
  const [selectedSubjectId, setSelectedSubjectId] = useState<number | "">("");
  const [selectedGroupId, setSelectedGroupId] = useState<number | "">("");

  const { getSubjectsByTeacher, getStudentGroupsBySubject } = useGradesApi();

  // Load subjects for teacher
  useEffect(() => {
    const load = async () => {
      setLoadingSubjects(true);
      try {
        const data = await getSubjectsByTeacher(id);
        setSubjects(
          data.map((s) => ({
            id: s.id,
            name: s.name,
          }))
        );
      } catch (err) {
        console.error(err);
        setSubjects([]);
      } finally {
        setLoadingSubjects(false);
      }
    };
    load();
  }, [id, getSubjectsByTeacher, user.role]);

  async function loadSubjectGroups(subjectId: number) {
    setLoadingGroups(true);
    try {
      const data = await getStudentGroupsBySubject(subjectId);
      setGroups(
        data.map((g) => ({
          id: g.id,
          name: g.name,
        }))
      );
    } catch (err) {
      console.error(err);
      setGroups([]);
    } finally {
      setLoadingGroups(false);
    }
  }

  function onSubjectChange(e: React.ChangeEvent<HTMLSelectElement>) {
    const val = e.target.value;
    if (!val) {
      setSelectedSubjectId("");
      setGroups([]);
      setSelectedGroupId("");
      return;
    }
    const subjectId = Number(val);
    setSelectedSubjectId(subjectId);
    setSelectedGroupId("");
    loadSubjectGroups(subjectId);
  }

  function onGroupChange(e: React.ChangeEvent<HTMLSelectElement>) {
    const val = e.target.value;
    if (!val) {
      setSelectedGroupId("");
      return;
    }
    setSelectedGroupId(Number(val));
    // aici poți ulterior să încarci notele / studenții pentru grup
  }

  return (
    <div className="teacher-grades-container">
      <h2>
        Profesor: {user.firstName} {user.lastName}
      </h2>

      <div style={{ display: "flex", gap: "16px", alignItems: "center", marginBottom: "12px" }}>
        <label>
          Materie:
          <select value={selectedSubjectId} onChange={onSubjectChange}>
            <option value="">Selectează materia</option>
            {loadingSubjects && <option disabled>Loading...</option>}
            {!loadingSubjects &&
              subjects.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
          </select>
        </label>

        {selectedSubjectId !== "" && (
          <label>
            Grupa:
            <select value={selectedGroupId} onChange={onGroupChange} disabled={loadingGroups}>
              <option value="">{loadingGroups ? "Se încarcă grupele..." : "Selectează grupa"}</option>
              {!loadingGroups &&
                groups.map((g) => (
                  <option key={g.id} value={g.id}>
                    {g.name}
                  </option>
                ))}
            </select>
          </label>
        )}
      </div>

      <table className="teacher-grades-table">
        <thead>
          <tr>
            <th>Grupa</th>
            <th>Locația</th>
            <th>Clasa</th>
            <th>Data</th>
            <th>Durata</th>
          </tr>
        </thead>
        <tbody>
          {loadingGroups && <TableGlimmer no_lines={8} no_cols={5} />}
          {!loadingGroups &&
            groups.map((g) => (
              <tr key={g.id}>
                <td>{g.name}</td>
                <td>{g.location ?? "-"}</td>
                <td>{g.className ?? "-"}</td>
                <td>{g.date ?? "-"}</td>
                <td>{g.duration ?? "-"}</td>
              </tr>
            ))}
          {!loadingGroups && groups.length === 0 && selectedSubjectId !== "" && (
            <tr>
              <td colSpan={5} style={{ textAlign: "center" }}>
                Nu există grupe pentru această materie.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default TeacherGradesPage;
