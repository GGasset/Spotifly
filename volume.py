import moviepy.editor as mp

media_url = ''
media = None
audio = None
duration = 0

def set_url(url):
    media_url = r'' + url
    media = mp.VideoFileClip(media_url)
    audio = media.audio
    duration = media.duration

def get_min_max_volume():
    min, max = 0, 0

    # Step is used to get frames each 50ms instead of each second
    step = 0.05
    for f in range(50, media.duration / step):
        f *= step
        if f > media.duration: break
        return audio.get_frame(f)

        