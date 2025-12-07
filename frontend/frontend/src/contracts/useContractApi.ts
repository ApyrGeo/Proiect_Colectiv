import useApiClient from "../core/useApiClient.ts";
import { useCallback } from "react";

const useContractApi = () => {
  const { axiosPdf } = useApiClient();

  const contractUrl = "/api/Contract";

  const getStudyContract = useCallback(
    async (userId: number) => {
      const response = await axiosPdf.get(`${contractUrl}/${userId}`);
      return response.data;
    },
    [axiosPdf]
  );

  return {
    getStudyContract,
  };
};

export default useContractApi;
