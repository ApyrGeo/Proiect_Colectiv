import React, { useState, useEffect } from "react";
import useExamApi from "../ExamApi.ts";
import type { TeacherProps, LocationProps, GroupRowProps, ExamProps } from "../props.ts";
import "../ExamPage.css";
import toast from "react-hot-toast";
import { t } from "i18next";
import TableGlimmer from "../../components/loading/TableGlimmer.tsx";

const ExamPageByTeacher: React.FC<TeacherProps> = ({ id, user }) => {
  const { getLocations, getSubjectsByTeacher, getExamsBySubject, updateExam } = useExamApi();

  const [locations, setLocations] = useState<LocationProps[]>([]);
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const [teacherSubjects, setTeacherSubjects] = useState<any[]>([]);
  const [groups, setGroups] = useState<GroupRowProps[]>([]);
  const [originalGroups, setOriginalGroups] = useState<GroupRowProps[]>([]);
  const [selectedSubjectId, setSelectedSubjectId] = useState<number | null>(null);
  const [updateList, setUpdateList] = useState<GroupRowProps[]>([]);
  const [loadingGroups, setLoadingGroups] = useState(false);

  useEffect(() => {
    const loadData = async () => {
      const locs = await getLocations();
      setLocations(locs);

      const subjects = await getSubjectsByTeacher(id);
      setTeacherSubjects(subjects);
    };
    loadData();
  }, [getLocations, getSubjectsByTeacher, id]);

  const getMaxLocationWidth = () => {
    const maxLength = Math.max(...locations.map((l) => l.name.length), 0);
    return maxLength * 8 + 20;
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
    return maxLength * 38 + 20;
  };

  const handleSubjectSelect = async (subjectId: number) => {
    setSelectedSubjectId(subjectId);
    setLoadingGroups(true);
    setGroups([]); // ascunde vechea tabelă în timpul încărcării

    const groupsFromApi = await getExamsBySubject(subjectId);
    const exams = await getExamsBySubject(subjectId);

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const mappedGroups: GroupRowProps[] = groupsFromApi.map((g: any) => {
      const exam = exams.find((e: ExamProps) => e.studentGroup!.name === g.studentGroup.name);

      const classroomId = exam?.classroom?.id ?? null;
      const groupId = exam?.studentGroup?.id ?? null;
      const locationId = classroomId
        ? (locations.find((loc) => loc.classrooms.some((c) => c.id === classroomId))?.id ?? null)
        : null;

      return {
        id: g.id,
        name: g.studentGroup.name,
        selectedLocationId: locationId,
        selectedClassroomId: classroomId,
        selectedGroupId: groupId,
        examDate: exam?.date ? new Date(exam.date).toISOString().slice(0, 16) : "",
        examDuration: exam?.duration ?? 0,
        examId: exam?.id ?? 0,
      };
    });

    setGroups(mappedGroups);
    setOriginalGroups(mappedGroups);
    setLoadingGroups(false);
  };

  const addToUpdateList = (group: GroupRowProps) => {
    setUpdateList((prev) => {
      const original = originalGroups.find((g) => g.id === group.id);

      // Dacă rândul e identic cu originalul, îl scoatem din updateList
      const isModified =
        group.selectedLocationId !== original?.selectedLocationId ||
        group.selectedClassroomId !== original?.selectedClassroomId ||
        group.examDate !== original?.examDate ||
        group.examDuration !== original?.examDuration;

      if (!isModified) {
        return prev.filter((g) => g.id !== group.id); // scoate dacă nu e modificat
      }

      // Dacă e modificat, adaugă sau actualizează în listă
      const exists = prev.find((g) => g.id === group.id);
      if (exists) {
        return prev.map((g) => (g.id === group.id ? group : g));
      }
      return [...prev, group];
    });
  };

  const handleLocationChange = (groupId: number, locationId: number) => {
    setGroups((prev) =>
      prev.map((g) => {
        if (g.id === groupId) {
          const updated = { ...g, selectedLocationId: locationId, selectedClassroomId: null };
          addToUpdateList(updated); // update inteligent
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
          addToUpdateList(updated); // întreaga linie
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
          addToUpdateList(updated); // întreaga linie
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
          addToUpdateList(updated); // întreaga linie
          return updated;
        }
        return g;
      })
    );
  };

  const handleUpdateAll = async () => {
    if (!selectedSubjectId) {
      toast.error(t("Select_a_subject"));
      return;
    }

    console.log("Lista de update înainte de trimis:", updateList);

    try {
      for (const group of updateList) {
        if (!group.selectedClassroomId) continue;

        await updateExam({
          id: group.examId,
          date: group.examDate ?? "",
          duration: group.examDuration ?? 0,
          classroomId: group.selectedClassroomId,
          subjectId: selectedSubjectId,
          studentGroupId: group.selectedGroupId!,
        });
      }
      toast.success(t("Exams_updated_successfully"));
      setUpdateList([]);
    } catch {
      toast.error(t("Error_updating_exams"));
    }
  };

  const locationWidth = getMaxLocationWidth();
  const classroomWidth = getMaxClassroomWidth();

  return (
    <div className="exam-page-container">
      <h2>
        Profesor: {user.firstName} {user.lastName}
      </h2>

      <select value={selectedSubjectId || ""} onChange={(e) => handleSubjectSelect(Number(e.target.value))}>
        <option value="" disabled hidden>
          Selectable materia
        </option>
        {teacherSubjects.map((s) => (
          <option key={s.id} value={s.id}>
            {s.name}
          </option>
        ))}
      </select>

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
          {loadingGroups && <TableGlimmer no_lines={10} no_cols={5} />}
          {!loadingGroups && groups.length > 0 && (
            <>
              {groups.map((group) => {
                const selectedLocation = locations.find((l) => l.id === group.selectedLocationId);

                return (
                  <tr key={group.id}>
                    <td>{group.name}</td>

                    <td>
                      <select
                        value={group.selectedLocationId || ""}
                        onChange={(e) => handleLocationChange(group.id, Number(e.target.value))}
                        style={{ width: `${locationWidth}px` }}
                      >
                        <option value="" disabled hidden>
                          Selectează locația
                        </option>
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
                        <option value="" disabled hidden>
                          Selectează clasa
                        </option>
                        {(
                          selectedLocation?.classrooms.filter(
                            (c) =>
                              // păstrează doar clasele care nu sunt deja selectate de alte grupe
                              !groups.some((g2) => g2.selectedClassroomId === c.id && g2.id !== group.id)
                          ) || []
                        ).map((c) => (
                          <option key={c.id} value={c.id}>
                            {c.name}
                          </option>
                        ))}
                      </select>
                    </td>

                    <td>
                      <input
                        type="datetime-local"
                        className="date-input"
                        value={group.examDate || ""}
                        onChange={(e) => handleExamDateChange(group.id, e.target.value)}
                      />
                    </td>

                    <td>
                      <input
                        type="text"
                        value={group.examDuration ?? ""}
                        onChange={(e) => {
                          const val = e.target.value;
                          if (/^\d*$/.test(val)) handleDurationChange(group.id, val === "" ? null : Number(val));
                        }}
                      />
                    </td>
                  </tr>
                );
              })}
            </>
          )}
        </tbody>
      </table>

      {groups.length > 0 && (
        <div style={{ marginTop: "20px", textAlign: "right" }}>
          <button onClick={handleUpdateAll}>Update</button>
        </div>
      )}
    </div>
  );
};

export default ExamPageByTeacher;
