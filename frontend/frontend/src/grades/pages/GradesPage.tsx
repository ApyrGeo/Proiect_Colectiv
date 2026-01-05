import React, { useEffect, useState } from "react";
import "../grades.css";
import useGradesApi from "../GradesApi.ts";
import TeacherGradesPage from "./TeacherGradesPage.tsx";
import StudentGradesPage from "./StudentGradesPage.tsx";
import type { TeacherProps, UserProps } from "../props.ts";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";
import toast from "react-hot-toast";
import { useTranslation } from "react-i18next";
import Circular from "../../components/loading/Circular.tsx";

const GradesPage: React.FC = () => {
  const { t } = useTranslation();
  const { getUserById, getTeacherbyUserId } = useGradesApi();

  const { userProps } = useAuthContext();

  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [student, setStudent] = useState<UserProps | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadUser = async () => {
      if (!userProps?.id) {
        setLoading(false);
        return;
      }

      try {
        const user = await getUserById(userProps.id);

        // profesor
        if (user.role === 1) {
          const teacherData = await getTeacherbyUserId(user.id);
          setTeacher(teacherData);
        }
        // student
        else if (user.role === 0) {
          setStudent(user);
        }
      } catch (err) {
        toast.error(t("Error_loading_user_data"));
      } finally {
        setLoading(false);
      }
    };

    loadUser();
  }, [getUserById, getTeacherbyUserId, userProps, t]);

  if (loading) {
    return <Circular />;
  }

  return (
    <div className="grades-page-container">
      {teacher && (
        <>
          <TeacherGradesPage
            id={teacher.id}
            userId={teacher.userId}
            user={teacher.user}
            facultyId={teacher.facultyId}
          />
        </>
      )}

      {student && (
        <>
          <StudentGradesPage id={student.id} />
        </>
      )}
    </div>
  );
};

export default GradesPage;
