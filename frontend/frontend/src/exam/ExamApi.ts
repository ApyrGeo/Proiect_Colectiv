import type { Location, ClassroomDetails, Teacher, StudentGroup } from "./props";
import { baseUrl } from "../core/index.ts";

// Obține locații folosind token-ul deja obținut
export const getLocations = async (accessToken: string | null): Promise<Location[]> => {
  if (!accessToken) throw new Error("Nu există token de autentificare MSAL");

  const response = await fetch(`${baseUrl}/api/Timetable/locations`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (!response.ok) throw new Error(`Failed to fetch locations: ${response.status}`);
  const data: Location[] = await response.json();
  return Array.isArray(data) ? data : [];
};

// Obține detalii despre o clasă
export const getClassroomDetails = async (
  accessToken: string | null,
  classroomId: number
): Promise<ClassroomDetails> => {
  if (!accessToken) throw new Error("Nu există token de autentificare MSAL");

  const response = await fetch(`${baseUrl}/api/Timetable/classrooms/${classroomId}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (!response.ok) throw new Error(`Failed to fetch classroom details: ${response.status}`);
  const data: ClassroomDetails = await response.json();
  return data;
};

// Verifică dacă utilizatorul este profesor
export const getTeacherById = async (accessToken: string | null, teacherId: number): Promise<Teacher | null> => {
  if (!accessToken) throw new Error("Nu există token de autentificare MSAL");

  const response = await fetch(`${baseUrl}/api/Academics/teachers/${teacherId}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (response.status === 404) return null; // nu e profesor
  if (!response.ok) throw new Error(`Failed to fetch teacher info: ${response.status}`);

  const data: Teacher = await response.json();
  return data;
};

export const getStudentGroupsByTeacher = async (
  accessToken: string | null,
  teacherId: number
): Promise<StudentGroup[]> => {
  if (!accessToken) throw new Error("Nu există token de autentificare MSAL");

  const response = await fetch(`${baseUrl}/api/Academics/student-groups/teachers/${teacherId}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (!response.ok) throw new Error(`Failed to fetch student groups: ${response.status}`);
  const data: StudentGroup[] = await response.json();
  return Array.isArray(data) ? data : [];
};
