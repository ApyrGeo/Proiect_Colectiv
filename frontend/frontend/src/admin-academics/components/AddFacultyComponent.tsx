import { Grid, TextField, Button, Card, ThemeProvider } from "@mui/material";
import { theme } from "../theme.ts";

const AddFacultyComponent: React.FC = () => {
  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Faculty</div>
          <TextField id="outlined-basic" label="Faculty name" variant="outlined" />
          <Button>Add Faculty</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddFacultyComponent;
