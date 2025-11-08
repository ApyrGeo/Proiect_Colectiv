import "./googleMaps.css";

import { Marker, useJsApiLoader, OverlayView } from "@react-google-maps/api";
import { GoogleMap } from "@react-google-maps/api";
import { useCallback, useMemo, useRef } from "react";

const locations = [
  {
    id: "Faculty+of+Economics+and+Business+Administration",
    name: "Fsega",
    position: { lat: 46.773167, lng: 23.621437 },
    textColor: "#222222",
  },
  {
    id: "BBU+Faculty+of+Physics",
    name: "Centru",
    position: { lat: 46.767673, lng: 23.591485 },
    textColor: "#222222",
  },
  {
    id: "Cluj+Innovation+Park",
    name: "Chinteni",
    position: { lat: 46.82067750939712, lng: 23.58030658559796 },
    textColor: "#222222",
  },
];

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

const GoogleMapsComponent = () => {
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: "x",
  });

  const mapRef = useRef(null);
  const options = useMemo(() => mapOptions, []);

  const onLoad = useCallback((map) => {
    mapRef.current = map;
    const bounds = new window.google.maps.LatLngBounds();
    locations.forEach((loc) => bounds.extend(loc.position));
    map.fitBounds(bounds);
  }, []);

  const openInGoogleMaps = (loc) => {
    console.log(`https://www.google.com/maps/place/${loc.id}`);
    window.open(`https://www.google.com/maps/search/?api=1&query=${loc.id}`, "_blank");
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
              width: "100%",
              height: "500px",
              borderRadius: "8px",
            }}
            options={options}
          >
            {locations.map((loc) => (
              <div key={loc.id}>
                <Marker position={loc.position} onClick={() => openInGoogleMaps(loc)} />
                <OverlayView
                  position={{
                    lat: loc.position.lat + 0.0004,
                    lng: loc.position.lng,
                  }}
                  mapPaneName={OverlayView.OVERLAY_MOUSE_TARGET}
                >
                  <div
                    onClick={() => openInGoogleMaps(loc)}
                    style={{
                      color: loc.textColor,
                      padding: "4px 10px",
                      borderRadius: "12px",
                      fontSize: "13px",
                      fontWeight: "600",
                      whiteSpace: "nowrap",
                      boxShadow: "0 2px 6px rgba(0,0,0,0.3)",
                      transform: "translate(-50%, -100%)",
                      textAlign: "center",
                      cursor: "pointer",
                      transition: "transform 0.15s ease, box-shadow 0.15s ease",
                    }}
                    onMouseEnter={(e) => (e.currentTarget.style.boxShadow = "0 4px 10px rgba(0,0,0,0.4)")}
                    onMouseLeave={(e) => (e.currentTarget.style.boxShadow = "0 2px 6px rgba(0,0,0,0.3)")}
                  >
                    {loc.name}
                  </div>
                </OverlayView>
              </div>
            ))}
          </GoogleMap>
        </div>
        <div className="widget-map-list">
          <div className="widget-list bg-none mb-4">
            <div className="widget-list-item">
              <div className="widget-list-media text-center">
                <a href="#">
                  <i className="fab fa-twitter fa-3x"></i>
                </a>
              </div>
              <p className="widget-list-desc">Chinteni</p>
              <div className="widget-list-action">
                <a href="#" data-bs-toggle="dropdown" className="text-body text-opacity-50">
                  <i className="fa fa-angle-right fa-2x"></i>
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default GoogleMapsComponent;
