import type { HourProps } from "./HourProps.ts";
import { useState, type SetStateAction } from "react";
import Timetable from "./Timetable.tsx";
import { useSearchParams } from "react-router";

const TimetablePage: React.FC = () => {
  const userInfo = {
    semigroup: "236/1",
    group: "236",
    spec: "I3",
  };

  const [selectedFilter, setSelectedFilter] = useState("semigroup");
  const [searchParams] = useSearchParams();

  // const navigate = useNavigate();

  const roomValue = searchParams.get("room");
  const teacherValue = searchParams.get("room");

  const filterSemiGroup: (hour: HourProps) => boolean = (hour) =>
    hour.format == userInfo.semigroup && hour.specialisation == userInfo.spec;
  const filterGroup: (hour: HourProps) => boolean = (hour) =>
    (hour.format == userInfo.group || hour.format == userInfo.semigroup) && hour.specialisation == userInfo.spec;
  const filterSpec: (hour: HourProps) => boolean = (hour) => hour.specialisation == userInfo.spec;
  const filterRoom: (hour: HourProps) => boolean = (hour) => hour.room == roomValue;
  const filterTeacher: (hour: HourProps) => boolean = (hour) => hour.teacher == teacherValue;

  // const curryFilter = function (func: (hour: HourProps, value: string | null) => boolean, value: string | null) {
  //   return function (hour: HourProps) {
  //     return func(hour, value);
  //   };
  // };

  const handleChange = (event: { target: { value: SetStateAction<string> } }) => {
    setSelectedFilter(event.target.value);
  };

  console.log(teacherValue);
  console.log(roomValue);

  return (
    <div className={"timetable-page"}>
      {/*specifiec filters*/}
      {roomValue && <Timetable filterFn={filterRoom}></Timetable>}
      {teacherValue && <Timetable filterFn={filterTeacher}></Timetable>}

      {/*main timetable*/}
      {!roomValue && !teacherValue && (
        <>
          <div className={"timetable-filter"}>
            <label>
              <input
                type="radio"
                name="filter"
                value="semigroup"
                checked={selectedFilter === "semigroup"}
                onChange={handleChange}
              />
              Semi-group
            </label>
            <br />
            <label>
              <input
                type="radio"
                name="filter"
                value="group"
                checked={selectedFilter === "group"}
                onChange={handleChange}
              />
              Group
            </label>
            <br />
            <label>
              <input
                type="radio"
                name="filter"
                value="spec"
                checked={selectedFilter === "spec"}
                onChange={handleChange}
              />
              All
            </label>
          </div>
          {selectedFilter === "semigroup" && <Timetable filterFn={filterSemiGroup}></Timetable>}
          {selectedFilter === "group" && <Timetable filterFn={filterGroup}></Timetable>}
          {selectedFilter === "spec" && <Timetable filterFn={filterSpec}></Timetable>}
        </>
      )}
    </div>
  );
};

export default TimetablePage;
