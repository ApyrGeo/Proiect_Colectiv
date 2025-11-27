import axios from "axios";
import { baseUrl, host } from "../core";

const contractUrl = `${baseUrl}/api/Contract`;

const config = {
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": host,
  },
};

export const getStudyContract: (userId: number) => Promise<string> = (userId: number) => {
  return axios
    .get(contractUrl + "/" + userId, config)
    .then((res) => {
      return URL.createObjectURL(res.data);
    })
    .catch(() => {
      throw Error("Error generating file!");
    });
};
