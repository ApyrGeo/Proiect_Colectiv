import React, { useState, useEffect } from "react";
import useExamApi from "../ExamApi.ts";
import ExamPageByTeacher from "./ExamPageByTeacher.tsx";
import ExamPageByStudent from "./ExamPageByStudent.tsx";
import type { TeacherProps, UserProps } from "../props.ts";
import "../ExamPage.css";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";
import toast from "react-hot-toast";
import { t } from "i18next";

const ExamPage: React.FC = () => {
  const { getUserById, getTeacherbyUserId } = useExamApi();
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [student, setStudent] = useState<UserProps | null>(null);

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
        toast.error(t("Error_loading_user_data"));
      }
    };

    loadUser();
  }, [getTeacherbyUserId, getUserById, userProps]);

  return (
    <div className="exam-page-container">
      {teacher && (
        <>
          <h1>Examene Profesor</h1>
          <section className="panel">
            <ExamPageByTeacher
              id={teacher.id}
              userId={teacher.userId}
              user={teacher.user}
              facultyId={teacher.facultyId}
            />
          </section>
        </>
      )}

      {student && (
        <>
          <h1>Examene Student</h1>

          <section className="panel">
            <ExamPageByStudent id={student.id} firstName={student.firstName} lastName={student.lastName} />
          </section>
        </>
      )}
    </div>
  );
};

export default ExamPage;
