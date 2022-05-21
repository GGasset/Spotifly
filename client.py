from ctypes.wintypes import MSG
from email import message
from http import client
import socket
import threading
from volume import VolumeProcessor

HEADER = 64
PORT = 7451
IP = socket.gethostbyname(socket.gethostname())
ADDR = (IP, PORT)

FORMAT = 'utf-8'
DISCONNECT_MSG = f'Disconnect from {PORT}'

socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

def start():
    threading.Thread(target=listen_and_process_messages)

def send_message(msg: str):
    msg = msg.encode(FORMAT)

    msg_len = len(message)
    encoded_length = str(msg_len).encode(FORMAT)
    encoded_length += b' ' * (HEADER - len(encoded_length))

    socket.send(encoded_length)
    socket.send(msg)

def listen_and_process_messages():
    v = VolumeProcessor(27)
    while True:
        msg_len = socket.recv(HEADER).decode(FORMAT)
        if msg_len:
            msg_len = int(msg_len)
            msg = socket.recv(msg_len).decode(FORMAT)
            
            # Process message
            msg = msg.split(IP)

            if msg[0] == 'url':
                v.set_min_max_volume(url=msg[1])
                send_message('Finished loading')
            else:
                volume = v.get_volume_percentage_from_ms(int(msg[1]))
                send_message(str(volume))