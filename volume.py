import moviepy.editor as mp

class volumeProcesser:
    def __init__(self):
        pass

    def set_url(self, url):
        self.url = url
        self.media = mp.VideoFileClip(url)
        self.audio = self.media.audio
        self.fps = 30000
        self.audio_arr = self.audio.to_soundarray(nbytes=4, fps=self.fps)

    def get_min_max_volume(self):
        min, max = 0, 0
        for frame in range(len(self.audio_arr)):
            frame_audio = self.audio_arr[frame]
            sum = frame_audio[0] + frame_audio[1]
            max += (sum - max) * int(sum > max)
            min -= (min - sum) * int(sum < min)

    def get_volume(ms):
        pass