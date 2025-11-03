import React, { type ReactNode, useEffect, useReducer } from "react";
import { getHours } from "./TimetableApi.ts";
import HourContext, { type HoursState, initialState } from "./HourContext.tsx";
import type { HourProps } from "./HourProps.ts";

type ActionProps =
  | { type: typeof FETCH_HOURS_STARTED }
  | { type: typeof FETCH_HOURS_SUCCEEDED; payload: { hours: HourProps[] } }
  | { type: typeof FETCH_HOURS_FAILED; payload: { error: Error } };

interface HourProviderProps {
  children: ReactNode;
}

const FETCH_HOURS_STARTED = "FETCH_HOURS_STARTED";
const FETCH_HOURS_SUCCEEDED = "FETCH_HOURS_SUCCEEDED";
const FETCH_HOURS_FAILED = "FETCH_HOURS_FAILED";

const reducer: (state: HoursState, action: ActionProps) => HoursState = (state, action) => {
  switch (action.type) {
    case FETCH_HOURS_STARTED:
      return { ...state, fetching: true, fetchingError: null };
    case FETCH_HOURS_SUCCEEDED:
      return { ...state, hours: action.payload.hours, fetching: false };
    case FETCH_HOURS_FAILED:
      return { ...state, fetchingError: action.payload.error, fetching: false };
    default:
      return state;
  }
};

export const HourProvider: React.FC<HourProviderProps> = ({ children }: HourProviderProps) => {
  const [state, dispatch] = useReducer(reducer, initialState);
  const { hours, fetching, fetchingError } = state;
  useEffect(getHoursEffect, []);
  const value = { hours, fetching, fetchingError };

  function getHoursEffect() {
    let canceled = false;
    fetchHours();
    return () => {
      canceled = true;
    };

    async function fetchHours() {
      try {
        dispatch({ type: FETCH_HOURS_STARTED });
        const hours = await getHours();
        if (!canceled) {
          dispatch({ type: FETCH_HOURS_SUCCEEDED, payload: { hours } });
        }
      } catch (error) {
        if (!canceled) {
          dispatch({ type: FETCH_HOURS_FAILED, payload: { error: error as Error } });
        }
      }
    }
  }

  return <HourContext.Provider value={value}>{children}</HourContext.Provider>;
};
