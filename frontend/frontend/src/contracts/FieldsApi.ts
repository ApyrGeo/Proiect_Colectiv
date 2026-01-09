import useApiClient from "../core/useApiClient";
import { useCallback } from "react";
import type { OptionalPackageProps, PromotionSelect } from "./props";

export const useOptionalSubjectsApi = () => {
  const { axios } = useApiClient();

  const getOptionalPackages = useCallback(
    async (promotionId: number): Promise<OptionalPackageProps[]> => {
      const res = await axios.get(`/api/timetable/promotions/${promotionId}/subjects/optional`);
      return res.data;
    },
    [axios]
  );

  return { getOptionalPackages };
};

export const usePromotionApi = () => {
  const { axios } = useApiClient();
  const getPromotionsOfUser = useCallback(
    async (userId: number) => {
      const res = await axios.get(`/api/academics/promotions/of-user/${userId}`);
      return res.data as { promotions: PromotionSelect[] };
    },
    [axios]
  );

  return { getPromotionsOfUser };
};
