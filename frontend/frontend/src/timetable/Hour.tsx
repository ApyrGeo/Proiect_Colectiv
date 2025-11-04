import type { HourProps } from "./props.ts";
import "./timetable.css";
import { useNavigate } from "react-router-dom";
import type { TimetableProps } from "./Timetable.tsx";

export interface HourPropsExtended extends HourProps {
  timetableProps: TimetableProps;
}

const Hour: React.FC<HourPropsExtended> = ({
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
}) => {
  const navigate = useNavigate();

  const openRoomTable = (classroomId: number) => {
    navigate("/classroom/" + classroomId);
  };

  const openTeacherTable = (teacherId: number) => {
    navigate("/teacher/" + teacherId);
  };

  const openSubjectTable = (subjectId: number) => {
    navigate("/subject/" + subjectId);
  };

  return (
    <tr className={"timetable-row"}>
      <td>{day}</td>
      <td>{hourInterval}</td>
      <td>{frequency}</td>
      <td>{location.name}</td>
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
      {/*<td>{specialisation}</td>*/}
    </tr>
  );
};

export default Hour;
