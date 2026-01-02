import useApiClient from "../../core/useApiClient";
import { baseUrl } from "../../core";
import type { LocationPostProps, LocationProps } from "./props";

const LOCATION_API_URL = `${baseUrl}/api/Location`;

const useLocationApi = () => {
  const { axios } = useApiClient();

  const fetchLocations = async (): Promise<LocationProps[]> => {
    try {
      const response = await axios.get<LocationProps[]>(LOCATION_API_URL);
      return response.data;
    } catch (err) {
      console.error(err);
      throw new Error("Failed to fetch locations");
    }
  };

  const createLocation = async (location: LocationPostProps) => {
    try {
      const response = await axios.post(LOCATION_API_URL, location);
      return response.data;
    } catch (err) {
      console.error(err);
      throw new Error("Failed to create location");
    }
  };

  return {
    fetchLocations,
    createLocation,
  };
};

export default useLocationApi;
