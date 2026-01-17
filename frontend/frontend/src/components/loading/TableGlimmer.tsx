import { LinearProgress } from "@mui/material";

interface GlimmerProps {
  no_lines: number;
  no_cols: number;
}

export default function TableGlimmer({ no_lines, no_cols }: GlimmerProps) {
  return (
    <>
      {Array.from({ length: no_lines }).map((_, i) => (
        <tr key={i}>
          <td colSpan={no_cols}>
            <LinearProgress sx={{ width: "100%", height: 5, borderRadius: 2, marginY: 0.7, paddingY: 1 }} />
          </td>
        </tr>
      ))}
    </>
  );
}
