import { structure, FieldCategory } from "./contractStructures.ts";
import { useCallback, useEffect, useState } from "react";
import "./contracts.css";
import useContractApi from "./useContractApi.ts";
import { toast } from "react-hot-toast";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import useUserApi from "../profile/ProfileApi.ts";
import { useTranslation } from "react-i18next";
import { useOptionalSubjectsApi, usePromotionApi } from "./FieldsApi.ts";
import { type PromotionSelect, type OptionalPackageProps } from "./props.ts";
import { Button, FormControl, InputLabel, MenuItem, Select, ThemeProvider } from "@mui/material";
import { type OptionalsCallback, ContractOptionals } from "./ContractOptionals.tsx";
import { muiThemeContracts } from "./muiThemeContracts.ts";

function optionalFormKey(semester1or2: number, packageNumber: number) {
  return `optional-semester-${semester1or2}-package-${packageNumber}`;
}

const ContractsPage: React.FC = () => {
  const { getStudyContract } = useContractApi();
  const { userProps } = useAuthContext();
  const userId = userProps?.id;

  const { fetchUserProfile } = useUserApi();
  const [, setIsError] = useState(false);
  const { t } = useTranslation();
  const { getOptionalPackages } = useOptionalSubjectsApi();
  const { getPromotionsOfUser } = usePromotionApi();

  const [promotionSelectData, setPromotionSelectData] = useState<PromotionSelect[]>([]);

  const [selectedPromotion, setSelectedPromotion] = useState<PromotionSelect | undefined>();
  const [selectedYear, setSelectedYear] = useState(1);

  const [allOptionalPackages, setAllOptionalPackages] = useState<OptionalPackageProps[]>([]);
  const [optionalsThisYear, setOptionalsThisYear] = useState<OptionalPackageProps[]>([]);

  const [formValues, setFormValues] = useState<Record<string, any>>({});
  const contract = structure[0];
  const fieldsBeforeOptionals = contract.fields.filter((x) => x.category != FieldCategory.CHECKBOX);
  const fieldsAfterOptionals = contract.fields.filter((x) => x.category == FieldCategory.CHECKBOX);

  const handleOptionalChange: OptionalsCallback = (data) => {
    const optionalFormName = `optional-semester-${data.semester1or2}-package-${data.packageId}`;
    setFormValues((formValues) => ({ ...formValues, [optionalFormName]: data.subjectId }));
  };

  useEffect(() => {
    if (!userId) return;

    fetchUserProfile(userId)
      .then((profile) => {
        const initialData = {
          fullName: `${profile.firstName} ${profile.lastName}`,
          email: profile.email,
          phone: profile.phoneNumber,
          signature: profile.signatureUrl,
        };
        setFormValues((formValues) => ({ ...formValues, ...initialData }));
      })
      .catch(() => {
        toast.error("Error_loading_user_data");
      });
  }, [userId]);

  useEffect(() => {
    if (!userId) return;

    getPromotionsOfUser(userId).then(({ promotions }) => {
      setPromotionSelectData(promotions);
      if (promotions.length > 0) setSelectedPromotion(promotions[0]);
    });
  }, [userId]);

  useEffect(() => {
    if (!selectedPromotion?.id) {
      setAllOptionalPackages([]);
      return;
    }

    getOptionalPackages(selectedPromotion.id).then(setAllOptionalPackages);
  }, [selectedPromotion?.id]);

  useEffect(() => {
    setSelectedYear(1);
  }, [selectedPromotion]);

  useEffect(() => {
    setOptionalsThisYear(allOptionalPackages.filter((x) => x.yearNumber == selectedYear));

    setFormValues((formValues) => {
      const result = { ...formValues };
      for (const key in result) if (key.startsWith("optional-semester-")) delete result[key];
      return result;
    });
  }, [selectedYear, allOptionalPackages]);

  const validateForm = () => {
    for (const field of ["cnp", "serie", "numar", "fullName", "email", "phone"]) {
      if (!formValues[field] || String(formValues[field]).trim() === "") {
        toast.error(`Field ${field} is required`);
        return false;
      }
    }
    if (!formValues["agree"]) {
      toast.error("You must accept the terms and conditions");
      return false;
    }
    for (const optField of optionalsThisYear) {
      const key = optionalFormKey(optField.semester1or2, optField.packageId);
      if (!formValues[key]) {
        toast.error(
          `You must select an optional subject in semseter ${optField.semesterNumber} for package ${optField.packageId}`
        );
        return false;
      }
    }
    return true;
  };

  const handleSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      if (!validateForm()) return;

      if (!userId || !selectedPromotion) return;

      getStudyContract(userProps.id, {
        promotionId: selectedPromotion.id,
        fields: { ...formValues, yearNumber: selectedYear },
      })
        .then((response) => {
          const data = new Blob([response]);
          const url = window.URL.createObjectURL(data);

          const link = document.createElement("a");
          link.id = "some-link";
          link.href = url;
          link.download = "contract.pdf";
          link.click();

          window.URL.revokeObjectURL(url);
          setIsError(false);
        })
        .catch((error) => {
          console.error(error as Error);
          toast.error(t("Error_generating_contract"));
          setIsError(true);
        });
    },
    [userId, selectedPromotion, formValues]
  );

  return (
    <ThemeProvider theme={muiThemeContracts}>
      <div className={"contracts-page"}>
        <div className={"contract-title"}>{t(contract.title)}</div>

        <div className="contracts-promotion-select">
          <FormControl>
            <InputLabel id="contracts-select-promotion-label">{t("Promotion")}</InputLabel>
            <Select
              labelId="contracts-select-promotion-label"
              id="contracts-select-promotion"
              label={t("Promotion")}
              className="white"
              value={selectedPromotion?.id ?? ''}
              onChange={(x) => setSelectedPromotion(promotionSelectData.filter((y) => y.id == x.target.value)[0])}
            >
              <MenuItem>{t("Promotion")}</MenuItem>
              {promotionSelectData?.map((x) => (
                <MenuItem value={x.id}>{x.prettyName}</MenuItem>
              ))}
            </Select>
          </FormControl>

          <FormControl>
            <InputLabel id="contracts-select-year-label">{t("YearOfStudy")}</InputLabel>
            <Select
              labelId="contracts-select-year-label"
              id="contracts-select-year"
              label={t("YearOfStudy")}
              name="year"
              className="white"
              value={selectedYear}
              onChange={(x) => setSelectedYear(x.target.value)}
            >
              {Array.from({ length: selectedPromotion?.yearDuration ?? 1 }).map((_, i) => (
                <MenuItem value={i + 1} key={`year-${i}`}>
                  {i + 1}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </div>

        <form onSubmit={handleSubmit}>
          {fieldsBeforeOptionals.map((field) => {
            if (field.category === FieldCategory.SELECT) {
              return (
                <label className={"contracts-page-label"} key={field.name}>
                  {t(field.label)}
                  <select
                    value={formValues[field.name] ?? ""}
                    onChange={(e) =>
                      setFormValues((prev) => ({
                        ...prev,
                        [field.name]: Number(e.target.value),
                      }))
                    }
                  >
                    <option value="">Select</option>
                    {"options" in field &&
                      field.options.map((opt) => (
                        <option key={opt} value={opt}>
                          {opt}
                        </option>
                      ))}
                  </select>
                </label>
              );
            }

            return (
              <label className={"contracts-page-label"} key={field.name}>
                {t(field.label)}
                <input
                  type="text"
                  value={formValues[field.name] ?? ""}
                  maxLength={
                    field.category === FieldCategory.SERIE
                      ? 2
                      : field.category === FieldCategory.NUMAR
                        ? 6
                        : field.category === FieldCategory.CNP
                          ? 13
                          : undefined
                  }
                  onChange={(e) => {
                    let value = e.target.value;

                    if (field.category === FieldCategory.PHONE) {
                      value = value.replace(/[^0-9+]/g, "");
                    }

                    if (field.category === FieldCategory.NUMAR) {
                      value = value.replace(/[^0-9]/g, "");
                    }

                    if (field.category === FieldCategory.CNP) {
                      value = value.replace(/[^0-9]/g, "");
                    }

                    if (field.category === FieldCategory.SERIE) {
                      value = value.replace(/[^a-zA-Z]/g, "");
                      value = value.toUpperCase();
                    }

                    setFormValues((prev) => ({
                      ...prev,
                      [field.name]: value,
                    }));
                  }}
                />
              </label>
            );
          })}

          <ContractOptionals optionals={optionalsThisYear} updateOptional={handleOptionalChange} />

          {fieldsAfterOptionals.map((field) => {
            if (field.category === FieldCategory.CHECKBOX) {
              return (
                <label className={"contracts-page-label"}>
                  {t(field.label)}
                  <input
                    type="checkbox"
                    checked={!!formValues[field.name]}
                    onChange={(e) =>
                      setFormValues((prev) => ({
                        ...prev,
                        [field.name]: e.target.checked,
                      }))
                    }
                  />
                </label>
              );
            }
          })}
          <Button type={"submit"}>{t("Generate")}</Button>
        </form>
      </div>
    </ThemeProvider>
  );
};

export default ContractsPage;
