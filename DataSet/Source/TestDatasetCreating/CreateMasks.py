from PIL import Image
from os import getcwd, path, listdir
import numpy as np

def GetMask(image):
    background = np.array([0, 100, 0], dtype=np.uint8)
    object_pos = np.array([100, 0, 0], dtype=np.uint8)
    boundary = 225
    width, height = image.size
    image_array = np.array(image)
    mask = np.zeros([height, width, 3], dtype=np.uint8)
    for x in range(height):
        for y in range(width):
            if image_array[x][y][0] < boundary:
                mask[x][y] = object_pos
            else:
                mask[x][y] = background

    return mask

rootDir = path.join(getcwd(), 'Seal\\Detection')
mask_root_folder = path.join(getcwd(), 'Seal\\Detection\\Masks')
files = listdir(rootDir)
paths = []
for file_name in files:
    full_path = path.join(rootDir, file_name)
    if path.isfile(full_path):
        paths.append(full_path)

for file_path in paths:
    mask = GetMask(Image.open(file_path).convert('LA'))
    mask_image = Image.fromarray(mask)
    mask_image.save(path.join(mask_root_folder, path.basename(file_path)))
