import { createTheme } from "@mui/material";

export const theme = createTheme({
  components: {
    MuiTextField: {
      styleOverrides: {
        root: {
          margin: 10,
          // Outlined
          "& .MuiOutlinedInput-root": {
            color: "#DCE3EB",
            fontFamily: "Arial",
            fontWeight: "bold",
            "& .MuiOutlinedInput-notchedOutline": {
              borderColor: "#DCE3EB",
              borderWidth: "2px",
            },
            "&.Mui-focused": {
              "& .MuiOutlinedInput-notchedOutline": {
                borderColor: "#0054A4",
                borderWidth: "3px",
              },
            },
            "&:hover:not(.Mui-focused)": {
              "& .MuiOutlinedInput-notchedOutline": {
                borderColor: "#0054A4",
              },
            },
          },
          "& .MuiInputLabel-outlined": {
            color: "#DCE3EB",
            fontWeight: "bold",
            "&.Mui-focused": {
              color: "secondary.main",
            },
          },
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          color: "#DCE3EB",
          background: "#0054A4",
          margin: 20,
          "&:hover": {
            background: "#326dae",
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          background: "#0f2439",
          padding: 20,
          width: 500,
          alignItems: "center",
          justifyItems: "center",
          alignContent: "center",
          justifyContent: "center",
        },
      },
    },
    MuiSelect: {
      styleOverrides: {
        root: {
          background: "#0f2439",
          // padding: 5,
          color: "#DCE3EB",
          borderColor: "#DCE3EB",
          borderWidth: "1px",
          ".MuiOutlinedInput-notchedOutline": {
            borderColor: "#DCE3EB",
            borderWidth: "1px",
          },
          "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
            borderColor: "#0054A4",
            borderWidth: "1px",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "#0054A4",
            borderWidth: "1px",
          },
        },
        icon: {
          color: "#DCE3EB",
          scale: 1.5,
        },
      },
    },
    MuiFormControl: {
      styleOverrides: {
        root: {
          background: "#0f2439",
          borderColor: "#DCE3EB",
          borderWidth: "1px",
        },
      },
    },
    MuiInputLabel: {
      styleOverrides: {
        root: {
          color: "#DCE3EB",
          "&.Mui-focused": {
            color: "#DCE3EB",
          },
        },
      },
    },
  },
});
