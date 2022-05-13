from ctypes.wintypes import MSG
from email import message
from http import client
import socket
import threading
import volume

HEADER = 64
PORT = 7451
SERVER = socket.gethostbyname(socket.gethostname)
ADDR = (SERVER, PORT)

FORMAT = 'utf-8'
DISCONET_MSG = f'Disconnect from {PORT}'

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

def send_message(msg):
    msg = msg.encode(FORMAT)

    msg_len = len(message)
    encoded_length = str(msg_len).encode(FORMAT)
    encoded_length += b' ' * (HEADER - len(encoded_length))

    client.send(encoded_length)
    client.send(msg)

def listen_and_process_messages():
    while True:
        msg_len = client.recv(HEADER).decode(FORMAT)
        if msg_len:
            msg_len = int(msg_len)
            msg = client.recv(msg_len).decode(FORMAT)