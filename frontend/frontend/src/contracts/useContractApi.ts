import useApiClient from "../core/useApiClient.ts";
import { useCallback } from "react";
import type { StudyContractPayload } from "./props.ts";

const useContractApi = () => {
  const { axiosPdf } = useApiClient();

  const contractUrl = "/api/Contract";

  const getStudyContract = useCallback(
    async (userId: number, payload: StudyContractPayload) => {
      const response = await axiosPdf.post(`${contractUrl}/${userId}`, payload);
      return response.data;
    },
    [axiosPdf]
  );

  return {
    getStudyContract,
  };
};

export default useContractApi;
