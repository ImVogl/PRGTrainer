import argparse
from os import listdir
from os.path import join, isfile
import cv2
from cv2 import COLOR_RGBA2GRAY, cvtColor, imread
from numpy import ones, full_like, uint8, array
from ImagesUtils import ChangeImageSize, ChangeForeshortening
from random import randint
from Utils import ClearFolder

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

    alpha_angles = [pi/4, pi/3, pi/2]
    phi_angles = [0, pi/12, pi/6, pi/4, pi/3]

    return alpha_angles, phi_angles, backgroundFiles, templatesFiles, outputDir, template_width, template_height

# Задание параметров для отладки.
def DebugSetUp():
    backgroundDir = 'I:\\Visual Studio 2017\\PRGTrainer\\DataSet\\BackgroundsForDataSet'
    templatesDir = 'I:\\Visual Studio 2017\\PRGTrainer\\DataSet\\Templates\\Seal\\DetectionTemplate'
    outputDir = 'I:\\Visual Studio 2017\\PRGTrainer\\DataSet\\Output'
    template_width = 41
    template_height = 41

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

    alpha_angles = [pi/4, pi/3, pi/2]
    phi_angles = [0, pi/12, pi/6, pi/4, pi/3]

    return alpha_angles, phi_angles, backgroundFiles, templatesFiles, outputDir, template_width, template_height


# Изменение размера шаблона в соответствии с маштабом заднего фона
# background - Изображение заднего фона.
# template - Изображение шаблона.
# template_size - размер шаблона в мм.
def NormalizeTemplate(background, template, template_size):
    sample_ratio = {'x':template_size['x']/210, 'y':template_size['y']/297}
    width_back, height_back, channels_back = background.shape
    width_template, height_template, channels_template = template.shape
    real_ratio = {'x':width_template/width_back, 'y':height_template/height_back}
    resize_ratio = {'x': sample_ratio['x']/real_ratio['x'], 'y': sample_ratio['y']/real_ratio['y']}

    return ChangeImageSize(template, resize_ratio)

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

# Создание изображения с маской объекта.
# image - конечное изображение.
# size - размер шаблона.
# pos - положение шаблона.
def CreateMaskForImage(image, template, pos):
    width, height, channels = template.shape
    size = { 'x': width, 'y': height }
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

# Получение коллекции положений шаблона на фоне.
# backgroung - задний фон.
# template - шаблон.
# resultes_per_path - число результирующих файлов на одну пару шаблон-бэкграунд.
def GetPositions(backgroung, template, resultes_per_path):
    back_w, back_h, channels = backgroung.shape
    template_w, template_h, channels = backgroung.shape
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
    backgroung = cvtColor(imread(pathToBackground), COLOR_RGBA2GRAY)
    template = cvtColor(imread(pathToTemplate), COLOR_RGBA2GRAY)
    normalized_template = NormalizeTemplate(backgroung, template, template_size)
    return backgroung, normalized_template

# Создание коллекции изображений на основе одной пары фон-шаблон.
# backgroung - фоновое изображение.
# template - изабражение шаблона.
# pair_num - номер пары фон-шаблон.
def CreateImagesBunch(backgroung, template, alpha_angles, phi_angles, pair_num):
    resultes_per_path = 15  # Количество результирующих файлов на одну пару шаблон-бэкграунд.
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
def SaveData(outputDir, images, masks, keys, save_proportion = 1.0 ):
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


# Основная функция.
def Main():
    alpha_angles, phi_angles, backgroundFiles, templatesFiles, outputDir, template_width, template_height = DebugSetUp()
    pair_num = 1

    images = {}
    masks = {}
    keys = []
    template_size = { 'x':template_width, 'y':template_height }
    for backgroundPath in backgroundFiles:
        for templatePath in templatesFiles:
            background, template = GetImages(backgroundPath, templatePath, template_size)
            images_bunch, masks_bunch, subkeys = CreateImagesBunch(background, template, alpha_angles, phi_angles, pair_num)
            
            keys += subkeys
            images.update(images_bunch)
            masks.update(masks_bunch)

            pair_num += 1

    SaveData(outputDir, images, masks, keys)
            

parser = argparse.ArgumentParser(description='Templates path folder and backgrounds path folder.')
parser.add_argument('--templates', help='Path to folder with templates of targer image.')
parser.add_argument('--background', help='Path to folder with background for image.')
parser.add_argument('--out', help='Path to output dir.')
parser.add_argument('--template_width', help='Width of template in mm.')
parser.add_argument('--template_height', help='Height of template in mm.')

# alpha_angles, phi_angles, backgroundFiles, templatesFiles = SetUp(parser)

