import Timetable from "../Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { TeacherProps } from "../props.ts";
import { useEffect, useState } from "react";
import { getTeacher } from "../TimetableApi.ts";

const TimetableTeacherPage: React.FC = () => {
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const handleBack = () => {
    navigate("");
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
  }, [params]);

  return (
    <>
      {teacher && (
        <>
          <div className={"timetable-title"}>Profesor: {teacher.user.lastName + " " + teacher.user.firstName}</div>
          <Timetable teacherId={teacher.id}></Timetable>
          <button className={"timetable-back-button"} onClick={handleBack}>
            Înapoi
          </button>
        </>
      )}
      {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
    </>
  );
};

export default TimetableTeacherPage;
