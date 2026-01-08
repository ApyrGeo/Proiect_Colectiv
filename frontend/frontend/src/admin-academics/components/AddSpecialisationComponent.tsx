import { TextField, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";
import type { Faculty } from "../props.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "react-hot-toast";

type AddSpecialisationProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};
const AddSpecialisationComponent: React.FC<AddSpecialisationProps> = (props) => {
  const { postSpecialisation } = useAdminAcademicsApi();
  const { t } = useTranslation();

  const [specName, setSpecName] = useState("");
  const [selectedFacultyId, setSelectedFacultyId] = useState<number | "">("");
  const [error, setError] = useState("");
  useEffect(() => {
    if (error) {
      toast.error(t("Error"));
    }
  }, [error, t]);

  const handleAddSpecialisation = () => {
    if (specName == "" || selectedFacultyId == "") return;

    postSpecialisation({ name: specName, facultyId: selectedFacultyId })
      .then(() => {
        toast.success(t("Success"));
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
          <div className={"academic-title"}>
            {t("New")} {t("Specialisation")}
          </div>
          <FormControl fullWidth>
            <InputLabel>{t("Faculty")}</InputLabel>
            <Select
              label={t("Faculty")}
              onChange={(event) => {
                if (!event.target.value) return;
                setSelectedFacultyId(Number(event.target.value));
              }}
              value={selectedFacultyId}
            >
              {props.faculties.map((f) => (
                <MenuItem value={f.id} key={f.id}>
                  {f.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <div className="academic-row">
            <TextField
              onChange={(event) => {
                setSpecName(event.target.value);
              }}
              key="spec-text"
              id="spec-text"
              label={t("SpecialisationName")}
              variant="outlined"
            />
            <Button onClick={handleAddSpecialisation}>{t("AddSpecialisation")}</Button>
          </div>
        </Card>
      </div>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
