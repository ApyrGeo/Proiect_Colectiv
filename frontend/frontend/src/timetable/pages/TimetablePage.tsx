import { useEffect, useState, type SetStateAction, useRef } from "react";
import Timetable from "../components/Timetable.tsx";
import GoogleMapsComponent from "../../googleMaps/GoogleMapsComponent.tsx";
import type { HourProps, LocationProps, SelectedLocationsProps } from "../props.ts";

const defaultSelectedLocations: SelectedLocationsProps = {
  currentLocation: null,
  nextLocation: null,
};

const TimetablePage: React.FC = () => {
  //TODO temporary user info, to be loaded from auth context
  //groupYear, spec, faculty invalid example id's
  const userInfo = {
    id: 11111,
    groupYear: 47,
    spec: 2,
    faculty: 3,
  };

  const [selectedFilter, setSelectedFilter] = useState("personal");
  const [selectedFreq, setSelectedFreq] = useState("all");
  const [activeHours, setActiveHours] = useState(true);
  const [locations, setLocations] = useState([]);
  const [selectedLocations, setSelectedLocations] = useState(defaultSelectedLocations);

  const sendLocationsToMaps = (locs: LocationProps[]) => {
    setLocations(locs);
  };

  const isCompatibleFrequency = (freq1: string, freq2: string) => {
    return (
      (freq1 === "FirstWeek" && freq2 !== "SecondWeek") ||
      (freq1 === "SecondWeek" && freq2 !== "FirstWeek") ||
      freq1 === "Weekly" ||
      freq2 === "Weekly"
    );
  };

  const handleCancelSelection = () => {
    setSelectedLocations(defaultSelectedLocations);
    setTimeout(() => {
      scrollToTopPage();
    }, 0);
  };

  const handleHourClick = (hourId: number, hours: HourProps[]) => {
    const hourIndex = hours.findIndex((h) => h.id === hourId);
    if (hourIndex === -1) return;

    const currentLocation = hours[hourIndex].location;
    const nextLocation =
      hourIndex < hours.length - 1 && // not the last hour
      hours[hourIndex].day === hours[hourIndex + 1].day && // same day
      hours[hourIndex].location.id !== hours[hourIndex + 1].location.id && // different location
      isCompatibleFrequency(hours[hourIndex].frequency, hours[hourIndex + 1].frequency) // possible to have both consecutive hours in the same week
        ? hours[hourIndex + 1].location
        : null;

    setLocations([currentLocation]);
    setSelectedLocations({ currentLocation, nextLocation });
  };

  const sectionNavigationButtonsRef = useRef<HTMLDivElement | null>(null);

  const handleNavigateFromCurrentLocation = () => {
    if (!selectedLocations.currentLocation) return;

    const { latitude: destLat, longitude: destLng } = selectedLocations.currentLocation!.googleMapsData;

    const mapsUrl = `https://www.google.com/maps/dir/?api=1&destination=${destLat},${destLng}`;
    window.open(mapsUrl, "_blank");
  };

  const handleNavigateBetweenLocations = () => {
    if (!selectedLocations.currentLocation) return;
    if (!selectedLocations.nextLocation) return;

    const { latitude: originLat, longitude: originLng } = selectedLocations.nextLocation!.googleMapsData;
    const { latitude: destLat, longitude: destLng } = selectedLocations.currentLocation!.googleMapsData;

    const mapsUrl = `https://www.google.com/maps/dir/?api=1&origin=${originLat},${originLng}&destination=${destLat},${destLng}`;
    window.open(mapsUrl, "_blank");
  };

  const scrollToNavigationButtonsSection = () => {
    sectionNavigationButtonsRef.current?.scrollIntoView({
      behavior: "smooth",
      block: "start",
    });
  };

  const scrollToTopPage = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  useEffect(() => {
    if (selectedLocations.currentLocation) {
      scrollToNavigationButtonsSection();
    }
  }, [selectedLocations.currentLocation]);

  const filterFreq1: (hour: HourProps) => boolean = (hour) =>
    hour.frequency == "FirstWeek" || hour.frequency == "Weekly";
  const filterFreq2: (hour: HourProps) => boolean = (hour) =>
    hour.frequency == "SecondWeek" || hour.frequency == "Weekly";

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
    <div className={"container"}>
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
          <label hidden={selectedFilter != "personal"}>
            <input
              type="checkbox"
              name="active"
              value="active"
              checked={activeHours}
              onChange={() => setActiveHours(!activeHours)}
            />
            Săptămâna curentă
          </label>
        </div>
        <div className={"timetable-filter"}>
          <label>
            <input
              disabled={activeHours && selectedFilter == "personal"}
              type="radio"
              name="freq"
              value="all"
              checked={selectedFreq === "all"}
              onChange={handleChangeFreq}
            />
            Oricând
          </label>
          <label>
            <input
              disabled={activeHours && selectedFilter == "personal"}
              type="radio"
              name="freq"
              value="1"
              checked={selectedFreq === "1"}
              onChange={handleChangeFreq}
            />
            Săpt. 1
          </label>
          <label>
            <input
              disabled={activeHours && selectedFilter == "personal"}
              type="radio"
              name="freq"
              value="2"
              checked={selectedFreq === "2"}
              onChange={handleChangeFreq}
            />
            Săpt. 2
          </label>
        </div>
        {selectedFilter == "personal" && !activeHours && (
          <Timetable
            userId={userInfo.id}
            filterFn={getFreqFilter()}
            onHourClick={handleHourClick}
            sendLocationsToMaps={sendLocationsToMaps}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "personal" && activeHours && (
          <Timetable
            userId={userInfo.id}
            currentWeekOnly={true}
            onHourClick={handleHourClick}
            sendLocationsToMaps={sendLocationsToMaps}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "group" && (
          <Timetable
            groupYearId={userInfo.groupYear}
            filterFn={getFreqFilter()}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "specialisation" && (
          <Timetable
            specialisationId={userInfo.spec}
            filterFn={getFreqFilter()}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "faculty" && (
          <Timetable facultyId={userInfo.faculty} filterFn={getFreqFilter()} selectedLocations={selectedLocations} />
        )}
      </div>

      <GoogleMapsComponent locations={locations} />

      <div ref={sectionNavigationButtonsRef}>
        {selectedLocations.currentLocation && (
          <button
            className="timetable-back-button"
            onClick={handleNavigateFromCurrentLocation}
            title="Open Google Maps"
          >
            Vezi rute către {selectedLocations.currentLocation.name}
          </button>
        )}
        {selectedLocations.currentLocation && selectedLocations.nextLocation && (
          <button className="timetable-back-button" onClick={handleNavigateBetweenLocations} title="Open Google Maps">
            Vezi rute de la {selectedLocations.currentLocation.name} la {selectedLocations.nextLocation.name}
          </button>
        )}
        {selectedLocations.currentLocation && (
          <button className="timetable-back-button" onClick={handleCancelSelection} title="Open Google Maps">
            Anuleaza selectia
          </button>
        )}
      </div>
    </div>
  );
};

export default TimetablePage;
