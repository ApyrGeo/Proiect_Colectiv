import React, { useEffect, useState } from "react";
import "../import-users.css";
import {
  Box,
  Button,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Autocomplete,
  TextField,
  Typography,
} from "@mui/material";
import { toast } from "react-hot-toast";
import Circular from "../../../components/loading/Circular.tsx";
import useImportPromotionApi, {
  type BulkEnrollmentItemResult,
  type BulkPromotionResult,
  type PromotionResult,
  type SpecialisationResult,
} from "../useImportPromotionApi.ts";
import type { AxiosError } from "axios";

const ImportPromotionPage = () => {
  const { uploadPromotionFile, getAllSpecialisations } = useImportPromotionApi();

  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [result, setResult] = useState<BulkPromotionResult | null>(null);
  const [startYear, setStartYear] = useState<number | "">("");
  const [endYear, setEndYear] = useState<number | "">("");
  const [specialisations, setSpecialisations] = useState<SpecialisationResult[]>([]);
  const [specialisation, setSpecialisation] = useState<SpecialisationResult | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const specs = await getAllSpecialisations();
        setSpecialisations(specs);
      } catch (error) {
        console.error("Failed to load specialisations", error);
        toast.error("Could not load specialisations.");
      }
    })();
  }, [getAllSpecialisations]);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] ?? null;
    setSelectedFile(file);
    setResult(null);
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      toast.error("Please select a file first.");
      return;
    }

    if (startYear === "" || endYear === "") {
      toast.error("Please enter both start and end year.");
      return;
    }

    if (!specialisation) {
      toast.error("Please select a specialisation first.");
      return;
    }

    setIsLoading(true);
    setResult(null);

    try {
      const promotionResult = await uploadPromotionFile({
        file: selectedFile,
        startYear: Number(startYear),
        endYear: Number(endYear),
        specialisationId: specialisation.id,
      });

      if (promotionResult.isValid) {
        toast.success("Promotion and enrollments were created successfully.");
      } else {
        toast.error("Some enrollments could not be created. See details below.");
      }

      setResult(promotionResult);
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as BulkPromotionResult & { StatusCode?: number; Description?: string };

        if (Array.isArray((data as BulkPromotionResult).enrollments)) {
          toast.error("Some enrollments could not be created. See details below.");
          setResult(data as BulkPromotionResult);
        } else if (typeof (data as { Description?: string }).Description === "string") {
          toast.error((data as { Description: string }).Description);
        } else {
          toast.error("The uploaded file is not valid.");
        }
      } else {
        console.error("Failed to upload promotion file", error);
        toast.error("Failed to process the file. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const hasErrors = (enrollment: BulkEnrollmentItemResult) =>
    !enrollment.isValid || (enrollment.errors && enrollment.errors.length > 0);

  const errorRows = result?.enrollments?.filter((e) => hasErrors(e)) ?? [];

  const promotion: PromotionResult | null = result?.promotion ?? null;

  return (
    <div className="create-users-page">
      <h1 className="create-users-title">Import promotion</h1>
      <p className="create-users-subtext">Configure the promotion and upload a CSV with student enrollments.</p>

      <Box className="create-users-template-row">
        <Typography variant="body2" className="create-users-template-text">
          You can download a CSV template and fill it in before uploading.
        </Typography>
        <Button
          variant="outlined"
          color="primary"
          size="small"
          component="a"
          href="/import-promotion-with-enrollments-example.csv"
          download
          className="create-users-template-button"
        >
          Download template
        </Button>
      </Box>

      <Card className="create-users-card">
        <CardContent>
          <Box className="promotion-fields-row">
            <TextField
              label="Start year"
              type="number"
              size="small"
              value={startYear}
              className="promotion-text-field"
              onChange={(e) => setStartYear(e.target.value === "" ? "" : Number(e.target.value))}
            />
            <TextField
              label="End year"
              type="number"
              size="small"
              value={endYear}
              className="promotion-text-field"
              onChange={(e) => setEndYear(e.target.value === "" ? "" : Number(e.target.value))}
            />
            <Autocomplete
              size="small"
              options={specialisations}
              getOptionLabel={(option) => option.name}
              value={specialisation}
              className="promotion-text-field"
              sx={{ width: 233 }}
              onChange={(_e, value) => setSpecialisation(value)}
              renderInput={(params) => <TextField {...params} label="Specialisation" />}
              slotProps={{
                paper: {
                  sx: { minWidth: 320 },
                },
                listbox: {
                  sx: { maxHeight: 320, overflowY: "auto" },
                },
              }}
              noOptionsText="No specialisations found"
            />
          </Box>

          <Box className="create-users-upload-section">
            <div className="create-users-input-group">
              <div className="create-users-buttons-row">
                <Button
                  variant="outlined"
                  component="label"
                  disabled={isLoading}
                  className="create-users-browse-button"
                >
                  Browse
                  <input
                    id="promotion-file"
                    type="file"
                    hidden
                    accept=".csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    onChange={handleFileChange}
                  />
                </Button>
                <span className="create-users-file-name">{selectedFile ? selectedFile.name : "No file selected"}</span>
              </div>
            </div>

            <Button
              variant="contained"
              color="primary"
              onClick={handleUpload}
              disabled={isLoading || !selectedFile}
              className="create-users-upload-button"
            >
              {isLoading ? "Uploading..." : "Upload"}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {result && result.enrollments && result.enrollments.length > 0 && (
            <Box className="create-users-results">
              <Typography variant="h6" className="create-users-results-title">
                Enrollment validation {errorRows.length > 0 ? `(errors: ${errorRows.length})` : "(all valid)"}
              </Typography>
              <div className="create-users-table-wrapper">
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Row</TableCell>
                      <TableCell>Email</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Errors</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {(errorRows.length > 0 ? errorRows : result.enrollments).map((enrollment) => (
                      <TableRow
                        key={enrollment.row}
                        className={hasErrors(enrollment) ? "create-users-row-error" : "create-users-row-success"}
                      >
                        <TableCell>{enrollment.row}</TableCell>
                        <TableCell>
                          {enrollment.email === "" || enrollment.email === null ? "-" : enrollment.email}
                        </TableCell>
                        <TableCell>{hasErrors(enrollment) ? "Invalid" : "Created"}</TableCell>
                        <TableCell>
                          {enrollment.errors && enrollment.errors.length > 0
                            ? enrollment.errors.map((err, idx) => <div key={idx}>{err}</div>)
                            : "-"}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </Box>
          )}

          {promotion && (
            <Box mt={3} className="promotion-summary">
              <Typography variant="h6" className="create-users-results-title">
                Created promotion
              </Typography>
              <Typography variant="body2">
                Years: {promotion.startYear} - {promotion.endYear}
              </Typography>
              <div className="promotion-groups">
                {promotion.studentGroups.map((group) => (
                  <div key={group.id} className="promotion-group">
                    <strong>{group.name}</strong>
                    {group.studentSubGroups.length > 0 && (
                      <ul>
                        {group.studentSubGroups.map((sub) => (
                          <li key={sub.id}>{sub.name}</li>
                        ))}
                      </ul>
                    )}
                  </div>
                ))}
              </div>
            </Box>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default ImportPromotionPage;
