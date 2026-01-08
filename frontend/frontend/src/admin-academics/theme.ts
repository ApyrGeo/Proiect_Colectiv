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
                borderColor: "secondary.main",
                borderWidth: "3px",
              },
            },
            "&:hover:not(.Mui-focused)": {
              "& .MuiOutlinedInput-notchedOutline": {
                borderColor: "#ccc",
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
          margin: 25,
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
          borderWidth: "2px",
          ".MuiOutlinedInput-notchedOutline": {
            borderColor: "rgb(255,255,255)",
          },
          "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgb(255,255,255)",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "rgb(255,255,255)",
          },
        },
      },
    },
    MuiFormControl: {
      styleOverrides: {
        root: {
          background: "#0f2439",
          borderColor: "#DCE3EB",
          borderWidth: "2px",
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
