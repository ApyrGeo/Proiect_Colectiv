import "./googleMaps.css";

import { Marker, useJsApiLoader, OverlayView } from "@react-google-maps/api";
import { GoogleMap } from "@react-google-maps/api";
import { useCallback, useEffect, useMemo, useRef } from "react";
import type { LocationProps } from "../timetable/props.ts";

const mapOptions = {
  streetViewControl: false,
  mapTypeControl: false,
  fullscreenControl: false,
  zoomControl: true,
  disableDoubleClickZoom: true,
  minZoom: 5,
  maxZoom: 18,
  gestureHandling: "cooperative",
  styles: [
    {
      featureType: "poi",
      stylers: [{ visibility: "off" }],
    },
    {
      featureType: "transit",
      stylers: [{ visibility: "off" }],
    },
    {
      featureType: "administrative",
      elementType: "labels",
      stylers: [{ visibility: "off" }],
    },
    {
      featureType: "road",
      elementType: "geometry",
      stylers: [{ visibility: "on" }],
    },
    {
      featureType: "road",
      elementType: "labels",
      stylers: [{ visibility: "on" }],
    },
  ],
};

interface LocationPropsArray {
  locations: LocationProps[];
}

const GoogleMapsComponent: React.FC<LocationPropsArray> = ({ locations }) => {
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: "x",
  });

  const mapRef = useRef<google.maps.Map | null>(null);
  const options = useMemo(() => mapOptions, []);

  const onLoad = useCallback(
    (map: google.maps.Map) => {
      mapRef.current = map;
      if (locations.length > 0) {
        const bounds = new window.google.maps.LatLngBounds();
        locations.forEach((loc) => {
          if (loc.googleMapsData?.latitude && loc.googleMapsData?.longitude) {
            bounds.extend({
              lat: loc.googleMapsData.latitude,
              lng: loc.googleMapsData.longitude,
            });
          }
        });

        if (!bounds.isEmpty()) {
          map.fitBounds(bounds);
        }
      } else {
        map.setCenter({ lat: 46.7712, lng: 23.6236 }); // Default: Cluj-Napoca
        map.setZoom(10);
      }
    },
    [locations]
  );

  useEffect(() => {
    if (!mapRef.current || !window.google?.maps) return;
    if (locations.length === 0) return;

    const bounds = new window.google.maps.LatLngBounds();
    locations.forEach((loc) => {
      if (loc.googleMapsData?.latitude && loc.googleMapsData?.longitude) {
        bounds.extend({
          lat: loc.googleMapsData.latitude,
          lng: loc.googleMapsData.longitude,
        });
      }
    });

    if (!bounds.isEmpty()) {
      mapRef.current.fitBounds(bounds);
    }
  }, [locations]);

  const openInGoogleMaps = (loc: LocationProps) => {
    window.open(`https://www.google.com/maps/search/?api=1&query=${loc.googleMapsData.id}`, "_blank");
  };

  if (!isLoaded) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <div className="widget-map rounded">
        <div className="widget-map-body">
          <GoogleMap
            onLoad={onLoad}
            mapContainerStyle={{
              height: "500px",
              borderRadius: "8px",
            }}
            options={options}
          >
            {locations.map((loc) => (
              <div key={loc.id}>
                <Marker
                  position={{ lat: loc.googleMapsData.latitude, lng: loc.googleMapsData.longitude }}
                  onClick={() => openInGoogleMaps(loc)}
                />
                <OverlayView
                  position={{
                    lat: loc.googleMapsData.latitude + 0.0004,
                    lng: loc.googleMapsData.longitude,
                  }}
                  mapPaneName={OverlayView.OVERLAY_MOUSE_TARGET}
                >
                  <div onClick={() => openInGoogleMaps(loc)} className="map-location-label">
                    {loc.name}
                  </div>
                </OverlayView>
              </div>
            ))}
          </GoogleMap>
        </div>
        <div className="widget-map-list"></div>
      </div>
    </>
  );
};

export default GoogleMapsComponent;
