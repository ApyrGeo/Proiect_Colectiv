import { useCallback } from "react";
import useApiClient from "../../core/useApiClient.ts";

export type StudentSubGroupResult = {
  id: number;
  name: string;
};

export type StudentGroupResult = {
  id: number;
  name: string;
  studentSubGroups: StudentSubGroupResult[];
};

export type PromotionResult = {
  id: number;
  startYear: number;
  endYear: number;
  studentGroups: StudentGroupResult[];
};

export type BulkEnrollmentItemResult = {
  row: number;
  email: string | null;
  isValid: boolean;
  isCreated: boolean;
  createdEnrollmentId?: number | null;
  errors: string[];
};

export type BulkPromotionResult = {
  promotion: PromotionResult | null;
  enrollments: BulkEnrollmentItemResult[];
  isValid: boolean;
};

export type SpecialisationResult = {
  id: number;
  name: string;
};

const useImportPromotionApi = () => {
  const { axios } = useApiClient();

  const academicsUrl = "/api/Academics";

  const uploadPromotionFile = useCallback(
    async (params: { file: File; startYear: number; endYear: number; specialisationId: number }) => {
      const formData = new FormData();
      formData.append("StartYear", params.startYear.toString());
      formData.append("EndYear", params.endYear.toString());
      formData.append("SpecialisationId", params.specialisationId.toString());
      formData.append("enrollmentFile", params.file);

      const response = await axios.post<BulkPromotionResult>(`${academicsUrl}/promotions/bulk`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      return response.data;
    },
    [axios]
  );

  const getAllSpecialisations = useCallback(async () => {
    const response = await axios.get<SpecialisationResult[]>(`${academicsUrl}/specialisations`);
    return response.data;
  }, [axios]);

  return { uploadPromotionFile, getAllSpecialisations };
};

export default useImportPromotionApi;
