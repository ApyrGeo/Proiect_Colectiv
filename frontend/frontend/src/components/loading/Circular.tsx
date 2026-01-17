import { CircularProgress, Box } from "@mui/material";

export default function Circular() {
  return (
    <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", padding: 6 }}>
      <CircularProgress size={60} />
    </Box>
  );
}
