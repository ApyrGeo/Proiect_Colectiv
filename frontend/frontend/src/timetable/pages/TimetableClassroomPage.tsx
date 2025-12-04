import Timetable from "../components/Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { ClassroomProps } from "../props.ts";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import useTimetableApi from "../useTimetableApi.ts";

const TimetableClassroomPage: React.FC = () => {
  const [classroom, setClassroom] = useState<ClassroomProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const { t } = useTranslation();

  const { getClassroom } = useTimetableApi();

  const handleBack = () => {
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id)) return;

    getClassroom(Number(params.id))
      .then((res) => {
        setClassroom(res);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, [params.id]);

  return (
    <div className={"timetable-page"}>
      {classroom && (
        <>
          <div className={"timetable-title"}>
            {t("Classroom")}: {classroom.name}
          </div>
          <button className={"timetable-back-button"} onClick={handleBack}>
            {t("Back")}
          </button>
          <Timetable classroomId={classroom.id}></Timetable>
        </>
      )}
      {fetchError && <div>{t("Error") + ": " + fetchError.message}</div>}
      {!classroom && !fetchError && <div>{t("Loading")}</div>}
    </div>
  );
};

export default TimetableClassroomPage;
