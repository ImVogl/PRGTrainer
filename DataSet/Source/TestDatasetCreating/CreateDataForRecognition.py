import numpy as np
import cv2
import matplotlib.pyplot as plt
from math import sin, cos, pi

path = 'I:\\Visual Studio 2017\\PRGTrainer\\DataSet\\Templates\\Seal\\Original.png'
img = cv2.imread(path)

def Affine(img):
    sourceTriangle = np.float32([[50,50],[200,50],[50,200]])
    destTriangle = np.float32([[10,100],[200,50],[100,250]])
    # sourceTriangle = np.float32([[-100, -100], [100, -100], [-100, 100]])
    # destTriangle = np.float32([[-125, -150], [75, -250], [-50, 150]])
    tranformMatrix = cv2.getAffineTransform(sourceTriangle, destTriangle)
    return cv2.warpAffine(img, tranformMatrix, (750, 750))

def Perspective(img, alpha, phi):
    height, width, channels = img.shape
    deltaX = height*sin(phi)
    deltaY = height*(1 - sin(alpha))
    source = np.float32([[0, 0], [width, 0], [0, height], [width, height]])
    dest = np.float32([[deltaX, 0], [width + deltaX, 0], [0, height - deltaY], [width, height - deltaY]])
    tranformMatrix = cv2.getPerspectiveTransform(source, dest)
    transformedImage = cv2.warpPerspective(img, tranformMatrix, (int(width + deltaX), int(height - deltaY)))
    white_bg = 255*np.ones_like(transformedImage)
    return transformedImage + white_bg


alpha_angles = [pi/6, pi/4, pi/3]
phi_angles = [0, pi/12, pi/6, pi/4, pi/3]

n_rows = len(alpha_angles) + 1
n_cols = len(phi_angles)
plt.subplot(n_rows, n_cols, 1),plt.imshow(img),plt.title('BaseImage')
plot_number = 3
for alpha in alpha_angles:
    for phi in phi_angles:
        title = 'alpha: {0:1.0f} degrees; phi:{1:1.0f} degrees'.format(alpha/pi*180, phi/pi*180)
        plt.subplot(n_rows, n_cols, plot_number),plt.imshow(Perspective(img, alpha, phi)),plt.title(title)
        plot_number += 1

plt.show()