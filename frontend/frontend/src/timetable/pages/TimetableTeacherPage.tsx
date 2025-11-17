import Timetable from "../components/Timetable.tsx";
import { useNavigate, useParams } from "react-router-dom";
import type { TeacherProps } from "../props.ts";
import { useEffect, useState } from "react";
import { getTeacher } from "../TimetableApi.ts";
import GoogleMapsComponent from "../../googleMaps/GoogleMapsComponent.tsx";

const TimetableTeacherPage: React.FC = () => {
  const [teacher, setTeacher] = useState<TeacherProps | null>(null);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const navigate = useNavigate();
  const params = useParams();

  const handleBack = () => {
    navigate("/timetable");
  };

  useEffect(() => {
    if (!params.id || !Number(params.id) || fetchError) return;

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
      <div className={"timetable-page"}>
        {teacher && (
          <>
            <div className={"timetable-title"}>Profesor: {teacher.user.lastName + " " + teacher.user.firstName}</div>
            <button className={"timetable-back-button"} onClick={handleBack}>
              Înapoi
            </button>
            <Timetable teacherId={teacher.id}></Timetable>
          </>
        )}
        {fetchError && <div>{"Fetch error: " + fetchError.message}</div>}
      </div>
    </>
  );
};

export default TimetableTeacherPage;
