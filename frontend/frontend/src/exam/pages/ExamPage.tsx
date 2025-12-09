import React, { useState, useEffect, Suspense } from "react";
import useExamApi from "../ExamApi.ts";
import ExamPageByTeacher from "./ExamPageByTeacher.tsx";
import ExamPageByStudent from "./ExamPageByStudent.tsx";
import type { TeacherProps, UserProps } from "../props.ts";
import "../ExamPage.css";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";

const ExamPage: React.FC = () => {
  const { getUserById, getTeacherbyUserId } = useExamApi();
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [student, setStudent] = useState<UserProps | null>(null);
  const [loading, setLoading] = useState(true);

  const { userProps } = useAuthContext();

  useEffect(() => {
    const loadUser = async () => {
      if (!userProps?.id) return;

      const id = userProps.id;

      try {
        const user = await getUserById(id);

        if (user.role === 1) {
          const teacherData = await getTeacherbyUserId(id);
          setTeacher(teacherData);
        } else if (user.role === 0) {
          setStudent(user);
        }
      } catch {
        console.error("Eroare la încărcarea datelor utilizatorului!");
      } finally {
        setLoading(false);
      }
    };

    loadUser();
  }, [userProps]);

  return (
    <div className="exam-page-container">
      <Suspense fallback={<BigSpinner />}>
        {!loading && teacher && (
          <>
            <h1>Examene Profesor</h1>

            <Suspense fallback={<Glimmer />}>
              <Panel>
                <ExamPageByTeacher
                  id={teacher.id}
                  userId={teacher.userId}
                  user={teacher.user}
                  facultyId={teacher.facultyId}
                />
              </Panel>
            </Suspense>
          </>
        )}

        {!loading && student && (
          <>
            <h1>Examene Student</h1>

            <Suspense fallback={<Glimmer />}>
              <Panel>
                <ExamPageByStudent id={student.id} firstName={student.firstName} lastName={student.lastName} />
              </Panel>
            </Suspense>
          </>
        )}
      </Suspense>
    </div>
  );
};

function BigSpinner() {
  return <h2 style={{ textAlign: "center" }}>🌀 Se încarcă utilizatorul...</h2>;
}

function Glimmer() {
  return (
    <div className="glimmer-panel">
      <div className="glimmer-line" />
      <div className="glimmer-line" />
      <div className="glimmer-line" />
    </div>
  );
}

function Panel({ children }) {
  return <section className="panel">{children}</section>;
}

export default ExamPage;
