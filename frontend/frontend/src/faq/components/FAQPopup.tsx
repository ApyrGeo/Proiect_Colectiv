import React, { useState } from "react";
import "../FAQPopup.css";
import type { FAQPopupProps } from "../props.ts";

const FAQPopup: React.FC<FAQPopupProps> = ({ faqs }) => {
  const [open, setOpen] = useState(false);

  return (
    <>
      {/* Iconița fixă în dreapta jos */}
      <div className="faq-icon" onClick={() => setOpen((prev) => !prev)}>
        ❓
      </div>

      {open && (
        <div className="faq-overlay" onClick={() => setOpen(false)}>
          <div className="faq-container" onClick={(e) => e.stopPropagation()}>
            <button className="faq-close" onClick={() => setOpen(false)}>
              ×
            </button>

            <h2 className="faq-title">Întrebări frecvente</h2>

            {faqs.map((faq, i) => (
              <div key={i} className="faq-item">
                <h3 className="faq-question">{faq.question}</h3>
                <p className="faq-answer">{faq.answer}</p>
              </div>
            ))}
          </div>
        </div>
      )}
    </>
  );
};

export default FAQPopup;
