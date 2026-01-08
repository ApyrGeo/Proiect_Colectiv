import React, { useEffect, useState } from "react";
import useLocationApi from "./LocationApi";
import useClassroomApi from "./ClassroomApi";
import type { LocationPostProps, LocationProps } from "./props";
import GoogleMapsComponent from "../../googleMaps/GoogleMapsComponent";
import { toast } from "react-hot-toast";

type Mode = "NEW" | "EXISTING";
const AdminLocationPage: React.FC = () => {
  const { fetchLocations, createLocation } = useLocationApi();
  const { createClassroom } = useClassroomApi();

  const [locations, setLocations] = useState<LocationProps[]>([]);
  const [selectedLocation, setSelectedLocation] = useState<LocationProps | null>(null);
  const [savedLocationId, setSavedLocationId] = useState<number | null>(null);
  const [mode, setMode] = useState<Mode>("NEW");
  const [newLocation, setNewLocation] = useState<LocationPostProps>({
    name: "",
    address: "",
    latitude: 46.7712,
    longitude: 23.6236,
  });

  const [classroomName, setClassroomName] = useState("");

  useEffect(() => {
    fetchLocations().then(setLocations);
  }, []);
  const handleSaveLocation = async () => {
    let locationId: number;

    if (selectedLocation) {
      locationId = selectedLocation.id;
    } else {
      const createdLocation = await createLocation(newLocation);
      locationId = createdLocation.id;
    }
    setSavedLocationId(locationId);
  };

  const handleAddClassroom = async () => {
    if (!savedLocationId) {
      toast.error("Save a location first!");
      return;
    }

    await createClassroom({
      name: classroomName,
      locationId: savedLocationId,
    });

    setClassroomName("");
  };

  return (
    <div>
      <h2>Create Location & Classroom</h2>
      <div style={{ marginBottom: "1rem" }}>
        <button
          onClick={() => {
            setMode("NEW");
            setSelectedLocation(null);
            setSavedLocationId(null);
          }}
        >
          Add new location
        </button>
        <button
          onClick={() => {
            setMode("EXISTING");
            setSavedLocationId(null);
          }}
        >
          Use existing location
        </button>
      </div>

      <hr />
      {mode === "NEW" && (
        <>
          <input
            placeholder="Location name"
            value={newLocation.name}
            onChange={(e) => setNewLocation({ ...newLocation, name: e.target.value })}
          />

          <input
            placeholder="Address"
            value={newLocation.address}
            onChange={(e) => setNewLocation({ ...newLocation, address: e.target.value })}
          />

          <GoogleMapsComponent
            editable
            locations={[
              {
                id: 0,
                name: newLocation.name || "New location",
                googleMapsData: {
                  latitude: newLocation.latitude,
                  longitude: newLocation.longitude,
                  id: "",
                },
              },
            ]}
            onMapClick={(lat, lng) => setNewLocation({ ...newLocation, latitude: lat, longitude: lng })}
          />
          <button onClick={handleSaveLocation}>Save Location</button>
        </>
      )}
      {mode === "EXISTING" && (
        <>
          <select
            value={selectedLocation?.id ?? ""}
            onChange={(e) => {
              const loc = locations.find((l) => l.id === Number(e.target.value));
              setSelectedLocation(loc ?? null);
              setSavedLocationId(loc ? loc.id : null);
            }}
          >
            <option value="">Select location</option>
            {locations.map((loc) => (
              <option key={loc.id} value={loc.id}>
                {loc.name}
              </option>
            ))}
          </select>

          {selectedLocation && (
            <div>
              {" "}
              <p>Name: {selectedLocation.name}</p> <p>Address: {selectedLocation.address}</p>{" "}
            </div>
          )}
        </>
      )}

      <hr />
      <input placeholder="Classroom name" value={classroomName} onChange={(e) => setClassroomName(e.target.value)} />

      <button onClick={handleAddClassroom} disabled={!savedLocationId}>
        Save
      </button>
    </div>
  );
};

export default AdminLocationPage;
