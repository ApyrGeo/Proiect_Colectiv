import React, { useEffect, useState } from "react";
import type { HourProps, LocationProps, SelectedLocationsProps } from "../props.ts";
import Hour from "./Hour.tsx";
import "../timetable.css";
import { useTranslation } from "react-i18next";
import useTimetableApi from "../useTimetableApi.ts";

export type TimetableProps = {
  userId?: number;
  groupYearId?: number;
  classroomId?: number;
  teacherId?: number;
  facultyId?: number;
  specialisationId?: number;
  subjectId?: number;
  filterFn?: (hour: HourProps) => boolean;
  currentWeekOnly?: boolean;
  onHourClick?: (hourId: number, hours: HourProps[]) => void;
  sendLocationsToMaps?: (locs: LocationProps[]) => void;
  selectedLocations?: SelectedLocationsProps;
};

const Timetable: React.FC<TimetableProps> = (props) => {
  const [hours, setHours] = useState<HourProps[]>([]);
  const [fetchError, setFetchError] = useState<Error | null>(null);

  const functions = useTimetableApi();

  const { t } = useTranslation();

  const sendLocationsToMaps = (hours: HourProps[]) => {
    if (props.sendLocationsToMaps) {
      const uniqueLocations: LocationProps[] = [];
      hours.forEach((hour) => {
        if (!uniqueLocations.find((loc) => loc.id === hour.location.id)) {
          uniqueLocations.push(hour.location);
        }
      });
      props.sendLocationsToMaps(uniqueLocations);
    }
  };

  const getFetchFunc = () => {
    if (props.userId && !props.currentWeekOnly) return functions.getUserHours(props.userId);
    if (props.userId && props.currentWeekOnly) return functions.getUserCurrentWeekHours(props.userId);
    if (props.groupYearId) return functions.getGroupYearHours(props.groupYearId);
    if (props.facultyId) return functions.getFacultyHours(props.facultyId);
    if (props.teacherId) return functions.getTeacherHours(props.teacherId);
    if (props.specialisationId) return functions.getSpecialisationHours(props.specialisationId);
    if (props.classroomId) return functions.getClassroomHours(props.classroomId);
    if (props.subjectId) return functions.getSubjectHours(props.subjectId);

    return functions.getHours();
  };

  useEffect(() => {
    if (!props.selectedLocations) return;
    if (props.selectedLocations.currentLocation !== null) return;
    if (fetchError) return;

    getFetchFunc()
      .then((res) => {
        setHours(res.hours);
        sendLocationsToMaps(res.hours);
      })
      .catch((err) => {
        setFetchError(err as Error);
      });
  }, [props.selectedLocations?.currentLocation]);

  return (
    <div className={"timetable"}>
      {hours && (
        <table>
          <thead>
            <tr className={"timetable-header"}>
              <th>{t("Day")}</th>
              <th>{t("Interval")}</th>
              <th>{t("Frequency")}</th>
              <th>{t("Location")}</th>
              {!props.classroomId && <th>{t("Classroom")}</th>}
              <th>{t("Format")}</th>
              <th>{t("Type")}</th>
              {!props.subjectId && <th>{t("Subject")}</th>}
              {!props.teacherId && <th>{t("Professor")}</th>}
            </tr>
          </thead>
          <tbody>
            {hours
              .filter((a) => !props.filterFn || props.filterFn(a))
              .map((a) => {
                if (a.hourInterval.length == 4) a.hourInterval = "0" + a.hourInterval;
                return a;
              })
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
                    id={id}
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
                    onLocationClick={props.onHourClick ? () => props.onHourClick!(id!, hours) : undefined}
                  />
                )
              )}
          </tbody>
        </table>
      )}

      {fetchError && <div>{fetchError.message || t("Error")}</div>}
    </div>
  );
};

export default Timetable;
