import React from "react";
import type { HourProps } from "./HourProps.ts";

export interface HoursState {
  hours?: HourProps[];
  fetching: boolean;
  fetchingError?: Error | null;
}

export const initialState: HoursState = {
  fetching: false,
};

const HourContext = React.createContext<HoursState>(initialState);

export default HourContext;
