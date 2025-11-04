import Timetable from "../components/Timetable.tsx";
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
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id) || fetchError) return;

    getClassroom(Number(params.id))
      .then((res) => {
        setClassroom(res);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, []);

  return (
    <div className={"timetable-page"}>
      {classroom && (
        <>
          <div className={"timetable-title"}>Sală: {classroom.name}</div>
          <button className={"timetable-back-button"} onClick={handleBack}>
            Înapoi
          </button>
          <Timetable classroomId={classroom.id}></Timetable>
        </>
      )}
      {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
      {!classroom && !fetchError && <div>Loading</div>}
    </div>
  );
};

export default TimetableClassroomPage;
