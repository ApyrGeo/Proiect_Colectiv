import { exampleStructures, FieldCategory } from "./contractStructures.ts";
import { useRef, useState } from "react";
import "./contracts.css";
import SignatureCanvas from "react-signature-canvas";
import useContractApi from "./useContractApi.ts";

const ContractsPage: React.FC = () => {
  // TODO remove hard coded user id
  const userId = 21005;

  const { getStudyContract } = useContractApi();

  const contractStructures = exampleStructures;

  const [selectedContract, setSelectedContract] = useState(0);

  const [isError, setIsError] = useState(false);

  const sigCanvas = useRef<SignatureCanvas>(null);

  const handleChange = (value: number) => {
    setSelectedContract(value);
  };

  const handleSubmit = (formData: FormData) => {
    console.log(formData);
    if (contractStructures[selectedContract].signature && sigCanvas.current) console.log(sigCanvas.current.toDataURL());

    getStudyContract(userId)
      .then((response) => {
        const data = new Blob([response]);
        const url = window.URL.createObjectURL(data);

        const link = document.createElement("a");
        link.id = 'some-link';
        link.href = url;
        link.download = "contract.pdf";
        link.click();

        window.URL.revokeObjectURL(url)
      })
      .catch((error) => {
        console.log(error as Error);
        setIsError(true);
      });
  };

  return (
    <div className={"contracts-page"}>
      <select className={"contracts-filter"} onChange={(e) => handleChange(Number(e.currentTarget.value))}>
        {contractStructures.map((s, index) => (
          <option key={index} value={index}>
            {s.title}
          </option>
        ))}
      </select>

      <div className={"contract-title"}>{contractStructures[selectedContract].title}</div>

      <form action={handleSubmit}>
        {contractStructures[selectedContract].fields.map((field) => {
          if (field.category === FieldCategory.SELECT)
            return (
              <label key={field.name}>
                {field.label}
                <select name={field.name}>
                  {field.options.map((opt) => (
                    <option key={opt} value={opt}>
                      {opt}
                    </option>
                  ))}
                </select>
              </label>
            );
          else {
            return (
              <label key={field.name}>
                {field.label}
                <input name={field.name} type={field.category.toString()} />
              </label>
            );
          }
        })}
        {contractStructures[selectedContract].signature && (
          <div className={"sigContainer"}>
            <SignatureCanvas ref={sigCanvas} canvasProps={{ className: "sigCanvas" }}></SignatureCanvas>
          </div>
        )}
        <button type="submit">Generate Contract</button>
        {isError && <div>Error generating file</div>}
      </form>
    </div>
  );
};

export default ContractsPage;
