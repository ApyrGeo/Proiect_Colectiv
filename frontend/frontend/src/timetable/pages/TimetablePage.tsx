import { useState, type SetStateAction } from "react";
import Timetable from "../Timetable.tsx";
import type { HourProps } from "../props.ts";

const TimetablePage: React.FC = () => {
  // temporary user info, to be loaded from auth context
  const userInfo = {
    id: 123,
    group: 1,
    spec: 2,
    faculty: 3,
  };

  const [selectedFilter, setSelectedFilter] = useState("personal");
  const [selectedFreq, setSelectedFreq] = useState("all");

  const filterFreq1: (hour: HourProps) => boolean = (hour) => hour.frequency == "sapt. 1" || hour.frequency == "";
  const filterFreq2: (hour: HourProps) => boolean = (hour) => hour.frequency == "sapt. 2" || hour.frequency == "";

  const getFreqFilter = () => {
    switch (selectedFreq) {
      case "1":
        return filterFreq1;
      case "2":
        return filterFreq2;
      default:
        return () => true;
    }
  };

  const handleChange = (event: { target: { value: SetStateAction<string> } }) => {
    setSelectedFilter(event.target.value);
  };

  const handleChangeFreq = (event: { target: { value: SetStateAction<string> } }) => {
    setSelectedFreq(event.target.value);
  };

  return (
    <>
      <div className={"timetable-page"}>
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
              value="specialisation"
              checked={selectedFilter === "specialisation"}
              onChange={handleChange}
            />
            Specializare
          </label>
          <label>
            <input
              type="radio"
              name="filter"
              value="faculty"
              checked={selectedFilter === "faculty"}
              onChange={handleChange}
            />
            Facultate
          </label>
        </div>
        <div className={"timetable-filter"}>
          <label>
            <input type="radio" name="freq" value="all" checked={selectedFreq === "all"} onChange={handleChangeFreq} />
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
        {selectedFilter == "personal" && <Timetable userId={userInfo.id} filterFn={getFreqFilter()} />}
        {selectedFilter == "group" && <Timetable groupYearId={userInfo.group} filterFn={getFreqFilter()} />}
        {selectedFilter == "specialisation" && (
          <Timetable specialisationId={userInfo.spec} filterFn={getFreqFilter()} />
        )}
        {selectedFilter == "faculty" && <Timetable facultyId={userInfo.faculty} filterFn={getFreqFilter()} />}
      </div>
    </>
  );
};

export default TimetablePage;
