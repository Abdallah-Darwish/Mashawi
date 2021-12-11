#!/usr/bin/env python3
import os
import subprocess
import socket
import sys
import time
import signal
import shutil
import json
from pathlib import Path
import platform


def print_error(err: str) -> None:
    print(err, file=sys.stderr)
    exit(0)


def print_info(info: str) -> None:
    print(info)


def check_ports() -> None:
    def check_port(port: int) -> bool:
        try:
            with socket.socket(socket.AddressFamily.AF_INET, socket.SOCK_STREAM) as sok:
                sok.bind((socket.gethostname(), port))
                sok.close()
        except Exception:
            return False
        return True

    ports = [(1234, 'Backend API')]
    for p in ports:
        if not check_port(p[0]):
            print_error(f"Port {p[0]} used for '{p[1]}' is currently used by another process")


def check_dependecies() -> None:
    docker = subprocess.run(['docker', '-v'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if docker.returncode != 0:
        print_error("Docker is not installed")

    docker_compose = subprocess.run(['docker-compose', '-v'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if docker.returncode != 0:
        docker_compose("Docker-Compose is not installed")


print_info('Checking dependencies.')
check_dependecies()

print_info('Checking ports.')
check_ports()

print_info('Checking directories')
save_directory = Path("~/Desktop").expanduser().joinpath("ChatServer")
data_directory = save_directory.joinpath("Data")

if data_directory.exists() == False:
    data_directory.mkdir(parents=True)

cd = Path(__file__).parent

print_info('Starting Backend API and DB server')
original_docker_compose = cd.joinpath('original-docker-compose.yml')
new_docker_compose = original_docker_compose.with_stem('docker-compose')

user_id, group_id = '1000', '1000'
if platform.system().lower() == 'linux':
    user_id = subprocess.run(['id', '-u'], capture_output=True, text=True).stdout.strip()
    group_id = subprocess.run(['id', '-g'], capture_output=True, text=True).stdout.strip()


new_docker_compose.write_text(original_docker_compose.read_text()
                              .replace('#BackendHostDataDirectory#', str(data_directory))
                              .replace('#HostUserId#', user_id)
                              .replace('#HostGroupId#', group_id)
                              )

subprocess.run(['docker-compose', '-f', str(new_docker_compose), 'build'])

compose = subprocess.Popen(['docker-compose', '-f', str(new_docker_compose), 'up'])

time.sleep(10)


terminate = False


def handler_stop_signals(signum, frame):
    global terminate
    terminate = True


signal.signal(signal.SIGINT, handler_stop_signals)
signal.signal(signal.SIGTERM, handler_stop_signals)

while not terminate:
    time.sleep(5)

print_info('Stopping Backend API and Db server')
compose.terminate()


print_info('C Ya')
