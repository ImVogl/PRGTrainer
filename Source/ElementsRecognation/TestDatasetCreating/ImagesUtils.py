import numpy as np
import cv2
from math import sin, cos, pi
from random import randint
from Utils import ClearFolder
from os.path import join

# Получает размер размера изображения.
# image - изображение.
def ImageShape(image):
    if len(image.shape) == 3:
        height, width, channels = image.shape
    else:
        height, width = image.shape
    return height, width

# Изменение угла съемки объекта.
# img - Изображение, которое предстоит преобразовать.
# alpha - вертикальный угол "съемки".
# phi - горизонтальный угол "съемки".
def ChangeForeshortening(img, alpha, phi):
    height, width = ImageShape(img)
    deltaX = abs(height*sin(phi))
    deltaY = height*(1 - sin(alpha))
    source = np.float32([[0, 0], [width, 0], [0, height], [width, height]])
    if phi > 0:
        dest = np.float32([[deltaX, 0], [width + deltaX, 0], [0, height - deltaY], [width, height - deltaY]])
    else:
        dest = np.float32([[0, 0], [width, 0], [deltaX, height - deltaY], [width + deltaX, height - deltaY]])

    tranformMatrix = cv2.getPerspectiveTransform(source, dest)
    transformedImage = cv2.warpPerspective(img, tranformMatrix, (int(width + deltaX), int(height - deltaY)))
    white_bg = 255*np.ones_like(transformedImage)
    return transformedImage + white_bg

# Изменение размера изображения.
# img - исходное изображение.
# k - словарь с коэффициента изменения размеров для каждого из измерений.
def ChangeImageSize(img, k):
    height, width = ImageShape(img)
    new_height = int(height*k['y'])
    new_width = int(width*k['x'])
    source = np.float32([[0, 0], [width, 0], [0, height], [width, height]])
    dest = np.float32([[0, 0], [new_width, 0], [0, new_height], [new_width, new_height]])
    tranformMatrix = cv2.getPerspectiveTransform(source, dest)
    transformedImage = cv2.warpPerspective(img, tranformMatrix, (new_width, new_height))
    return transformedImage

# Выдает значение, которое определяет, нужно ли сохранять изображение.
# save_proportion - доля изображений, которая будет случайным образом отобрана и сохранена.
def MustSave(save_proportion):
    percent = int(100 * save_proportion)
    return percent > randint(0, 100)

# Сохранение сгенерированных файлов и масок.
# outputDir - путь до выходной директории.
# images - словарь изображений.
# masks - словарь масок с изображениями.
# keys - коллекция ключей к словарю.
# save_proportion - доля изображений, которая будет случайным образом отобрана и сохранена.
def SaveImages(outputDir, images, masks, keys, save_proportion = 1.0):
    if (save_proportion > 1.0):
        save_proportion = 1.0

    if (save_proportion < 0.0):
        save_proportion = 0.0

    imagesDir = join(outputDir, 'Images')
    masksDir = join(outputDir, 'Masks')

    ClearFolder(imagesDir)
    ClearFolder(masksDir)
    for key in keys:
        if not MustSave(save_proportion):
            continue

        cv2.imwrite(join(imagesDir, key + '.png'), images[key])
        cv2.imwrite(join(masksDir, key + '.png'), masks[key])
