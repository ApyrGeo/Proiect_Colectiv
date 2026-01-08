import { TextField, Button, Card, ThemeProvider } from "@mui/material";
import { theme } from "../theme.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "react-hot-toast";

type AddFacultyProps = {
  refreshFaculties: () => void;
};

const AddFacultyComponent: React.FC<AddFacultyProps> = (props) => {
  const { t } = useTranslation();
  const { postFaculty } = useAdminAcademicsApi();

  const [facultyName, setFacultyName] = useState("");
  const [error, setError] = useState("");
  useEffect(() => {
    if (error) {
      toast.error(t("Error"));
    }
  }, [error, t]);

  const handleAddFaculty = () => {
    if (facultyName == "") return;
    postFaculty({ name: facultyName })
      .then(() => {
        props.refreshFaculties();
      })
      .catch(() => {
        setError("Error");
      });
  };

  return (
    <ThemeProvider theme={theme}>
      <div className={"admin-academic-component"}>
        <Card>
          <div className={"academic-title"}>{t("NewFaculty")}</div>
          <TextField
            onChange={(event) => {
              setFacultyName(event.target.value);
            }}
            id="outlined-basic"
            label={t("FacultyName")}
            variant="outlined"
          />
          <Button onClick={handleAddFaculty}>{t("AddFaculty")}</Button>
        </Card>
      </div>
    </ThemeProvider>
  );
};

export default AddFacultyComponent;
