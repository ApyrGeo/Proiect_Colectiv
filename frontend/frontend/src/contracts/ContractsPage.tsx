import { exampleStructures, FieldCategory } from "./contractStructures.ts";
import { useRef, useState } from "react";
import "./contracts.css";
import SignatureCanvas from "react-signature-canvas";

const ContractsPage: React.FC = () => {
  const contractStructures = exampleStructures;

  const [selectedContract, setSelectedContract] = useState(0);

  const sigCanvas = useRef<SignatureCanvas>(null);

  const handleChange = (value: number) => {
    setSelectedContract(value);
  };

  const handleSubmit = (formData: FormData) => {
    console.log(formData);
    if (contractStructures[selectedContract].signature && sigCanvas.current) console.log(sigCanvas.current.toDataURL());
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
      </form>
    </div>
  );
};

export default ContractsPage;
