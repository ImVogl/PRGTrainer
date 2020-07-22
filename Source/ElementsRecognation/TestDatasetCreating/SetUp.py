import argparse
from sys import argv
from math import pi
from os import listdir, getcwd
from os.path import join, isfile

# Задание параметров для отладки.
def DebugSetUp():
    backgroundDir = join(getcwd(), '..\\..\\..\\DataSet\\BackgroundsForDataSet')
    templatesDir = join(getcwd(), '..\\..\\..\\DataSet\\Templates\\Seal\\DetectionTemplate')
    outputDir = join(getcwd(), '..\\..\\..\\DataSet\\Output')
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

# Установка параметров
def SetUp():
    if len(argv) <= 1:
        return DebugSetUp()

    parser = argparse.ArgumentParser(description='Templates path folder and backgrounds path folder.')
    parser.add_argument('--templates', help='Path to folder with templates of targer image.')
    parser.add_argument('--background', help='Path to folder with background for image.')
    parser.add_argument('--out', help='Path to output dir.')
    parser.add_argument('--template_width', help='Width of template in mm.')
    parser.add_argument('--template_height', help='Height of template in mm.')

    arguments = parser.parse_args()
    backgroundDir = arguments.background
    templatesDir = arguments.templates
    outputDir = arguments.out
    template_width = int(arguments.template_width)
    template_height = int(arguments.template_height)

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
    phi_angles = [0, pi/12, pi/6]

    return alpha_angles, phi_angles, backgroundFiles, templatesFiles, outputDir, template_width, template_height
