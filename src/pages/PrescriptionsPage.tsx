import React, { useState } from "react";

const PrescriptionPage: React.FC = () => {
  const [showStreamlit, setShowStreamlit] = useState(false);

  // Toggle function to show or hide Streamlit app
  const handleToggleStreamlit = () => {
    setShowStreamlit((prev) => !prev);
  };

  return (
    <div
      style={{
        padding: "20px",
        fontFamily: "'Arial', sans-serif",
        backgroundColor: "#f9f9f9",
        minHeight: "100vh",
      }}
    >
      {/* Title Section */}
      <h1
        style={{
          textAlign: "center",
          color: "#4CAF50",
          marginBottom: "30px",
          fontSize: "3rem",
          letterSpacing: "2px",
          borderBottom: "3px solid #4CAF50",
          paddingBottom: "10px",
        }}
      >
        Upload Prescription
      </h1>

      {/* Button Section */}
      <div style={{ textAlign: "center", marginBottom: "20px" }}>
        <button
          onClick={handleToggleStreamlit}
          style={{
            backgroundColor: showStreamlit ? "#f44336" : "#4CAF50",
            color: "#fff",
            padding: "12px 24px",
            fontSize: "16px",
            border: "none",
            borderRadius: "5px",
            cursor: "pointer",
            boxShadow: "0px 4px 6px rgba(0, 0, 0, 0.2)",
            transition: "background-color 0.3s ease",
          }}
        >
          {showStreamlit ? "Close Streamlit" : "Open Streamlit"}
        </button>
      </div>

      {/* Streamlit IFrame Section */}
      {showStreamlit && (
        <div
          style={{
            marginTop: "20px",
            border: "2px solid #ddd",
            borderRadius: "8px",
            boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
            overflow: "hidden",
            height: "600px",
          }}
        >
          <iframe
            src="http://localhost:8501/"
            title="Streamlit App"
            width="100%"
            height="100%"
            style={{ border: "none" }}
          />
        </div>
      )}
    </div>
  );
};

export default PrescriptionPage;
