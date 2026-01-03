import { useCallback } from "react";
import useApiClient from "../core/useApiClient";
import type { ScholarshipStatus, SpecializationResponse } from "./props";

const useGradesApi = () => {
  const { axios } = useApiClient();

  const gradesUrl = "/api/Grades";
  const userUrl = "/api/User";

  // 📌 Specializările la care este înscris studentul
  const getUserSpecializations = useCallback(
    async (userId: number): Promise<string[]> => {
      const response = await axios.get<SpecializationResponse[]>(`${userUrl}/${userId}/enrolled-specialisations`);

      return response.data?.map((s) => s.name) ?? [];
    },
    [axios]
  );

  // 📌 Status bursă student (poate fi null)
  const getScholarshipStatusForUser = useCallback(
    async (
      userId: number,
      specialization: string,
      studyYear: number,
      semester: number
    ): Promise<ScholarshipStatus | null> => {
      const response = await axios.get<ScholarshipStatus | null>(`${gradesUrl}/status`, {
        params: {
          userId,
          specialisation: specialization,
          yearOfStudy: studyYear,
          semester,
        },
        validateStatus: (status) => status === 200 || status === 204,
      });

      return response.status === 204 ? null : response.data;
    },
    [axios]
  );

  // 📌 Notele studentului (cu filtre opționale)
  const getGradesForUser = useCallback(
    async (userId: number, specialization?: string | null, studyYear?: number | null, semester?: number | null) => {
      const response = await axios.get(`${gradesUrl}`, {
        params: {
          userId,
          ...(specialization && { specialisation: specialization }),
          ...(studyYear && { yearOfStudy: studyYear }),
          ...(semester && { semester }),
        },
        validateStatus: (status) => status === 200 || status === 204,
      });

      if (response.status === 204) return [];
      return Array.isArray(response.data) ? response.data : [];
    },
    [axios]
  );

  return {
    getUserSpecializations,
    getScholarshipStatusForUser,
    getGradesForUser,
  };
};

export default useGradesApi;
