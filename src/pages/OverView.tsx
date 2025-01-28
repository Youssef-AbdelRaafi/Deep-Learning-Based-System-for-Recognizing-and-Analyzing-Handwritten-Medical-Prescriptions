import React, { useState, useEffect } from "react";
import axios from "axios";

const OverviewPage: React.FC = () => {
  const [prescriptions, setPrescriptions] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch data from the backend
  const fetchPrescriptions = async () => {
    setLoading(true);
    setError(null);
    try {
        const pharmacistId = localStorage.getItem("pharmacistId");
        if (!pharmacistId) {
            setError("Pharmacist ID not found. Please log in again.");
            return;
        }

        console.log(`Fetching prescriptions for pharmacistId: ${pharmacistId}`);
        const response = await axios.get(
            `http://localhost:5198/api/Handler/GetPrescriptions/${pharmacistId}`
        );

        if (response.data && Array.isArray(response.data)) {
            console.log("API Response: ", response.data);
            setPrescriptions(response.data);
        } else if (response.data.message) {
            console.warn(response.data.message);
        }
    } catch (err) {
        console.error("Error fetching prescriptions: ", err);
        setError("Failed to fetch prescriptions.");
    } finally {
        setLoading(false);
    }
};

  // Delete prescription
  
  const handleDeletePrescription = async (id: number) => {
    try {
      await axios.delete(`http://localhost:5198/api/Handler/DeletePrescription/${id}`);
      setPrescriptions((prev) => prev.filter((prescription) => prescription.id !== id));
      alert("Prescription deleted successfully!");
    } catch (err) {
      console.error("Failed to delete prescription:", err);
      alert("Failed to delete prescription. Please try again.");
    }
  };
  // Fetch prescriptions on component mount
  useEffect(() => {
    fetchPrescriptions();
  }, []);

  return (
    <div
      style={{
        padding: "20px",
        fontFamily: "'Arial', sans-serif",
        backgroundColor: "#f9f9f9",
        minHeight: "100vh",
      }}
    >
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
        Overview
      </h1>

      <div style={{ marginTop: "40px" }}>
        <h2 style={{ textAlign: "center", color: "#333", marginBottom: "20px" }}>Saved Prescriptions</h2>

        {loading ? (
          <p style={{ textAlign: "center" }}>Loading...</p>
        ) : error ? (
          <p style={{ textAlign: "center", color: "red" }}>{error}</p>
        ) : prescriptions.length === 0 ? (
          <p style={{ textAlign: "center" }}>No prescriptions found.</p>
        ) : (
          <ul
            style={{
              listStyle: "none",
              padding: "0",
              display: "flex",
              flexWrap: "wrap",
              justifyContent: "center",
              gap: "20px",
            }}
          >
            {prescriptions.map((prescription) => (
              <li
                key={prescription.id}
                style={{
                  width: "300px",
                  border: "1px solid #ddd",
                  borderRadius: "8px",
                  boxShadow: "0px 4px 8px rgba(0, 0, 0, 0.1)",
                  padding: "20px",
                  backgroundColor: "#fff",
                }}
              >
                <h3 style={{ color: "#4CAF50", marginBottom: "10px" }}>Prediction:</h3>
                <p style={{ color: "#333", marginBottom: "10px" }}>{prescription.predictedText}</p>
                <h3 style={{ color: "#4CAF50", marginBottom: "10px" }}>Image:</h3>
                <p>{prescription.imageName}</p>
                <p style={{ color: "#777", fontSize: "12px" }}>Created At: {new Date(prescription.createdAt).toLocaleString()}</p>
                <button
                  onClick={() => handleDeletePrescription(prescription.id)}
                  style={{
                    backgroundColor: "#f44336",
                    color: "#fff",
                    padding: "8px 16px",
                    border: "none",
                    borderRadius: "5px",
                    cursor: "pointer",
                    marginTop: "10px",
                  }}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default OverviewPage;
