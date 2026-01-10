import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();

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
        toast.error(t("createUsers.importPromotion.couldNotLoadSpecialisations"));
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
      toast.error(t("createUsers.importPromotion.selectFileFirst"));
      return;
    }

    if (startYear === "" || endYear === "") {
      toast.error(t("createUsers.importPromotion.enterYears"));
      return;
    }

    if (!specialisation) {
      toast.error(t("createUsers.importPromotion.selectSpecialisationFirst"));
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
        toast.success(t("createUsers.importPromotion.promotionCreated"));
      } else {
        toast.error(t("createUsers.importPromotion.someEnrollmentsFailed"));
      }

      setResult(promotionResult);
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as BulkPromotionResult & { StatusCode?: number; Description?: string };

        if (Array.isArray((data as BulkPromotionResult).enrollments)) {
          toast.error(t("createUsers.importPromotion.someEnrollmentsFailed"));
          setResult(data as BulkPromotionResult);
        } else if (typeof (data as { Description?: string }).Description === "string") {
          toast.error((data as { Description: string }).Description);
        } else {
          toast.error(t("createUsers.importPromotion.invalidUploadedFile"));
        }
      } else {
        console.error("Failed to upload promotion file", error);
        toast.error(t("createUsers.importPromotion.failedProcessFile"));
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
      <h1 className="create-users-title">{t("createUsers.importPromotion.title")}</h1>
      <p className="create-users-subtext">{t("createUsers.importPromotion.subtext")}</p>

      <Box className="create-users-template-row">
        <Typography variant="body2" className="create-users-template-text">
          {t("createUsers.importPromotion.templateText")}
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
          {t("createUsers.importPromotion.downloadTemplate")}
        </Button>
      </Box>

      <Card className="create-users-card">
        <CardContent>
          <Box className="promotion-fields-row">
            <TextField
              label={t("createUsers.importPromotion.startYear")}
              type="number"
              size="small"
              value={startYear}
              className="promotion-text-field"
              onChange={(e) => setStartYear(e.target.value === "" ? "" : Number(e.target.value))}
            />
            <TextField
              label={t("createUsers.importPromotion.endYear")}
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
              renderInput={(params) => (
                <TextField {...params} label={t("createUsers.importPromotion.specialisation")} />
              )}
              slotProps={{
                paper: {
                  sx: { minWidth: 320 },
                },
                listbox: {
                  sx: { maxHeight: 320, overflowY: "auto" },
                },
              }}
              noOptionsText={t("createUsers.importPromotion.noSpecialisations")}
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
                  {t("createUsers.importPromotion.browse")}
                  <input
                    id="promotion-file"
                    type="file"
                    hidden
                    accept=".csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    onChange={handleFileChange}
                  />
                </Button>
                <span className="create-users-file-name">
                  {selectedFile ? selectedFile.name : t("createUsers.importPromotion.noFileSelected")}
                </span>
              </div>
            </div>

            <Button
              variant="contained"
              color="primary"
              onClick={handleUpload}
              disabled={isLoading || !selectedFile}
              className="create-users-upload-button"
            >
              {isLoading ? t("createUsers.importPromotion.uploading") : t("createUsers.importPromotion.upload")}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {result && result.enrollments && result.enrollments.length > 0 && (
            <Box className="create-users-results">
              <Typography variant="h6" className="create-users-results-title">
                {t("createUsers.importPromotion.enrollmentValidation")}{" "}
                {errorRows.length > 0 ? `(errors: ${errorRows.length})` : "(all valid)"}
              </Typography>
              <div className="create-users-table-wrapper">
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>{t("createUsers.importUsers.table.row")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.email")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.status")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.errors")}</TableCell>
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
                        <TableCell>
                          {hasErrors(enrollment)
                            ? t("createUsers.importUsers.statuses.invalid")
                            : t("createUsers.importUsers.statuses.created")}
                        </TableCell>
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
