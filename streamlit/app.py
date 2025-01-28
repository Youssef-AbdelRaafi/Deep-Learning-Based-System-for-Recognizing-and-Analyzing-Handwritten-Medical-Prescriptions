import os
import base64
import streamlit as st
import cv2
import numpy as np
from PIL import Image
from inferenceModel import ImageToWordModel
import requests
import time

# Initialize the model
model = ImageToWordModel(model_path="D:\\streamlit\\Model\\model.onnx")

# Backend API configuration
API_BASE_URL = "http://localhost:5198/api/Handler"

# Streamlit app setup
st.title("Recognizing and Analyzing Handwritten Medical Prescriptions")

# Folder to store Base64 encoded images
UPLOAD_FOLDER = "uploads"
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

# Pharmacist ID input
pharmacist_id = st.text_input("Enter your Pharmacist ID", value="")
if not pharmacist_id:
    st.warning("Please enter your Pharmacist ID to proceed.")

# Upload image
uploaded_file = st.file_uploader("Upload an image", type=["jpg", "png", "jpeg"])

if uploaded_file is not None:
    # Display uploaded image
    image = Image.open(uploaded_file)
    st.image(image, caption="Uploaded Image", use_column_width=True)

    # Convert the image to OpenCV format
    image = np.array(image)
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

    # Make prediction
    prediction_text = model.predict(image)

    # Display the prediction result
    st.markdown(f"<h2 style='text-align: center; color: black;'>Prediction: {prediction_text}</h2>", unsafe_allow_html=True)

    # Save to database button
    if st.button("Save to Database"):
        if not pharmacist_id:
            st.error("Pharmacist ID is required to save prescriptions.")
        else:
            try:
                # Convert the image to Base64
                image_bytes = uploaded_file.read()
                base64_image = base64.b64encode(image_bytes).decode("utf-8")
                
                # Generate a unique filename
                unique_filename = f"{int(time.time())}.png"

                # Save data to the backend
                payload = {
                    "ImageName": unique_filename,
                    "PredictedText": prediction_text,
                    "PharmacistId": int(pharmacist_id),
                }

                response = requests.post(f"{API_BASE_URL}/AddPrescription", json=payload)
                
                if response.status_code == 200:
                    st.success("Prescription and Predicted text saved successfully!")
                else:
                    st.error(f"Failed to save prescription: {response.text}")
            except Exception as e:
                st.error(f"Error: {e}")

# Clean old files function
def clean_old_files(folder, max_age=3600):  # Delete files older than 1 hour
    current_time = time.time()
    for filename in os.listdir(folder):
        file_path = os.path.join(folder, filename)
        if os.path.isfile(file_path) and current_time - os.path.getmtime(file_path) > max_age:
            os.remove(file_path)

# Periodic cleanup of old files
clean_old_files(UPLOAD_FOLDER)
