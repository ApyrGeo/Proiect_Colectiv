import { structure, FieldCategory } from "./contractStructures.ts";
import { useEffect, useState } from "react";
import "./contracts.css";
import useContractApi from "./useContractApi.ts";
import { toast } from "react-hot-toast";
import { useAuthContext } from "../auth/context/AuthContext.tsx";
import useUserApi from "../profile/ProfileApi.ts";
import { useTranslation } from "react-i18next";

const ContractsPage: React.FC = () => {
  const { getStudyContract } = useContractApi();
  const { userProps } = useAuthContext();
  const { fetchUserProfile } = useUserApi();
  const [, setUserData] = useState<Record<string, any>>({});
  const [formValues, setFormValues] = useState<Record<string, any>>({});
  const contract = structure[0];
  const [, setIsError] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    if (!userProps?.id) return;

    fetchUserProfile(userProps.id)
      .then((profile) => {
        const initialData = {
          fullName: `${profile.firstName} ${profile.lastName}`,
          email: profile.email,
          phone: profile.phoneNumber,
          signature: profile.signatureUrl,
        };

        setUserData(initialData);
        setFormValues(initialData);
      })
      .catch(() => {
        toast.error(t("Error_loading_user_data"));
      });
  }, [userProps?.id]);
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log(e);

    if (!userProps?.id) return;

    getStudyContract(userProps.id, {
      fields: formValues,
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
        console.log(error as Error);
        toast.error(t("Error_generating_contract"));
        setIsError(true);
      });
  };

  return (
    <div className={"contracts-page"}>
      <div className={"contract-title"}>{t(contract.title)}</div>
      <form onSubmit={handleSubmit}>
        {contract.fields.map((field) => {
          if (field.category === FieldCategory.SELECT) {
            return (
              <label key={field.name}>
                {field.label}
                <select
                  value={formValues[field.name] ?? ""}
                  onChange={(e) =>
                    setFormValues((prev) => ({
                      ...prev,
                      [field.name]: e.target.value,
                    }))
                  }
                >
                  <option value="">Select</option>
                  {field.options.map((opt) => (
                    <option key={opt} value={opt}>
                      {opt}
                    </option>
                  ))}
                </select>
              </label>
            );
          }
          if (field.category === FieldCategory.CHECKBOX) {
            return (
              <label key={field.name}>
                <label className={"checkbox-label"}>{t(field.label)}</label>
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

          return (
            <label key={field.name}>
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

        <button type="submit">{t("Generate")}</button>
      </form>
    </div>
  );
};

export default ContractsPage;
