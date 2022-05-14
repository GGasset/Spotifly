import moviepy.editor as mp

class volumeProcessor:
    def __init__(self, min_percentage):
        self.min_percentage = min_percentage

    def set_url(self, url):
        self.url = url
        self.media = mp.VideoFileClip(url)
        self.audio = self.media.audio
        self.fps = 30000
        self.audio_arr = self.audio.to_soundarray(nbytes=4, fps=self.fps)

    def set_min_max_volume(self):
        min, max = 0, 0
        for frame in range(len(self.audio_arr)):
            frame_audio = self.audio_arr[frame]
            sum = frame_audio[0] + frame_audio[1]
            max += (sum - max) * int(sum > max)
            min -= (min - sum) * int(sum < min)
            self.min = min
            self.max = max

    def get_volume(self, ms):
        frame = ms * (self.fps / 1000)
        frame_audio = self.audio_arr[frame]
        sum = frame_audio[0] + frame_audio[1]
        return sum

    def get_volume_percentage(self, volume):
        percentage = volume - self.min
        new_max = self.max - self.min
        percentage /= new_max
        percentage *= 100

        # max between percentage self.min_percentage
        percentage = percentage * (percentage > self.min_percentage) + self.min_percentage * (percentage < self.min_percentage)
        return percentage

    def get_volume_percentage_from_ms(self, ms):
        return self.get_volume_percentage(self.get_volume(ms))