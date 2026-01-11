import { useEffect, useState } from "react";
import useExamGenerationApi from "./exam-generation-api.ts";
import type { FacultyProps, TeacherProps, SubjectProps, StudentGroupProps } from "./props";
import "./AdminExamPage.css";
import toast from "react-hot-toast";

const ExamGenerationPage = () => {
  const {
    getFaculties,
    getTeachers,
    getSubjectsByTeacher,
    getStudentGroupsBySubject,
    generateExamEntries,
  } = useExamGenerationApi();

  const [faculties, setFaculties] = useState<FacultyProps[]>([]);
  const [selectedFacultyId, setSelectedFacultyId] = useState<number | "">("");
  const [teachers, setTeachers] = useState<TeacherProps[]>([]);
  const [selectedTeacherId, setSelectedTeacherId] = useState<number | "">("");
  const [subjects, setSubjects] = useState<SubjectProps[]>([]);
  const [selectedSubjectId, setSelectedSubjectId] = useState<number | "">("");
  const [studentGroups, setStudentGroups] = useState<StudentGroupProps[]>([]);
  const [selectedGroupIds, setSelectedGroupIds] = useState<number[]>([]); // multi select

  const [loadingFaculties, setLoadingFaculties] = useState(false);
  const [loadingTeachers, setLoadingTeachers] = useState(false);
  const [loadingSubjects, setLoadingSubjects] = useState(false);
  const [loadingGroups, setLoadingGroups] = useState(false);
  const [loadingGenerate, setLoadingGenerate] = useState(false); // 🔹 loading pentru generare

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null); // 🔹 mesaj de succes

  // Toggle grupuri
  const toggleGroupSelection = (id: number) => {
    if (selectedGroupIds.includes(id)) {
      setSelectedGroupIds(selectedGroupIds.filter((gid) => gid !== id));
    } else {
      setSelectedGroupIds([...selectedGroupIds, id]);
    }
  };

  // 🔹 funcția de generare examen
  const handleGenerateExam = async () => {
    if (!selectedSubjectId || selectedGroupIds.length === 0) {
      setError("Selectează o materie și cel puțin o grupă.");
      return;
    }
    try {
      setError(null);
      setSuccess(null);
      setLoadingGenerate(true);
      await generateExamEntries(selectedSubjectId as number, selectedGroupIds);
      toast.success("Examenul a fost generat cu succes!");
    } catch (e) {
      toast.error("Eroare la generarea examenului.");
    } finally {
      setLoadingGenerate(false);
    }
  };

  // === useEffects pentru încărcări (facultăți, profesori, materii, grupe) ===
  useEffect(() => {
    const loadFaculties = async () => {
      try {
        setLoadingFaculties(true);
        const data = await getFaculties();
        setFaculties(data);
      } catch {
        setError("Eroare la încărcarea facultăților");
      } finally {
        setLoadingFaculties(false);
      }
    };
    loadFaculties();
  }, [getFaculties]);

  useEffect(() => {
    const loadTeachers = async () => {
      if (selectedFacultyId === "") {
        setTeachers([]);
        setSelectedTeacherId("");
        setSubjects([]);
        setSelectedSubjectId("");
        setStudentGroups([]);
        setSelectedGroupIds([]);
        return;
      }
      try {
        setLoadingTeachers(true);
        const data = await getTeachers(selectedFacultyId);
        setTeachers(data);
        setSelectedTeacherId("");
        setSubjects([]);
        setSelectedSubjectId("");
        setStudentGroups([]);
        setSelectedGroupIds([]);
      } catch {
        setError("Eroare la încărcarea profesorilor");
      } finally {
        setLoadingTeachers(false);
      }
    };
    loadTeachers();
  }, [selectedFacultyId, getTeachers]);

  useEffect(() => {
    const loadSubjects = async () => {
      if (selectedTeacherId === "") {
        setSubjects([]);
        setSelectedSubjectId("");
        setStudentGroups([]);
        setSelectedGroupIds([]);
        return;
      }
      try {
        setLoadingSubjects(true);
        const data = await getSubjectsByTeacher(selectedTeacherId);
        setSubjects(data);
        setSelectedSubjectId("");
        setStudentGroups([]);
        setSelectedGroupIds([]);
      } catch {
        setError("Eroare la încărcarea materiilor");
      } finally {
        setLoadingSubjects(false);
      }
    };
    loadSubjects();
  }, [selectedTeacherId, getSubjectsByTeacher]);

  useEffect(() => {
    const loadStudentGroups = async () => {
      if (selectedSubjectId === "") {
        setStudentGroups([]);
        setSelectedGroupIds([]);
        return;
      }
      try {
        setLoadingGroups(true);
        const data = await getStudentGroupsBySubject(selectedSubjectId);
        setStudentGroups(data);
        setSelectedGroupIds([]);
      } catch {
        setError("Eroare la încărcarea grupelor de studenți");
      } finally {
        setLoadingGroups(false);
      }
    };
    loadStudentGroups();
  }, [selectedSubjectId, getStudentGroupsBySubject]);

  return (
    <div className="exam-container">
      <h2 className="exam-title">Generare Examen</h2>

      {error && <p className="exam-error">{error}</p>}
      {success && <p className="exam-success">{success}</p>}

      {/* Facultăți */}
      <div className="exam-field">
        <label htmlFor="faculty">Facultate</label>
        {loadingFaculties ? (
          <p>Se încarcă facultățile...</p>
        ) : (
          <select
            id="faculty"
            value={selectedFacultyId}
            onChange={(e) => setSelectedFacultyId(e.target.value === "" ? "" : Number(e.target.value))}
            className="exam-select"
          >
            <option value="">-- Selectează o facultate --</option>
            {faculties.map((f) => (
              <option key={f.id} value={f.id}>
                {f.name}
              </option>
            ))}
          </select>
        )}
      </div>

      {/* Profesori */}
      {selectedFacultyId && (
        <div className="exam-field">
          <label htmlFor="teacher">Profesor</label>
          {loadingTeachers ? (
            <p>Se încarcă profesorii...</p>
          ) : teachers.length > 0 ? (
            <select
              id="teacher"
              value={selectedTeacherId}
              onChange={(e) => setSelectedTeacherId(e.target.value === "" ? "" : Number(e.target.value))}
              className="exam-select"
            >
              <option value="">-- Selectează un profesor --</option>
              {teachers.map((t) => (
                <option key={t.id} value={t.id}>
                  {t.user.firstName} {t.user.lastName}
                </option>
              ))}
            </select>
          ) : (
            <p>Nu există profesori pentru această facultate.</p>
          )}
        </div>
      )}

      {/* Materii */}
      {selectedTeacherId && (
        <div className="exam-field">
          <label htmlFor="subject">Materie</label>
          {loadingSubjects ? (
            <p>Se încarcă materiile...</p>
          ) : subjects.length > 0 ? (
            <select
              id="subject"
              value={selectedSubjectId}
              onChange={(e) => setSelectedSubjectId(e.target.value === "" ? "" : Number(e.target.value))}
              className="exam-select"
            >
              <option value="">-- Selectează o materie --</option>
              {subjects.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          ) : (
            <p>Nu există materii pentru acest profesor.</p>
          )}
        </div>
      )}

      {/* Multi-select vizual grupe */}
      {selectedSubjectId && (
        <div className="exam-field">
          <label>Grupe</label>
          {loadingGroups ? (
            <p>Se încarcă grupele...</p>
          ) : studentGroups.length > 0 ? (
            <div className="group-grid">
              {studentGroups.map((g) => {
                const selected = selectedGroupIds.includes(g.id);
                return (
                  <div
                    key={g.id}
                    className={`group-item ${selected ? "selected" : ""}`}
                    onClick={() => toggleGroupSelection(g.id)}
                  >
                    <span className="group-name">{g.name}</span>
                    <span className="group-checkbox">{selected && <span className="checkmark">✔</span>}</span>
                  </div>
                );
              })}
            </div>
          ) : (
            <p>Nu există grupe pentru această materie.</p>
          )}
        </div>
      )}

      {/* Buton generare examen */}
      {selectedSubjectId && selectedGroupIds.length > 0 && (
        <button className="generate-button" onClick={handleGenerateExam} disabled={loadingGenerate}>
          {loadingGenerate ? "Se generează..." : "Generează Examen"}
        </button>
      )}
    </div>
  );
};

export default ExamGenerationPage;
