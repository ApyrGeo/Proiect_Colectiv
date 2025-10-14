import axios from "axios";
import type { HourProps } from "./HourProps";

const baseUrl = "http://localhost:3000";
const hourUrl = `${baseUrl}/hours`;

interface ResponseProps<T> {
  data: T;
}

async function withLogs<T>(promise: Promise<ResponseProps<T>>, fnName: string): Promise<T> {
  console.log(fnName);
  try {
    const res = await promise;
    return res.data;
  } catch (err) {
    return await Promise.reject(err);
  }
}

const config = {
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "localhost:3000",
  },
};

export const getHours: () => Promise<HourProps[]> = () => {
  return withLogs(axios.get(hourUrl, config), "getHours");
};
