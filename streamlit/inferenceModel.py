import cv2
import numpy as np
from PIL import Image
import onnxruntime as ort
from sklearn.metrics import precision_score, recall_score, f1_score
from mltu.inferenceModel import OnnxInferenceModel 
from mltu.utils.text_utils import ctc_decoder, get_cer 

class ImageToWordModel(OnnxInferenceModel):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def predict(self, image: np.ndarray):
        # Resize image to the model's expected input size
        image = cv2.resize(image, self.input_shapes[0][1:3][::-1])
        image_pred = np.expand_dims(image, axis=0).astype(np.float32)
        
        # Run inference
        preds = self.model.run(self.output_names, {self.input_names[0]: image_pred})[0]
        
        # Decode predictions
        text = ctc_decoder(preds, self.metadata["vocab"])[0]
        return text

def compute_f1(pred_text, true_text):
    # Convert text to character-level binary classification
    pred_chars = list(pred_text)
    true_chars = list(true_text)
    
    # Pad shorter string with spaces to match lengths
    max_len = max(len(pred_chars), len(true_chars))
    pred_chars.extend([' '] * (max_len - len(pred_chars)))
    true_chars.extend([' '] * (max_len - len(true_chars)))

    # Create binary arrays for precision/recall computation
    y_true = np.array([1 if c != ' ' else 0 for c in true_chars])
    y_pred = np.array([1 if c != ' ' and c in true_chars else 0 for c in pred_chars])

    # Compute precision, recall, and F1 score
    precision = precision_score(y_true, y_pred)
    recall = recall_score(y_true, y_pred)
    f1 = f1_score(y_true, y_pred)

    return precision, recall, f1

if __name__ == "__main__":
    import pandas as pd
    from tqdm import tqdm

    model = ImageToWordModel(model_path="Models/08_handwriting_recognition_torch/202411152259/model.onnx")

    # Load validation data
    df = pd.read_csv("Models/08_handwriting_recognition_torch/202411152259/val.csv").values.tolist()

    accum_cer = []
    precision_list, recall_list, f1_list = [], [], []

    for image_path, label in tqdm(df):
        image = cv2.imread(image_path.replace("\\", "/"))

        prediction_text = model.predict(image)

        cer = get_cer(prediction_text, label)
        precision, recall, f1 = compute_f1(prediction_text, label)

        print(f"Image: {image_path}, Label: {label}, Prediction: {prediction_text}, CER: {cer}, Precision: {precision}, Recall: {recall}, F1: {f1}")

        accum_cer.append(cer)
        precision_list.append(precision)
        recall_list.append(recall)
        f1_list.append(f1)

    print(f"Average CER: {np.average(accum_cer)}")
    print(f"Accuracy = {1 - np.average(accum_cer)}")
    print(f"Average Precision: {np.mean(precision_list)}")
    print(f"Average Recall: {np.mean(recall_list)}")
    print(f"Average F1 Score: {np.mean(f1_list)}")
