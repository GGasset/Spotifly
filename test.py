import numpy as np
from volume import VolumeProcessor

v = VolumeProcessor(20)

v.set_url('./current_media/Daviles de Novelda - Las calles de oro (Videoclip Oficial).mp4')

v.set_min_max_volume()

print(v.get_volume_percentage_from_ms(20 * 1000))

#print(volume.get_min_max_volume())