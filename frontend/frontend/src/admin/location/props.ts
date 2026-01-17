export interface ClassroomProps {
  name: string;
  locationId: number;
}

export interface LocationPostProps {
  name: string;
  address: string;
  latitude: number;
  longitude: number;
}

export interface LocationProps {
  id: number;
  name: string;
  address?: string;
  googleMapsData: {
    latitude: number;
    longitude: number;
    id: string;
  };
}
