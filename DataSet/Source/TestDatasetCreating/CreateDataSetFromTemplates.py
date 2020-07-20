import argparse
from os import listdir
from os.path import join, isfile
import cv2
from cv2 import COLOR_RGBA2GRAY, cvtColor, imread
from numpy import ones, full_like, uint8, array
from ImagesUtils import ChangeImageSize, ChangeForeshortening

# Установка параметров
# parser - парсер входных аргументов.
def SetUp(parser):
    args = parser.parse_args()
    backgroundDir = args.background
    templatesDir = args.templates
    outputDir = args.out
    template_width = args.template_width
    template_height = args.template_height

    backgroundFiles = []
    for file_name in listdir(backgroundDir):
        full_path = join(backgroundDir, file_name)
        if isfile(full_path):
            backgroundFiles.append(full_path)

    templatesFiles = []
    for file_name in listdir(templatesDir):
        full_path = join(templatesDir, file_name)
        if isfile(full_path):
            templatesFiles.append(full_path)

    alpha_angles = [pi/6, pi/4, pi/3]
    phi_angles = [0, pi/12, pi/6, pi/4, pi/3]

    return alpha_angles, phi_angles, backgroundFiles, templatesFiles

# Объединение заднего фона и изображения.
# background - изображение с заднем фоном.
# template - изображение шаблона.
# pos - положение шаблона на изображении.
def UnionImages(background, template, pos):
    boundary = 225
    width, height, channels = template.shape
    image = full_like(background)
    for x in range(width):
        for y in range(height):
            if template[x][y][0] < boundary and background[x + pos['x']][y + pos['y']][0] < template[x][y][0]:
                image[x][y] = template[x][y]

    return image

# Создание изображения для обучения.
# pathToBackground - путь до папки с задним фоном.
# pathToTemplate - путь до папки с шаблонами изображений, которые предстоит найти 
# pos - положение шаблона на фоне.
def CreateTrainImage(pathToBackground, pathToTemplate, pos):
    backgroung = cvtColor(imread(pathToBackground), COLOR_RGBA2GRAY)
    template = cvtColor(imread(pathToTemplate), COLOR_RGBA2GRAY)
    return UnionImages(backgroung, template, pos)

# Создание изображения с маской объекта.
# image - конечное изображение.
# size - размер шаблона.
# pos - положение шаблона.
def CreateMaskForImage(image, size, pos):
    x_location = range(pos['x'], pos['x'] + size['x'])
    y_location = range(pos['y'], pos['y'] + size['y'])
    background = array([0, 100, 0], dtype=uint8)
    object_pos = array([100, 0, 0], dtype=uint8)
    width, height, channels = image.shape
    img = ones(width, height, 3, dtype=uint8)
    for x in range(width):
        for y in range(height):
            if x in x_location and y in y_location:
                img[x][y] = object_pos
            else:
                img[x][y] = background

    return img

# Изменение размера шаблона в соответствии с маштабом заднего фона
# background - Изображение заднего фона.
# template - Изображение шаблона.
# template_size - размер шаблона в мм.
def NormalizeTemplate(background, template, template_size):
    sample_ratio = {'x':41/210, 'y':41/297}
    width_back, height_back, channels_back = background.shape
    width_template, height_template, channels_template = template.shape
    real_ratio = {'x':width_template/width_back, 'y':height_template/height_back}
    resize_ratio = {'x': sample_ratio['x']/real_ratio['x'], 'y': sample_ratio['y']/real_ratio['y']}

    return ChangeImageSize(template, resize_ratio)

parser = argparse.ArgumentParser(description='Templates path folder and backgrounds path folder.')
parser.add_argument('--templates', help='Path to folder with templates of targer image.')
parser.add_argument('--background', help='Path to folder with background for image.')
parser.add_argument('--out', help='Path to output dir.')
parser.add_argument('--template_width', help='Width of template in mm.')
parser.add_argument('--template_height', help='Height of template in mm.')

alpha_angles, phi_angles, backgroundFiles, templatesFiles = SetUp(parser)
