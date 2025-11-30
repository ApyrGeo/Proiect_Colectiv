import { baseUrl } from "../core/index.ts";
import type { ScholarshipStatus, SpecializationResponse } from "./props.ts";

const GRADE_API_URL = `${baseUrl}/api/Grades`;
const USER_API_URL = `${baseUrl}/api/User`;

export const fetchUserSpecializations = async (userId: number): Promise<string[]> => {
  const response = await fetch(`${USER_API_URL}/${userId}/enrolled-specialisations`);
  if (!response.ok) {
    throw new Error("Failed to fetch user specializations");
  }

  const specializations: SpecializationResponse[] = await response.json();
  return specializations.map((s) => s.name);
};

export const fetchStatusForUser = async (
  userId: number,
  specialization: string,
  studyYear: number,
  semester: number
): Promise<ScholarshipStatus> => {
  const response = await fetch(
    `${GRADE_API_URL}/status?userId=${userId}&specialisation=${specialization}&yearOfStudy=${studyYear}&semester=${semester}`
  );
  if (!response.ok) {
    throw new Error("Failed to fetch status for user");
  }
  if (response.status === 204) {
    return null as unknown as ScholarshipStatus;
  }

  const status: ScholarshipStatus = await response.json();
  return status;
};

export const fetchGradesForUser = async (
  userId: number,
  specialization: string,
  studyYear: number,
  semester: number
) => {
  const response = await fetch(
    `${GRADE_API_URL}?userId=${userId}&specialisation=${specialization}&yearOfStudy=${studyYear}&semester=${semester}`
  );

  if (response.status === 204) {
    return [];
  }

  return await response.json();
};
