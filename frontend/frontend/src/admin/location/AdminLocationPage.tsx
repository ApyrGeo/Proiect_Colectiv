import React, { useEffect, useState } from "react";
import {
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Box,
  Typography,
  Paper,
  Divider,
  Fab,
  type SelectChangeEvent,
} from "@mui/material";
import {
  Add as AddIcon,
  LocationOn as LocationOnIcon,
  Save as SaveIcon,
  KeyboardArrowUp as KeyboardArrowUpIcon,
} from "@mui/icons-material";
import { useTranslation } from "react-i18next";
import useLocationApi from "./LocationApi";
import useClassroomApi from "./ClassroomApi";
import type { LocationPostProps, LocationProps } from "./props";
import GoogleMapsComponent from "../../googleMaps/GoogleMapsComponent";
import { toast } from "react-hot-toast";
import "./AdminLocationPage.css";

type Mode = "NEW" | "EXISTING";
const AdminLocationPage: React.FC = () => {
  const { fetchLocations, createLocation } = useLocationApi();
  const { createClassroom } = useClassroomApi();
  const { t } = useTranslation();

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
  const [locationErrors, setLocationErrors] = useState({ name: false, address: false });
  const [classroomError, setClassroomError] = useState(false);
  const [showScrollTop, setShowScrollTop] = useState(false);

  useEffect(() => {
    fetchLocations().then(setLocations);
  }, []);

  useEffect(() => {
    const handleScroll = () => {
      setShowScrollTop(window.scrollY > 300);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSaveLocation = async () => {
    let locationId: number;

    if (selectedLocation) {
      locationId = selectedLocation.id;
    } else {
      const nextErrors = {
        name: newLocation.name.trim().length === 0,
        address: newLocation.address.trim().length === 0,
      };
      setLocationErrors(nextErrors);

      if (nextErrors.name || nextErrors.address) {
        toast.error(t("adminLocation.toast.fillLocationFields"));
        scrollToTop();
        return;
      }

      try {
        const createdLocation = await createLocation(newLocation);
        locationId = createdLocation.id;
      } catch {
        toast.error(t("adminLocation.toast.locationCreateFail"));
        return;
      }
    }
    setSavedLocationId(locationId);
    toast.success(t("adminLocation.toast.locationSaved"));
  };

  const handleAddClassroom = async () => {
    if (!savedLocationId) {
      toast.error(t("adminLocation.toast.saveLocationFirst"));
      return;
    }

    if (classroomName.trim().length === 0) {
      setClassroomError(true);
      toast.error(t("adminLocation.toast.classroomRequired"));
      return;
    }

    try {
      await createClassroom({
        name: classroomName,
        locationId: savedLocationId,
      });
      setClassroomName("");
      setClassroomError(false);
      toast.success(t("adminLocation.toast.classroomAdded"));
    } catch {
      toast.error(t("adminLocation.toast.classroomCreateFail"));
    }
  };

  return (
    <div className="admin-location-container">
      <Paper className="main-container-wrapper" elevation={4}>
        <Typography variant="h3" className="page-header" gutterBottom>
          <LocationOnIcon sx={{ fontSize: "2.5rem", verticalAlign: "middle", mr: 1 }} />
          {t("adminLocation.title")}
        </Typography>

        <Box className="mode-selector">
          <Button
            variant={mode === "NEW" ? "contained" : "outlined"}
            color="primary"
            size="large"
            startIcon={<AddIcon />}
            onClick={() => {
              setMode("NEW");
              setSelectedLocation(null);
              setSavedLocationId(null);
            }}
          >
            {t("adminLocation.addNew")}
          </Button>
          <Button
            variant={mode === "EXISTING" ? "contained" : "outlined"}
            color="primary"
            size="large"
            startIcon={<LocationOnIcon />}
            onClick={() => {
              setMode("EXISTING");
              setSavedLocationId(null);
            }}
          >
            {t("adminLocation.useExisting")}
          </Button>
        </Box>

        <Divider className="section-divider" />

        {mode === "NEW" && (
          <Paper className="location-form" elevation={3}>
            <Typography variant="h5" color="primary" gutterBottom sx={{ mb: 3 }}>
              {t("adminLocation.newDetails")}
            </Typography>
            <Box className="form-inputs">
              <TextField
                fullWidth
                label={t("adminLocation.locationName")}
                variant="outlined"
                value={newLocation.name}
                error={locationErrors.name}
                helperText={locationErrors.name ? t("adminLocation.helper.locationName") : undefined}
                onChange={(e) => {
                  setLocationErrors((prev) => ({ ...prev, name: false }));
                  setNewLocation({ ...newLocation, name: e.target.value });
                }}
                placeholder={t("adminLocation.enterLocationName")}
              />

              <TextField
                fullWidth
                label={t("adminLocation.address")}
                variant="outlined"
                value={newLocation.address}
                error={locationErrors.address}
                helperText={locationErrors.address ? t("adminLocation.helper.address") : undefined}
                onChange={(e) => {
                  setLocationErrors((prev) => ({ ...prev, address: false }));
                  setNewLocation({ ...newLocation, address: e.target.value });
                }}
                placeholder={t("adminLocation.enterAddress")}
              />
            </Box>

            <Box className="map-container">
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
            </Box>

            <Box className="save-button-container">
              <Button
                variant="contained"
                color="primary"
                size="large"
                startIcon={<SaveIcon />}
                onClick={handleSaveLocation}
              >
                {t("adminLocation.saveLocation")}
              </Button>
            </Box>
          </Paper>
        )}

        {mode === "EXISTING" && (
          <Paper className="location-form" elevation={3}>
            <Typography variant="h5" color="primary" gutterBottom sx={{ mb: 3 }}>
              {t("adminLocation.selectExisting")}
            </Typography>
            <FormControl fullWidth variant="outlined">
              <InputLabel id="location-select-label">{t("adminLocation.selectLabel")}</InputLabel>
              <Select
                labelId="location-select-label"
                value={selectedLocation?.id ?? ""}
                onChange={(e: SelectChangeEvent<number | string>) => {
                  const loc = locations.find((l) => l.id === Number(e.target.value));
                  setSelectedLocation(loc ?? null);
                  setSavedLocationId(loc ? loc.id : null);
                }}
                label={t("adminLocation.selectLabel")}
              >
                <MenuItem value="">
                  <em>{t("adminLocation.noneOption")}</em>
                </MenuItem>
                {locations.map((loc) => (
                  <MenuItem key={loc.id} value={loc.id}>
                    {loc.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            {selectedLocation && (
              <Box className="location-details">
                <Typography variant="body1">
                  <strong>{t("adminLocation.nameLabel")}</strong> {selectedLocation.name}
                </Typography>
                <Typography variant="body1">
                  <strong>{t("adminLocation.addressLabel")}</strong> {selectedLocation.address}
                </Typography>
              </Box>
            )}
          </Paper>
        )}

        <Divider className="section-divider" />

        <Paper className="classroom-section" elevation={3}>
          <Typography variant="h5" color="primary" gutterBottom sx={{ mb: 3 }}>
            {t("adminLocation.addClassroom")}
          </Typography>
          <Box className="classroom-form">
            <TextField
              className="classroom-input"
              label={t("adminLocation.classroomName")}
              variant="outlined"
              value={classroomName}
              error={classroomError}
              helperText={classroomError ? t("adminLocation.helper.classroomName") : undefined}
              onChange={(e) => {
                setClassroomError(false);
                setClassroomName(e.target.value);
              }}
              placeholder={t("adminLocation.enterClassroomName")}
            />

            {showScrollTop && (
              <Fab
                color="primary"
                size="medium"
                onClick={scrollToTop}
                sx={{
                  position: "fixed",
                  bottom: 32,
                  right: 32,
                  zIndex: 1000,
                }}
              >
                <KeyboardArrowUpIcon />
              </Fab>
            )}
            <Button
              variant="contained"
              color="success"
              size="large"
              startIcon={<SaveIcon />}
              onClick={handleAddClassroom}
              disabled={!savedLocationId}
            >
              {t("adminLocation.saveClassroom")}
            </Button>
          </Box>
        </Paper>
      </Paper>
    </div>
  );
};

export default AdminLocationPage;
