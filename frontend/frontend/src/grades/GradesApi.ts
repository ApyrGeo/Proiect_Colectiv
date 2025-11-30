import { baseUrl } from "../core/index.ts";
import type { GradesDataProps, ScholarshipStatus, SpecializationResponse } from "./props.ts";

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

//TODO: Remove mock data when backend is ready
export const mockGrades: GradesDataProps = {
  average_score: 4.2,
  result: [
    {
      id: "1",
      subject: { name: "Analiza", credits: 5 },
      score: 10,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "2",
      subject: { name: "Algebra", credits: 4 },
      score: 2,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "3",
      subject: { name: "ASC", credits: 4 },
      score: 6,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "4",
      subject: { name: "Fizica", credits: 3 },
      score: 4,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Fizica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "5",
      subject: { name: "Chimie", credits: 3 },
      score: 5,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "6",
      subject: { name: "Sport", credits: 2 },
      score: 10,
      for_score: false,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "7",
      subject: { name: "Informatica", credits: 5 },
      score: 8,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "8",
      subject: { name: "Matematica", credits: 4 },
      score: 3,
      for_score: true,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
    {
      id: "9",
      subject: { name: "Logica", credits: 3 },
      score: 7,
      for_score: true,
      academicYear: "2024/2025",
      specialization: "Informatica",
      studyYear: 2,
      semester: 2,
    },
    {
      id: "10",
      subject: { name: "Engleza", credits: 2 },
      score: 0,
      for_score: true,
      academicYear: "2023/2024",
      specialization: "Fizica",
      studyYear: 1,
      semester: 2,
    },
    {
      id: "11",
      subject: { name: "Desen", credits: 2 },
      score: 10,
      for_score: false,
      academicYear: "2025/2026",
      specialization: "Matematica",
      studyYear: 1,
      semester: 1,
    },
  ],
};
