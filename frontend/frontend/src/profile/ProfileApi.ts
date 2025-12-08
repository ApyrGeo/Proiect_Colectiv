import { baseUrl } from "../core/index.ts";
import type { Profile } from "./props.ts";

const USER_API_URL = `${baseUrl}/api/User`;

export const fetchUserProfile = async (userId: number): Promise<Profile> => {
  const response = await fetch(`${USER_API_URL}/${userId}/profile`);

  if (!response.ok) {
    throw new Error("Failed to fetch user profile");
  }

  return await response.json();
};

export const updateUserSignature = async (userId: number, signatureBase64: string) => {
  const response = await fetch(`${USER_API_URL}/${userId}/profile`, {
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
