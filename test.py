import numpy as np
from volume import VolumeProcessor
import time

v = VolumeProcessor(20)


initial_time = time.time()
v.set_url('./media/Daviles de Novelda - Las calles de oro (Videoclip Oficial).mp4')
print('Seconds to get media object: ', time.time() - initial_time)

initial_time = time.time()
audio_sum = v.get_audio_sum_list(writeToFile=True)
print('Seconds to get and write audio: ', time.time() - initial_time)

initial_time = time.time()
encoded_len = len(str(len(str(audio_sum).encode('ascii'))).encode('ascii'))
print('Time to parse to bytes: ', time.time() - initial_time)

initial_time = time.time()
v.set_min_max_volume()
print('Seconds set min-max volume: ', time.time() - initial_time)

print(v.audio_len / v.fps)
for i in range(0, int(v.audio_len / v.fps), 1):
    print(v.get_volume_percentage_from_ms(i * 1000))
