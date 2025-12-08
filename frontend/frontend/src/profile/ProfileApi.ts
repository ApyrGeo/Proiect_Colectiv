import useApiClient from "../core/useApiClient.ts";
import { useCallback } from "react";
import type { HourPropsDto } from "../timetable/props.ts";

const ProfileApi = () => {
  const { axios } = useApiClient();

  const profileUrl = "/api/User";
  const getProfile = useCallback(async () => {
    const response = await axios.get<HourPropsDto>(`${profileUrl}/hours`);
    return response.data;
  }, [axios]);
};
import { baseUrl } from "../core/index.ts";
import type { UserProfileResponse } from "./props.ts";

const USER_API_URL = `${baseUrl}/api/User`;

export const fetchUserProfile = async (userId: number): Promise<UserProfileResponse> => {
  const response = await fetch(`${USER_API_URL}/${userId}/profile`);

  if (!response.ok) {
    throw new Error("Failed to fetch user profile");
  }

  return await response.json();
};

export const updateUserSignature = async (userId: number, signatureBase64: string) => {
  const response = await fetch(`${USER_API_URL}/${userId}/profile/signature`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      id: userId,
      signatureBase64,
    }),
  });

  if (!response.ok) {
    throw new Error("Failed to update signature");
  }
};
