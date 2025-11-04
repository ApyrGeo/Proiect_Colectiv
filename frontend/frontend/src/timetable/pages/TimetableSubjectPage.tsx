import Timetable from "../components/Timetable.tsx";
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
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id) || fetchError) return;

    getSubject(Number(params.id))
      .then((res) => {
        setSubject(res);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, [params]);

  return (
    <div className={"timetable-page"}>
      {subject && (
        <>
          <div className={"timetable-title"}>Materie: {subject.name}</div>
          <button className={"timetable-back-button"} onClick={handleBack}>
            Înapoi
          </button>
          <Timetable subjectId={subject.id}></Timetable>
        </>
      )}
      {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
    </div>
  );
};

export default TimetableSubjectPage;
