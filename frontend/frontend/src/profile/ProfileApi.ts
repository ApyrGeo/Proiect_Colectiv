import useApiClient from "../core/useApiClient.ts";
import { useCallback } from "react";
import type { HourPropsDto } from "../timetable/props.ts";

const ProfileApi = () => {
  const { axios } = useApiClient();

  const profileUrl = "/api/User";



  const getProfile = useCallback(async () => {
    const response = await axios.get<HourPropsDto>(`${profileUrl}/hours`);
    return response.data;
  }, [axios]);
};
