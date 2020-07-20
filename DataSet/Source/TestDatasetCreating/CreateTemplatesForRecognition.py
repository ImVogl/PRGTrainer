import matplotlib.pyplot as plt
from math import pi
from ImagesUtils import ChangeForeshortening
from cv2 import imread

path = 'I:\\Visual Studio 2017\\PRGTrainer\\DataSet\\Templates\\Seal\\Original.png'

img = imread(path)
alpha_angles = [pi/6, pi/4, pi/3]
phi_angles = [0, pi/12, pi/6, pi/4, pi/3]

n_rows = len(alpha_angles) + 1
n_cols = len(phi_angles)
plt.subplot(n_rows, n_cols, 1),plt.imshow(img),plt.title('BaseImage')
plot_number = 3
for alpha in alpha_angles:
    for phi in phi_angles:
        title = 'alpha: {0:1.0f} degrees; phi:{1:1.0f} degrees'.format(alpha/pi*180, phi/pi*180)
        plt.subplot(n_rows, n_cols, plot_number), plt.imshow(ChangeForeshortening(img, alpha, phi)), plt.title(title)
        plot_number += 1

plt.show()