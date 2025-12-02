import { useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";
import { InteractionRequiredAuthError, InteractionStatus, type AccountInfo } from "@azure/msal-browser";
import { loginRequest } from "../authConfig";

const useAuth = () => {
  const { instance, inProgress } = useMsal();
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const account: AccountInfo | null = instance.getActiveAccount();

  useEffect(() => {
    let mounted = true;

    const getToken = async () => {
      if (inProgress !== InteractionStatus.None) {
        if (mounted) {
          setLoading(false);
        }

        return;
      }

      if (!account) {
        setToken(null);
        setLoading(false);
        return;
      }

      const request = {
        account,
        scopes: loginRequest.scopes,
      };

      setLoading(true);
      instance
        .acquireTokenSilent(request)
        .then((response) => {
          if (!mounted) {
            return;
          }

          setToken(response.accessToken ?? null);
        })
        .catch(async (err) => {
          if (!mounted) {
            return;
          }

          if (err instanceof InteractionRequiredAuthError) {
            instance
              .acquireTokenPopup(request)
              .then((response) => {
                if (mounted) {
                  setToken(response.accessToken ?? null);
                }
              })
              .catch((popupErr) => {
                console.error("acquireTokenPopup failed:", popupErr);

                if (mounted) {
                  setToken(null);
                }
              });
          } else {
            console.error("acquireTokenSilent error:", err);

            if (mounted) {
              setToken(null);
            }
          }
        })
        .finally(() => {
          if (mounted) {
            setLoading(false);
          }
        });
    };

    getToken();

    return () => {
      mounted = false;
    };
  }, [instance, account, inProgress]);

  return { token, account, loading };
};

export default useAuth;
