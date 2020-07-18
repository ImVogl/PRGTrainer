import torch
from os import path, listdir
from PIL import Image
import numpy as np

# Получение списка файлов в папке.
def GetFiles(base_folder):
    paths = {}
    for name in listdir(base_folder):
        full_path = path.join(base_folder, name)
        if path.isfile(full_path):
            paths[name] = full_path
    
    return paths

class PennFudanDataset(object):
    def __init__(self, root, transforms):
        self.root = root
        self.transforms = transforms
        base_folder = path.join(root, "Detection")
        self.images_paths = GetFiles(base_folder)
        self.masks_paths = GetFiles(path.join(base_folder, "Masks"))
        self.keys = list(self.images_paths.keys())

    def __getitem__(self, idx):
        image_path = self.images_paths[self.keys[idx]]
        mask_path = self.masks_paths[self.keys[idx]]

        # LA - оттенки серего
        image = Image.open(image_path).convert('LA')
        mask = Image.open(mask_path).convert('RGB')
        
        # convert the PIL Image into a numpy array
        mask = np.array(mask)
        # instances are encoded as different colors
        obj_ids = np.unique(mask)
        # first id is the background, so remove it
        obj_ids = obj_ids[1:]

        # split the color-encoded mask into a set
        # of binary masks
        masks = mask == obj_ids[:, None, None]

        # get bounding box coordinates for each mask
        num_objs = len(obj_ids)
        boxes = []
        for i in range(num_objs):
            pos = np.where(masks[i])
            xmin = np.min(pos[1])
            xmax = np.max(pos[1])
            ymin = np.min(pos[0])
            ymax = np.max(pos[0])
            boxes.append([xmin, ymin, xmax, ymax])

        # convert everything into a torch.Tensor
        boxes = torch.as_tensor(boxes, dtype=torch.float32)
        # there is only one class
        labels = torch.ones((num_objs,), dtype=torch.int64)
        masks = torch.as_tensor(masks, dtype=torch.uint8)

        image_id = torch.tensor([idx])
        area = (boxes[:, 3] - boxes[:, 1]) * (boxes[:, 2] - boxes[:, 0])
        # suppose all instances are not crowd
        iscrowd = torch.zeros((num_objs,), dtype=torch.int64)

        target = {}
        target["boxes"] = boxes
        target["labels"] = labels
        target["masks"] = masks
        target["image_id"] = image_id
        target["area"] = area
        target["iscrowd"] = iscrowd

        if self.transforms is not None:
            image, target = self.transforms(image, target)

        return image, target

    def __len__(self):
        return len(self.keys)
