import React, { useEffect, useState } from "react";
import type { HourProps } from "./props.ts";
import Hour from "./Hour.tsx";
import "./timetable.css";
import {
  getClassroomHours,
  getFacultyHours,
  getGroupYearHours,
  getHours,
  getSpecialisationHours,
  getSubjectHours,
  getTeacherHours,
  getUserHours,
} from "./TimetableApi.ts";

export type TimetableProps = {
  userId?: number;
  groupYearId?: number;
  classroomId?: number;
  teacherId?: number;
  facultyId?: number;
  specialisationId?: number;
  subjectId?: number;

  filterFn?: (hour: HourProps) => boolean;
};

const Timetable: React.FC<TimetableProps> = (props) => {
  const [hours, setHours] = useState<HourProps[]>([]);
  const [getError, setGetError] = useState<Error | null>(null);

  const getFetchFunc = () => {
    if (props.userId) return getUserHours(props.userId);
    if (props.groupYearId) return getGroupYearHours(props.groupYearId);
    if (props.facultyId) return getFacultyHours(props.facultyId);
    if (props.teacherId) return getTeacherHours(props.teacherId);
    if (props.specialisationId) return getSpecialisationHours(props.specialisationId);
    if (props.classroomId) return getClassroomHours(props.classroomId);
    if (props.subjectId) return getSubjectHours(props.subjectId);

    return getHours();
  };

  useEffect(() => {
    getFetchFunc()
      .then((res) => {
        setHours(res);
      })
      .catch((err) => {
        setGetError(err as Error);
      });
  });

  const sortOrder = ["Luni", "Marti", "Miercuri", "Joi", "Vineri"];

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
              <th>Tip</th>
              <th>Materie</th>
              <th>Profesor</th>
              {/*<th>Specializare</th>*/}
            </tr>
          </thead>
          <tbody>
            {hours
              .sort((a, b) => a.frequency.localeCompare(b.frequency))
              .sort((a, b) => a.hourInterval.localeCompare(b.hourInterval))
              .sort((a, b) => sortOrder.indexOf(a.day) - sortOrder.indexOf(b.day))
              .map(
                ({
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
                  specialisation,
                }: HourProps) => (
                  <Hour
                    key={id}
                    day={day}
                    hourInterval={hourInterval}
                    frequency={frequency}
                    specialisation={specialisation}
                    location={location}
                    classroom={classroom}
                    format={format}
                    category={category}
                    subject={subject}
                    teacher={teacher}
                    timetableProps={props}
                  />
                )
              )}
          </tbody>
        </table>
      )}

      {getError && <div>{getError.message || "Failed to fetch meshes"}</div>}
    </div>
  );
};

export default Timetable;
