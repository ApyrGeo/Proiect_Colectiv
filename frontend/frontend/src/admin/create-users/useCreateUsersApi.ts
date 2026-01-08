import { useCallback } from "react";
import useApiClient from "../../core/useApiClient.ts";

export type BulkUserItemResult = {
  row: number;
  email: string | null;
  isValid: boolean;
  isCreated: boolean;
  createdUserId?: number | null;
  errors: string[];
};

export type BulkUserResult = {
  users: BulkUserItemResult[];
  isValid: boolean;
};

const useCreateUsersApi = () => {
  const { axios } = useApiClient();

  const userUrl = "/api/User";

  const uploadUsersFile = useCallback(
    async (file: File) => {
      const formData = new FormData();
      formData.append("file", file);

      const response = await axios.post<BulkUserResult>(`${userUrl}/bulk`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      return response.data;
    },
    [axios]
  );

  return { uploadUsersFile };
};

export default useCreateUsersApi;
