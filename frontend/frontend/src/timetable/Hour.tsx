import type { HourProps } from "./HourProps.ts";
import "./timetable.css";

const Hour: React.FC<HourProps> = ({ day, period, freq, location, room, format, subject, teacher, specialisation }) => {
  return (
    <tr className={"timetable-row"}>
      <td>{day}</td>
      <td>{period}</td>
      <td>{freq}</td>
      <td>{location}</td>
      <td>{room}</td>
      <td>{format}</td>
      <td>{subject}</td>
      <td>{teacher}</td>
      <td>{specialisation}</td>
    </tr>
  );
};

export default Hour;
