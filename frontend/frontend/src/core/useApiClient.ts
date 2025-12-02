import axios from "axios";
import { baseUrl, host } from ".";
import { useEffect, useMemo } from "react";
import { useAuthContext } from "../auth/context/AuthContext";

const useApiClient = () => {
  const { accessToken, activeAccount } = useAuthContext();

  const apiClient = useMemo(
    () =>
      axios.create({
        baseURL: baseUrl,
        headers: {
          "Content-Type": "application/json",
          "Access-Control-Allow-Origin": host,
        },
      }),
    []
  );

  useEffect(() => {
    const responseInterceptor = apiClient.interceptors.response.use(
      (response) => response,
      (error) => {
        console.error("API call failed:", error);
        if (error.response.status === 401) {
          // Unauthorized
        } else if (error.response.status === 404) {
          // Not found
        }

        return Promise.reject(error);
      }
    );
    return () => {
      apiClient.interceptors.response.eject(responseInterceptor);
    };
  }, []);

  useEffect(() => {
    const reqInterceptor = apiClient.interceptors.request.use((config) => {
      if (activeAccount && accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
      return config;
    });

    return () => {
      apiClient.interceptors.request.eject(reqInterceptor);
    };
  }, [accessToken, activeAccount]);

  return { axios: apiClient };
};

export default useApiClient;
