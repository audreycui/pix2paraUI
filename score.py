#test to get type
#from keras.applications.vgg19 import VGG19
import json 
from keras.preprocessing.image import load_img
from keras.preprocessing.image import img_to_array
from keras.applications.vgg19 import preprocess_input
from keras.applications.vgg19 import decode_predictions
from keras.applications.vgg19 import VGG19
import numpy as np
import io
from PIL import Image
import json
from numpy import array
import os
import joblib

def init():
    global model
    model = VGG19()
    
def run(image_bytes):
    image_bytes = json.loads(image_bytes)['data'] #str to string
    image_bytes = image_bytes.encode('utf-8')
    image = Image.frombytes('RGBA', (224,224), image_bytes, 'raw')
    image = image.resize((224,224),Image.ANTIALIAS)    
    image = img_to_array(image)
    image = image[:,:,0:3]
    image = image.reshape((1, image.shape[0], image.shape[1], image.shape[2]))
    image = preprocess_input(image)
    yhat = model.predict(image)
    label = decode_predictions(yhat)
    label = label[0][0][1]
    return(label)
   
    
    

