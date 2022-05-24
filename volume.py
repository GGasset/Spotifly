import enum
import moviepy.editor as mp

class VolumeProcessor:
    def __init__(self, min_percentage):
        # If min percentage is less or equal to one multiply it times 100
        min_percentage = (min_percentage * 100) * min_percentage <= 1 + min_percentage * min_percentage > 1

        self.min_percentage = min_percentage

    def set_url(self, url):
        self.url = url
        self.media = mp.VideoFileClip(url)
        self.audio = self.media.audio
        self.fps = 20000
        self.audio_arr = self.audio.to_soundarray(nbytes=4, fps=self.fps)
        self.audio_len = len(self.audio_arr)

    def get_audio_sum_list(self, writeToFile=False):
        file = None
        if writeToFile:
            file = open('./media/media_audio.txt', 'w')
        output = []
        for i, sound in enumerate(self.audio_arr):
            sum = sound[0] + sound[1]
            output.append(sum)
            if writeToFile:
                to_write = str(sum) + (', ' * (i < self.audio_len - 1))
                file.write(to_write)
        if writeToFile:
            file.close()
        return output

    def set_min_max_volume(self, url='None'):
        """You must use for each media set_url before using this function or use url='url.extension'"""
        if url != 'None':
            self.set_url(url)

        min, max = 0, 0
        for frame, sound in enumerate(self.audio_arr):
            sum = sound[0] + sound[1]
            max += (sum - max) * int(sum > max)
            min -= (min - sum) * int(sum < min)
            self.min = min
            self.max = max

    def get_volume(self, ms):
        """You must use for each media set_url before using this function"""
        frame = int(ms * (self.fps / 1000))
        frame_audio = self.audio_arr[frame]
        sum = frame_audio[0] + frame_audio[1]
        return sum

    def get_volume_percentage(self, volume):
        """You must use for each media set_min_max_volume before using this function"""
        percentage = volume - self.min
        new_max = self.max - self.min
        percentage /= new_max
        percentage *= 100

        # max between percentage self.min_percentage
        percentage = percentage * (percentage > self.min_percentage) + self.min_percentage * (percentage < self.min_percentage)
        return int(percentage)

    def get_volume_percentage_from_ms(self, ms):
        """You must use for each media set_min_max_volume before using this function"""
        return self.get_volume_percentage(self.get_volume(ms))