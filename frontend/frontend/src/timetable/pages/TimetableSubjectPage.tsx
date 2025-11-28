import Timetable from "../components/Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { SubjectProps } from "../props.ts";
import { useEffect, useState } from "react";
import { getSubject } from "../TimetableApi.ts";
import { useTranslation } from "react-i18next";

const TimetableSubjectPage: React.FC = () => {
  const [subject, setSubject] = useState<SubjectProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const { t } = useTranslation();

  const handleBack = () => {
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id)) return;

    getSubject(Number(params.id))
      .then((res) => {
        setSubject(res);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, [params.id]);

  return (
    <div className={"timetable-page"}>
      {subject && (
        <>
          <div className={"timetable-title"}>
            {t("Subject")}: {subject.name}
          </div>
          <button className={"timetable-back-button"} onClick={handleBack}>
            {t("Back")}
          </button>
          <Timetable subjectId={subject.id}></Timetable>
        </>
      )}
      {fetchError && <div>{t("Error") + ": " + fetchError.message}</div>}
      {!subject && !fetchError && <div>{t("Loading")}</div>}
    </div>
  );
};

export default TimetableSubjectPage;
