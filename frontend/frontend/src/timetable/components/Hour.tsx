import type { HourProps, SelectedLocationsProps } from "../props.ts";
import "../timetable.css";
import { useNavigate } from "react-router-dom";
import type { TimetableProps } from "./Timetable.tsx";

export interface HourPropsExtended extends HourProps {
  timetableProps: TimetableProps;
  setSelectedCurrentLocation?: (location: SelectedLocationsProps, key: number) => void;
}

const Hour: React.FC<HourPropsExtended> = ({
  id,
  day,
  hourInterval,
  frequency,
  location,
  classroom,
  format,
  category,
  subject,
  teacher,
  timetableProps,
  isNext,
  isCurrent,
  setSelectedCurrentLocation: setSelectedCurrentLocation,
}) => {
  const navigate = useNavigate();

  const openRoomTable = (classroomId: number | undefined | null) => {
    if (!classroomId) return;
    navigate("/timetable/classroom/" + classroomId);
  };

  const openTeacherTable = (teacherId: number | undefined | null) => {
    if (!teacherId) return;

    navigate("/timetable/teacher/" + teacherId);
  };

  const openSubjectTable = (subjectId: number | undefined | null) => {
    if (!subjectId) return;
    navigate("/timetable/subject/" + subjectId);
  };

  const isMainTimetable: boolean =
    !timetableProps.subjectId && !timetableProps.teacherId && !timetableProps.classroomId;

  return (
    <tr
      className={`timetable-row 
      ${isNext && isMainTimetable ? "timetable-row-next" : ""}
      ${isCurrent && isMainTimetable ? "timetable-row-current" : ""}`}
    >
      <td>{day}</td>
      <td>{hourInterval}</td>
      <td>{frequency}</td>
      {setSelectedCurrentLocation && (
        <td>
          <button
            className={"timetable-button"}
            onClick={() => setSelectedCurrentLocation({ currentLocation: location }, id!)}
          >
            {location.name}
          </button>
        </td>
      )}
      {!timetableProps.classroomId && (
        <td>
          <button className={"timetable-button"} onClick={() => openRoomTable(classroom.id)}>
            {classroom.name}
          </button>
        </td>
      )}
      <td>{format}</td>
      <td>{category}</td>
      {!timetableProps.subjectId && (
        <td>
          <button className={"timetable-button"} onClick={() => openSubjectTable(subject.id)}>
            {subject.name}
          </button>
        </td>
      )}
      {!timetableProps.teacherId && (
        <td>
          <button className={"timetable-button"} onClick={() => openTeacherTable(teacher.id)}>
            {teacher.user.lastName + " " + teacher.user.firstName}
          </button>
        </td>
      )}
    </tr>
  );
};

export default Hour;
