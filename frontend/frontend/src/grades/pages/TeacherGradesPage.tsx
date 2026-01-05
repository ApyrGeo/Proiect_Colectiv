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
  location?: string;
  className?: string;
  date?: string;
  duration?: string;
};

type Student = {
  id: number;
  userId: number;
  user: {
    id: number;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    email: string;
    owner: string;
    role: number;
  };
  subGroupId: number;
  subGroup: {
    id: number;
    name: string;
  };
};

// specializare: { id, name, groupYears: [] }
type Specialisation = {
  id: number;
  name: string;
  groupYears: unknown[];
};

// map: studentUserId -> lista de specializări (obiecte)
type StudentSpecialisations = Record<number, Specialisation[]>;

const TeacherGradesPage: React.FC<TeacherProps> = ({ id, user }) => {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [groups, setGroups] = useState<Group[]>([]);
  const [students, setStudents] = useState<Student[]>([]);
  const [loadingSubjects, setLoadingSubjects] = useState(false);
  const [loadingGroups, setLoadingGroups] = useState(false);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [loadingSpecs, setLoadingSpecs] = useState(false);
  const [selectedSubjectId, setSelectedSubjectId] = useState<number | "">("");
  const [selectedGroupId, setSelectedGroupId] = useState<number | "">("");
  const [studentSpecialisations, setStudentSpecialisations] = useState<StudentSpecialisations>({});

  const {
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    getStudentByGroup,
    getSpecialisationsByStudent,
  } = useGradesApi();

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
          location: (g as any).location,
          className: (g as any).className,
          date: (g as any).date,
          duration: (g as any).duration,
        }))
      );
    } catch (err) {
      console.error(err);
      setGroups([]);
    } finally {
      setLoadingGroups(false);
    }
  }

  async function loadStudentsForGroup(groupId: number) {
    setLoadingStudents(true);
    setStudentSpecialisations({});
    try {
      const data = await getStudentByGroup(groupId);
      setStudents(data);
    } catch (err) {
      console.error(err);
      setStudents([]);
    } finally {
      setLoadingStudents(false);
    }
  }

  function onSubjectChange(e: React.ChangeEvent<HTMLSelectElement>) {
    const val = e.target.value;
    if (!val) {
      setSelectedSubjectId("");
      setGroups([]);
      setSelectedGroupId("");
      setStudents([]);
      setStudentSpecialisations({});
      return;
    }
    const subjectId = Number(val);
    setSelectedSubjectId(subjectId);
    setSelectedGroupId("");
    setStudents([]);
    setStudentSpecialisations({});
    loadSubjectGroups(subjectId);
  }

  function onGroupChange(e: React.ChangeEvent<HTMLSelectElement>) {
    const val = e.target.value;
    if (!val) {
      setSelectedGroupId("");
      setStudents([]);
      setStudentSpecialisations({});
      return;
    }
    const groupId = Number(val);
    setSelectedGroupId(groupId);
    loadStudentsForGroup(groupId);
  }

  // încarcă specializările pentru toți studenții afișați (folosind noul tip Specialisation)
  useEffect(() => {
    const loadSpecs = async () => {
      if (students.length === 0) return;
      setLoadingSpecs(true);
      try {
        const newMap: StudentSpecialisations = {};
        for (const s of students) {
          if (studentSpecialisations[s.userId]) {
            newMap[s.userId] = studentSpecialisations[s.userId];
            continue;
          }
          try {
            const specs = await getSpecialisationsByStudent(s.userId);
            // specs este de forma [{ id, name, groupYears: [] }]
            newMap[s.userId] = specs as Specialisation[];
          } catch (err) {
            console.error("error loading specs for student", s.userId, err);
            newMap[s.userId] = [];
          }
        }
        setStudentSpecialisations((prev) => ({ ...prev, ...newMap }));
      } finally {
        setLoadingSpecs(false);
      }
    };
    loadSpecs();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [students]);

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

      {/* Tabel studenți */}
      <table className="teacher-grades-table">
        <thead>
          <tr>
            <th>Nume student</th>
            <th>Telefon</th>
            <th>Email</th>
            <th>Subgrupă</th>
          </tr>
        </thead>
        <tbody>
          {loadingStudents && <TableGlimmer no_lines={8} no_cols={4} />}
          {!loadingStudents &&
            students.map((s) => (
              <tr key={s.id}>
                <td>
                  {s.user.firstName} {s.user.lastName}
                </td>
                <td>{s.user.phoneNumber}</td>
                <td>{s.user.email}</td>
                <td>{s.subGroup?.name}</td>
              </tr>
            ))}
          {!loadingStudents && selectedGroupId !== "" && students.length === 0 && (
            <tr>
              <td colSpan={4} style={{ textAlign: "center" }}>
                Nu există studenți în această grupă.
              </td>
            </tr>
          )}
        </tbody>
      </table>

      {/* Tabel specializări pentru studenți */}
      {students.length > 0 && (
        <table className="teacher-grades-table" style={{ marginTop: "24px" }}>
          <thead>
            <tr>
              <th>Student</th>
              <th>Specializări</th>
            </tr>
          </thead>
          <tbody>
            {loadingSpecs && <TableGlimmer no_lines={students.length} no_cols={2} />}
            {!loadingSpecs &&
              students.map((s) => {
                const specs = studentSpecialisations[s.userId] || [];
                return (
                  <tr key={s.id}>
                    <td>
                      {s.user.firstName} {s.user.lastName}
                    </td>
                    <td>
                      {specs.length > 0
                        ? specs.map((sp) => sp.name).join(", ")
                        : "-"}
                    </td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default TeacherGradesPage;
