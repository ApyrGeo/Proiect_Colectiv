import useApiClient from "../core/useApiClient";
import type { Profile } from "./props.ts";
import { baseUrl } from "../core/index.ts";

const USER_API_URL = `${baseUrl}api/User`;

const useUserApi = () => {
  const { axios } = useApiClient();

  const fetchUserProfile = async (userId: number): Promise<Profile> => {
    try {
      const response = await axios.get<Profile>(`${USER_API_URL}/${userId}/profile`);
      return response.data;
    } catch (err) {
      console.error(err);
      throw new Error("Failed to fetch user profile");
    }
  };

  const updateUserSignature = async (userId: number, signatureBase64: string) => {
    try {
      const response = await axios.put(`${USER_API_URL}/${userId}/profile`, {
        id: userId,
        signatureBase64,
      });
      return response.data;
    } catch (err) {
      console.error(err);
      throw new Error("Failed to update signature");
    }
  };

  return {
    fetchUserProfile,
    updateUserSignature,
  };
};

export default useUserApi;
