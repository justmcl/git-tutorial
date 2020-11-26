#!/usr/bin/env python
# -*- coding:utf-8 -*-

import socket
import traceback
import time
import json
import csv
import math
import datetime

open_file_name = "ServoP_data.csv"
IP = '192.168.1.101'

def test_servo_j():
    tcp_server_addr = (IP, 10001)
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        sock.connect(tcp_server_addr)
        print('connect success.')

    except Exception():
        print(traceback.format_exc())

    # 解析文件
    reader = []
    print("\r\n open file")
    with open(open_file_name) as csvfile:
        print("\r\n open file succeed")
        csv_reader = csv.reader(csvfile)  # 使用csv.reader读取csvfile中的文件
        for row in csv_reader:  # 将csv 文件中的数据保存到birth_data中
            reader.append(row)

    # 文件第一个点使用joint_move移动到初始点
    pose_0=reader[0]
    jointPosition = [float(pose_0[0]), float(pose_0[1]),float(pose_0[2]), float(pose_0[3]),float(pose_0[4]), float(pose_0[5])]
    print(jointPosition)
    # 使用 joint_move 绝对运动到初始姿态
    joint_move_Pos = jointPosition  # 初始位置
    testDict = '{"cmdName":"joint_move","jointPosition":[-54.793,88.734,119.004,39.184,-105.342,32.032],"speed":30,"relFlag":0}'
    #data = json.loads(testDict)
    #data["jointPosition"] = joint_move_Pos
    #testDict = json.dumps(data)
    sock.sendall(testDict.encode())
    recvdata = sock.recv(2048).decode()
    print("senddata to server:", testDict)
    print("recvdata in client:", recvdata)
    time.sleep(3)
    print("gogogo")
    # servo_move
    testDict = '{"cmdName":"servo_move","relFlag":1}'
    sock.sendall(testDict.encode())
    recvdata = sock.recv(2048).decode()
    print("senddata to server:", testDict)
    print("recvdata in client:", recvdata)
    time.sleep(1)

    # 测试，stepNum = steptime/8ms
    AllDir = []
    testDict = '{"cmdName":"servo_p","catPosition":[350,-67,226,180,0,-69.612],"relFlag":0}'
    mdata = json.loads(testDict, strict=False)

    print("start ready data")
    for tmp_pos in reader:
        # 比较耗时，会导致时间误差过大
        joint_move_Pos = [(float(tmp_pos[0])), (float(tmp_pos[1])),(float(tmp_pos[2])), (float(tmp_pos[3])),(float(tmp_pos[4])), (float(tmp_pos[5]))]
        # stepTime = int(tmp_pos[6]) + 4  # +4是四舍五入
        # # stepMs = float(tmp_pos[6]) * 0.001
        # mdata["stepNum"] = int(stepTime / 8)
        mdata["catPosition"] = joint_move_Pos
        testDict = json.dumps(mdata)
        AllDir.append(testDict)
    print("data has ready")
    end = datetime.datetime.now()
    time.sleep(0.016)
    print("play starting...")
    play_start = datetime.datetime.now()
    #print(AllDir)
    for repeat_cnt in range(1):
        for pointDir in AllDir:
            start = end
            sock.sendall(pointDir.encode())
            #TCP接收阻塞，测试时间为了准确不接收
            recvdata = sock.recv(2048).decode()
            #print("senddata to server:", pointDir)
            print("recvdata in client:", recvdata)
            time.sleep(0.007)
            end = datetime.datetime.now()
            print(end - start)  # , "recvdata in client:", recvdata)
    play_end = datetime.datetime.now()
    time.sleep(0.5)
    testDict = '{"cmdName":"servo_move","relFlag":0}'
    sock.sendall(testDict.encode())
    print("play end!")
    #sock.close()



test_servo_j()
