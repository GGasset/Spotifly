import numpy as np
from volume import volumeProcessor

v = volumeProcessor()

v.set_url('./current_media/Daviles de Novelda - Las calles de oro (Videoclip Oficial).mp4')

v.get_min_max_volume()

#print(volume.get_min_max_volume())