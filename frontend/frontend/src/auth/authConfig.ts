import { LogLevel, PublicClientApplication } from "@azure/msal-browser";

export const msalConfig = {
  auth: {
    clientId: "b648c811-e717-42d3-8a06-b54cba38e8ce",
    authority: "https://login.microsoftonline.com/f94455c3-bca1-4dc9-9c78-22b2438672d9",
    redirectUri: "http://localhost:5173/",
  },
  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: LogLevel, message: string, containsPii: boolean) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
    },
  },
};

export const loginRequest = {
  scopes: [
    "api://98597d4f-08f9-4e43-8387-16cf8d7c510b/TrackForUBB.Read",
    "api://98597d4f-08f9-4e43-8387-16cf8d7c510b/TrackForUBB.ReadWrite",
  ],
};

export const msalInstance = new PublicClientApplication(msalConfig);
