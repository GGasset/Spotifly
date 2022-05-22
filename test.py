import numpy as np
from volume import VolumeProcessor
import time

v = VolumeProcessor(20)


initial_time = time.time()
v.set_url('./current_media/Daviles de Novelda - Las calles de oro (Videoclip Oficial).mp4')
print('Seconds to get media object: ', time.time() - initial_time)

initial_time = time.time()
print('Audio len: ', len(v.get_audio_sum_list()))
print('Seconds to get audio sum array: ', time.time() - initial_time)

initial_time = time.time()
v.set_min_max_volume()
print('Seconds set min-max volume: ', time.time() - initial_time)

print(v.get_volume_percentage_from_ms(20 * 1000))
