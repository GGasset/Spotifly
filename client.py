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

FORMAT = 'ascii'
DISCONNECT_MSG = f'Disconnect from {PORT}'

socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
socket.bind(ADDR)
socket.connect(ADDR)

def start():
    #threading.Thread(target=listen_and_process_messages)
    listen_and_process_messages()

def send_message(msg):
    msg = str(msg).encode(FORMAT)

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
            if msg != 'Handshake':
                print(f'Received url: {msg} and started processing it')
                v.set_url(msg)
                v.get_audio_sum_list(writeToFile=True)
            else:
                print('Handshake')

if __name__ == '__main__':
    start()