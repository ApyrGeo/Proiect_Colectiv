import React, { useContext } from "react";
import HourContext from "./HourContext.tsx";
import type { HourProps } from "./HourProps.ts";
import Hour from "./Hour.tsx";
import "./timetable.css";

type TimetableProps = {
  filterFn: (hour: HourProps) => boolean;
};

const Timetable: React.FC<TimetableProps> = ({ filterFn }) => {
  const { hours, fetchingError } = useContext(HourContext);

  return (
    <div className={"timetable"}>
      {hours && (
        <table>
          <thead>
            <tr className={"timetable-header"}>
              <th>Zi</th>
              <th>Interval</th>
              <th>Frecv.</th>
              <th>Locație</th>
              <th>Sală</th>
              <th>Format</th>
              <th>Materie</th>
              <th>Profesor</th>
              <th>Specializare</th>
            </tr>
          </thead>
          <tbody>
            {hours
              .filter(filterFn)
              .map(({ id, day, period, freq, location, room, format, subject, teacher, specialisation }: HourProps) => (
                <Hour
                  key={id}
                  day={day}
                  period={period}
                  freq={freq}
                  specialisation={specialisation}
                  location={location}
                  room={room}
                  format={format}
                  subject={subject}
                  teacher={teacher}
                />
              ))}
          </tbody>
        </table>
      )}

      {fetchingError && <div>{fetchingError.message || "Failed to fetch meshes"}</div>}
    </div>
  );
};

export default Timetable;
