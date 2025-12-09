import { Box, LinearProgress } from "@mui/material";

interface GlimmerProps {
  no_lines: number;
}

export default function Glimmer({ no_lines }: GlimmerProps) {
  return (
    <Box sx={{ width: "60vw", display: "flex", flexDirection: "column", gap: 2 }}>
      {Array.from({ length: no_lines }).map((_, i) => (
        <LinearProgress key={i} sx={{ height: 5, borderRadius: 2, marginY: 0.7, paddingY: 1 }} />
      ))}
    </Box>
  );
}
