import { Combobox } from "@headlessui/react";
import { useEffect, useState } from "react";

export interface ComboOption<T> {
  value: T;
  label: string;
}

interface ComboBoxProps<T> {
  options: ComboOption<T>[];
  value?: ComboOption<T>;
  onChange: (value: ComboOption<T>) => void;
  placeholder?: string;
  disabled?: boolean;
}

export function ComboBox<T>({ options, value, onChange, placeholder, disabled }: ComboBoxProps<T>) {
  const [query, setQuery] = useState("");
  const [lastValid, setLastValid] = useState<ComboOption<T> | null>(value ?? null);

  /* Keep lastValid in sync when parent changes value */
  useEffect(() => {
    if (value) {
      setLastValid(value);
    }
  }, [value]);

  const filteredOptions =
    query === "" ? options.slice(0, 3) : options.filter((o) => o.label.toLowerCase().includes(query.toLowerCase()));

  return (
    <Combobox
      value={value ?? null}
      onChange={(v) => {
        setLastValid(v); // ✅ commit valid choice
        onChange(v);
        setQuery("");
      }}
      disabled={disabled}
    >
      <div className="combo-container">
        <div className="combo-input-wrapper">
          <Combobox.Input
            className="combo-input"
            placeholder={placeholder}
            displayValue={(opt: ComboOption<T>) => opt?.label ?? ""}
            onChange={(e) => setQuery(e.target.value)}
            onBlur={() => {
              if (!value && lastValid) {
                onChange(lastValid);
              }
              setQuery("");
            }}
            onKeyDown={(e) => {
              if (e.key === "Escape") {
                setQuery("");
              }
            }}
          />

          <Combobox.Button className="combo-button">▾</Combobox.Button>
        </div>

        <Combobox.Options className="combo-options">
          {filteredOptions.length === 0 && <div className="combo-option-disabled">No results</div>}

          {filteredOptions.map((opt) => (
            <Combobox.Option
              key={opt.label}
              value={opt}
              className={({ active }) => `combo-option ${active ? "active" : ""}`}
            >
              {opt.label}
            </Combobox.Option>
          ))}
        </Combobox.Options>
      </div>
    </Combobox>
  );
}
