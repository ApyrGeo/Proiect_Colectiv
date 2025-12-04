import React, { useEffect, useState } from "react";
import {getLocations, getStudentGroupsByTeacher, getTeacherById} from "./ExamApi";
import type { Location, Classroom, Teacher } from "./props";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import "./ExamPage.css";

interface GroupRow {
  id: number;
  name: string;
  selectedLocationId: number | null;
  selectedClassroomId: number | null;
}

const ExamPage: React.FC = () => {
  const { accessToken, acquireToken, loading: authLoading } = useAuthContext();
  const [locations, setLocations] = useState<Location[]>([]);
  const [groups, setGroups] = useState<GroupRow[]>([]);
  const [teacher, setTeacher] = useState<Teacher | null>(null);
  const [teacherIdInput, setTeacherIdInput] = useState<number | "">("");
  const [loading, setLoading] = useState(false);

  // Inițializare grupuri
  useEffect(() => {
    const initialGroups: GroupRow[] = [
      { id: 1, name: "Grupa 1", selectedLocationId: null, selectedClassroomId: null },
      { id: 2, name: "Grupa 2", selectedLocationId: null, selectedClassroomId: null },
      { id: 3, name: "Grupa 3", selectedLocationId: null, selectedClassroomId: null },
      { id: 4, name: "Grupa 4", selectedLocationId: null, selectedClassroomId: null },
      { id: 5, name: "Grupa 5", selectedLocationId: null, selectedClassroomId: null },
    ];
    setGroups(initialGroups);
  }, []);

  // Fetch locații
  const fetchLocationsData = async () => {
    setLoading(true);
    try {
      let token = accessToken;
      if (!token) token = await acquireToken();
      if (!token) return;

      const locs = await getLocations(token);
      setLocations(Array.isArray(locs) ? locs : []);
    } catch (err) {
      console.error("Eroare la încărcarea locațiilor:", err);
      setLocations([]);
    } finally {
      setLoading(false);
    }
  };

  // Verifică dacă e teacher după ID
  const handleCheckTeacher = async () => {
    if (teacherIdInput === "") return;

    setLoading(true);
    try {
      let token = accessToken;
      if (!token) token = await acquireToken();
      if (!token) return;

      const t = await getTeacherById(token, Number(teacherIdInput));
      setTeacher(t);

      if (t) {
        await fetchLocationsData();

        // Fetch student groups pentru profesor
        const groupsFromApi = await getStudentGroupsByTeacher(token, Number(teacherIdInput));
        const mappedGroups: GroupRow[] = groupsFromApi.map((g) => ({
          id: g.id,
          name: g.name,
          selectedLocationId: null,
          selectedClassroomId: null,
        }));
        setGroups(mappedGroups);
      } else {
        alert("Nu este un profesor valid!");
        setGroups([]);
      }
    } catch (err) {
      console.error("Eroare la verificarea profesorului:", err);
      setTeacher(null);
      setGroups([]);
    } finally {
      setLoading(false);
    }
  };

  const handleLocationChange = (groupId: number, locationId: number) => {
    setGroups((prev) =>
      prev.map((g) => (g.id === groupId ? { ...g, selectedLocationId: locationId, selectedClassroomId: null } : g))
    );
  };

  const handleClassroomChange = (groupId: number, classroomId: number) => {
    setGroups((prev) => prev.map((g) => (g.id === groupId ? { ...g, selectedClassroomId: classroomId } : g)));
  };

  if (loading || authLoading) return <p>Se încarcă datele...</p>;

  return (
    <div className="exam-page-container">
      <h1>Verificare profesor</h1>
      <div className="teacher-check">
        <input
          type="number"
          placeholder="Introduceți ID-ul profesorului"
          value={teacherIdInput}
          onChange={(e) => setTeacherIdInput(e.target.value === "" ? "" : Number(e.target.value))}
        />
        <button onClick={handleCheckTeacher}>Verifică</button>
      </div>

      {teacher && (
        <>
          <h2>
            Profesor: {teacher.user.firstName} {teacher.user.lastName}
          </h2>
          <table className="exam-table">
            <thead>
              <tr>
                <th>Grupă</th>
                <th>Locație</th>
                <th>Clase</th>
              </tr>
            </thead>
            <tbody>
              {groups.map((group) => {
                const selectedLocation = locations.find((l) => l.id === group.selectedLocationId);
                const classroomOptions: Classroom[] = selectedLocation?.classrooms || [];

                return (
                  <tr key={group.id}>
                    <td>{group.name}</td>
                    <td>
                      <select
                        value={group.selectedLocationId || ""}
                        onChange={(e) => handleLocationChange(group.id, Number(e.target.value))}
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
                      >
                        <option value="">Selectează clasa</option>
                        {classroomOptions.map((c) => (
                          <option key={c.id} value={c.id}>
                            {c.name}
                          </option>
                        ))}
                      </select>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </>
      )}
    </div>
  );
};

export default ExamPage;
