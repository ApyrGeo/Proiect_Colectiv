import React, { useEffect, useState, type SetStateAction, useRef } from "react";
import Timetable from "../components/Timetable.tsx";
import GoogleMapsComponent from "../../googleMaps/GoogleMapsComponent.tsx";
import type { HourProps, LocationProps, SelectedLocationsProps } from "../props.ts";
import { faqsTimetable } from "../../faq/FAQData.ts";
import FAQPopup from "../../faq/components/FAQPopup.tsx";
import { useTranslation } from "react-i18next";
import { useAuthContext } from "../../auth/context/AuthContext.tsx";

const defaultSelectedLocations: SelectedLocationsProps = {
  currentLocation: null,
  nextLocation: null,
};

const userGroupIds = {
  groupId: 1,
  specId: 1,
  facultyId: 1,
};

const TimetablePage: React.FC = () => {
  const { t } = useTranslation();

  const { userProps } = useAuthContext();

  const [selectedFilter, setSelectedFilter] = useState<string>("personal");
  const [selectedFreq, setSelectedFreq] = useState<string>("all");
  const [activeHours, setActiveHours] = useState<boolean>(true);
  const [locations, setLocations] = useState<LocationProps[]>([]);
  const [selectedLocations, setSelectedLocations] = useState<SelectedLocationsProps>(defaultSelectedLocations);

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

  if (!userProps || !userProps.id) return <div>{t("Error")}</div>;

  return (
    <div className={"container"}>
      <div className={"timetable-page"}>
        <div className={"timetable-title"}>{t("Timetable")}</div>
        <div className={"timetable-filter"}>
          <label>
            <input
              type="radio"
              name="filter"
              value="personal"
              checked={selectedFilter === "personal"}
              onChange={handleChange}
            />
            {t("Personalized")}
          </label>
          <label>
            <input
              type="radio"
              name="filter"
              value="group"
              checked={selectedFilter === "group"}
              onChange={handleChange}
            />
            {t("Group")}
          </label>
          <label>
            <input
              type="radio"
              name="filter"
              value="specialisation"
              checked={selectedFilter === "specialisation"}
              onChange={handleChange}
            />
            {t("Specialization")}
          </label>
          <label>
            <input
              type="radio"
              name="filter"
              value="faculty"
              checked={selectedFilter === "faculty"}
              onChange={handleChange}
            />
            {t("Faculty")}
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
            {t("CurrentWeek")}
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
            {t("Anytime")}
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
            {t("FirstWeek")}
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
            {t("SecondWeek")}
          </label>
        </div>
        {selectedFilter == "personal" && !activeHours && (
          <Timetable
            userId={userProps.id}
            filterFn={getFreqFilter()}
            onHourClick={handleHourClick}
            sendLocationsToMaps={sendLocationsToMaps}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "personal" && activeHours && (
          <Timetable
            userId={userProps.id}
            currentWeekOnly={true}
            onHourClick={handleHourClick}
            sendLocationsToMaps={sendLocationsToMaps}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "group" && (
          <Timetable
            groupYearId={userGroupIds.groupId}
            filterFn={getFreqFilter()}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "specialisation" && (
          <Timetable
            specialisationId={userGroupIds.specId}
            filterFn={getFreqFilter()}
            selectedLocations={selectedLocations}
          />
        )}
        {selectedFilter == "faculty" && (
          <Timetable
            facultyId={userGroupIds.facultyId}
            filterFn={getFreqFilter()}
            selectedLocations={selectedLocations}
          />
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
            {t("SeeRoutesTo")} {selectedLocations.currentLocation.name}
          </button>
        )}
        {selectedLocations.currentLocation && selectedLocations.nextLocation && (
          <button className="timetable-back-button" onClick={handleNavigateBetweenLocations} title="Open Google Maps">
            {t("SeeRoutesBetween")} {selectedLocations.currentLocation.name} & {selectedLocations.nextLocation.name}
          </button>
        )}
        {selectedLocations.currentLocation && (
          <button className="timetable-back-button" onClick={handleCancelSelection} title="Open Google Maps">
            {t("CancelSelection")}
          </button>
        )}
      </div>
    </div>
  );
};

export default TimetablePage;
