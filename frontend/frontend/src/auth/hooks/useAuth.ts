import { useState, useEffect, useCallback } from "react";
import { useMsal } from "@azure/msal-react";
import { InteractionRequiredAuthError, InteractionStatus, type AccountInfo } from "@azure/msal-browser";
import { loginRequest } from "../authConfig";

const useAuth = () => {
  const { instance, accounts, inProgress } = useMsal();
  const activeAccount: AccountInfo | null = instance.getActiveAccount() ?? (accounts.length > 0 ? accounts[0] : null);

  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [error, setError] = useState<unknown | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const acquireToken = useCallback(async () => {
    if (!activeAccount) {
      setAccessToken(null);
      return null;
    }

    setLoading(true);
    const request = {
      scopes: loginRequest.scopes,
      account: activeAccount,
    };

    try {
      const response = await instance.acquireTokenSilent(request);
      setAccessToken(response.accessToken);
      setError(null);
      return response.accessToken;
    } catch (err) {
      if (err instanceof InteractionRequiredAuthError) {
        await instance.acquireTokenRedirect(request);
      } else {
        setError(err);
        setAccessToken(null);
      }
      return null;
    } finally {
      setLoading(false);
    }
  }, [instance, activeAccount]);

  useEffect(() => {
    if (inProgress === InteractionStatus.None && activeAccount) {
      acquireToken();
    }
  }, [inProgress, activeAccount, acquireToken]);

  return { accessToken, acquireToken, loading, error, activeAccount };
};

export default useAuth;
