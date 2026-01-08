import useApiClient from "../core/useApiClient";
import { useCallback } from "react";
import type { OptionalPackageProps } from "./props";

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
  const getPromotion = useCallback(
    async (promotionId: number) => {
      const res = await axios.get(`/api/academics/promotions/${promotionId}`);
      return res.data;
    },
    [axios]
  );

  return { getPromotion };
};
