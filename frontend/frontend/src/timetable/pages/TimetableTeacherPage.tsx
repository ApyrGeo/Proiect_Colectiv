import Timetable from "../components/Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { TeacherProps } from "../props.ts";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import useTimetableApi from "../useTimetableApi.ts";
import useAuth from "../../auth/hooks/useAuth.ts";
import { UserRole } from "../../core/props.ts";

const TimetableTeacherPage: React.FC = () => {
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const { t } = useTranslation();

  const { getTeacher } = useTimetableApi();
  const { userProps } = useAuth();

  const handleBack = () => {
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id)) return;

    getTeacher(Number(params.id))
      .then((res) => {
        setTeacher(res);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, [getTeacher, params.id]);

  return (
    <>
      <div className={"timetable-page"}>
        {teacher && (
          <div className="container">
            <div className={"timetable-title"}>
              {t("Professor")}: {teacher.user.lastName + " " + teacher.user.firstName}
            </div>
            {userProps?.role != UserRole.TEACHER && (
              <button className={"timetable-back-button"} onClick={handleBack}>
                {t("Back")}
              </button>
            )}
            <Timetable teacherId={teacher.id}></Timetable>
          </div>
        )}
        {fetchError && <div>{t("Error") + ": " + fetchError.message}</div>}
        {!teacher && !fetchError && <div>{t("Loading")}</div>}
      </div>
    </>
  );
};

export default TimetableTeacherPage;
