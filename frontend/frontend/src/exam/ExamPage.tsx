import React, { useState } from "react";
import useExamApi from "../exam/ExamApi.ts";
import type { Location, Teacher } from "./props";
import "./ExamPage.css";

interface GroupRow {
  id: number;
  name: string;
  selectedLocationId: number | null;
  selectedClassroomId: number | null;
  examDate: string | null;
  examDuration: number | null;
  examId: number;
  selectedGroupId: number | null;
}

const ExamPage: React.FC = () => {
  const { getLocations, getSubjectsByTeacher, getExamsBySubject, updateExam, getUserById } = useExamApi();

  const [locations, setLocations] = useState<Location[]>([]);
  const [groups, setGroups] = useState<GroupRow[]>([]);
  const [teacher, setTeacher] = useState<Teacher | null>(null);
  const [teacherIdInput, setTeacherIdInput] = useState<number | "">("");
  const [teacherSubjects, setTeacherSubjects] = useState<any[]>([]);
  const [selectedSubjectId, setSelectedSubjectId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [updateList, setUpdateList] = useState<GroupRow[]>([]);

  const fetchLocationsData = async () => {
    const locs = await getLocations();
    setLocations(locs);
  };

  const addToUpdateList = (group: GroupRow) => {
    setUpdateList((prev) => {
      const exists = prev.find((g) => g.id === group.id);
      if (exists) {
        // înlocuiește grupul existent
        return prev.map((g) => (g.id === group.id ? group : g));
      }
      return [...prev, group];
    });
  };

  const getMaxLocationWidth = () => {
    const maxLength = Math.max(...locations.map((l) => l.name.length), 0);
    return maxLength * 8 + 20; // aproximativ 8px per caracter + padding
  };

  const getMaxClassroomWidth = () => {
    let maxLength = 0;
    groups.forEach((g) => {
      const selectedLocation = locations.find((l) => l.id === g.selectedLocationId);
      const classroomOptions = selectedLocation?.classrooms || [];
      classroomOptions.forEach((c) => {
        if (c.name.length > maxLength) maxLength = c.name.length;
      });
    });
    return maxLength * 35 + 20;
  };

  const handleCheckUser = async () => {
    if (teacherIdInput === "") return;
    setLoading(true);

    try {
      const user = await getUserById(Number(teacherIdInput));

      if (user.role === 1) {
        // e profesor
        alert("Acesta este un teacher!");
        // setTeacher(user);
        // await fetchLocationsData();
        // const subjects = await getSubjectsByTeacher(user.id);
        // setTeacherSubjects(subjects);
        // setGroups([]);
        // setSelectedSubjectId(null);
      } else if (user.role === 0) {
        // e student
        alert("Acesta este un student!");
        setTeacher(null);
      } else {
        alert("ID invalid!");
        setTeacher(null);
      }
    } catch (err: never) {
      if (err.response && err.response.status === 404) {
        alert("ID invalid!");
      } else {
        console.error(err);
      }
      setTeacher(null);
    } finally {
      setLoading(false);
    }
  };
  const handleSubjectSelect = async (subjectId: number) => {
    setSelectedSubjectId(subjectId);
    setLoading(true);
    try {
      const groupsFromApi = await getExamsBySubject(subjectId);
      const exams = await getExamsBySubject(subjectId);

      const mappedGroups: GroupRow[] = groupsFromApi.map((g) => {
        const exam = exams.find((e) => e.studentGroup.name === g.studentGroup.name);

        const classroomId = exam?.classroom?.id ?? null;
        const groupId = exam?.studentGroup?.id ?? null;
        const locationId = classroomId
          ? (locations.find((loc) => loc.classrooms.some((c) => c.id === classroomId))?.id ?? null)
          : null;

        const examId = exam?.id ?? 0;

        return {
          id: g.id,
          name: g.studentGroup.name,
          selectedLocationId: locationId,
          selectedClassroomId: classroomId,
          selectedGroupId: groupId,
          examDate: exam?.date ? new Date(exam.date).toISOString().slice(0, 16) : "",
          examDuration: exam?.duration ?? 0,
          examId: examId,
        };
      });

      setGroups(mappedGroups);
    } catch (err) {
      console.error("Eroare la grupuri:", err);
      setGroups([]);
    } finally {
      setLoading(false);
    }
  };

  const handleLocationChange = (groupId: number, locationId: number) => {
    setGroups((prev) =>
      prev.map((g) => {
        if (g.id === groupId) {
          const updated = { ...g, selectedLocationId: locationId, selectedClassroomId: null };
          addToUpdateList(updated); // adăugăm/actualizăm în lista de update
          return updated;
        }
        return g;
      })
    );
  };

  const handleDurationChange = (groupId: number, duration: number | null) => {
    setGroups((prev) =>
      prev.map((g) => {
        if (g.id === groupId) {
          const updated = { ...g, examDuration: duration };
          addToUpdateList(updated);
          return updated;
        }
        return g;
      })
    );
  };

  const handleClassroomChange = (groupId: number, classroomId: number) => {
    setGroups((prev) =>
      prev.map((g) => {
        if (g.id === groupId) {
          const updated = { ...g, selectedClassroomId: classroomId };
          addToUpdateList(updated);
          return updated;
        }
        return g;
      })
    );
  };

  const handleExamDateChange = (groupId: number, date: string) => {
    setGroups((prev) =>
      prev.map((g) => {
        if (g.id === groupId) {
          const updated = { ...g, examDate: date };
          addToUpdateList(updated);
          return updated;
        }
        return g;
      })
    );
  };

  const handleUpdateAll = async () => {
    if (!selectedSubjectId) return alert("Selectează materia!");

    try {
      for (const group of updateList) {
        if (!group.selectedClassroomId) continue; // sărim dacă nu e selectată clasa

        await updateExam({
          id: group.examId,
          date: group.examDate ?? "",
          duration: group.examDuration ?? 0,
          classroomId: group.selectedClassroomId,
          subjectId: selectedSubjectId,
          studentGroupId: group.selectedGroupId!,
        });
      }
      alert("Update realizat cu succes!");
      setUpdateList([]); // curățăm lista
    } catch (err) {
      console.error(err);
      alert("Eroare la update!");
    }
  };

  if (loading) return <p>Se încarcă datele...</p>;

  const locationWidth = getMaxLocationWidth();
  const classroomWidth = getMaxClassroomWidth();

  return (
    <div className="exam-page-container">
      <h1>Verificare profesor</h1>

      <div className="teacher-check">
        <input
          type="number"
          placeholder="Introduceți ID profesor"
          value={teacherIdInput}
          onChange={(e) => setTeacherIdInput(e.target.value === "" ? "" : Number(e.target.value))}
        />
        <button onClick={handleCheckUser}>Verifică</button>
      </div>

      {teacher && (
        <>
          <h2>
            Profesor: {teacher.user.firstName} {teacher.user.lastName}
          </h2>

          <select value={selectedSubjectId || ""} onChange={(e) => handleSubjectSelect(Number(e.target.value))}>
            <option value="">Selectează materia</option>
            {teacherSubjects.map((s) => (
              <option key={s.id} value={s.id}>
                {s.name}
              </option>
            ))}
          </select>

          {groups.length > 0 && (
            <table className="exam-table">
              <thead>
                <tr>
                  <th>Grupa</th>
                  <th>Locația</th>
                  <th>Clasa</th>
                  <th>Data examen</th>
                  <th>Durata (min)</th>
                </tr>
              </thead>
              <tbody>
                {groups.map((group) => {
                  const selectedLocation = locations.find((l) => l.id === group.selectedLocationId);
                  const classroomOptions = selectedLocation?.classrooms || [];

                  return (
                    <tr key={group.id}>
                      <td>{group.name}</td>

                      <td>
                        <select
                          value={group.selectedLocationId || ""}
                          onChange={(e) => handleLocationChange(group.id, Number(e.target.value))}
                          style={{ width: `${locationWidth}px` }}
                        >
                          <option value="">Selectează locația</option>
                          {locations.map((l) => (
                            <option key={l.id} value={l.id}>
                              {l.name}
                            </option>
                          ))}
                        </select>
                      </td>

                      <td>
                        <select
                          value={group.selectedClassroomId || ""}
                          onChange={(e) => handleClassroomChange(group.id, Number(e.target.value))}
                          disabled={!selectedLocation}
                          style={{ width: `${classroomWidth}px` }}
                        >
                          <option value="">Selectează clasa</option>
                          {classroomOptions.map((c) => (
                            <option key={c.id} value={c.id}>
                              {c.name}
                            </option>
                          ))}
                        </select>
                      </td>

                      <td>
                        <input
                          type="datetime-local"
                          value={group.examDate || ""}
                          onChange={(e) => handleExamDateChange(group.id, e.target.value)}
                        />
                      </td>

                      <td className="duration-col">
                        <input
                          type="text"
                          value={group.examDuration ?? ""}
                          onChange={(e) => {
                            const val = e.target.value;
                            if (/^\d*$/.test(val)) {
                              handleDurationChange(group.id, val === "" ? null : Number(val));
                            }
                          }}
                        />
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
          <div style={{ marginTop: "20px", textAlign: "right" }}>
            <button onClick={handleUpdateAll}>Update</button>
          </div>
        </>
      )}
    </div>
  );
};

export default ExamPage;
