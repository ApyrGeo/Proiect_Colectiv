import Timetable from "../Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { ClassroomProps } from "../props.ts";
import { useEffect, useState } from "react";
import { getClassroom } from "../TimetableApi.ts";

const TimetableClassroomPage: React.FC = () => {
  const [classroom, setClassroom] = useState<ClassroomProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const handleBack = () => {
    navigate("");
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
  }, [params]);

  return (
    <>
      {classroom && (
        <>
          <div className={"timetable-title"}>Materie: {classroom.name}</div>
          <Timetable classroomId={classroom.id}></Timetable>
          <button className={"timetable-back-button"} onClick={handleBack}>
            Înapoi
          </button>
        </>
      )}
      {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
    </>
  );
};

export default TimetableClassroomPage;
