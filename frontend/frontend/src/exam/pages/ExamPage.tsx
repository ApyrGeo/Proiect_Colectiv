import React, { useState } from "react";
import useExamApi from "../ExamApi.ts";
import ExamPageByTeacher from "./ExamPageByTeacher.tsx";
import ExamPageByStudent from "./ExamPageByStudent.tsx";
import type { TeacherProps } from "../props.ts";
import "../ExamPage.css";

const ExamPage: React.FC = () => {
  const { getUserById, getTeacherbyUserId } = useExamApi();
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [studentId, setStudentId] = useState<number | null>(null);
  const [teacherIdInput, setTeacherIdInput] = useState<number | "">("");
  const [loading, setLoading] = useState(false);

  const handleCheckUser = async () => {
    if (teacherIdInput === "") return;
    setLoading(true);

    try {
      const user = await getUserById(Number(teacherIdInput));

      if (user.role === 1) {
        const teacherData = await getTeacherbyUserId(user.id);
        setTeacher(teacherData);
        setStudentId(null);
      } else if (user.role === 0) {
        setStudentId(user.id);
        setTeacher(null);
      } else {
        alert("ID invalid!");
        setTeacher(null);
        setStudentId(null);
      }
    } catch {
      alert("ID invalid!");
      setTeacher(null);
      setStudentId(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="exam-page-container">
      <h1>Verificare profesor/student</h1>

      <div className="teacher-check">
        <input
          type="number"
          placeholder="Introduceți ID profesor/student"
          value={teacherIdInput}
          onChange={(e) => setTeacherIdInput(e.target.value === "" ? "" : Number(e.target.value))}
        />
        <button onClick={handleCheckUser}>Verifică</button>
      </div>

      {loading && <p>Se încarcă datele...</p>}

      {/* Afișăm componentele specializate doar dacă avem date */}
      {teacher && (
        <ExamPageByTeacher id={teacher.id} userId={teacher.userId} user={teacher.user} facultyId={teacher.facultyId} />
      )}
      {studentId && <ExamPageByStudent id={studentId} />}
    </div>
  );
};

export default ExamPage;
