import useApiClient from "../../core/useApiClient";
import { baseUrl } from "../../core";
import type { ClassroomProps } from "./props";

const CLASSROOM_API_URL = `${baseUrl}/api/timetable/classrooms`;

const useClassroomApi = () => {
  const { axios } = useApiClient();

  const createClassroom = async (classroom: ClassroomProps) => {
    try {
      const response = await axios.post(CLASSROOM_API_URL, classroom);
      return response.data;
    } catch (err) {
      console.error(err);
      throw new Error("Failed to create classroom");
    }
  };

  return {
    createClassroom,
  };
};

export default useClassroomApi;
