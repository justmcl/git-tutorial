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
IP = '192.168.218.132'

def test_servo_j():
    tcp_server_addr = (IP, 10001)
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        sock.connect(tcp_server_addr)
        print('connect success.')

    except Exception():
        print(traceback.format_exc())

    # 获取数据
    

    listenSocket = socket.socket()         # 创建 socket 对象
    host = socket.gethostname() # 获取本地主机名
    print(host)
    port = 12345                # 设置端口
    listenSocket.bind((host, port))        # 绑定端口

    listenSocket.listen(10)
    print ('Waitting for connect...')
    dataSocket,addr = listenSocket.accept()     # 建立客户端连接
    print ('连接成功，地址：', addr)   
    recData=dataSocket.recv(51200)
    Data=recData.decode()
    dic = json.loads(Data) #得到字典
    if dic['type']=='areuok?':
        dataSocket.send('yes'.encode())
        print ('准备接收数据...')
        recData=dataSocket.recv(51200)
        Data=recData.decode()
        dic = json.loads(Data) #得到字典
        print(f'收到信息{Data}')


    testDict  = '{"cmdName":"moveL","cartPosition":[ 0,0,0,0,0,0],"speed":80.0, "relFlag":0}'
    data = json.loads(testDict)
    data["cartPosition"] = dic['points'][0]
    data["cartPosition"][2]=35.0
    data["cartPosition"][3]=178.0
    testDict = json.dumps(data)
    sock.sendall(testDict.encode())
    time.sleep(5)

    # servo_move
    print('启动：伺服模式')
    testDict = '{"cmdName":"servo_move","relFlag":1}'
    sock.sendall(testDict.encode())
    recvdata = sock.recv(2048).decode()
    print("senddata to server:", testDict)
    print("recvdata in client:", recvdata)
    time.sleep(10)

    # 测试，stepNum = steptime/8ms
    AllDir = []
    testDict = '{"cmdName":"servo_p","catPosition":[350,-67,226,180,0,-69.612],"relFlag":0}'
    mdata = json.loads(testDict, strict=False)
    carPositionLast=dic['points'][0]
    print('插补计算中')
    for point in dic['points']:
        carPosition=[(((point[0]))+3*carPositionLast[0])/4,(((point[1]))+3*carPositionLast[1])/4,35.0,178.0,(((point[4]))+3*carPositionLast[4])/4,(((point[5]))+3*carPositionLast[5])/4]
        mdata["catPosition"] = carPosition
        testDict = json.dumps(mdata)
        AllDir.append(testDict)
        carPosition=[(((point[0]))+carPositionLast[0])/2,(((point[1]))+carPositionLast[1])/2,35.0,178.0,(((point[4]))+carPositionLast[4])/2,(((point[5]))+carPositionLast[5])/2]
        mdata["catPosition"] = carPosition
        testDict = json.dumps(mdata)
        AllDir.append(testDict)
        carPosition=[(3*((point[0]))+carPositionLast[0])/4,(3*((point[1]))+carPositionLast[1])/4,35.0,178.0,(3*((point[4]))+carPositionLast[4])/4,(3*((point[5]))+carPositionLast[5])/4]
        mdata["catPosition"] = carPosition
        testDict = json.dumps(mdata)
        AllDir.append(testDict)
        carPosition = [((point[0])), ((point[1])),35.0,178.0,((point[4])), ((point[5]))]
        mdata["catPosition"] = carPosition
        testDict = json.dumps(mdata)
        AllDir.append(testDict)
        
        carPositionLast=point
    print("插补计算完毕，data has ready：")


    print(AllDir)
    end = datetime.datetime.now()
    time.sleep(0.016)
    print("play starting...")
    play_start = datetime.datetime.now()
    #print(AllDir)
    for repeat_cnt in range(1):
        for pointDir in AllDir:
            print('#####time########')
            start = end
            print(start)

            sendmsg = pointDir.encode()
            t1 = datetime.datetime.now()
            print(t1)
            sock.sendall(sendmsg)
        # TCP接收阻塞，测试时间为了准确不接收
            recvdata = sock.recv(2048).decode()
            #print("senddata to server:", pointDir)
            print("recvdata in client:", recvdata)
            time.sleep(0.007)
            end = datetime.datetime.now()
            print(end)
            print(end - start)  # , "recvdata in client:", recvdata)
    time.sleep(0.5)
    testDict = '{"cmdName":"servo_move","relFlag":0}'
    sock.sendall(testDict.encode())
    play_end = datetime.datetime.now()
    print("play end!")
    #sock.close()



test_servo_j()
