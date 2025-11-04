import Timetable from "../Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { SubjectProps } from "../props.ts";
import { useEffect, useState } from "react";
import { getSubject } from "../TimetableApi.ts";

const TimetableSubjectPage: React.FC = () => {
  const [subject, setSubject] = useState<SubjectProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const handleBack = () => {
    navigate("");
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
  }, [params]);

  return (
    <>
      {subject && (
        <>
          <div className={"timetable-title"}>Materie: {subject.name}</div>
          <Timetable subjectId={subject.id}></Timetable>
          <button className={"timetable-back-button"} onClick={handleBack}>
            Înapoi
          </button>
        </>
      )}
      {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
    </>
  );
};

export default TimetableSubjectPage;
