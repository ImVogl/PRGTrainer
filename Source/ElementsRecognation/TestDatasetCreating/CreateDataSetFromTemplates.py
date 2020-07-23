import cv2
from cv2 import COLOR_RGB2GRAY, cvtColor, imread
import numpy as np
from ImagesUtils import ChangeImageSize, ChangeForeshortening, ImageShape, SaveImages
from random import randint
from progress.bar import Bar
from SetUp import SetUp

# Изменение размера шаблона в соответствии с маштабом заднего фона
# background - Изображение заднего фона.
# template - Изображение шаблона.
# template_size - размер шаблона в мм.
def NormalizeTemplate(background, template, template_size):
    sample_ratio = {'x':template_size['x']/210, 'y':template_size['y']/297}
    height_back, width_back = ImageShape(background)
    height_template, width_template = ImageShape(template)
    real_ratio = {'x':width_template/width_back, 'y':height_template/height_back}
    resize_ratio = {'x': sample_ratio['x']/real_ratio['x'], 'y': sample_ratio['y']/real_ratio['y']}

    return ChangeImageSize(template, resize_ratio)

# Объединение заднего фона и изображения.
# background - изображение с заднем фоном.
# template - изображение шаблона.
# pos - положение шаблона на изображении.
def UnionImages(background, template, pos):
    boundary = 252
    height, width = ImageShape(template)
    image = np.copy(background)
    for x in range(width):
        for y in range(height):
            if template[y][x] < boundary and background[y + pos['y']][x + pos['x']] > template[y][x]:
                image[y + pos['y']][x + pos['x']] = template[y][x]

    return image

# Создание изображения с маской объекта.
# image - конечное изображение.
# size - размер шаблона.
# pos - положение шаблона.
def CreateMaskForImage(image, template, pos):
    height, width = ImageShape(template)
    size = { 'x': width, 'y': height }
    x_location = range(pos['x'], pos['x'] + size['x'])
    y_location = range(pos['y'], pos['y'] + size['y'])
    background = np.array([0, 100, 0], dtype=np.uint8)
    object_pos = np.array([100, 0, 0], dtype=np.uint8)
    height, width = ImageShape(image)
    img = np.ones((height, width, 3), dtype=np.uint8)
    for x in range(width):
        for y in range(height):
            if x in x_location and y in y_location:
                img[y][x] = object_pos
            else:
                img[y][x] = background

    return img

# Получение коллекции положений шаблона на фоне.
# backgroung - задний фон.
# template - шаблон.
# resultes_per_path - число результирующих файлов на одну пару шаблон-бэкграунд.
def GetPositions(backgroung, template, resultes_per_path):
    back_h, back_w = ImageShape(backgroung)
    template_h, template_w = ImageShape(template)
    x_min = 10
    x_max = back_w - 10 - template_w
    y_min = 10
    y_max = back_h - 10 - template_h

    positions = []
    for num in range(resultes_per_path):
        positions.append({'x': randint(x_min, x_max), 'y': randint(y_min, y_max)})
    
    return positions

# Получает экземпляр изображений
# pathToBackground - путь до файла бэкграунда.
# pathToTemplate - путь до файла шаблона.
# template_size - размер шаблона в мм.
def GetImages(pathToBackground, pathToTemplate, template_size):
    backgroung = cvtColor(imread(pathToBackground), COLOR_RGB2GRAY)
    template = cvtColor(imread(pathToTemplate), COLOR_RGB2GRAY)
    normalized_template = NormalizeTemplate(backgroung, template, template_size)
    return backgroung, normalized_template

# Создание коллекции изображений на основе одной пары фон-шаблон.
# backgroung - фоновое изображение.
# template - изабражение шаблона.
# pair_num - номер пары фон-шаблон.
def CreateImagesBunch(backgroung, template, alpha_angles, phi_angles, pair_num):
    resultes_per_path = 12  # Количество результирующих файлов на одну пару шаблон-бэкграунд.
    images_bunch = {}
    masks_bunch = {}
    keys = []
    sub_num = 1

    for pos in GetPositions(backgroung, template, resultes_per_path):
        base_image = UnionImages(backgroung, template, pos)
        base_mask = CreateMaskForImage(backgroung, template, pos)
        
        for alpha in alpha_angles:
            for phi in phi_angles:
                name = str(pair_num) + '_' + str(sub_num)
                images_bunch[name] = ChangeForeshortening(base_image, alpha, phi)
                masks_bunch[name] = ChangeForeshortening(base_mask, alpha, phi)
                keys.append(name)
                sub_num += 1
    
    return images_bunch, masks_bunch, keys

# Основная функция.
def Main():
    alpha_angles, phi_angles, backgroundFiles, templatesFiles, outputDir, template_width, template_height = SetUp()
    print()

    pair_num = 1
    template_size = { 'x':template_width, 'y':template_height }
    progressBar = Bar('Creating dataset', max = len(backgroundFiles) * len(templatesFiles))
    progressBar.start()
    for backgroundPath in backgroundFiles:
        for templatePath in templatesFiles:
            background, template = GetImages(backgroundPath, templatePath, template_size)
            images_bunch, masks_bunch, subkeys = CreateImagesBunch(background, template, alpha_angles, phi_angles, pair_num)
            SaveImages(outputDir, images_bunch, masks_bunch, subkeys)
            progressBar.next()
            pair_num += 1
    
    progressBar.finish()

Main()
