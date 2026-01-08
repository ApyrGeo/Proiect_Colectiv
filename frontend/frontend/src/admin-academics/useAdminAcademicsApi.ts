import useApiClient from "../core/useApiClient.ts";
import { useCallback } from "react";
import type {
  FacultyPostData,
  PromotionPostData,
  SpecialisationPostData,
  StudentGroup,
  StudentGroupPostData,
  StudentSubGroupPostData,
} from "./props.ts";

const useAdminAcademicsApi = () => {
  const { axios } = useApiClient();

  const academicsApi = "/api/Academics";

  const postPromotion = useCallback(
    async (promotion: PromotionPostData) => {
      await axios.post(`${academicsApi}/promotions`, promotion);
    },
    [axios]
  );

  const postFaculty = useCallback(
    async (faculty: FacultyPostData) => {
      await axios.post(`${academicsApi}/faculties`, faculty);
    },
    [axios]
  );

  const postSpecialisation = useCallback(
    async (specialisation: SpecialisationPostData) => {
      await axios.post(`${academicsApi}/specialisations`, specialisation);
    },
    [axios]
  );

  const postGroup = useCallback(
    async (group: StudentGroupPostData) => {
      const result = await axios.post<StudentGroup>(`${academicsApi}/student-groups`, group);
      return result.data;
    },
    [axios]
  );

  const postSubGroup = useCallback(
    async (subgroup: StudentSubGroupPostData) => {
      await axios.post(`${academicsApi}/student-subgroups`, subgroup);
    },
    [axios]
  );

  const getFaculties = useCallback(async () => {
    const response = await axios.get(`${academicsApi}/faculties`);
    return response.data;
  }, [axios]);

  return {
    getFaculties,
    postPromotion,
    postFaculty,
    postSpecialisation,
    postGroup,
    postSubGroup,
  };
};

export default useAdminAcademicsApi;
