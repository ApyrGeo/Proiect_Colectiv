import { useState, useEffect, useCallback, useRef, useMemo } from "react";
import { useMsal } from "@azure/msal-react";
import { EventType, InteractionRequiredAuthError, InteractionStatus, type AccountInfo } from "@azure/msal-browser";
import { loginRequest } from "../authConfig";
import { type LoggedUserInfo, type UserEnrollments, type UserProps } from "../../core/props.ts";
import axios from "axios";
import { baseUrl, host } from "../../core";

type Waiter = (token: string | null) => void;

const useAuth = () => {
  const { instance, accounts, inProgress } = useMsal();
  const [activeAccount, setActiveAccount] = useState<AccountInfo | null>(
    instance.getActiveAccount() ?? (accounts.length > 0 ? accounts[0] : null)
  );

  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [error, setError] = useState<unknown | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [isFullfilled, setFullfilled] = useState(false);

  const tokenWaitersRef = useRef<Waiter[]>([]);
  const initializedRef = useRef<boolean>(false);

  const [userProps, setUserProps] = useState<UserProps | null>(null);
  const [userEnrollments, setUserEnrollments] = useState<UserEnrollments[] | null>(null);

  const authApiClient = useMemo(
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

  const resolveWaiters = useCallback((token: string | null) => {
    const waiters = tokenWaitersRef.current.splice(0);
    waiters.forEach((res) => {
      try {
        res(token);
      } catch (e) {
        console.error("waiter resolver threw", e);
      }
    });

    if (!token) setFullfilled(true);
  }, []);

  const waitForAccessToken = useCallback((): Promise<string | null> => {
    if (accessToken) {
      return Promise.resolve(accessToken);
    }

    if (initializedRef.current && !activeAccount) {
      return Promise.resolve(null);
    }

    return new Promise<string | null>((resolve) => {
      tokenWaitersRef.current.push(resolve);
    });
  }, [accessToken, activeAccount]);

  const acquireToken = useCallback(async () => {
    let acct = activeAccount;

    if (!acct && accounts.length > 0) {
      acct = accounts[0];
    }

    if (!acct) {
      setAccessToken(null);
      resolveWaiters(null);
      return null;
    }

    setLoading(true);
    const request = {
      scopes: loginRequest.scopes,
      account: acct,
    };

    try {
      const response = await instance.acquireTokenSilent(request);
      setAccessToken(response.accessToken);
      setError(null);
      setActiveAccount(acct);
      resolveWaiters(response.accessToken);

      return response.accessToken;
    } catch (err) {
      if (err instanceof InteractionRequiredAuthError) {
        await instance.acquireTokenRedirect(request);
      } else {
        setError(err);
        setAccessToken(null);
        resolveWaiters(null);
      }
      return null;
    } finally {
      setLoading(false);
    }
  }, [instance, activeAccount, resolveWaiters, accounts]);

  useEffect(() => {
    const reqInterceptor = authApiClient.interceptors.request.use(async (config) => {
      if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
        return config;
      }

      const token = await waitForAccessToken();

      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      return config;
    });

    return () => {
      authApiClient.interceptors.request.eject(reqInterceptor);
    };
  }, [accessToken, authApiClient, waitForAccessToken]);

  const getLoggedInUser = useCallback(async () => {
    const response = await authApiClient.get<LoggedUserInfo>("api/User/logged-user");
    return response.data;
  }, [authApiClient]);

  useEffect(() => {
    const callbackId = instance.addEventCallback((event) => {
      if (
        event.eventType === EventType.LOGIN_SUCCESS ||
        event.eventType === EventType.ACQUIRE_TOKEN_SUCCESS ||
        event.eventType === EventType.SSO_SILENT_SUCCESS ||
        event.eventType === EventType.ACCOUNT_ADDED
      ) {
        acquireToken();
      }
    });

    if (inProgress === InteractionStatus.None) {
      initializedRef.current = true;
      acquireToken();
    }

    return () => {
      if (callbackId) {
        instance.removeEventCallback(callbackId);
      }
    };
  }, [instance, inProgress, acquireToken]);

  useEffect(() => {
    const newActive = accounts.length > 0 ? accounts[0] : null;
    setActiveAccount(newActive);
  }, [instance, accounts]);

  useEffect(() => {
    if (accessToken) {
      resolveWaiters(accessToken);
    }
  }, [accessToken, resolveWaiters]);

  useEffect(() => {
    if (!accessToken) return;
    getLoggedInUser()
      .then((res) => {
        if (!res) return;
        setUserProps(res.user);
        setUserEnrollments(res.enrollments);
      })
      .finally(() => {
        setFullfilled(true);
      });
  }, [accessToken, getLoggedInUser]);

  return { accessToken, loading, error, activeAccount, waitForAccessToken, userProps, userEnrollments, isFullfilled };
};

export default useAuth;
