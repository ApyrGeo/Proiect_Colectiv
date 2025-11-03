import type { HourProps } from "./HourProps.ts";
import "./timetable.css";
import { useNavigate } from "react-router-dom";
import { useSearchParams } from "react-router";

const Hour: React.FC<HourProps> = ({ day, period, freq, location, room, format, category, subject, teacher }) => {
  const navigate = useNavigate();

  const [searchParam] = useSearchParams();

  const openRoomTable = (room: string) => {
    if (searchParam.get("room") != room) navigate("?room=" + room);
  };

  const openTeacherTable = (teacher: string) => {
    if (searchParam.get("teacher") != teacher) navigate("?teacher=" + teacher);
  };

  const openSubjectTable = (subject: string) => {
    if (searchParam.get("subject") != subject) navigate("?subject=" + subject);
  };

  return (
    <tr className={"timetable-row"}>
      <td>{day}</td>
      <td>{period}</td>
      <td>{freq}</td>
      <td>{location}</td>
      <td>
        <button className={"timetable-button"} onClick={() => openRoomTable(room)}>
          {room}
        </button>
      </td>
      <td>{format}</td>
      <td>{category}</td>
      <td>
        <button className={"timetable-button"} onClick={() => openSubjectTable(subject)}>
          {subject}
        </button>
      </td>
      <td>
        <button className={"timetable-button"} onClick={() => openTeacherTable(teacher)}>
          {teacher}
        </button>
      </td>
      {/*<td>{specialisation}</td>*/}
    </tr>
  );
};

export default Hour;
