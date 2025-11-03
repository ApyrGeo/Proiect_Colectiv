import type { HourProps } from "./HourProps.ts";
import { useState, type SetStateAction } from "react";
import Timetable from "./Timetable.tsx";
import { useSearchParams } from "react-router";
import { HourProvider } from "./HourProvider.tsx";

const TimetablePage: React.FC = () => {
  // temporary user info, to be loaded from auth context
  const userInfo = {
    semigroup: "236/1",
    group: "236",
    spec: "I3",
    optionals: ["IOC"],
  };

  const [selectedFilter, setSelectedFilter] = useState("semigroup");
  const [selectedFreq, setSelectedFreq] = useState("all");
  const [searchParams, setSearchParams] = useSearchParams();

  const roomValue = searchParams.get("room");
  const teacherValue = searchParams.get("teacher");
  const subjectValue = searchParams.get("subject");

  const filterPersonal: (hour: HourProps) => boolean = (hour) =>
    (hour.format == userInfo.semigroup ||
      hour.format == userInfo.group ||
      userInfo.optionals.includes(hour.subject) ||
      hour.format == userInfo.spec) &&
    hour.specialisation == userInfo.spec;
  const filterSemiGroup: (hour: HourProps) => boolean = (hour) =>
    (hour.format == userInfo.group || hour.format == userInfo.semigroup) && hour.specialisation == userInfo.spec;
  const filterGroup: (hour: HourProps) => boolean = (hour) =>
    (hour.format == userInfo.group || hour.format.split("/")[0] == userInfo.group) &&
    hour.specialisation == userInfo.spec;

  const filterSpec: (hour: HourProps) => boolean = (hour) => hour.specialisation == userInfo.spec;
  const filterFreq1: (hour: HourProps) => boolean = (hour) => hour.freq == "sapt. 1" || hour.freq == "";
  const filterFreq2: (hour: HourProps) => boolean = (hour) => hour.freq == "sapt. 2" || hour.freq == "";

  const filterRoom: (hour: HourProps) => boolean = (hour) => hour.room == roomValue;
  const filterTeacher: (hour: HourProps) => boolean = (hour) => hour.teacher == teacherValue;
  const filterSubject: (hour: HourProps) => boolean = (hour) => hour.subject == subjectValue;

  const generateFilterPrimary: () => (hour: HourProps) => boolean = () => {
    switch (selectedFilter) {
      case "personal": {
        return filterPersonal;
      }
      case "semigroup": {
        return filterSemiGroup;
      }
      case "group": {
        return filterGroup;
      }
      case "spec": {
        return filterSpec;
      }
      default: {
        return () => true;
      }
    }
  };

  const generateFilterFreq: () => (hour: HourProps) => boolean = () => {
    switch (selectedFreq) {
      case "all": {
        return () => true;
      }
      case "1": {
        return filterFreq1;
      }
      case "2": {
        return filterFreq2;
      }
      default: {
        return () => true;
      }
    }
  };

  const generateFilters: () => (hour: HourProps) => boolean = () => {
    return (hour: HourProps) => {
      return generateFilterPrimary()(hour) && generateFilterFreq()(hour);
    };
  };

  const handleChange = (event: { target: { value: SetStateAction<string> } }) => {
    setSelectedFilter(event.target.value);
  };

  const handleChangeFreq = (event: { target: { value: SetStateAction<string> } }) => {
    setSelectedFreq(event.target.value);
  };

  const handleBack = () => {
    setSearchParams("");
  };

  return (
    <HourProvider>
      <div className={"timetable-page"}>
        {/*specific filters*/}
        {roomValue && (
          <>
            <div className={"timetable-title"}>Sală: {roomValue}</div>
            <Timetable filterFn={filterRoom}></Timetable>
            <button className={"timetable-back-button"} onClick={handleBack}>
              Înapoi
            </button>
          </>
        )}
        {teacherValue && (
          <>
            <div className={"timetable-title"}>Profesor: {teacherValue}</div>
            <Timetable filterFn={filterTeacher}></Timetable>
            <button className={"timetable-back-button"} onClick={handleBack}>
              Înapoi
            </button>
          </>
        )}
        {subjectValue && (
          <>
            <div className={"timetable-title"}>Materie: {subjectValue}</div>
            <Timetable filterFn={filterSubject}></Timetable>
            <button className={"timetable-back-button"} onClick={handleBack}>
              Înapoi
            </button>
          </>
        )}

        {/*main timetable*/}
        {!roomValue && !teacherValue && !subjectValue && (
          <>
            <div className={"timetable-title"}>Orar</div>
            <div className={"timetable-filter"}>
              <label>
                <input
                  type="radio"
                  name="filter"
                  value="personal"
                  checked={selectedFilter === "personal"}
                  onChange={handleChange}
                />
                Personalizat
              </label>
              <label>
                <input
                  type="radio"
                  name="filter"
                  value="semigroup"
                  checked={selectedFilter === "semigroup"}
                  onChange={handleChange}
                />
                Semi-grupă
              </label>
              <label>
                <input
                  type="radio"
                  name="filter"
                  value="group"
                  checked={selectedFilter === "group"}
                  onChange={handleChange}
                />
                Grupă
              </label>
              <label>
                <input
                  type="radio"
                  name="filter"
                  value="spec"
                  checked={selectedFilter === "spec"}
                  onChange={handleChange}
                />
                Toate
              </label>
            </div>
            <div className={"timetable-filter"}>
              <label>
                <input
                  type="radio"
                  name="freq"
                  value="all"
                  checked={selectedFreq === "all"}
                  onChange={handleChangeFreq}
                />
                Oricând
              </label>
              <label>
                <input type="radio" name="freq" value="1" checked={selectedFreq === "1"} onChange={handleChangeFreq} />
                Săpt. 1
              </label>
              <label>
                <input type="radio" name="freq" value="2" checked={selectedFreq === "2"} onChange={handleChangeFreq} />
                Săpt. 2
              </label>
            </div>
            <Timetable filterFn={generateFilters()} />
          </>
        )}
      </div>
    </HourProvider>
  );
};

export default TimetablePage;
